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

        // Cargar diálogo de prueba si no hay ninguno asignado
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

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    private IEnumerator EsperarYReanudar()
    {
        if (agente != null)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
        }

        enDialogo = true;
        int intentos = 0;
        // Mostrar diálogo visual
        if (panelDialogo != null)
        {
            Debug.Log("📣 Mostrando diálogo de mamá");
            panelDialogo.SetActive(true);
            textoDialogo.text = dialogos.Length > 0 ? dialogos[0] : "Test";
        }
        else
        {
            Debug.LogWarning("🚨 No se asignó el panel de diálogo en el Inspector");
        }

        while (intentos < 2) // espera dos veces como máximo
        {
            yield return new WaitForSeconds(3f);

            if (jugador != null)
            {
                float distancia = Vector2.Distance(jugador.position, transform.position);
                Debug.Log($"📏 Intento {intentos + 1} - Distancia al jugador: {distancia}");

                if (distancia > rango)
                {
                    break; // se alejó, ya podemos continuar
                }
            }

            intentos++;
        }

        Debug.Log("▶️ Reanudando camino de la madre");
        FinalizarDialogo();

        if (agente != null)
        {
            agente.isStopped = false;
            IrAlSiguientePunto();
        }
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
        indiceDialogo = 0;
        enDialogo = true;

        if (agente != null)
        {
            Debug.Log("🛑 Agente detenido");
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
            //agente.ResetPath(); 
        }

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
            textoDialogo.text = dialogos.Length > 0 ? dialogos[0] : "Test";
        }

        Invoke(nameof(FinalizarDialogo), 5f);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = other.transform;
            CancelInvoke(nameof(FinalizarDialogo)); // cancela cualquier cierre anterior en cola
            StartCoroutine(EsperarYReanudar()); // usamos la lógica de espera y reanudación real
        }
    }


    private void ResetEsperando()
    {
        esperando = false;
    }

}
