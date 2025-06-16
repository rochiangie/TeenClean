using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
    //public TextMeshProUGUI mensajeUI;

    [Header("Transporte Objetos")]
    public Transform puntoDeCarga;
    public Transform puntoCarga;
    [SerializeField] private string[] tagsRecogibles = { "Platos", "RopaSucia", "RopaLimpia", "Tarea" };

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 input;
    private ControladorEstados objetoInteractuableCercano;
    private CabinetController gabinetePlatosCercano;
    private InteraccionSilla sillaCercana;

    [Header("UI de interacción")]
    public GameObject panelInteraccion;
    public TextMeshProUGUI textoInteraccion;

    public GameObject objetoEnMano;


    private GameObject objetoCercanoRecogible;
    private GameObject objetoTransportado;
    private bool llevaObjeto = false;
    public GameObject ObjetoTransportado => objetoTransportado;
    private GameObject objetoCercano;
    private bool enSuelo = true;

    [Header("Prefabs")]
    public GameObject prefabPlatosDefinitivo;
    public GameObject prefabRopaLimpia;
    public GameObject prefabBookOpen;
    public GameObject prefabTarea;
    public GameObject platosLimpiosPrefab;
    public GameObject prefabRopaSucia;

    private GabineteRopa gabineteRopaCercano;
    private GabinetePlatos gabinetePlatosNuevoCercano;
    private GabineteTarea gabineteTareaCercano;


    public Transform puntoSpawnLimpios;

    private bool cercaDelSink = false;
    private GameObject sinkCercano;

    private Dictionary<string, GameObject> prefabsPorTag = new Dictionary<string, GameObject>();

    //[SerializeField] private GameObject panelPopUp; // Para E
    //[SerializeField] public TextMeshProUGUI textoPopUp;

    [SerializeField] private GameObject panelTasks; // Para R (tasks)

    //[SerializeField] private GameObject canvasTextoE;
    //[SerializeField] private TextMeshProUGUI textoE;  // El texto dentro del Canvas


    [Header("Teleport")]
    public Transform puntoSpawn1;
    public Transform puntoSpawn2;
    public Transform puntoInicial; // este es el punto original del primer Player

    private GameObject objetoRecogibleCercano;

    [Header("Ataque")]
    public KeyCode teclaAtaque = KeyCode.F;
    public int dañoAtaque = 10;
    public float rangoAtaque = 1.5f;
    public LayerMask capaEnemigos;

    private bool isAlive = true;
    private TareasManager tareasManager;
    private Madre madreCercana;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isAlive = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        animator.SetBool("isAlive", true);

        if (tagsRecogibles == null || tagsRecogibles.Length == 0)
        {
            tagsRecogibles = new string[] { "Platos", "RopaSucia", "PlatosLimpios", "Tarea" };
        }

        prefabsPorTag.Add("Platos", prefabPlatosDefinitivo);
        prefabsPorTag.Add("RopaSucia", prefabRopaSucia);
        prefabsPorTag.Add("Tarea", prefabTarea);

        tareasManager = FindObjectOfType<TareasManager>();
        if (tareasManager == null)
        {
            Debug.LogError("🚨 No se encontró el TareasManager en la escena.");
        }
    }

    void Update()
    {
        if (!isAlive) return;

        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Abrir/cerrar panel de tareas
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (panelTasks != null)
            {
                bool isActive = panelTasks.activeSelf;
                panelTasks.SetActive(!isActive);
            }
        }

        // Movimiento y animaciones
        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, 10f);
            animator.SetBool("isJumping", true);
            enSuelo = false;
        }

        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        DetectarObjetosCercanos();

        // Ataque
        if (Input.GetKeyDown(teclaAtaque))
        {
            EjecutarAtaque();
        }

        // === INTERACCIONES ===
        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (objetoInteractuableCercano != null)
            {
                string mensaje = $"Presiona {teclaInteraccion} para interactuar";

                if (objetoInteractuableCercano.CompareTag("Madre"))
                {
                    mensaje = $"Presiona {teclaInteraccion} para hablar con Mamá";
                }
                else if (objetoInteractuableCercano.TryGetComponent(out ControladorEstados estado))
                {
                    mensaje = $"Presiona {teclaInteraccion} para usar {estado.ObtenerNombreEstado()}";
                }

                MostrarInteraccion(mensaje);

                GameObject obj = objetoInteractuableCercano.gameObject;

                if (obj.CompareTag("Madre"))
                {
                    Madre madre = obj.GetComponent<Madre>();
                    if (madre != null)
                    {
                        madre.IniciarDialogo();
                        return;
                    }
                }
                else if (obj.TryGetComponent(out ControladorEstados estado2))
                {
                    estado2.AlternarEstado();

                    if (obj.CompareTag("Cama") && tareasManager != null)
                    {
                        tareasManager.CompletarTarea("Cama");
                    }

                    return;
                }

                // Revisar si es la cama para completar la tarea
                if (objetoInteractuableCercano.CompareTag("Cama") && tareasManager != null)
                {
                    tareasManager.CompletarTarea("Cama");
                }

                return;
            }

            // Gabinete para guardar
            if (llevaObjeto)
            {
                if (gabineteRopaCercano != null && objetoTransportado.CompareTag("RopaLimpia") && gabineteRopaCercano.IntentarGuardar(objetoTransportado))
                {
                    objetoTransportado = null;
                    llevaObjeto = false;
                    return;
                }
                if (gabinetePlatosNuevoCercano != null && objetoTransportado.CompareTag("PlatosLimpios") && gabinetePlatosNuevoCercano.IntentarGuardar(objetoTransportado))
                {
                    objetoTransportado = null;
                    llevaObjeto = false;
                    return;
                }
                if (gabineteTareaCercano != null && objetoTransportado.CompareTag("Tarea") && gabineteTareaCercano.IntentarGuardar(objetoTransportado))
                {
                    objetoTransportado = null;
                    llevaObjeto = false;
                    return;
                }
            }

            // Guardar con objeto cercano
            if (objetoCercano != null)
            {
                if (objetoCercano.TryGetComponent(out GabineteRopa ropa))
                    ropa.IntentarGuardar(objetoEnMano);
                else if (objetoCercano.TryGetComponent(out GabinetePlatos platos))
                    platos.IntentarGuardar(objetoEnMano);
                else if (objetoCercano.TryGetComponent(out GabineteTarea tarea))
                    tarea.IntentarGuardar(objetoEnMano);
                else if (objetoCercano.CompareTag("Sink") && sinkCercano != null)
                    return;
            }

            // Heladera (pollo)
            if (objetoInteractuableCercano != null && objetoInteractuableCercano.CompareTag("CabinetePollo"))
            {
                HeladeraController heladera = objetoInteractuableCercano.GetComponent<HeladeraController>();
                if (heladera != null)
                {
                    heladera.IntentarSacarPollo(gameObject);
                    return;
                }
            }

            // Recoger objeto
            if (objetoRecogibleCercano != null && !llevaObjeto)
            {
                objetoTransportado = objetoRecogibleCercano;
                llevaObjeto = true;

                objetoTransportado.transform.SetParent(puntoDeCarga);
                objetoTransportado.transform.localPosition = Vector3.zero;

                Rigidbody2D rb = objetoTransportado.GetComponent<Rigidbody2D>();
                if (rb != null) rb.isKinematic = true;

                Collider2D col = objetoTransportado.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                objetoRecogibleCercano = null;
                return;
            }

            // Soltar objeto si llevamos algo
            if (llevaObjeto)
            {
                SoltarObjeto();
                return;
            }
        }

        // Platos en fregadero
        if (cercaDelSink && objetoTransportado != null && objetoTransportado.CompareTag("Platos"))
        {
            if (Input.GetKeyDown(teclaInteraccion))
            {
                Transform puntoSpawn = puntoSpawnLimpios != null ? puntoSpawnLimpios : transform;

                Destroy(objetoTransportado);
                Instantiate(platosLimpiosPrefab, puntoSpawn.position, Quaternion.identity);
                objetoTransportado = null;

                if (tareasManager != null)
                {
                    tareasManager.CompletarTarea("Platos");
                }
            }
        }

        ActualizarUI();
    }


    void FixedUpdate()
    {
        if (!isAlive) return;

        if (!this.enabled || rb.bodyType == RigidbodyType2D.Static)
            return;

        float velocidad = Input.GetKey(teclaCorrer) ? velocidadCorrer : velocidadMovimiento;
        rb.velocity = input.normalized * velocidad;
    }

    void DetectarObjetosCercanos()
    {
        objetoInteractuableCercano = null;
        gabinetePlatosCercano = null;
        gabineteRopaCercano = null;
        gabineteTareaCercano = null;
        gabinetePlatosNuevoCercano = null;
        objetoCercanoRecogible = null;
        sillaCercana = null;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rango, capaInteractuable);

        float distanciaMinima = float.MaxValue;

        foreach (var col in objetos)
        {
            // Detectar gabinetes nuevos
            if (col.TryGetComponent(out GabineteRopa gRopa))
                gabineteRopaCercano = gRopa;

            if (col.TryGetComponent(out GabinetePlatos gPlatos))
                gabinetePlatosNuevoCercano = gPlatos;

            if (col.TryGetComponent(out GabineteTarea gTarea))
                gabineteTareaCercano = gTarea;

            // Detectar objeto interactuable (ej. inodoro, cama, etc.)
            if (col.TryGetComponent(out ControladorEstados interactuable))
            {
                objetoInteractuableCercano = interactuable;
                //Debug.Log("Asignando objeto interactuable");
            }

            // Detectar madre
            if (col.CompareTag("Madre"))
            {
                if (objetoInteractuableCercano == null)
                    objetoInteractuableCercano = col.GetComponent<ControladorEstados>();

                madreCercana = col.GetComponent<Madre>();
            }

            // Gabinete viejo (por compatibilidad si lo estás usando aún)
            if (col.TryGetComponent(out CabinetController gabinete))
                gabinetePlatosCercano = gabinete;

            // Detectar objetos recogibles
            if (EsRecogible(col.tag) && !llevaObjeto)
            {
                float distancia = Vector2.Distance(transform.position, col.transform.position);
                if (distancia < distanciaMinima)
                {
                    objetoCercanoRecogible = col.gameObject;
                    distanciaMinima = distancia;
                }
            }

            // Detectar silla
            if (col.TryGetComponent(out InteraccionSilla silla))
                sillaCercana = silla;
        }
    }



    void ActualizarUI()
    {
        // Mostrar el panel de interacción solo si hay objetos interactuables cerca
        if (objetoInteractuableCercano != null)
        {
            string texto = $"Presiona {teclaInteraccion} para interactuar";

            if (madreCercana != null)
            {
                texto = $"Presiona {teclaInteraccion} para hablar con Mamá";
            }
            else if (objetoInteractuableCercano.TryGetComponent(out ControladorEstados estado))
            {
                texto = $"Presiona {teclaInteraccion} para usar {estado.ObtenerNombreEstado()}";
            }

            MostrarInteraccion(texto);
        }
        else if (gabinetePlatosCercano != null)
        {
            string nombreObjeto = gabinetePlatosCercano.EstaLleno()
                ? (prefabPlatosDefinitivo != null ? prefabPlatosDefinitivo.name : "objeto")
                : gabinetePlatosCercano.TagObjetoRequerido;

            string texto = $"Presiona {teclaInteraccion} para {(gabinetePlatosCercano.EstaLleno() ? "sacar" : "guardar")} {nombreObjeto}";
            MostrarInteraccion(texto);
        }
        else if (objetoCercanoRecogible != null && !llevaObjeto)
        {
            string texto = $"Presiona {teclaInteraccion} para recoger {objetoCercanoRecogible.tag}";
            MostrarInteraccion(texto);
        }
        else if (objetoCercano != null)
        {
            string texto = "";

            if (objetoCercano.CompareTag("Sink"))
                texto = "Presioná E para lavar los platos";
            else if (objetoCercano.TryGetComponent(out GabineteRopa _))
                texto = "Presioná E para guardar la ropa limpia";
            else if (objetoCercano.TryGetComponent(out GabinetePlatos _))
                texto = "Presioná E para guardar los platos limpios";
            else if (objetoCercano.TryGetComponent(out GabineteTarea _))
                texto = "Presioná E para entregar la tarea";

            if (!string.IsNullOrEmpty(texto))
                MostrarInteraccion(texto);
        }
        else
        {
            OcultarInteraccion();
        }
    }
    public bool LlevaObjetoConTag(string tag)
    {
        return llevaObjeto && objetoTransportado != null && objetoTransportado.CompareTag(tag);
    }


    public void EliminarObjetoTransportado()
    {
        if (objetoTransportado != null)
        {
            Destroy(objetoTransportado);
            objetoTransportado = null;
            llevaObjeto = false;
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
        //Debug.Log("Se destruyó: " + nombreObjeto);

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
            MostrarPopUp($"Presiona {teclaInteraccion} para interactuar con {collision.collider.tag}");
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject raiz = collision.transform.root.gameObject;

        if (collision.gameObject == objetoCercano)
        {
            objetoCercano = null;
            //Debug.Log("Salió de la colisión");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Tapete
        if (other.CompareTag("Tapete"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 10f);
            animator.SetBool("isJumping", true);
            enSuelo = false;
        }

        // Objetos interactuables varios
        if (other.CompareTag("Inodoro") || other.CompareTag("Lavamanos") || other.CompareTag("Bañera"))
        {
            objetoCercano = other.gameObject;
        }

        // Teletransportes
        if (other.CompareTag("Misterio") && puntoSpawn1 != null)
            TeleportarAPunto(puntoSpawn1);
        else if (other.CompareTag("Misterio2") && puntoSpawn2 != null)
            TeleportarAPunto(puntoSpawn2);
        else if (other.CompareTag("Misterio3") && puntoInicial != null)
            TeleportarAPunto(puntoInicial);
        else if (other.CompareTag("Misterio4") && puntoInicial != null)
            TeleportarAPunto(puntoInicial);

        // Sink
        if (other.CompareTag("Sink"))
        {
            cercaDelSink = true;
            sinkCercano = other.gameObject;
        }

        // Silla o sofá
        if (other.CompareTag("Silla") || other.CompareTag("Sofa"))
        {
            animator.SetBool("isTouchingObject", true);
            sillaCercana = other.GetComponent<InteraccionSilla>();
        }

        // Objeto recogible
        if (tagsRecogibles.Contains(other.tag))
        {
            objetoRecogibleCercano = other.gameObject;
        }

        // Gabinetes con lógica centralizada de panel
        if (other.TryGetComponent(out GabineteRopa ropa) && !ropa.EstaLleno())
        {
            objetoCercano = other.gameObject;
            MostrarInteraccion("Presioná E para guardar la ropa limpia");
        }
        else if (other.TryGetComponent(out GabinetePlatos platos) && !platos.EstaLleno())
        {
            objetoCercano = other.gameObject;
            MostrarInteraccion("Presioná E para guardar los platos limpios");
        }
        else if (other.TryGetComponent(out GabineteTarea tarea) && !tarea.EstaLleno())
        {
            objetoCercano = other.gameObject;
            MostrarInteraccion("Presioná E para entregar la tarea");
        }
    }
    void MostrarInteraccion(string mensaje)
    {
        if (panelInteraccion != null && textoInteraccion != null)
        {
            textoInteraccion.text = mensaje;
            panelInteraccion.SetActive(true);
        }
    }

    void OcultarInteraccion()
    {
        if (panelInteraccion != null)
            panelInteraccion.SetActive(false);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Silla") || other.CompareTag("Sofa"))
        {
            //Debug.Log("🪑 Salió de la silla");
            animator.SetBool("isTouchingObject", false);
            sillaCercana = null;
        }

        if (other.gameObject == objetoCercano)
        {
            objetoCercano = null;
            //Debug.Log("Objeto salió de alcance");
            OcultarPopUp();

        }
        if (other.CompareTag("Sink"))  // ✅ Correcto
        {
            Debug.Log("✅ Entré al fregadero con: " + other.name);

            cercaDelSink = false;
            sinkCercano = null;
        }
        if (other.gameObject == objetoRecogibleCercano)
        {
            objetoRecogibleCercano = null;
        }
        if (other.gameObject == objetoCercano)
        {
            OcultarInteraccion();
            objetoCercano = null;
        }
    }

    public void AsignarObjetoTransportado(GameObject objeto)
    {
        objetoTransportado = objeto;
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
        if (prefabRopaSucia == null || puntoDeCarga == null) return;

        GameObject ropa = Instantiate(prefabRopaSucia);
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
            //Debug.LogWarning("⚠️ No hay prefab asignado para el tag: " + tag);
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

    void EjecutarAtaque()
    {
        //Debug.Log("🎯 EjecutarAtaque() fue llamado");

        if (animator != null)
        {
            animator.SetTrigger("Atacar");
            //Debug.Log("🎬 Trigger de animación Atacar enviado");
        }

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(transform.position, rangoAtaque, capaEnemigos);
        //Debug.Log($"🔍 Detectó {enemigos.Length} colliders");

        foreach (Collider2D col in enemigos)
        {
            Debug.Log($"🔍 Analizando: {col.name}");

            if (col.CompareTag("Enemy"))
            {
                Debug.Log($"💥 Detected tag Enemy en {col.name}");

                if (col.TryGetComponent(out Enemigo enemigo))
                {
                    enemigo.RecibirDaño(dañoAtaque);
                    //Debug.Log($"💥 Atacando a {col.name} con {dañoAtaque} de daño");
                }
                else
                {
                    Debug.LogWarning($"❌ {col.name} no tiene componente Enemigo");
                }
            }
            else
            {
                Debug.Log($"❌ {col.name} tiene tag '{col.tag}', no es Enemy");

            }
        }
    }

    void MostrarPopUp(string mensaje)
    {
        Debug.Log($"🟢 MostrarPopUp llamado con mensaje: {mensaje}");
        MostrarInteraccion(mensaje);
    }

    void OcultarPopUp()
    {
        OcultarInteraccion();
    }



    public bool EsTagRecogible(string tag)
    {
        return tagsRecogibles.Contains(tag);
    }

    

    public void Die()
    {
        GetComponent<SaludJugador>()?.Morir();

       /* if (!isAlive) return;

        isAlive = false;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        if (animator != null)
        {
            animator.SetBool("isAlive", false);
        }

        Debug.Log("¡El jugador ha muerto!");*/
    }



}
