using UnityEngine;
using TMPro;

public class GabineteRopa : MonoBehaviour
{
    [Header("Estados Visuales")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;
    public GameObject prefabObjetoLleno;

    [Header("UI de interacción")]
    public GameObject panelUI;
    public TextMeshProUGUI textoUI;
    [TextArea] public string mensajeInteraccion = "Presioná E para guardar la ropa limpia";

    private bool estaLleno = false;
    private TareasManager tareasManager;

    void Awake()
    {
        tareasManager = FindObjectOfType<TareasManager>();
        if (tareasManager == null)
            Debug.LogError("🚨 No se encontró el TareasManager en la escena.");

        estadoVacio?.SetActive(true);
        estadoLleno?.SetActive(false);

        // Asegurarse de que el panel esté oculto al inicio
        if (panelUI != null)
            panelUI.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        Debug.Log("🧪 Intentando guardar objeto...");

        if (estaLleno)
        {
            Debug.Log("❌ No se puede guardar: gabinete lleno.");
            return false;
        }

        if (!objeto.CompareTag("RopaLimpia"))
        {
            Debug.Log($"❌ Tag incorrecto: {objeto.tag} (esperado: RopaLimpia)");
            return false;
        }

        Destroy(objeto);
        estaLleno = true;

        if (estadoVacio != null && estadoLleno != null)
        {
            estadoVacio.SetActive(false);
            estadoLleno.SetActive(true);
            Debug.Log("✅ Estado del gabinete cambiado a 'Lleno'");
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignaron estadoVacio o estadoLleno en el Inspector.");
        }

        if (tareasManager != null)
        {
            tareasManager.CompletarTarea("Ropa");
            Debug.Log("🎯 Tarea de ropa completada");
        }

        // Ocultar panel al completar
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
