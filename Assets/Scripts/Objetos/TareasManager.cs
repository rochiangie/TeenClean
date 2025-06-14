using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TareasManager : MonoBehaviour
{
    // === SINGLETON PATTERN ===
    public static TareasManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Opcional: Si quieres que el TareasManager persista entre escenas
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destruye esta instancia duplicada
        }
    }
    // ========================

    [Header("Panel de Tasks")]
    [SerializeField] private GameObject panelTasks;
    [SerializeField] private GameObject panelWin;

    [Header("Toggles de Tasks")]
    [SerializeField] private Toggle RopaToggle;
    [SerializeField] private Toggle PlatosToggle;
    [SerializeField] private Toggle TareaToggle;
    [SerializeField] private Toggle CamaToggle;

    private int ropaContador = 0;
    private int platosContador = 0;
    private int tareaContador = 0;
    private const int tareasNecesariasRopa = 2; // Asumo que ropa y platos tienen un contador
    private const int tareasNecesariasPlatos = 2;
    private const int tareasNecesariasTarea = 1; // Asumo que tarea y cama son de una sola vez

    private bool ropaCompletada = false;
    private bool platosCompletados = false;
    private bool tareaCompletada = false; // "Tarea" se refiere a la tarea académica/deberes?
    private bool camaCompletada = false;


    void Start()
    {
        // Inicializar toggles y estado (interactable = false, isOn = false)
        if (RopaToggle != null)
        {
            RopaToggle.interactable = false;
            RopaToggle.isOn = false;
        }
        if (PlatosToggle != null)
        {
            PlatosToggle.interactable = false;
            PlatosToggle.isOn = false;
        }
        if (TareaToggle != null)
        {
            TareaToggle.interactable = false;
            TareaToggle.isOn = false;
        }
        if (CamaToggle != null) // Añadido para la cama también
        {
            CamaToggle.interactable = false;
            CamaToggle.isOn = false;
        }

        // Asegurarse de que los paneles estén en su estado inicial
        if (panelTasks != null)
        {
            panelTasks.SetActive(false);
        }
        else
        {
            Debug.LogError("🚨 TareasManager: 'panelTasks' no está asignado en el Inspector.");
        }
        if (panelWin != null)
        {
            panelWin.SetActive(false);
        }
        else
        {
            Debug.LogError("🚨 TareasManager: 'panelWin' no está asignado en el Inspector.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Asumo que 'R' es para toggler el panel de tasks
        {
            ActivarPanelTasks(!panelTasks.activeSelf);
        }
    }

    private void ActivarPanelTasks(bool activar)
    {
        if (panelTasks == null)
        {
            Debug.LogError("🚨 TareasManager: 'panelTasks' no está asignado en el Inspector.");
            return;
        }

        panelTasks.SetActive(activar);
        Debug.Log(activar ? "✅ Panel de tasks activado." : "❌ Panel de tasks desactivado.");

        // Pausar el tiempo del juego cuando el panel está activo
        Time.timeScale = activar ? 0f : 1f;
    }

    // Método principal para completar tareas
    public void CompletarTarea(string tarea)
    {
        switch (tarea)
        {
            case "Ropa":
                ropaContador++;
                // La tarea de ropa se completa cuando el contador alcanza 'tareasNecesariasRopa'
                // O cuando se llama a RegistrarGabineteRopa (ver más abajo)
                // Aquí, si solo hay una forma de completar ropa (llenar 2 gabinetes)
                // esta parte del contador podría ser redundante si RegistrarGabineteRopa ya llama a CompletarTarea("Ropa").
                // Si tienes un caso donde se completa ropa sin usar gabinetes, déjalo.
                // Si la lógica de ropa es SOLO por gabinetes, puedes quitar 'ropaContador'
                // y que RegistrarGabineteRopa sea el único que llame a `CompletarTarea("Ropa")`
                // y marque `ropaCompletada = true; RopaToggle.isOn = true;`
                if (ropaContador >= tareasNecesariasRopa && !ropaCompletada) // Mantengo por si el contador tiene otro uso
                {
                    ropaCompletada = true;
                    if (RopaToggle != null) RopaToggle.isOn = true;
                    Debug.Log("✅ Tarea de ropa completada (por contador).");
                }
                break;
            case "Platos":
                platosContador++;
                if (platosContador >= tareasNecesariasPlatos && !platosCompletados)
                {
                    platosCompletados = true;
                    if (PlatosToggle != null) PlatosToggle.isOn = true;
                    Debug.Log("✅ Tarea de platos completada.");
                }
                break;
            case "Tarea": // Asumo tarea académica o de deberes
                if (!tareaCompletada) // Solo se completa una vez
                {
                    tareaCompletada = true;
                    if (TareaToggle != null) TareaToggle.isOn = true;
                    Debug.Log("✅ Tarea académica completada.");
                }
                break;
            case "Cama":
                if (!camaCompletada) // Solo se completa una vez
                {
                    camaCompletada = true;
                    if (CamaToggle != null) CamaToggle.isOn = true;
                    Debug.Log("✅ Cama hecha.");
                }
                break;
            default:
                Debug.LogWarning($"⚠️ TareasManager: Tarea '{tarea}' no reconocida.");
                break;
        }

        VerificarVictoria(); // Siempre verifica si ya ganó después de completar una tarea
    }


    private void VerificarVictoria()
    {
        // Puedes definir la condición de victoria aquí
        // Por ejemplo, que todas las tareas estén completadas
        if (ropaCompletada && platosCompletados && tareaCompletada && camaCompletada)
        {
            if (panelWin != null)
            {
                panelWin.SetActive(true);
                Time.timeScale = 0f; // Pausa el juego al ganar
                Debug.Log("🎉 ¡Felicidades! Has completado todas las tareas y ganado.");
            }
            else
            {
                Debug.LogError("🚨 TareasManager: 'panelWin' no está asignado en el Inspector.");
            }
        }
    }

    // =========================================================================================
    // NUEVO MÉTODO PARA QUE OTROS SCRIPTS (COMO MADRE) PUEDAN VERIFICAR EL ESTADO DE LAS TAREAS
    // =========================================================================================

    /// <summary>
    /// Devuelve verdadero si TODAS las tareas que la madre "esperaría" que estén completadas, lo están.
    /// Define aquí qué tareas son las que la madre considera para su evaluación.
    /// </summary>
    public bool TodasLasTareasCompletadasParaMadre()
    {
        // Define aquí qué tareas son críticas para la madre.
        // Por ejemplo, quizás la madre solo se preocupa por la cama y la tarea académica,
        // o quizás por todas. Ajusta esto según la lógica de tu juego.
        // Aquí asumo que la madre quiere TODAS las tareas hechas:
        return ropaCompletada && platosCompletados && tareaCompletada && camaCompletada;

        // EJEMPLO: Si la madre solo se preocupa por la cama y los deberes:
        // return camaCompletada && tareaCompletada;
    }


    // El resto de tus métodos específicos para las tareas...

    private HashSet<GabineteRopa> gabinetesConRopa = new HashSet<GabineteRopa>();

    public void RegistrarGabineteRopa(GabineteRopa gabinete)
    {
        if (!gabinetesConRopa.Contains(gabinete))
        {
            gabinetesConRopa.Add(gabinete);
            Debug.Log($"👕 Gabinete registrado ({gabinetesConRopa.Count}/{tareasNecesariasRopa})");

            if (gabinetesConRopa.Count >= tareasNecesariasRopa)
            {
                CompletarTarea("Ropa"); // Llama a CompletarTarea para actualizar el estado y el Toggle
                Debug.Log("✅ Tarea de ropa completada por llenar gabinetes.");
            }
        }
    }


    public void VolverAlMenu()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo esté corriendo antes de cargar la escena
        SceneManager.LoadScene("MenuPrincipal"); // Cambiar a tu nombre real de la escena de menú
    }

    public void ReiniciarTareas()
    {
        ropaContador = 0;
        platosContador = 0;
        tareaContador = 0;

        ropaCompletada = false;
        platosCompletados = false;
        tareaCompletada = false;
        camaCompletada = false; // Reiniciar también el estado de la cama

        if (RopaToggle != null) RopaToggle.isOn = false;
        if (PlatosToggle != null) PlatosToggle.isOn = false;
        if (TareaToggle != null) TareaToggle.isOn = false;
        if (CamaToggle != null) CamaToggle.isOn = false; // Reiniciar el toggle de la cama

        Debug.Log("🔄 TareasManager: Todas las tareas han sido reiniciadas.");

        // Opcional: Desactivar panel de victoria si estaba activo
        if (panelWin != null) panelWin.SetActive(false);
    }
}