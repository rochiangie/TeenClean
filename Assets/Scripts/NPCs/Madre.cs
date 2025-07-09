using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Madre : MonoBehaviour
{
    [Header("Navegación")]
    public NavMeshAgent agente;
    public Transform[] puntosRuta;
    private int indiceRuta = 0;

    [Header("Diálogo UI")]
    [SerializeField] private GameObject panelDialogoMadre;
    [SerializeField] private TextMeshProUGUI textoDialogo;
    [SerializeField] private Button botonSi;
    [SerializeField] private Button botonNo;
    [SerializeField] private Button botonEntendi;
    [SerializeField] private Button botonNoEntendi;
    [SerializeField] private Button botonCerrar;

    [Header("Configuración")]
    public float rangoInteraccion = 3f;
    public int danoPorMentir = 20;
    public int danoAlFallar = 10;

    private bool enDialogo = false;
    private Transform jugador;

    public bool interactuableDesdeSegundaVez = true;
    public string tagInteractuable = "InteractuableMadre";

    private bool primeraInteraccion = true;
    private bool jugadorEnRango = false;

    void Start()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            agente.updateRotation = false;
            agente.updateUpAxis = false;

            if (TareasManager.Instance != null)
            {
                float velocidad = TareasManager.Instance.ObtenerVelocidadMadre();
                agente.speed = velocidad;
                Debug.Log("👟 Velocidad de la madre según dificultad: " + velocidad);
            }
            else
            {
                agente.speed = 3f;
            }
        }

        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jugador == null) Debug.LogError("❌ No se encontró al jugador");

        if (panelDialogoMadre != null) panelDialogoMadre.SetActive(false);

        OcultarBotones(botonSi, botonNo, botonEntendi, botonNoEntendi, botonCerrar);

        IrAlSiguientePunto();
    }

    void Update()
    {
        if (agente != null && agente.isOnNavMesh && !enDialogo && !agente.pathPending &&
            agente.remainingDistance <= agente.stoppingDistance)
        {
            IrAlSiguientePunto();
        }
        if (!primeraInteraccion && jugadorEnRango && !enDialogo && Input.GetKeyDown(KeyCode.E))
        {
            DetenerMovimiento();
            IniciarDialogo();
        }
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            if (!agente.hasPath || agente.velocity.sqrMagnitude == 0f)
            {
                indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
                IrAlSiguientePunto();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;

            if (primeraInteraccion && !enDialogo)
            {
                primeraInteraccion = false;
                DetenerMovimiento();
                IniciarDialogo();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
        }
    }

    private void DetenerMovimiento()
    {
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
        }
    }

    private void ReanudarMovimiento()
    {
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = false;
            IrAlSiguientePunto();
        }
    }

    private void IrAlSiguientePunto()
    {
        if (puntosRuta != null && puntosRuta.Length > 0 && agente != null && agente.isOnNavMesh)
        {
            agente.SetDestination(puntosRuta[indiceRuta].position);
            Debug.Log("📍 Madre va hacia el punto " + indiceRuta);
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo mover: ruta vacía o no está en NavMesh");
        }
    }

    public void IniciarDialogo()
    {
        enDialogo = true;

        if (panelDialogoMadre != null && textoDialogo != null)
        {
            panelDialogoMadre.SetActive(true);
            textoDialogo.text = "Hola hija! ¿Hiciste tus tareas hoy?";

            MostrarBotones(botonSi, botonNo);
            OcultarBotones(botonEntendi, botonNoEntendi, botonCerrar);

            botonSi.onClick.RemoveAllListeners();
            botonNo.onClick.RemoveAllListeners();

            botonSi.onClick.AddListener(() => ResponderSi());
            botonNo.onClick.AddListener(() => ResponderNo());
        }
    }

    private void ResponderSi()
    {
        bool tareasHechas = TareasManager.Instance?.TodasLasTareasCompletadasParaMadre() ?? false;

        if (tareasHechas)
        {
            textoDialogo.text = "¡Muy bien! Estoy orgullosa de ti.";
            OcultarBotones(botonSi, botonNo, botonEntendi, botonNoEntendi);
            MostrarBotones(botonCerrar);

            // Espera a que el jugador cierre el diálogo antes de mostrar el final feliz
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(() =>
            {
                CerrarDialogo();
                StartCoroutine(EsperarYMostrarFinalFeliz());
            });
        }
        else
        {
            textoDialogo.text = "Me has dicho una mentira, entonces hay castigo!";
            PenalizarJugador(danoPorMentir);

            MostrarBotones(botonCerrar);
            OcultarBotones(botonSi, botonNo, botonEntendi, botonNoEntendi);

            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(CerrarDialogo);
        }
    }

    private IEnumerator EsperarYMostrarFinalFeliz()
    {
        yield return new WaitForSeconds(2f); // ⏳ Da tiempo a leer el mensaje de la madre
        if (TareasManager.Instance != null)
        {
            TareasManager.Instance.MostrarFinalFelizYCreditos();
        }
    }

    private void ResponderNo()
    {
        textoDialogo.text = "Debes lavar los platos, la ropa, sacar el pollo, hacer la cama, guardar todo y luego chequear conmigo.\n¿Entendiste?";

        MostrarBotones(botonEntendi, botonNoEntendi, botonCerrar);
        OcultarBotones(botonSi, botonNo);

        botonEntendi.onClick.RemoveAllListeners();
        botonNoEntendi.onClick.RemoveAllListeners();

        botonEntendi.onClick.AddListener(() =>
        {
            CerrarDialogo();
        });

        botonNoEntendi.onClick.AddListener(() =>
        {
            textoDialogo.text = "Ni que fuera tan difìcil 😅";
            MostrarBotones(botonCerrar);
            OcultarBotones(botonEntendi, botonNoEntendi);
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(CerrarDialogo);
        });
    }

    private void PenalizarJugador(int daño)
    {
        var salud = jugador?.GetComponent<SaludJugador>();
        if (salud != null)
        {
            salud.RecibirDaño(daño);
        }
    }

    private void CerrarDialogo()
    {
        if (panelDialogoMadre != null)
            panelDialogoMadre.SetActive(false);

        enDialogo = false;
        ReanudarMovimiento();
    }

    private void MostrarBotones(params Button[] botones)
    {
        foreach (var btn in botones)
            if (btn != null) btn.gameObject.SetActive(true);
    }

    private void OcultarBotones(params Button[] botones)
    {
        foreach (var btn in botones)
            if (btn != null) btn.gameObject.SetActive(false);
    }
}
