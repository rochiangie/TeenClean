using UnityEngine;
using TMPro;

public class LavarropasController : MonoBehaviour
{
    public enum EstadoLavarropas
    {
        Inicial,
        ConRopa,
        ConAgua,
        Final
    }

    [Header("Estados del Lavarropas")]
    [SerializeField] private GameObject estadoInicial;
    [SerializeField] private GameObject estadoConRopa;
    [SerializeField] private GameObject estadoConAgua;
    [SerializeField] private GameObject estadoFinal;

    [Header("Sonidos")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoRopa;
    [SerializeField] private AudioClip sonidoAgua;
    [SerializeField] private AudioClip sonidoFinLavado;


    [Header("Configuración")]
    [SerializeField] private string tagRopaSucia = "RopaSucia";
    [SerializeField] private string tagRopaLimpia = "RopaLimpia";

    [SerializeField] private GameObject prefabRopaLimpia;
    [SerializeField] private Transform puntoInstanciaRopaLimpia;

    [Header("UI")]
    [SerializeField] private GameObject panelPopUp;
    [SerializeField] private TextMeshProUGUI textoPopUp;

    private EstadoLavarropas estadoActual = EstadoLavarropas.Inicial;
    private bool jugadorEnRango = false;

    private void Start()
    {
        ActualizarEstados();
        OcultarMensaje();
    }

    private void Update()
    {
        if (panelPopUp != null)
            //Debug.Log("👀 Estado del panel: " + panelPopUp.activeSelf);
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.E))
        {
            AvanzarEstado();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = true;
            MostrarMensaje();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = false;
            OcultarMensaje();
        }
    }

    private void AvanzarEstado()
    {
        if (estadoActual == EstadoLavarropas.Inicial)
        {
            // Verificar si el jugador lleva ropa sucia
            InteraccionJugador jugador = FindObjectOfType<InteraccionJugador>();
            if (jugador != null && jugador.LlevaObjetoConTag(tagRopaSucia))
            {
                estadoActual = EstadoLavarropas.ConRopa;
                jugador.EliminarObjetoTransportado();
                audioSource.PlayOneShot(sonidoRopa); 

            }
            else
            {
                Debug.Log("Debes traer ropa sucia para colocarla.");
                return;
            }
        }
        else if (estadoActual == EstadoLavarropas.ConRopa)
        {
            estadoActual = EstadoLavarropas.ConAgua;
            audioSource.PlayOneShot(sonidoAgua);
        }
        else if (estadoActual == EstadoLavarropas.ConAgua)
        {
            estadoActual = EstadoLavarropas.Final;
            audioSource.PlayOneShot(sonidoFinLavado);
            InstanciarRopaLimpia();
        }
        else if (estadoActual == EstadoLavarropas.Final)
        {
            estadoActual = EstadoLavarropas.Inicial;
        }

        ActualizarEstados();
        MostrarMensaje();
    }

    private void ActualizarEstados()
    {
        estadoInicial.SetActive(estadoActual == EstadoLavarropas.Inicial);
        estadoConRopa.SetActive(estadoActual == EstadoLavarropas.ConRopa);
        estadoConAgua.SetActive(estadoActual == EstadoLavarropas.ConAgua);
        estadoFinal.SetActive(estadoActual == EstadoLavarropas.Final);
    }

    private void InstanciarRopaLimpia()
    {
        if (prefabRopaLimpia != null && puntoInstanciaRopaLimpia != null)
        {
            Instantiate(prefabRopaLimpia, puntoInstanciaRopaLimpia.position, Quaternion.identity);
        }
    }

    private void MostrarMensaje()
    {
        Debug.Log("💡 Intentando mostrar mensaje...");

        if (panelPopUp == null)
            Debug.LogWarning("⚠️ panelPopUp no asignado.");
        if (textoPopUp == null)
            Debug.LogWarning("⚠️ textoPopUp no asignado.");

        if (panelPopUp != null && textoPopUp != null)
        {
            panelPopUp.SetActive(true);

            switch (estadoActual)
            {
                case EstadoLavarropas.Inicial:
                    textoPopUp.text = "Presiona E para poner la ropa sucia.";
                    break;
                case EstadoLavarropas.ConRopa:
                    textoPopUp.text = "Presiona E para iniciar el lavado.";
                    break;
                case EstadoLavarropas.ConAgua:
                    textoPopUp.text = "Presiona E para sacar la ropa limpia.";
                    break;
                case EstadoLavarropas.Final:
                    textoPopUp.text = "Presiona E para reiniciar el ciclo.";
                    break;
            }

            Debug.Log("✅ Mensaje mostrado: " + textoPopUp.text);
        }
    }


    private void OcultarMensaje()
    {
        if (panelPopUp != null)
        {
            panelPopUp.SetActive(false);
        }
    }
}
