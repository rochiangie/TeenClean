using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TareasManager : MonoBehaviour
{
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
    private const int tareasNecesarias = 2;

    private bool camaCompletada = false;


    void Start()
    {
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (panelTasks != null)
            {
                bool isActive = !panelTasks.activeSelf;
                ActivarPanelTasks(isActive);
            }
            else
            {
                Debug.LogError("🚨 panelTasks no está asignado en el Inspector.");
            }
        }
    }

    private void ActivarPanelTasks(bool activar)
    {
        if (panelTasks == null)
        {
            Debug.LogError("🚨 panelTasks no está asignado en el Inspector.");
            return;
        }

        panelTasks.SetActive(activar);
        Debug.Log(activar ? "✅ Panel de tasks activado." : "❌ Panel de tasks desactivado.");
    }

    public void CompletarTarea(string tarea)
    {
        switch (tarea)
        {
            case "RopaLimpia":
                ropaContador++;
                Debug.Log($"👕 Ropa entregada: {ropaContador}/{tareasNecesarias}");
                if (ropaContador >= tareasNecesarias)
                {
                    if (RopaToggle != null) RopaToggle.isOn = true;
                }
                break;
            case "Platos":
                platosContador++;
                Debug.Log($"🍽️ Platos entregados: {platosContador}/{tareasNecesarias}");
                if (platosContador >= tareasNecesarias)
                {
                    if (PlatosToggle != null) PlatosToggle.isOn = true;
                }
                break;
            case "Tarea":
                tareaContador++;
                Debug.Log($"📚 Tareas entregadas: {tareaContador}/{tareasNecesarias}");
                if (tareaContador >= tareasNecesarias)
                {
                    if (TareaToggle != null) TareaToggle.isOn = true;
                }
                break;

            case "Cama":
                camaCompletada = true;
                if (CamaToggle != null)
                {
                    CamaToggle.isOn = true;
                }
                break;
        }

        VerificarVictoria();
    }

    private void VerificarVictoria()
    {
        if (ropaContador >= tareasNecesarias &&
            platosContador >= tareasNecesarias &&
            tareaContador >= tareasNecesarias)
        {
            if (panelWin != null)
            {
                panelWin.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("🎉 ¡Felicidades! Has completado todas las tareas!");
            }
        }
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); // Cambiar a tu nombre real de la escena de menú
    }

    public void ReiniciarTareas()
    {
        ropaContador = 0;
        platosContador = 0;
        tareaContador = 0;

        if (RopaToggle != null) RopaToggle.isOn = false;
        if (PlatosToggle != null) PlatosToggle.isOn = false;
        if (TareaToggle != null) TareaToggle.isOn = false;
    }
}
