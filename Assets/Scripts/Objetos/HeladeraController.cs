using UnityEngine;

public class HeladeraController : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject polloPrefab;
    public Transform puntoDeSalida;

    private bool polloYaRetirado = false;

    private TareasManager tareasManager;

    private void Start()
    {
        tareasManager = FindObjectOfType<TareasManager>();
    }

    public void IntentarSacarPollo(GameObject jugador)
    {
        if (polloYaRetirado)
        {
            Debug.Log("❌ Ya sacaste el pollo.");
            return;
        }

        if (polloPrefab != null && puntoDeSalida != null)
        {
            GameObject pollo = Instantiate(polloPrefab, puntoDeSalida.position, Quaternion.identity);

            // Colocar el pollo en el punto de carga del jugador
            Transform puntoCarga = jugador.GetComponent<InteraccionJugador>().puntoDeCarga;
            pollo.transform.SetParent(puntoCarga);
            pollo.transform.localPosition = Vector3.zero;

            polloYaRetirado = true;
            Debug.Log("✅ Pollo retirado de la heladera.");

            // ✅ Completar la tarea del pollo (si existe en el sistema de tareas)
            if (tareasManager != null)
            {
                tareasManager.CompletarTarea("Pollo");
                Debug.Log("✅ Tarea del pollo completada.");
            }
        }
    }
}
