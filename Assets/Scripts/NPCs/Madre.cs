using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using UnityEngine.UI;


public class Madre : MonoBehaviour
{
    [Header("Navegación")]
    public NavMeshAgent agente;
    public Transform[] puntosRuta;
    private int indiceRuta = 0;

    [Header("Componentes UI")]
    [SerializeField] private GameObject panelDialogoPadre;
    [SerializeField] private GameObject panelDialogoPrincipal;
    [SerializeField] private TextMeshProUGUI textoDialogoPrincipal;
    [SerializeField] private GameObject panelDialogoRespuestaSi;
    [SerializeField] private TextMeshProUGUI textoDialogoRespuestaSi;
    [SerializeField] private GameObject panelDialogoRespuestaNo;
    [SerializeField] private TextMeshProUGUI textoDialogoRespuestaNo;

    [Header("Contenido Diálogos")]
    [SerializeField] private string[] dialogosNormales;
    [SerializeField] private string dialogoPreguntaTareas;
    [SerializeField] private string dialogoVerdadTareas;
    [SerializeField] private string dialogoMentiraTareas;
    [SerializeField] private int maxInteraccionesNormales = 3;
    [SerializeField] private Button botonSiUI;
    [SerializeField] private Button botonNoUI;
    [SerializeField] private Button botonCerrarUI;
    [SerializeField] private GameObject canvasMom;


    [Header("Configuración")]
    public float rangoInteraccion = 3f;
    public float tiempoEsperaDialogo = 3f;
    public int danoPorMentir = 20;
    public int danoAlFallar = 10;
    [SerializeField] private KeyCode teclaInteractuar = KeyCode.E; // ← agregá esta línea


    private int interaccionesConJugador = 0;
    private bool enDialogo = false;
    private Transform jugador;
    private bool esperandoRespuestaTareas = false;

    void Start()
    {
        InicializarComponentes();
        ActivarCanvasMomCompleto();
        IrAlSiguientePunto();
    }

    private void ActivarCanvasMomCompleto()
    {
        if (canvasMom != null)
        {
            canvasMom.SetActive(true); // por si estaba desactivado
            foreach (Transform hijo in canvasMom.transform)
            {
                hijo.gameObject.SetActive(true); // activa todos los paneles e hijos
            }

            Debug.Log("✅ Canvas-Mom y todos sus hijos activados.");
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó Canvas-Mom en el Inspector.");
        }
    }

