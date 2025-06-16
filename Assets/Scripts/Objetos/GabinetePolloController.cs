using UnityEngine;
using TMPro;

public class GabinetePolloController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string tagObjetoRequerido = "Pollo";
    [SerializeField] private TareasManager tareasManager;
    [SerializeField] private TextMeshProUGUI mensajeUI;
    [SerializeField] private string mensajeInteraccion = "Presiona E para guardar el pollo";

    private bool tareaCompletada = false;

    void Start()
    {
        Debug.Log("✅ GabinetePolloController inicializado");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (tareaCompletada || !other.CompareTag("Player")) return;

        InteraccionJugador interaccion = other.GetComponent<InteraccionJugador>();
        if (interaccion == null) return;

        MostrarMensaje();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("🟢 Presionaste E cerca de la heladera con el pollo");

            if (interaccion.LlevaObjetoConTag(tagObjetoRequerido))
            {
                Debug.Log("✅ Pollo entregado");
                interaccion.EliminarObjetoTransportado();
                tareasManager?.CompletarTarea("Pollo");
                tareaCompletada = true;
                OcultarMensaje();
            }
            else
            {
                Debug.Log("❌ No llevás el pollo");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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
