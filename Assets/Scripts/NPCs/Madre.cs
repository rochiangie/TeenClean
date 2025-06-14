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
    [SerializeField] private GameObject panelDialogoPrincipal;
    [SerializeField] private TextMeshProUGUI textoDialogoPrincipal;

    [SerializeField] private GameObject panelDialogoRespuestaSi;
    [SerializeField] private TextMeshProUGUI textoDialogoRespuestaSi;

    [SerializeField] private GameObject panelDialogoRespuestaNo;
    [SerializeField] private TextMeshProUGUI textoDialogoRespuestaNo;

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
    [SerializeField] private GameObject panelDialogoPadre;

    void Start()
    {
        if (agente == null)
            agente = GetComponent<NavMeshAgent>();

        if (agente != null)
        {
            agente.updateRotation = false;
            agente.updateUpAxis = false;
        }

        // --- LOGS DE DEPURACIÓN EN START (Muestra si están asignados o son null) ---
        Debug.Log($"[Madre START] panelDialogoPadre ASIGNADO: {(panelDialogoPadre != null ? panelDialogoPadre.name : "NULL")}");
        Debug.Log($"[Madre START] panelDialogoPrincipal ASIGNADO: {(panelDialogoPrincipal != null ? panelDialogoPrincipal.name : "NULL")}");
        Debug.Log($"[Madre START] textoDialogoPrincipal ASIGNADO: {(textoDialogoPrincipal != null ? textoDialogoPrincipal.name : "NULL")}");
        Debug.Log($"[Madre START] panelDialogoRespuestaSi ASIGNADO: {(panelDialogoRespuestaSi != null ? panelDialogoRespuestaSi.name : "NULL")}");
        Debug.Log($"[Madre START] textoDialogoRespuestaSi ASIGNADO: {(textoDialogoRespuestaSi != null ? textoDialogoRespuestaSi.name : "NULL")}");
        Debug.Log($"[Madre START] panelDialogoRespuestaNo ASIGNADO: {(panelDialogoRespuestaNo != null ? panelDialogoRespuestaNo.name : "NULL")}");
        Debug.Log($"[Madre START] textoDialogoRespuestaNo ASIGNADO: {(textoDialogoRespuestaNo != null ? textoDialogoRespuestaNo.name : "NULL")}");
        // --- FIN LOGS DE DEPURACIÓN EN START ---

        // Asegúrate de que todos los paneles estén inicialmente desactivados al iniciar el juego
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
        if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);

        // Desactiva también el padre de los paneles si está asignado
        if (panelDialogoPadre != null)
            panelDialogoPadre.SetActive(false);


        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jugador == null)
        {
            Debug.LogError("🚨 Madre: No se encontró un GameObject con el tag 'Player'. Asegúrate de que tu jugador tenga ese tag.");
        }


        // ** ADVERTENCIAS para que asignes en el Inspector ** (Estas se mantienen para recordatorio)
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
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log("➡️ Jugador responde: No, aún no he hecho las tareas.");
                EvaluarRespuestaTareas(false);
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

        // --- LOGS DE DEPURACIÓN ANTES DE ACTIVAR ---
        Debug.Log($"[Madre INICIAR_DIALOGO] Padre ANTES: {(panelDialogoPadre != null ? panelDialogoPadre.name : "NULL")}, ActiveSelf: {(panelDialogoPadre != null ? panelDialogoPadre.activeSelf.ToString() : "N/A")}, ActiveInHierarchy: {(panelDialogoPadre != null ? panelDialogoPadre.activeInHierarchy.ToString() : "N/A")}");
        Debug.Log($"[Madre INICIAR_DIALOGO] Principal ANTES: {(panelDialogoPrincipal != null ? panelDialogoPrincipal.name : "NULL")}, ActiveSelf: {(panelDialogoPrincipal != null ? panelDialogoPrincipal.activeSelf.ToString() : "N/A")}, ActiveInHierarchy: {(panelDialogoPrincipal != null ? panelDialogoPrincipal.activeInHierarchy.ToString() : "N/A")}");
        // --- FIN LOGS DE DEPURACIÓN ANTES DE ACTIVAR ---


        // Primero, activa el GameObject padre que contiene todos los paneles de diálogo.
        if (panelDialogoPadre != null)
        {
            panelDialogoPadre.SetActive(true);
            Debug.Log($"✅ Madre: Activando panelDialogoPadre: {panelDialogoPadre.name}. ActiveSelf DESPUES: {panelDialogoPadre.activeSelf}, ActiveInHierarchy DESPUES: {panelDialogoPadre.activeInHierarchy}");
        }
        else
        {
            Debug.LogWarning("❌ Madre: 'panelDialogoPadre' es NULL. No se puede activar.");
        }


        if (interaccionesConJugador == 0 || interaccionesConJugador >= maxInteraccionesNormales)
        {
            dialogoAMostrar = dialogoPreguntaTareas;
            panelAMostrar = panelDialogoPrincipal;
            textoAMostrarEn = textoDialogoPrincipal;
            esperandoRespuestaTareas = true;
            Debug.Log("❓ Madre: Preguntando por las tareas.");
        }
        else
        {
            if (dialogosNormales != null && dialogosNormales.Length > 0)
            {
                dialogoAMostrar = dialogosNormales[interaccionesConJugador % dialogosNormales.Length];
                interaccionesConJugador++;
                panelAMostrar = panelDialogoPrincipal;
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
            StartCoroutine(EsperarYReanudar());
        }


        // MOSTRAR EL DIÁLOGO SELECCIONADO
        if (panelAMostrar != null)
        {
            if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
            if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);

            panelAMostrar.SetActive(true);
            Debug.Log($"✅ Madre: Activando panel de diálogo: {panelAMostrar.name}. ActiveSelf DESPUES: {panelAMostrar.activeSelf}, ActiveInHierarchy DESPUES: {panelAMostrar.activeInHierarchy}");
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

        if (jugadorDiceSi)
        {
            if (todasLasTareasHechas)
            {
                dialogoFinal = dialogoVerdadTareas;
                panelAMostrar = panelDialogoRespuestaSi;
                textoAMostrarEn = textoDialogoRespuestaSi;
                Debug.Log("😊 Madre: Jugador dijo la verdad. ¡Bien hecho!");
            }
            else
            {
                dialogoFinal = dialogoMentiraTareas;
                panelAMostrar = panelDialogoRespuestaNo;
                textoAMostrarEn = textoDialogoRespuestaNo;
                Debug.Log("😡 Madre: ¡Jugador mintió sobre las tareas!");
                PenalizarJugador(jugador.gameObject, danoPorMentir);
            }
        }
        else
        {
            if (todasLasTareasHechas)
            {
                dialogoFinal = "Pero... ¿por qué dices que no si ya las hiciste? ¡Qué raro eres!";
                panelAMostrar = panelDialogoRespuestaNo;
                textoAMostrarEn = textoDialogoRespuestaNo;
                Debug.Log("🤔 Madre: Jugador dijo 'no' pero las tareas están hechas.");
            }
            else
            {
                dialogoFinal = "Bueno, al menos eres honesto. ¡Ve a hacerlas ahora mismo!";
                panelAMostrar = panelDialogoRespuestaNo;
                textoAMostrarEn = textoDialogoRespuestaNo;
                Debug.Log("😐 Madre: Jugador dijo la verdad, las tareas no están hechas.");
                PenalizarJugador(jugador.gameObject, danoAlFallar);
            }
        }

        if (panelAMostrar != null)
        {
            if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
            if (panelDialogoRespuestaSi != null && panelAMostrar != panelDialogoRespuestaSi) panelDialogoRespuestaSi.SetActive(false);
            if (panelDialogoRespuestaNo != null && panelAMostrar != panelDialogoRespuestaNo) panelDialogoRespuestaNo.SetActive(false);

            panelAMostrar.SetActive(true);
            Debug.Log($"✅ Madre: Activando panel de respuesta: {panelAMostrar.name}. ActiveSelf DESPUES: {panelAMostrar.activeSelf}, ActiveInHierarchy DESPUES: {panelAMostrar.activeInHierarchy}");
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

        if (!esperandoRespuestaTareas)
        {
            while (jugador != null && Vector2.Distance(jugador.position, transform.position) <= rango)
            {
                yield return null;
            }
        }

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
        esperandoRespuestaTareas = false;

        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
        if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);

        if (panelDialogoPadre != null)
        {
            panelDialogoPadre.SetActive(false);
            Debug.Log($"✅ Madre: Desactivando panelDialogoPadre: {panelDialogoPadre.name}. ActiveSelf DESPUES: {panelDialogoPadre.activeSelf}, ActiveInHierarchy DESPUES: {panelDialogoPadre.activeInHierarchy}");
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