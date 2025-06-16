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
        Debug.Log("🟡 Algo entró al trigger del horno: " + other.name);

        if (other.CompareTag(tagEsperado))
        {
            // ✅ Asegurarse de que NO esté parentado (o sea, que ya fue soltado)
            if (other.transform.parent != null)
            {
                Debug.Log("❌ El pollo sigue en manos del jugador, no se puede entregar todavía");
                return;
            }

            Debug.Log("🍗 Pollo entregado correctamente");

            // Centrar en el punto de entrega
            other.transform.position = transform.position;

            // Completar la tarea
            tareasManager?.CompletarTarea("Pollo");
        }
    }

}

