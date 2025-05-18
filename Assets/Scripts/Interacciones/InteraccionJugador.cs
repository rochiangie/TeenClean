// InteraccionJugador.cs
using UnityEngine;
using TMPro;

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
    [SerializeField] private string[] tagsRecogibles = { "Platos", "RopaSucia", "Tarea" };

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

    [Header("Teleport")]
    public Transform puntoSpawn1;
    public Transform puntoSpawn2;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (mensajeUI != null)
        {
            mensajeUI.text = "PROBANDO MENSAJE";
            mensajeUI.gameObject.SetActive(true);
        }
    }


    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        if (mensajeUI != null)
        {
            mensajeUI.transform.position = transform.position + new Vector3(0, 1.5f, 0); // Ajustá el offset vertical
        }

        DetectarObjetosCercanos();

        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (gabinetePlatosCercano != null)
            {
                if (!gabinetePlatosCercano.EstaLleno() && llevaObjeto && objetoTransportado != null && objetoTransportado.CompareTag(gabinetePlatosCercano.TagObjetoRequerido))
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

        foreach (var col in objetos)
        {
            ControladorEstados interactuable = col.GetComponentInParent<ControladorEstados>();
            if (interactuable != null)
            {
                objetoInteractuableCercano = interactuable;
                Debug.Log($"Detectado objeto interactuable: {interactuable.nombreMostrado}");

            }

            if (col.TryGetComponent(out CabinetController gabinete))
            {
                gabinetePlatosCercano = gabinete;
            }

            if (!llevaObjeto && EsRecogible(col.tag))
            {
                objetoCercanoRecogible = col.gameObject;
            }

            if (col.TryGetComponent(out InteraccionSilla silla))
            {
                sillaCercana = silla;
            }
        }
    }

    void ActualizarUI()
    {
        if (mensajeUI == null) return;

        if (gabinetePlatosCercano != null)
        {
            string nombrePlato = gabinetePlatosCercano.PrefabPlatos != null
                ? gabinetePlatosCercano.PrefabPlatos.name
                : "objeto";

            mensajeUI.text = gabinetePlatosCercano.EstaLleno()
                ? $"Presiona {teclaInteraccion} para sacar {nombrePlato}"
                : $"Presiona {teclaInteraccion} para guardar {gabinetePlatosCercano.TagObjetoRequerido}";

            mensajeUI.gameObject.SetActive(true);
        }
        else if (objetoInteractuableCercano != null)
        {
            string nombreEstado = objetoInteractuableCercano.ObtenerNombreEstado();
            mensajeUI.text = $"Presiona {teclaInteraccion} para usar {nombreEstado}";
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
        objetoTransportado.transform.localScale = Vector3.one;

        // 🔽 AQUI insertás este bloque:
        SpriteRenderer srJugador = GetComponent<SpriteRenderer>();
        SpriteRenderer srObjeto = objetoTransportado.GetComponent<SpriteRenderer>();

        if (srJugador != null && srObjeto != null)
        {
            srObjeto.sortingLayerName = srJugador.sortingLayerName;
            srObjeto.sortingOrder = Mathf.Max(srJugador.sortingOrder + 10, 10);
        }

        if (objetoTransportado.TryGetComponent(out Collider2D col)) col.enabled = false;
        if (objetoTransportado.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = false;
            rb.isKinematic = true;
        }

        objetoCercanoRecogible = null;
        ActualizarUI();
    }


    void SoltarObjeto()
    {
        if (objetoTransportado == null) return;

        objetoTransportado.transform.SetParent(null);
        objetoTransportado.transform.position = transform.position + Vector3.right;

        if (objetoTransportado.TryGetComponent(out Collider2D col)) col.enabled = true;
        if (objetoTransportado.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = true;
            rb.velocity = Vector2.zero;
        }

        llevaObjeto = false;
        objetoTransportado = null;
    }

    public void SoltarYDestruirObjeto()
    {
        if (!llevaObjeto || objetoTransportado == null) return;

        Destroy(objetoTransportado);
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
        if (collision.collider.CompareTag("Suelo") || collision.collider.CompareTag("Piso"))
        {
            enSuelo = true;
            animator.SetBool("isJumping", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Suelo"))
        {
            enSuelo = false;
            animator.SetBool("isJumping", true);
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

        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", true);
            sillaCercana = other.GetComponent<InteraccionSilla>();
        }

        if (other.CompareTag("Misterio") && puntoSpawn1 != null)
        {
            TeleportarAPunto(puntoSpawn1);
        }

        if (other.CompareTag("Misterio2") && puntoSpawn2 != null)
        {
            TeleportarAPunto(puntoSpawn2);
        }

        if (other.CompareTag("Reloj"))
        {
            if (mensajeUI != null)
            {
                mensajeUI.text = $"Tocaste el reloj a las {System.DateTime.Now:HH:mm:ss}";
                mensajeUI.gameObject.SetActive(true);
                mensajeUI.transform.position = transform.position + new Vector3(0, 2f, 0); // flotando sobre el jugador
            }
        }

    }

    void TeleportarAPunto(Transform punto)
    {
        transform.position = punto.position;
        rb.velocity = Vector2.zero;
        animator.SetBool("isJumping", false);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", false);
            sillaCercana = null;
        }
    }
}