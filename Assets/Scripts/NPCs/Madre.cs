using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

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

    void Start()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            agente.updateRotation = false;
            agente.updateUpAxis = false;
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
    }
    void LateUpdate()
    {
        // Mantener la madre siempre visible en Z = 0
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        // También podés fijar la altura (Y) si flota o se hunde
        // pos.y = Mathf.Clamp(pos.y, -5f, 5f);
        // transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !enDialogo)
        {
            DetenerMovimiento();
            IniciarDialogo();
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
            indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
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

            TareasManager.Instance?.PanelVictoria?.SetActive(true);
            StartCoroutine(TareasManager.Instance.CargarMenuPrincipalTrasDelay());
        }
        else
        {
            textoDialogo.text = "Me has dicho una mentira, entonces hay castigo!";
            PenalizarJugador(danoPorMentir);

            MostrarBotones(botonCerrar);
            OcultarBotones(botonSi, botonNo, botonEntendi, botonNoEntendi);
        }

        botonCerrar.onClick.RemoveAllListeners();
        botonCerrar.onClick.AddListener(CerrarDialogo);
    }

    private void ResponderNo()
    {
        textoDialogo.text = "Debes lavar los platos, la ropa, sacar el pollo, guardar todo y luego chequear conmigo.\n¿Entendiste?";

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
            textoDialogo.text = "No te preocupes... pero no es tan difícil eh 😅";
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
