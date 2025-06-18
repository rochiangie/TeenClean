using UnityEngine;
using TMPro;

public class HeladeraController : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject polloPrefab;
    public Transform puntoDeSalida;

    private bool polloYaRetirado = false;
    private bool jugadorEnRango = false;
    private GameObject jugador;
    private TareasManager tareasManager;

    [Header("UI de interacción")]
    [SerializeField] private GameObject panelInteraccion;
    [SerializeField] private TextMeshProUGUI textoInteraccion;

    private void Start()
    {
        tareasManager = FindObjectOfType<TareasManager>();
    }

    private void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.E))
        {
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

            if (panelInteraccion != null && textoInteraccion != null)
            {
                panelInteraccion.SetActive(true);
                textoInteraccion.text = "Presioná E para sacar el pollo";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            jugador = null;
            Debug.Log("👋 Jugador salió de la heladera");

            if (panelInteraccion != null)
            {
                panelInteraccion.SetActive(false);
            }
        }
    }

    public void IntentarSacarPollo(GameObject jugador)
    {
        if (polloYaRetirado)
        {
            Debug.Log("❌ Ya sacaste el pollo.");
            return;
        }

        if (polloPrefab == null || puntoDeSalida == null)
        {
            Debug.LogError("❌ Prefab o punto de salida no asignado");
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

        if (panelInteraccion != null)
        {
            panelInteraccion.SetActive(false);
        }

        tareasManager?.CompletarTarea("Pollo");
    }
}
