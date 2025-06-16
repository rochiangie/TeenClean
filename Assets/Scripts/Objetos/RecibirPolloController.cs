using UnityEngine;

public class RecibirPolloController : MonoBehaviour
{
    [Header("Configuración")]
    public string tagEsperado = "Pollo";
    public TareasManager tareasManager;

    private void Start()
    {
        if (tareasManager == null)
            tareasManager = FindObjectOfType<TareasManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagEsperado))
        {
            Debug.Log("🍗 Pollo entregado correctamente");

            // ✅ Soltar el pollo si está cargado por el jugador
            other.transform.SetParent(null);

            // ✅ Mover el pollo al centro del punto de entrega
            other.transform.position = transform.position;

            // ✅ Completar tarea
            tareasManager?.CompletarTarea("Pollo");
        }
    }
}
