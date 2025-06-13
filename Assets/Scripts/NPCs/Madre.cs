using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

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

    [Header("Penalizacion")]
    [SerializeField] private int danoAlFallar = 10;

    [Header("Interacción")]
    public float rango = 5f;

    private int indiceDialogo = 0;
    private bool enDialogo = false;
    private Transform jugador;

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

        if (dialogos == null || dialogos.Length == 0)
        {
            dialogos = new string[]
            {
                "Test",
                "¿Hiciste la tarea?",
                "No olvides lavar los platos."
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = other.transform;

            if (!enDialogo)
            {
                StartCoroutine(EsperarYReanudar());
            }
        }
    }

    private IEnumerator EsperarYReanudar()
    {
        enDialogo = true;

        if (agente != null)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
        }

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
            textoDialogo.text = dialogos.Length > 0 ? dialogos[0] : "Test";
        }

        while (true)
        {
            yield return new WaitForSeconds(10.5f);

            if (jugador == null) break;

            float distancia = Vector2.Distance(jugador.position, transform.position);
            Debug.Log($"📏 Distancia actual: {distancia}");

            if (distancia > rango)
            {
                Debug.Log("👋 Jugador se alejó. Reanudando movimiento.");
                break;
            }
        }

        FinalizarDialogo();

        if (agente != null)
        {
            agente.isStopped = false;
            IrAlSiguientePunto();
        }
    }

    public void IniciarInteraccionConJugador()
    {
        if (!enDialogo)
            StartCoroutine(EsperarYReanudar());
    }


    private void FinalizarDialogo()
    {
        enDialogo = false;

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    private void IrAlSiguientePunto()
    {
        if (puntosRuta == null || puntosRuta.Length == 0) return;

        agente.SetDestination(puntosRuta[indiceRuta].position);
        indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
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
}
