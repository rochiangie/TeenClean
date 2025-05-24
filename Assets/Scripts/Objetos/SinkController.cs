using UnityEngine;
using TMPro;

public class SinkController : MonoBehaviour
{
    [Header("Estados visuales")]
    public GameObject estadoVacio;
    public GameObject estadoSucio;
    public GameObject estadoLimpio;

    [Header("Configuración")]
    public KeyCode teclaInteraccion = KeyCode.E;
    public string tagPlatosSucios = "Platos";
    public GameObject prefabPlatosLimpios;

    [Header("UI")]
    public TextMeshProUGUI mensajeUI;

    private int estadoActual = 0; // 0: vacío, 1: sucio, 2: limpio
    private bool jugadorEnRango = false;
    private GameObject jugador;
    private InteraccionJugador interaccionJugador;
    public Transform puntoCarga;
    public Transform puntoDeCarga;



    private void Start()
    {
        ActualizarEstados();
        OcultarMensaje();
    }

    private void Update()
    {
        if (jugadorEnRango && interaccionJugador != null)
        {
            ActualizarMensaje();

            if (Input.GetKeyDown(teclaInteraccion))
            {
                // Paso 1: dejar platos sucios
                if (estadoActual == 0 && interaccionJugador.LlevaObjetoConTag(tagPlatosSucios))
                {
                    interaccionJugador.EliminarObjetoTransportado();
                    estadoActual = 1;
                    ActualizarEstados();
                    Debug.Log("🧽 Platos sucios entregados. Estado: Sucio");
                }
                // Paso 2: limpiar fregadero
                else if (estadoActual == 1 && !interaccionJugador.EstaLlevandoObjeto())
                {
                    estadoActual = 2;
                    ActualizarEstados();
                    Debug.Log("🧼 Fregadero limpio. Listo para entregar platos limpios.");
                }
                // Paso 3: entregar platos limpios
                else if (estadoActual == 2 && !interaccionJugador.EstaLlevandoObjeto())
                {
                    GameObject platos = Instantiate(prefabPlatosLimpios, transform.position + Vector3.right * 1f, Quaternion.identity);
                    platos.transform.localScale = Vector3.one * 3f;
                    platos.tag = "PlatosLimpios";
                    interaccionJugador.RecogerObjeto(platos);
                    estadoActual = 0;
                    ActualizarEstados();
                    Debug.Log("🍽️ Platos limpios entregados al jugador.");
                }
            }
        }
    }

    private void ActualizarEstados()
    {
        estadoVacio.SetActive(estadoActual == 0);
        estadoSucio.SetActive(estadoActual == 1);
        estadoLimpio.SetActive(estadoActual == 2);
    }

    private void ActualizarMensaje()
    {
        if (mensajeUI == null) return;

        if (estadoActual == 0 && interaccionJugador.LlevaObjetoConTag(tagPlatosSucios))
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para dejar los platos sucios";
        }
        else if (estadoActual == 1 && !interaccionJugador.EstaLlevandoObjeto())
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para lavar los platos";
        }
        else if (estadoActual == 2 && !interaccionJugador.EstaLlevandoObjeto())
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para recoger platos limpios";
        }
        else
        {
            mensajeUI.text = "";
        }

        mensajeUI.gameObject.SetActive(mensajeUI.text != "");
    }

    private void OcultarMensaje()
    {
        if (mensajeUI != null)
        {
            mensajeUI.text = "";
            mensajeUI.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            jugador = other.gameObject;
            interaccionJugador = jugador.GetComponent<InteraccionJugador>();

            // 🔥 Asignar el punto de carga desde el jugador
            if (interaccionJugador != null)
            {
                puntoCarga = interaccionJugador.puntoCarga;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            jugador = null;
            interaccionJugador = null;
            OcultarMensaje();
        }
    }

    public bool EstaListoConPlatosLimpios() => estadoActual == 2;

    public void SacarPlatosLimpios()
    {
        if (estadoActual == 2 && prefabPlatosLimpios != null)
        {
            GameObject platos = Instantiate(prefabPlatosLimpios, puntoCarga.position, Quaternion.identity);
            platos.transform.SetParent(puntoCarga);
            platos.transform.localPosition = Vector3.zero;
            platos.transform.localRotation = Quaternion.identity;
            platos.transform.localScale = Vector3.one * 3f;

            platos.transform.localScale = Vector3.one * 3f;
            platos.tag = "PlatosLimpios";
            estadoActual = 0;
            ActualizarEstados();
        }
    }
}
