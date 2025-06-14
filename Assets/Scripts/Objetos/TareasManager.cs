using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    private bool ropaCompletada = false;
    private bool platosCompletados = false;
    private bool tareaCompletada = false;
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
            case "Ropa":
                ropaContador++;
                if (ropaContador >= tareasNecesarias) ropaCompletada = true;
                if (RopaToggle != null) RopaToggle.isOn = true;
                break;
            case "Platos":
                platosContador++;
                if (platosContador >= tareasNecesarias) platosCompletados = true;
                if (PlatosToggle != null) PlatosToggle.isOn = true;
                break;
            case "Tarea":
                tareaContador++;
                if (tareaContador >= tareasNecesarias) tareaCompletada = true;
                if (TareaToggle != null) TareaToggle.isOn = true;
                break;
            case "Cama":
                camaCompletada = true;
                if (CamaToggle != null) CamaToggle.isOn = true;
                break;
        }

        VerificarVictoria();
    }


    private void VerificarVictoria()
    {
        if (ropaCompletada && platosCompletados && tareaCompletada && camaCompletada)
        {
            if (panelWin != null)
            {
                panelWin.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("🎉 ¡Felicidades! Has completado todas las tareas!");
            }
        }
    }


    private HashSet<GabineteRopa> gabinetesConRopa = new HashSet<GabineteRopa>();

    public void RegistrarGabineteRopa(GabineteRopa gabinete)
    {
        if (!gabinetesConRopa.Contains(gabinete))
        {
            gabinetesConRopa.Add(gabinete);
            Debug.Log($"👕 Gabinete registrado ({gabinetesConRopa.Count}/2)");

            if (gabinetesConRopa.Count >= 2)
            {
                CompletarTarea("Ropa");
                Debug.Log("✅ Tarea de ropa completada por llenar 2 gabinetes distintos");
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
