using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Madre : MonoBehaviour
{
    [Header("Navegacion")]
    public NavMeshAgent agente;
    public Transform[] puntosRuta;
    private int indiceRuta = 0;

    [Header("Dialogo")]
    [SerializeField] private GameObject panelDialogoPrincipal; // Panel principal para la mayoría de los diálogos (antes 'panelDialogo')
    [SerializeField] private TextMeshProUGUI textoDialogoPrincipal; // Texto principal para la mayoría de los diálogos (antes 'textoDialogo')

    // --- NUEVOS CAMPOS PARA DIÁLOGOS ESPECÍFICOS DE RESPUESTA DE TAREAS ---
    [SerializeField] private GameObject panelDialogoRespuestaSi;   // Panel para cuando el jugador dice SÍ
    [SerializeField] private TextMeshProUGUI textoDialogoRespuestaSi;

    [SerializeField] private GameObject panelDialogoRespuestaNo;    // Panel para cuando el jugador dice NO
    [SerializeField] private TextMeshProUGUI textoDialogoRespuestaNo;
    // ---------------------------------------------------------------------

    [SerializeField] private string[] dialogosNormales;
    [SerializeField] private string dialogoMolesto;
    [SerializeField] private string dialogoPreguntaTareas;
    [SerializeField] private string dialogoVerdadTareas;
    [SerializeField] private string dialogoMentiraTareas;
    [SerializeField] private int maxInteraccionesNormales = 3;

    [Header("Penalizacion")]
    [SerializeField] private int danoPorMentir = 20;
    [SerializeField] private int danoAlFallar = 10;

    [Header("Interacción")]
    public float rango = 5f;

    private int interaccionesConJugador = 0;
    private bool enDialogo = false;
    private Transform jugador;
    private bool esperandoRespuestaTareas = false;

    [Header("UI Control")]
    [SerializeField] private GameObject panelDialogoPadre; // Padre que contiene todos los paneles de diálogo

    void Start()
    {
        if (agente == null)
            agente = GetComponent<NavMeshAgent>();

        if (agente != null)
        {
            agente.updateRotation = false;
            agente.updateUpAxis = false;
        }

        // Asegúrate de que todos los paneles estén inicialmente desactivados
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
        if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);

        if (panelDialogoPadre != null)
            panelDialogoPadre.SetActive(false);


        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jugador == null)
        {
            Debug.LogError("🚨 Madre: No se encontró un GameObject con el tag 'Player'. Asegúrate de que tu jugador tenga ese tag.");
        }


        // ** ADVERTENCIAS para que asignes en el Inspector **
        // Se han actualizado los nombres de las referencias para las advertencias
        if (panelDialogoPrincipal == null) Debug.LogWarning("⚠️ Madre: 'panelDialogoPrincipal' no está asignado en el Inspector.");
        if (textoDialogoPrincipal == null) Debug.LogWarning("⚠️ Madre: 'textoDialogoPrincipal' no está asignado en el Inspector.");
        if (panelDialogoRespuestaSi == null) Debug.LogWarning("⚠️ Madre: 'panelDialogoRespuestaSi' no está asignado en el Inspector.");
        if (textoDialogoRespuestaSi == null) Debug.LogWarning("⚠️ Madre: 'textoDialogoRespuestaSi' no está asignado en el Inspector.");
        if (panelDialogoRespuestaNo == null) Debug.LogWarning("⚠️ Madre: 'panelDialogoRespuestaNo' no está asignado en el Inspector.");
        if (textoDialogoRespuestaNo == null) Debug.LogWarning("⚠️ Madre: 'textoDialogoRespuestaNo' no está asignado en el Inspector.");

        if (dialogosNormales == null || dialogosNormales.Length == 0)
        {
            Debug.LogWarning("⚠️ Madre: 'dialogosNormales' está vacío o no asignado en el Inspector.");
        }
        if (string.IsNullOrEmpty(dialogoMolesto))
        {
            Debug.LogWarning("⚠️ Madre: 'dialogoMolesto' está vacío en el Inspector.");
        }
        if (string.IsNullOrEmpty(dialogoPreguntaTareas))
        {
            Debug.LogWarning("⚠️ Madre: 'dialogoPreguntaTareas' está vacío en el Inspector. Asigna un texto para la pregunta de las tareas.");
        }
        if (string.IsNullOrEmpty(dialogoVerdadTareas))
        {
            Debug.LogWarning("⚠️ Madre: 'dialogoVerdadTareas' está vacío en el Inspector.");
        }
        if (string.IsNullOrEmpty(dialogoMentiraTareas))
        {
            Debug.LogWarning("⚠️ Madre: 'dialogoMentiraTareas' está vacío en el Inspector.");
        }
        if (panelDialogoPadre == null && panelDialogoPrincipal != null && panelDialogoPrincipal.transform.parent != null && panelDialogoPrincipal.transform.parent.GetComponent<Canvas>() == null)
        {
            Debug.LogWarning("⚠️ Madre: 'panelDialogoPadre' no está asignado en el Inspector, pero 'panelDialogoPrincipal' tiene un padre que no es el Canvas. Asegúrate de asignar el padre si quieres controlarlo.");
        }


        IrAlSiguientePunto();
    }

    void Update()
    {
        if (agente != null && agente.isOnNavMesh && !enDialogo && !agente.pathPending)
        {
            if (agente.remainingDistance <= agente.stoppingDistance &&
                (!agente.hasPath || agente.velocity.sqrMagnitude == 0f))
            {
                IrAlSiguientePunto();
            }
        }

        if (esperandoRespuestaTareas)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Debug.Log("➡️ Jugador responde: ¡Sí, las tareas están hechas!");
                EvaluarRespuestaTareas(true);
                // No resetear esperandoRespuestaTareas aquí, lo hace EvaluarRespuestaTareas
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log("➡️ Jugador responde: No, aún no he hecho las tareas.");
                EvaluarRespuestaTareas(false);
                // No resetear esperandoRespuestaTareas aquí, lo hace EvaluarRespuestaTareas
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !enDialogo)
        {
            Debug.Log("🚶‍♀️ Madre: Jugador entró en el trigger.");
            jugador = other.transform;
            IniciarDialogoConMadre();
        }
    }

    public void IniciarDialogoConMadre()
    {
        if (enDialogo) return;

        enDialogo = true;
        if (agente != null)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
            Debug.Log("🛑 Madre: agente detenido para diálogo.");
        }

        string dialogoAMostrar = "";
        TextMeshProUGUI textoAMostrarEn = null;
        GameObject panelAMostrar = null;

        // Activa el padre del panelDialogo si está asignado
        if (panelDialogoPadre != null)
        {
            panelDialogoPadre.SetActive(true);
            Debug.Log($"✅ Madre: Activando panelDialogoPadre: {panelDialogoPadre.name}");
        }


        if (interaccionesConJugador == 0 || interaccionesConJugador >= maxInteraccionesNormales)
        {
            // --- Pregunta de tareas ---
            dialogoAMostrar = dialogoPreguntaTareas;
            panelAMostrar = panelDialogoPrincipal; // Usamos el panel principal para la pregunta
            textoAMostrarEn = textoDialogoPrincipal;
            esperandoRespuestaTareas = true;
            Debug.Log("❓ Madre: Preguntando por las tareas.");
        }
        else
        {
            // --- Diálogos normales ---
            if (dialogosNormales != null && dialogosNormales.Length > 0)
            {
                dialogoAMostrar = dialogosNormales[interaccionesConJugador % dialogosNormales.Length];
                interaccionesConJugador++;
                panelAMostrar = panelDialogoPrincipal; // Usamos el panel principal para diálogos normales
                textoAMostrarEn = textoDialogoPrincipal;
                Debug.Log($"🗨️ Madre: Mostrando diálogo normal. Interacciones: {interaccionesConJugador}");
            }
            else
            {
                dialogoAMostrar = "Hola.";
                panelAMostrar = panelDialogoPrincipal;
                textoAMostrarEn = textoDialogoPrincipal;
                Debug.LogWarning("⚠️ Madre: No hay diálogos normales en el Inspector. Mostrando mensaje por defecto.");
            }
            StartCoroutine(EsperarYReanudar()); // Para diálogos normales, la madre no espera respuesta explícita
        }


        // MOSTRAR EL DIÁLOGO SELECCIONADO
        if (panelAMostrar != null)
        {
            // Desactivar otros paneles de respuesta para asegurar que solo uno esté activo
            if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
            if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);

            panelAMostrar.SetActive(true);
            Debug.Log($"✅ Madre: Activando panel de diálogo: {panelAMostrar.name}. Ahora debería ser visible.");
            if (textoAMostrarEn != null)
            {
                textoAMostrarEn.text = dialogoAMostrar;
            }
            else
            {
                Debug.LogWarning($"⚠️ Madre: El texto asociado al panel {panelAMostrar.name} no está asignado en el Inspector.");
            }
        }
        else
        {
            Debug.LogWarning("❌ Madre: Ningún panel de diálogo fue asignado o seleccionado para mostrar.");
        }
    }

    private void EvaluarRespuestaTareas(bool jugadorDiceSi)
    {
        string dialogoFinal = "";
        GameObject panelAMostrar = null;
        TextMeshProUGUI textoAMostrarEn = null;

        // Desactivar el panel principal de pregunta antes de mostrar la respuesta
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);


        bool todasLasTareasHechas = TareasManager.Instance != null && TareasManager.Instance.TodasLasTareasCompletadasParaMadre();

        if (jugadorDiceSi) // El jugador afirma haber hecho las tareas
        {
            if (todasLasTareasHechas)
            {
                dialogoFinal = dialogoVerdadTareas;
                panelAMostrar = panelDialogoRespuestaSi; // Usar panel de SI
                textoAMostrarEn = textoDialogoRespuestaSi;
                Debug.Log("😊 Madre: Jugador dijo la verdad. ¡Bien hecho!");
            }
            else
            {
                // El jugador miente: las tareas NO están completadas, pero el jugador dice que SÍ.
                dialogoFinal = dialogoMentiraTareas;
                panelAMostrar = panelDialogoRespuestaNo; // Usar panel de NO para la mentira
                textoAMostrarEn = textoDialogoRespuestaNo;
                Debug.Log("😡 Madre: ¡Jugador mintió sobre las tareas!");
                PenalizarJugador(jugador.gameObject, danoPorMentir); // Penalización por mentir
            }
        }
        else // El jugador admite no haber hecho las tareas
        {
            if (todasLasTareasHechas)
            {
                // Jugador dice 'no' pero sí hizo las tareas (caso extraño pero posible)
                dialogoFinal = "Pero... ¿por qué dices que no si ya las hiciste? ¡Qué raro eres!";
                panelAMostrar = panelDialogoRespuestaNo; // Podrías usar este o uno específico para "verdad-raro"
                textoAMostrarEn = textoDialogoRespuestaNo;
                Debug.Log("🤔 Madre: Jugador dijo 'no' pero las tareas están hechas.");
            }
            else
            {
                // Jugador dice la verdad: las tareas NO están completadas y lo admite.
                dialogoFinal = "Bueno, al menos eres honesto. ¡Ve a hacerlas ahora mismo!";
                panelAMostrar = panelDialogoRespuestaNo; // Usar panel de NO
                textoAMostrarEn = textoDialogoRespuestaNo;
                Debug.Log("😐 Madre: Jugador dijo la verdad, las tareas no están hechas.");
                PenalizarJugador(jugador.gameObject, danoAlFallar); // Penalización por no haberlas hecho y admitirlo
            }
        }

        // MOSTRAR EL DIÁLOGO DE RESPUESTA
        if (panelAMostrar != null)
        {
            // Desactivar el panel principal y el de la otra respuesta
            if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
            if (panelDialogoRespuestaSi != null && panelAMostrar != panelDialogoRespuestaSi) panelDialogoRespuestaSi.SetActive(false);
            if (panelDialogoRespuestaNo != null && panelAMostrar != panelDialogoRespuestaNo) panelDialogoRespuestaNo.SetActive(false);

            panelAMostrar.SetActive(true);
            Debug.Log($"✅ Madre: Activando panel de respuesta: {panelAMostrar.name}");
            if (textoAMostrarEn != null)
            {
                textoAMostrarEn.text = dialogoFinal;
            }
            else
            {
                Debug.LogWarning($"⚠️ Madre: El texto asociado al panel {panelAMostrar.name} no está asignado en el Inspector.");
            }
        }
        else
        {
            Debug.LogWarning("❌ Madre: Ningún panel de respuesta fue asignado o seleccionado para mostrar.");
        }

        StartCoroutine(EsperarYReanudar());
        interaccionesConJugador++;
    }

    private IEnumerator EsperarYReanudar()
    {
        Debug.Log("⏳ Madre: EsperarYReanudar iniciado.");
        yield return new WaitForSeconds(3.0f);

        // Ya no verificamos esperandoRespuestaTareas aquí, el StartCoroutine se encarga
        // de reanudar el flujo general de la Madre después de un diálogo.

        Debug.Log("👋 Madre: Jugador se alejó o diálogo terminó.");
        FinalizarDialogo();

        if (agente != null)
        {
            agente.isStopped = false;
            Debug.Log("🏃‍♀️ Madre: Retoma movimiento.");
            IrAlSiguientePunto();
        }
    }

    private void FinalizarDialogo()
    {
        Debug.Log("🔚 Madre: Finaliza diálogo.");
        enDialogo = false;
        esperandoRespuestaTareas = false; // Resetear este estado aquí al final del diálogo.

        // Desactiva todos los paneles de diálogo
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
        if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);

        // Desactiva el padre de los paneles de diálogo si está asignado
        if (panelDialogoPadre != null)
        {
            panelDialogoPadre.SetActive(false);
            Debug.Log($"✅ Madre: Desactivando panelDialogoPadre: {panelDialogoPadre.name}");
        }
    }

    private void IrAlSiguientePunto()
    {
        if (puntosRuta == null || puntosRuta.Length == 0) return;

        agente.SetDestination(puntosRuta[indiceRuta].position);
        Debug.Log($"📍 Madre: Moviéndose a punto {indiceRuta}.");
        indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
    }

    public void PenalizarJugador(GameObject jugadorObj, int dano)
    {
        if (jugadorObj == null) return;

        var salud = jugadorObj.GetComponent<SaludJugador>();
        if (salud != null)
        {
            salud.RecibirDaño(dano);
            Debug.Log($"💔 Jugador: Recibió {dano} de daño de la madre. Vida actual: {salud.GetVidaActual()}");
        }
        else
        {
            Debug.LogWarning("⚠️ Madre: El jugador no tiene un componente 'SaludJugador'. No se pudo aplicar daño.");
        }
    }

    public void ReanudarMovimiento()
    {
        Debug.Log("🟢 Madre: ReanudarMovimiento manual.");
        if (agente != null)
        {
            agente.isStopped = false;
            IrAlSiguientePunto();
        }
        enDialogo = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}