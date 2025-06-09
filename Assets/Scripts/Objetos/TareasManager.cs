using UnityEngine;
using UnityEngine.UI;

public class TareasManager : MonoBehaviour
{
    [Header("Panel de Tasks")]
    [SerializeField] private GameObject panelTasks; // Panel de tareas (Canvas - tasks > panelTasks)
    [SerializeField] private GameObject panelWin;   // Panel de victoria final

    [Header("Toggles de Tasks")]
    [SerializeField] private Toggle RopaToggle;
    [SerializeField] private Toggle PlatosToggle;
    [SerializeField] private Toggle TareaToggle;

    private bool ropaCompletada = false;
    private bool platosCompletados = false;
    private bool tareaCompletada = false;

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

        // Primero activa el panel
        panelTasks.SetActive(true);

        if (activar)
        {
            // Luego activa todos los hijos (incluidos los inactivos)
            foreach (Transform child in panelTasks.GetComponentsInChildren<Transform>(true))
            {
                if (child != panelTasks.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            Debug.Log("✅ Panel de tasks activado.");
        }
        else
        {
            panelTasks.SetActive(false);
            Debug.Log("❌ Panel de tasks desactivado.");
        }
    }


    // Método para marcar la tarea como completada
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
                Time.timeScale = 0f; // Pausar el juego
                Debug.Log("🎉 ¡Felicidades! Has completado todas las tareas.");
            }
        }
    }
}
