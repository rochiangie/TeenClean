using UnityEngine;
using TMPro;

public class HeladeraController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject prefabPollo;
    [SerializeField] private TextMeshProUGUI mensajeUI;
    [SerializeField] private string mensajeInteraccion = "Presiona E para sacar el pollo";
    [SerializeField] private TareasManager tareasManager;

    private bool jugadorEnRango = false;
    private GameObject jugador;
    private bool tareaCompletada = false;

    private void Update()
    {
        if (!jugadorEnRango || tareaCompletada || jugador == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("🍗 Sacando pollo de la heladera");

            InteraccionJugador interaccion = jugador.GetComponent<InteraccionJugador>();
            if (interaccion != null && !interaccion.EstaLlevandoObjeto())
            {
                GameObject pollo = Instantiate(prefabPollo);
                interaccion.RecogerObjeto(pollo);

                tareasManager?.CompletarTarea("Pollo");
                tareaCompletada = true;
                OcultarMensaje();
            }
            else
            {
                Debug.Log("⚠️ Ya estás llevando otro objeto o algo salió mal.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = other.gameObject;
            jugadorEnRango = true;
            MostrarMensaje();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            jugador = null;
            OcultarMensaje();
        }
    }

    private void MostrarMensaje()
    {
        if (mensajeUI != null && !tareaCompletada)
        {
            mensajeUI.text = mensajeInteraccion;
            mensajeUI.gameObject.SetActive(true);
        }
    }

    private void OcultarMensaje()
    {
        if (mensajeUI != null)
        {
            mensajeUI.gameObject.SetActive(false);
        }
    }
}
