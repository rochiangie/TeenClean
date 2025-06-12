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
    [SerializeField] private KeyCode teclaContinuar = KeyCode.Space;

    [Header("Penalizacion")]
    [SerializeField] private int danoAlFallar = 10;

    private int indiceDialogo = 0;
    private bool enDialogo = false;

    void Start()
    {
        if (agente == null)
            agente = GetComponent<NavMeshAgent>();

        if (agente != null)
        {
            agente.updateRotation = false; // NavMesh 2D
            agente.updateUpAxis = false;
        }

        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        IrAlSiguientePunto();
    }

    void Update()
    {
        if (agente != null && !enDialogo && !agente.pathPending)
        {
            Debug.Log("📍 Distancia restante: " + agente.remainingDistance);

            if (agente.remainingDistance <= agente.stoppingDistance &&
                (!agente.hasPath || agente.velocity.sqrMagnitude == 0f))
            {
                Debug.Log("✅ Llegó al punto. Pasando al siguiente.");
                IrAlSiguientePunto();
            }
        }

        if (enDialogo && Input.GetKeyDown(teclaContinuar))
        {
            SiguienteLinea();
        }
    }


    private void IrAlSiguientePunto()
    {
        if (puntosRuta == null || puntosRuta.Length == 0)
        {
            Debug.LogWarning("🚨 No hay puntos de ruta asignados.");
            return;
        }

        Debug.Log("🚶‍♀️ Yendo al punto " + indiceRuta + ": " + puntosRuta[indiceRuta].name);
        agente.SetDestination(puntosRuta[indiceRuta].position);
        indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
    }


    private void IniciarDialogo()
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IniciarDialogo();
        }
    }
}
