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
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;

    [Header("Penalizacion")]
    [SerializeField] private int danoAlFallar = 10;

    public int indiceDialogo = 0;
    public bool enDialogo = false;
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

        // Chequeo de interacción con tecla E
        if (!enDialogo && jugador != null && Input.GetKeyDown(teclaInteraccion))
        {
            float distancia = Vector2.Distance(jugador.position, transform.position);
            if (distancia < 2f) // distancia de interacción
            {
                IniciarDialogo();
            }
        }

        if (enDialogo && Input.GetKeyDown(KeyCode.Space))
        {
            SiguienteLinea();
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
}
