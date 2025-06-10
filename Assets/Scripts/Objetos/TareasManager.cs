using UnityEngine;
using UnityEngine.UI;

public class TareasManager : MonoBehaviour
{
    [Header("Panel de Tasks")]
    [SerializeField] private GameObject panelTasks;
    [SerializeField] private GameObject panelWin;

    [Header("Toggles de Tasks")]
    [SerializeField] private Toggle RopaToggle;
    [SerializeField] private Toggle PlatosToggle;
    [SerializeField] private Toggle TareaToggle;

    private bool ropaCompletada = false;
    private bool platosCompletados = false;
    private bool tareaCompletada = false;

    void Start()
    {
        if (RopaToggle != null) RopaToggle.interactable = false;
        if (PlatosToggle != null) PlatosToggle.interactable = false;
        if (TareaToggle != null) TareaToggle.interactable = false;
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
            case "Ropa":
                ropaCompletada = true;
                if (RopaToggle != null) RopaToggle.isOn = true;
                break;
            case "Platos":
                platosCompletados = true;
                if (PlatosToggle != null) PlatosToggle.isOn = true;
                break;
            case "Tarea":
                tareaCompletada = true;
                if (TareaToggle != null) TareaToggle.isOn = true;
                break;
        }

        VerificarVictoria();
    }

    private void VerificarVictoria()
    {
        if (ropaCompletada && platosCompletados && tareaCompletada)
        {
            if (panelWin != null)
            {
                panelWin.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("🎉 ¡Felicidades! Has completado todas las tareas.");
            }
        }
    }
}
