using UnityEngine;
using TMPro;

public class GabinetePlatos : MonoBehaviour
{
    [Header("Estados Visuales")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;
    public GameObject prefabObjetoLleno;

    [Header("UI de interacción")]
    public GameObject panelUI;
    public TextMeshProUGUI textoUI;
    [TextArea] public string mensajeInteraccion = "Presioná E para guardar los platos limpios";

    private bool estaLleno = false;
    private TareasManager tareasManager;

    void Awake()
    {
        tareasManager = FindObjectOfType<TareasManager>();
        if (tareasManager == null)
            Debug.LogError("🚨 No se encontró el TareasManager en la escena.");

        estadoVacio?.SetActive(true);
        estadoLleno?.SetActive(false);

        if (panelUI != null)
            panelUI.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        if (estaLleno || !objeto.CompareTag("PlatosLimpios"))
        {
            Debug.Log("❌ No se puede guardar plato: o ya está lleno o el tag es incorrecto.");
            return false;
        }

        Destroy(objeto);
        estaLleno = true;
        estadoVacio?.SetActive(false);
        estadoLleno?.SetActive(true);

        tareasManager?.CompletarTarea("Platos");
        Debug.Log("🍽️ Tarea de platos completada");

        if (panelUI != null)
            panelUI.SetActive(false);

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !estaLleno && panelUI != null && textoUI != null)
        {
            panelUI.SetActive(true);
            textoUI.text = mensajeInteraccion;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && panelUI != null)
        {
            panelUI.SetActive(false);
        }
    }
}