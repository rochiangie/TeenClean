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

        if (panelDialogoMadre != null)
            panelDialogoMadre.SetActive(false);

        // Configurar eventos de botones
        if (botonSi != null) botonSi.onClick.AddListener(RespuestaSi);
        if (botonNo != null) botonNo.onClick.AddListener(RespuestaNo);
        if (botonCerrar != null) botonCerrar.onClick.AddListener(CerrarDialogo);

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !enDialogo)
        {
            DetenerMovimiento();
            IniciarDialogoConMadre();
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
            indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
        }
    }

    public void IniciarDialogoConMadre()
    {
        enDialogo = true;

        if (panelDialogoMadre != null && textoDialogo != null)
        {
            panelDialogoMadre.SetActive(true);
            textoDialogo.text = "Hola hija! Espero estés bien. ¿Hiciste tus tasks de hoy?";
        }
    }

    private void RespuestaSi()
    {
        bool tareasHechas = TareasManager.Instance?.TodasLasTareasCompletadasParaMadre() ?? false;

        if (tareasHechas)
        {
            textoDialogo.text = "¡Muy bien! Estoy orgullosa de ti.";
            // Podés agregar aquí la victoria si querés:
            // TareasManager.Instance.PanelVictoria.SetActive(true);
            // StartCoroutine(TareasManager.Instance.CargarMenuPrincipalTrasDelay());
        }
        else
        {
            textoDialogo.text = "Me has dicho una mentira, entonces hay castigo!";
            PenalizarJugador(danoPorMentir);
        }
    }

    private void RespuestaNo()
    {
        textoDialogo.text = "Debes lavar los platos y la ropa, luego guardar todo en su lugar y venir a chequear conmigo.";
        PenalizarJugador(danoAlFallar);
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
}
