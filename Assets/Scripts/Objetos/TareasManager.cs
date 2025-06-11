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

    private int ropaContador = 0;
    private int platosContador = 0;
    private int tareaContador = 0;
    private const int tareasNecesarias = 2;

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
                ropaContador++;
                Debug.Log($"👕 Ropa entregada: {ropaContador}/2");
                if (ropaContador >= tareasNecesarias)
                {
                    if (RopaToggle != null) RopaToggle.isOn = true;
                }
                break;
            case "Platos":
                platosContador++;
                Debug.Log($"🍽️ Platos entregados: {platosContador}/2");
                if (platosContador >= tareasNecesarias)
                {
                    if (PlatosToggle != null) PlatosToggle.isOn = true;
                }
                break;
            case "Tarea":
                tareaContador++;
                Debug.Log($"📚 Tareas entregadas: {tareaContador}/2");
                if (tareaContador >= tareasNecesarias)
                {
                    if (TareaToggle != null) TareaToggle.isOn = true;
                }
                break;
        }

        VerificarVictoria();
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


    private void VerificarVictoria()
    {
        if (ropaContador >= tareasNecesarias && platosContador >= tareasNecesarias && tareaContador >= tareasNecesarias)
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
