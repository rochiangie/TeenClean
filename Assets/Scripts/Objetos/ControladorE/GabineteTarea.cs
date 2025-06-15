using UnityEngine;

public class GabineteTarea : MonoBehaviour
{
    [Header("Estados Visuales")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;
    public GameObject prefabObjetoLleno;

    private bool estaLleno = false;
    private TareasManager tareasManager;

    void Awake()
    {
        tareasManager = FindObjectOfType<TareasManager>();
        if (tareasManager == null)
            Debug.LogError("🚨 No se encontró el TareasManager en la escena.");

        estadoVacio?.SetActive(true);
        estadoLleno?.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        if (estaLleno || !objeto.CompareTag("Tarea"))
        {
            Debug.Log("❌ No se puede guardar: gabinete lleno o tag incorrecto.");
            return false;
        }

        Destroy(objeto);
        estaLleno = true;
        estadoVacio?.SetActive(false);
        estadoLleno?.SetActive(true);

        tareasManager?.CompletarTarea("Tarea");
        Debug.Log("📚 Tarea entregada");

        return true;
    }

    public bool EstaLleno()
    {
        return estaLleno;
    }
}
