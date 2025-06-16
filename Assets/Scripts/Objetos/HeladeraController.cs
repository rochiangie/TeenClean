using UnityEngine;

public class HeladeraController : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject polloPrefab;
    public Transform puntoDeSalida;

    private bool polloYaRetirado = false;
    private bool jugadorEnRango = false;
    private GameObject jugador;

    private TareasManager tareasManager;

    private void Start()
    {
        tareasManager = FindObjectOfType<TareasManager>();
    }

    private void Update()
    {

        if (jugadorEnRango)
        {
            //Debug.Log("✅ En rango de la heladera");

            // 👉 Reemplazamos GetKeyDown por GetKey para testear
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("🎯 E detectada (con GetKey)");
                IntentarSacarPollo(jugador);
            }
        }
        if (Input.anyKey)
        {
            Debug.Log("⚠️ Se presionó alguna tecla");
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("🎯 E detectada (con GetKey)");
            IntentarSacarPollo(jugador);
        }

    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            jugador = other.gameObject;
            Debug.Log("👀 Jugador entró en la heladera");

            // 👉 Test directo: sacar pollo sin presionar tecla
            IntentarSacarPollo(jugador);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            jugador = null;
            Debug.Log("👋 Jugador salió de la heladera");
        }
    }

    public void IntentarSacarPollo(GameObject jugador)
    {
        Debug.Log("👉 Intentando sacar el pollo");

        if (polloYaRetirado)
        {
            Debug.Log("❌ Ya sacaste el pollo.");
            return;
        }

        if (polloPrefab == null)
        {
            Debug.LogError("❌ polloPrefab no asignado");
            return;
        }

        if (puntoDeSalida == null)
        {
            Debug.LogError("❌ puntoDeSalida no asignado");
            return;
        }

        Transform puntoCarga = jugador.GetComponent<InteraccionJugador>()?.puntoDeCarga;
        if (puntoCarga == null)
        {
            Debug.LogError("❌ puntoDeCarga del jugador no encontrado");
            return;
        }

        GameObject pollo = Instantiate(polloPrefab, puntoDeSalida.position, Quaternion.identity);
        pollo.transform.SetParent(puntoCarga);
        pollo.transform.localPosition = Vector3.zero;

        polloYaRetirado = true;
        Debug.Log("✅ Pollo retirado y puesto en jugador");

        if (tareasManager != null)
        {
            tareasManager.CompletarTarea("Pollo");
            Debug.Log("✅ Tarea del pollo completada");
        }
    }
}