    private void InicializarComponentes()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            agente.updateRotation = false;
            agente.updateUpAxis = false;
        }

        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jugador == null) Debug.LogError("No se encontró al jugador");

        // Asegurarse que todos los paneles estén desactivados al inicio
        if (panelDialogoPadre != null) panelDialogoPadre.SetActive(false);
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
        if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);
    }

    void Update()
    {
        if (panelDialogoPadre != null)
        {
            if (!panelDialogoPadre.activeSelf)
            {
                Debug.LogWarning("⚠️ PanelDialogoPadre fue desactivado por otro script.");
            }
        }

        // Tu código normal...
        ControlarMovimiento();
        ControlarRespuestas();
    }





    private void ControlarMovimiento()
    {
        if (agente != null && agente.isOnNavMesh && !enDialogo && !agente.pathPending &&
            agente.remainingDistance <= agente.stoppingDistance)
        {
            IrAlSiguientePunto();
        }
    }

    private void ControlarRespuestas()
    {
        if (esperandoRespuestaTareas)
        {
            if (Input.GetKeyDown(KeyCode.Y)) EvaluarRespuestaTareas(true);
            else if (Input.GetKeyDown(KeyCode.N)) EvaluarRespuestaTareas(false);
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && enDialogo && !esperandoRespuestaTareas)
        {
            FinalizarDialogo();
        }
    }


    private void DetenerMovimiento()
    {
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
            Debug.Log("🚫 Madre detenida");
        }
        else
        {
            Debug.LogWarning("⚠️ Madre no está sobre NavMesh o agente no asignado");
        }
    }


    public void IniciarDialogoConMadre()
    {
        // Asegurar que todo el Canvas esté activo
        if (canvasMom != null)
        {
            canvasMom.SetActive(true);
            foreach (Transform hijo in canvasMom.transform)
            {
                hijo.gameObject.SetActive(true);
            }
        }

        if (enDialogo) return;

        enDialogo = true;
        DetenerMovimiento();

        if (panelDialogoPadre != null)
        {
            panelDialogoPadre.SetActive(true);
            Debug.Log("Panel padre activado");
        }

        if (interaccionesConJugador == 0 || interaccionesConJugador >= maxInteraccionesNormales)
        {
            MostrarDialogo(panelDialogoPrincipal, dialogoPreguntaTareas, textoDialogoPrincipal);
            esperandoRespuestaTareas = true;

            // Botones respuesta
            if (botonSiUI != null && botonNoUI != null)
            {
                botonSiUI.gameObject.SetActive(true);
                botonNoUI.gameObject.SetActive(true);
                botonSiUI.onClick.RemoveAllListeners();
                botonNoUI.onClick.RemoveAllListeners();
                botonSiUI.onClick.AddListener(() => EvaluarRespuestaTareas(true));
                botonNoUI.onClick.AddListener(() => EvaluarRespuestaTareas(false));
            }
        }
        else
        {
            string dialogo = dialogosNormales.Length > 0 ?
                dialogosNormales[interaccionesConJugador % dialogosNormales.Length] : "Hola.";

            MostrarDialogo(panelDialogoPrincipal, dialogo, textoDialogoPrincipal);
            interaccionesConJugador++;

            if (botonSiUI != null) botonSiUI.gameObject.SetActive(false);
            if (botonNoUI != null) botonNoUI.gameObject.SetActive(false);

            StartCoroutine(EsperarYReanudar());
        }

        // Activar botón cerrar
        if (botonCerrarUI != null)
        {
            botonCerrarUI.gameObject.SetActive(true);
            botonCerrarUI.onClick.RemoveAllListeners();
            botonCerrarUI.onClick.AddListener(FinalizarDialogo);
        }
    }



    private void MostrarDialogo(GameObject panel, string texto, TextMeshProUGUI componenteTexto = null)
    {

        // Desactivar todos los paneles primero
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
        if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);

        // Activar el panel solicitado
        if (panel != null)
        {
            panel.SetActive(true);
            Debug.Log($"Activando panel: {panel.name}");
        }

        // Establecer el texto
        if (componenteTexto != null)
        {
            componenteTexto.text = texto;
        }
        else if (panel != null)
        {
            var textoComponent = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (textoComponent != null) textoComponent.text = texto;
        }
    }

    private void EvaluarRespuestaTareas(bool jugadorDiceSi)
    {
        bool tareasHechas = TareasManager.Instance?.TodasLasTareasCompletadasParaMadre() ?? false;
        string dialogo;
        GameObject panel;
        TextMeshProUGUI texto;

        if (jugadorDiceSi)
        {
            dialogo = tareasHechas ? dialogoVerdadTareas : dialogoMentiraTareas;
            panel = tareasHechas ? panelDialogoRespuestaSi : panelDialogoRespuestaNo;
            texto = tareasHechas ? textoDialogoRespuestaSi : textoDialogoRespuestaNo;

            if (!tareasHechas) PenalizarJugador(danoPorMentir);
        }
        else
        {
            dialogo = tareasHechas ?
                "¿Por qué dices que no si ya las hiciste?" :
                "Bueno, al menos eres honesto. ¡Hazlas ahora!";
            panel = panelDialogoRespuestaNo;
            texto = textoDialogoRespuestaNo;

            if (!tareasHechas) PenalizarJugador(danoAlFallar);
        }

        MostrarDialogo(panel, dialogo, texto);
        esperandoRespuestaTareas = false;
        interaccionesConJugador++;
        StartCoroutine(EsperarYReanudar());
    }

    private IEnumerator EsperarYReanudar()
    {
        yield return new WaitForSeconds(tiempoEsperaDialogo);

        if (!esperandoRespuestaTareas)
        {
            yield return new WaitUntil(() =>
                jugador == null || Vector2.Distance(jugador.position, transform.position) > rangoInteraccion);
        }

        FinalizarDialogo();
    }

    private void FinalizarDialogo()
    {
        enDialogo = false;
        esperandoRespuestaTareas = false;
        DesactivarTodosLosPaneles();
        ReanudarMovimiento();

        if (botonSiUI != null)
        {
            botonSiUI.onClick.RemoveAllListeners();
            botonSiUI.gameObject.SetActive(false);
        }
        if (botonNoUI != null)
        {
            botonNoUI.onClick.RemoveAllListeners();
            botonNoUI.gameObject.SetActive(false);
        }
        if (botonCerrarUI != null)
        {
            botonCerrarUI.onClick.RemoveAllListeners();
            botonCerrarUI.gameObject.SetActive(false);
        }

    }


    private void ReanudarMovimiento()
    {
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = false;
            Debug.Log("Madre reanudando movimiento");
            IrAlSiguientePunto();
        }
    }

    private void IrAlSiguientePunto()
    {
        if (puntosRuta != null && puntosRuta.Length > 0 && agente != null && agente.isOnNavMesh)
        {
            agente.SetDestination(puntosRuta[indiceRuta].position);
            indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
            Debug.Log($"Madre moviéndose al punto {indiceRuta}");
        }
    }

    private void PenalizarJugador(int dano)
    {
        var salud = jugador?.GetComponent<SaludJugador>();
        if (salud != null)
        {
            salud.RecibirDaño(dano);
            Debug.Log($"Jugador penalizado con {dano} de daño");
        }
    }

    private void DesactivarTodosLosPaneles()
    {
        if (panelDialogoPadre != null) panelDialogoPadre.SetActive(false);
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelDialogoRespuestaSi != null) panelDialogoRespuestaSi.SetActive(false);
        if (panelDialogoRespuestaNo != null) panelDialogoRespuestaNo.SetActive(false);
        Debug.Log("Todos los paneles desactivados");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoInteraccion);
    }
}