using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Madre : MonoBehaviour
{
    [Header("Navegacion")]
    public NavMeshAgent agente;
    public Transform[] puntosRuta;
    private int indiceRuta = 0;

    [Header("Dialogo")]
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private TextMeshProUGUI textoDialogo;
    [SerializeField] private string[] dialogos;
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.Space;

    [Header("Penalizacion")]
    [SerializeField] private int danoAlFallar = 10;

    public int indiceDialogo = 0;
    public bool enDialogo = false;
    private Transform jugador;

    [Header("Interacción")]
    public float rango = 7f; // rango ajustable desde el Inspector


    private bool esperando = false;


    void Start()
    {
        if (agente == null)
            agente = GetComponent<NavMeshAgent>();

        if (agente != null)
        {
            agente.updateRotation = false;
            agente.updateUpAxis = false;
        }

        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Carga un diálogo de test si no hay ninguno
        if (dialogos == null || dialogos.Length == 0)
        {
            dialogos = new string[]
            {
            "¡Hola! Soy mamá.",
            "Recordá limpiar tu habitación.",
            "¡Y no te olvides de la tarea!"
            };
        }

        IrAlSiguientePunto();
    }


    void Update()
    {
        if (agente != null && !enDialogo && !agente.pathPending)
        {
            if (agente.remainingDistance <= agente.stoppingDistance &&
                (!agente.hasPath || agente.velocity.sqrMagnitude == 0f))
            {
                IrAlSiguientePunto();
            }
        }

        if (jugador != null)
        {
            float distancia = Vector2.Distance(jugador.position, transform.position); // ✅ definida acá

            if (!enDialogo && distancia < rango)
            {
                IniciarDialogo();
            }
            else if (enDialogo && distancia >= rango + 0.5f) // margen para no cerrarlo al toque
            {
                FinalizarDialogo();
            }
        }
    }



    private void IrAlSiguientePunto()
    {
        if (puntosRuta == null || puntosRuta.Length == 0) return;

        agente.SetDestination(puntosRuta[indiceRuta].position);
        indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
    }

    public void IniciarDialogo()
    {
        if (dialogos == null || dialogos.Length == 0) return;

        indiceDialogo = 0;
        enDialogo = true;

        if (agente != null)
            agente.isStopped = true;

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
            textoDialogo.text = dialogos[indiceDialogo];
        }
    }




    private void SiguienteLinea()
    {
        indiceDialogo++;
        if (indiceDialogo < dialogos.Length)
        {
            if (textoDialogo != null)
                textoDialogo.text = dialogos[indiceDialogo];
        }
        else
        {
            FinalizarDialogo();
        }
    }

    private void FinalizarDialogo()
    {
        enDialogo = false;

        if (agente != null)
            agente.isStopped = false; // 🟢 Retoma el movimiento

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }


    public void PenalizarJugador(GameObject jugador)
    {
        if (jugador == null) return;
        var salud = jugador.GetComponent<SaludJugador>();
        if (salud != null)
        {
            salud.RecibirDaño(danoAlFallar);
        }
    }


    private void IniciarDialogoConTemporizador()
    {
        IniciarDialogo();
        Invoke(nameof(FinalizarDialogo), 5f); // Cierra el diálogo a los 5 segundos
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !enDialogo && !esperando)
        {
            jugador = other.transform;
            IniciarDialogoConTemporizador();
            esperando = true;
            Invoke(nameof(ResetEsperando), 6f); // Evita repetir diálogo en 6 segundos
        }
    }
    private void ResetEsperando()
    {
        esperando = false;
    }
}
