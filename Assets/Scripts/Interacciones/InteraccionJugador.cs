using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class InteraccionJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    public float velocidadCorrer = 8f;
    public KeyCode teclaCorrer = KeyCode.LeftShift;

    [Header("Interacción")]
    public float rango = 1.5f;
    public LayerMask capaInteractuable;
    public KeyCode teclaInteraccion = KeyCode.E;

    [Header("UI")]
    public TextMeshProUGUI mensajeUI;

    [Header("Transporte Objetos")]
    public Transform puntoDeCarga;
    [SerializeField] private string[] tagsRecogibles = { "Platos", "Ropa", "Tarea" };

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 input;
    private ControladorEstados objetoInteractuableCercano;
    private CabinetController gabinetePlatosCercano;
    private InteraccionSilla sillaCercana;

    private GameObject objetoCercanoRecogible;
    private GameObject objetoTransportado;
    private bool llevaObjeto = false;
    public GameObject ObjetoTransportado => objetoTransportado;
    private GameObject objetoCercano;
    private bool enSuelo = true;

    [Header("Prefabs")]
    public GameObject prefabPlatosDefinitivo;
    //public GameObject PrefabPlatosDefinitivo;
    public GameObject prefabRopa;
    public GameObject prefabBookOpen;
    public GameObject prefabTarea;


    private Dictionary<string, GameObject> prefabsPorTag = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject panelPopUp;

    [Header("Teleport")]
    [Header("Teleport")]
    public Transform puntoSpawn1;
    public Transform puntoSpawn2;
    public Transform puntoInicial; // este es el punto original del primer Player




    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (tagsRecogibles == null || tagsRecogibles.Length == 0)
        {
            tagsRecogibles = new string[] { "Platos", "Ropa", "Tarea" };
        }

        prefabsPorTag.Add("Platos", prefabPlatosDefinitivo);
        prefabsPorTag.Add("Ropa", prefabRopa);
        prefabsPorTag.Add("Tarea", prefabTarea);
    }


    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Movimiento y animación
        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        // Flip de sprite
        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        // Detectar objetos
        DetectarObjetosCercanos();

        // Interacciones
        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (gabinetePlatosCercano != null)
            {
                if (!gabinetePlatosCercano.EstaLleno() && llevaObjeto && objetoTransportado.CompareTag(gabinetePlatosCercano.TagObjetoRequerido))
                {
                    gabinetePlatosCercano.IntentarGuardarPlatos(this);
                    return;
                }
                else if (gabinetePlatosCercano.EstaLleno() && !llevaObjeto)
                {
                    gabinetePlatosCercano.SacarPlatosDelGabinete(this);
                    return;
                }
                return;
            }
            else if (objetoInteractuableCercano != null)
            {
                objetoInteractuableCercano.AlternarEstado();
                return;
            }
            else if (!llevaObjeto && objetoCercanoRecogible != null && EsRecogible(objetoCercanoRecogible.tag))
            {
                RecogerObjeto(objetoCercanoRecogible);
                return;
            }
            else if (!llevaObjeto && sillaCercana != null)
            {
                sillaCercana.EjecutarAccion(gameObject);
            }
        }

        // Soltar objeto (si lleva algo y presiona E)
        if (Input.GetKeyDown(KeyCode.E) && llevaObjeto)
        {
            SoltarObjeto();
        }

        ActualizarUI();
    }

    void FixedUpdate()
    {
        float velocidad = Input.GetKey(teclaCorrer) ? velocidadCorrer : velocidadMovimiento;
        rb.velocity = input.normalized * velocidad;
    }

    void DetectarObjetosCercanos()
    {
        objetoInteractuableCercano = null;
        gabinetePlatosCercano = null;
        objetoCercanoRecogible = null;
        sillaCercana = null;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rango, capaInteractuable);

        float distanciaMinima = float.MaxValue;

        foreach (var col in objetos)
        {
            Debug.Log("🔍 Detectado: " + col.name + " | Tag: " + col.tag);

            if (col.TryGetComponent(out ControladorEstados interactuable))
                objetoInteractuableCercano = interactuable;

            if (col.TryGetComponent(out CabinetController gabinete))
                gabinetePlatosCercano = gabinete;

            if (EsRecogible(col.tag) && !llevaObjeto)
            {
                float distancia = Vector2.Distance(transform.position, col.transform.position);
                if (distancia < distanciaMinima)
                {
                    objetoCercanoRecogible = col.gameObject;
                    distanciaMinima = distancia;

                    Debug.Log("🧺 Ropa asignada como objeto cercano recogible: " + col.name);
                }
            }


            if (col.TryGetComponent(out InteraccionSilla silla))
                sillaCercana = silla;
        }
    }


    void ActualizarUI()
    {
        if (mensajeUI == null) return;

        if (gabinetePlatosCercano != null)
        {
            string nombreObjeto = gabinetePlatosCercano.EstaLleno()
                ? (prefabPlatosDefinitivo != null ? prefabPlatosDefinitivo.name : "objeto")
                : gabinetePlatosCercano.TagObjetoRequerido;

            mensajeUI.text = $"Presiona {teclaInteraccion} para {(gabinetePlatosCercano.EstaLleno() ? "sacar" : "guardar")} {nombreObjeto}";
            mensajeUI.gameObject.SetActive(true);
        }
        else if (objetoInteractuableCercano != null)
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para usar {objetoInteractuableCercano.ObtenerNombreEstado()}";
            mensajeUI.gameObject.SetActive(true);
        }
        else if (objetoCercanoRecogible != null && !llevaObjeto)
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para recoger {objetoCercanoRecogible.tag}";
            mensajeUI.gameObject.SetActive(true);
        }
        else
        {
            mensajeUI.gameObject.SetActive(false);
        }
    }


    bool EsRecogible(string tag)
    {
        foreach (string recogible in tagsRecogibles)
        {
            if (tag == recogible) return true;
        }
        return false;
    }

    public void RecogerObjeto(GameObject objeto)
    {
        if (llevaObjeto || objeto == null) return;

        llevaObjeto = true;
        objetoTransportado = objeto;

        objetoTransportado.transform.SetParent(puntoDeCarga);
        objetoTransportado.transform.localPosition = Vector3.zero;
        objetoTransportado.transform.localRotation = Quaternion.identity;

        // Escala adaptada a la dirección del jugador y tamaño original del objeto
        Vector3 escalaOriginal = objeto.transform.localScale;
        Vector3 escalaFinal = new Vector3(
            Mathf.Abs(escalaOriginal.x) * Mathf.Sign(transform.localScale.x),
            Mathf.Abs(escalaOriginal.y),
            1f
        );
        objeto.transform.localScale = escalaFinal;

        // Asegurar orden de render
        SpriteRenderer srJugador = GetComponent<SpriteRenderer>();
        SpriteRenderer srObjeto = objetoTransportado.GetComponent<SpriteRenderer>();

        if (srJugador != null && srObjeto != null)
        {
            srObjeto.sortingLayerName = srJugador.sortingLayerName;
            srObjeto.sortingOrder = srJugador.sortingOrder + 1;
        }

        Collider2D col = objetoTransportado.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = objetoTransportado.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
            rb.isKinematic = true;
        }

        objetoCercanoRecogible = null;
        ActualizarUI();
    }



    public void SoltarObjeto()
    {
        if (objetoTransportado == null) return;

        objetoTransportado.transform.SetParent(null);
        objetoTransportado.transform.position = transform.position + Vector3.right;

        // Restaurar escala visual si hace falta
        Vector3 escalaOriginal = objetoTransportado.transform.localScale;
        objetoTransportado.transform.localScale = new Vector3(
            Mathf.Abs(escalaOriginal.x),
            Mathf.Abs(escalaOriginal.y),
            1f
        );

        // Reactivar colisión pero NO física
        if (objetoTransportado.TryGetComponent(out Collider2D col)) col.enabled = true;

        if (objetoTransportado.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = true;
            rb.isKinematic = true; // ✅ Mantener Kinematic para que NO se caiga
            rb.velocity = Vector2.zero;
        }

        llevaObjeto = false;
        objetoTransportado = null;
    }



    public void SoltarYDestruirObjeto()
    {
        if (!llevaObjeto || objetoTransportado == null) return;

        string nombreObjeto = objetoTransportado.name;
        Destroy(objetoTransportado);
        Debug.Log("Se destruyó: " + nombreObjeto);

        objetoTransportado = null;
        llevaObjeto = false;
    }



    public bool EstaLlevandoObjeto() => llevaObjeto;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject raiz = collision.transform.root.gameObject;

        if (collision.collider.CompareTag("Suelo") || collision.collider.CompareTag("Piso"))
        {
            enSuelo = true;
            animator.SetBool("isJumping", false);
        }
        if (collision.collider.CompareTag("Inodoro") || collision.collider.CompareTag("Lavamanos") || collision.collider.CompareTag("Bañera"))
        {
            objetoCercano = collision.gameObject;
            Debug.Log("Colisionó con: " + objetoCercano.name);
            MostrarPopUp();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject raiz = collision.transform.root.gameObject;

        if (collision.gameObject == objetoCercano)
        {
            objetoCercano = null;
            Debug.Log("Salió de la colisión");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tapete"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 10f);
            animator.SetBool("isJumping", true);
            enSuelo = false;
        }

        if (other.CompareTag("Inodoro") || other.CompareTag("Lavamanos") || other.CompareTag("Bañera"))
        {
            objetoCercano = other.gameObject;
            Debug.Log("Objeto cercano: " + objetoCercano.name);

        }

        if (other.CompareTag("Misterio") && puntoSpawn1 != null)
        {
            TeleportarAPunto(puntoSpawn1);
        }
        else if (other.CompareTag("Misterio2") && puntoSpawn2 != null)
        {
            TeleportarAPunto(puntoSpawn2);
        }
        else if (other.CompareTag("Misterio3") && puntoInicial != null)
        {
            TeleportarAPunto(puntoInicial);
        }
        else if (other.CompareTag("Misterio4") && puntoInicial != null)
        {
            TeleportarAPunto(puntoInicial);
        }



        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", true);
            sillaCercana = other.GetComponent<InteraccionSilla>();
        }
        if (other.CompareTag("Misterio") && puntoSpawn1 != null)
        {
            TeleportarAPunto(puntoSpawn1);
        }
        else if (other.CompareTag("Misterio2") && puntoSpawn2 != null)
        {
            TeleportarAPunto(puntoSpawn2);
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", false);
            sillaCercana = null;
        }

        if (other.gameObject == objetoCercano)
        {
            objetoCercano = null;
            Debug.Log("Objeto salió de alcance");
            OcultarPopUp();

        }
    }

    public void CrearPlatosEnMano()
    {
        if (prefabPlatosDefinitivo == null || puntoDeCarga == null) return;

        GameObject platos = Instantiate(prefabPlatosDefinitivo);
        platos.transform.SetParent(puntoDeCarga);
        platos.transform.localPosition = Vector3.zero;
        platos.transform.localRotation = Quaternion.identity;
        platos.transform.localScale = Vector3.one * 3f;

        RecogerObjeto(platos);
    }

    public void InstanciarPlatosDefinitivo()
    {
        if (prefabPlatosDefinitivo == null || puntoDeCarga == null) return;

        GameObject nuevosPlatos = Instantiate(prefabPlatosDefinitivo);
        nuevosPlatos.transform.SetParent(puntoDeCarga);
        nuevosPlatos.transform.localPosition = Vector3.zero;
        nuevosPlatos.transform.localRotation = Quaternion.identity;
        nuevosPlatos.transform.localScale = Vector3.one * 3f;

        RecogerObjeto(nuevosPlatos);
    }

    public void InstanciarRopa()
    {
        if (prefabRopa == null || puntoDeCarga == null) return;

        GameObject ropa = Instantiate(prefabRopa);
        ropa.transform.SetParent(puntoDeCarga);
        ropa.transform.localPosition = Vector3.zero;
        ropa.transform.localRotation = Quaternion.identity;
        ropa.transform.localScale = Vector3.one * 3f;

        RecogerObjeto(ropa);
    }

    public void InstanciarObjetoPorTag(string tag)
    {
        if (!prefabsPorTag.ContainsKey(tag))
        {
            Debug.LogWarning("⚠️ No hay prefab asignado para el tag: " + tag);
            return;
        }

        GameObject prefab = prefabsPorTag[tag];
        if (prefab == null || puntoDeCarga == null) return;

        GameObject nuevo = Instantiate(prefab);
        nuevo.transform.SetParent(puntoDeCarga);
        nuevo.transform.localPosition = Vector3.zero;
        nuevo.transform.localRotation = Quaternion.identity;

        float escala = tag == "Tarea" ? 10f : 3f;
        Vector3 escalaFinal = new Vector3(
            escala * Mathf.Sign(transform.localScale.x),
            escala * Mathf.Sign(transform.localScale.y),
            1f
        );
        nuevo.transform.localScale = escalaFinal;

        RecogerObjeto(nuevo);
        // Asegurar orden de render
        SpriteRenderer srJugador = GetComponent<SpriteRenderer>();
        SpriteRenderer srObjeto = nuevo.GetComponent<SpriteRenderer>();

        if (srJugador != null && srObjeto != null)
        {
            srObjeto.sortingLayerName = srJugador.sortingLayerName;
            srObjeto.sortingOrder = srJugador.sortingOrder + 1; // adelante del jugador
        }

    }

    void TeleportarAPunto(Transform punto)
    {
        transform.position = punto.position;
        rb.velocity = Vector2.zero;
        animator.SetBool("isJumping", false);
    }


    void MostrarPopUp()
    {
        panelPopUp.SetActive(true);
    }

    void OcultarPopUp()
    {
        panelPopUp.SetActive(false);
    }


}
