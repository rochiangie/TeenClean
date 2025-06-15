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
    public GameObject platoslimpios;

    [Header("UI")]
    [SerializeField] private GameObject panelPopUp;
    [SerializeField] private TextMeshProUGUI textoPopUp;

    [Header("Punto de carga del jugador")]
    public Transform puntoCarga;

    private int estadoActual = 0; // 0: vacío, 1: sucio, 2: limpio
    private bool jugadorEnRango = false;
    private GameObject jugador;
    private InteraccionJugador interaccionJugador;

    private void Start()
    {
        ActualizarEstados();
        OcultarPopUp();
    }

    private void Update()
    {
        if (jugadorEnRango && interaccionJugador != null)
        {
            ActualizarMensaje();

            if (Input.GetKeyDown(teclaInteraccion))
            {
                if (estadoActual == 0 && interaccionJugador.LlevaObjetoConTag(tagPlatosSucios))
                {
                    interaccionJugador.EliminarObjetoTransportado();
                    estadoActual = 1;
                    ActualizarEstados();
                    Debug.Log("🧽 Platos sucios entregados. Estado: Sucio");
                }
                else if (estadoActual == 1 && !interaccionJugador.EstaLlevandoObjeto())
                {
                    estadoActual = 2;
                    ActualizarEstados();
                    Debug.Log("🧼 Fregadero limpio. Listo para entregar platoslimpios.");
                }
                else if (estadoActual == 2 && !interaccionJugador.EstaLlevandoObjeto())
                {
                    InstanciarYAsignarPlatos(platoslimpios, puntoCarga, interaccionJugador);
                    estadoActual = 0;
                    ActualizarEstados();
                    Debug.Log("🍽️ platoslimpios entregados al jugador.");
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
        if (textoPopUp == null || panelPopUp == null || interaccionJugador == null)
            return;

        if (estadoActual == 0 && interaccionJugador.LlevaObjetoConTag(tagPlatosSucios))
        {
            MostrarPopUp($"Presiona {teclaInteraccion} para dejar los platos sucios");
        }
        else if (estadoActual == 1 && !interaccionJugador.EstaLlevandoObjeto())
        {
            MostrarPopUp($"Presiona {teclaInteraccion} para lavar los platos");
        }
        else if (estadoActual == 2 && !interaccionJugador.EstaLlevandoObjeto())
        {
            MostrarPopUp($"Presiona {teclaInteraccion} para recoger los platos limpios");
        }
        else
        {
            OcultarPopUp();
        }
    }

    private void MostrarPopUp(string mensaje)
    {
        panelPopUp.SetActive(true);
        textoPopUp.text = mensaje;
    }

    private void OcultarPopUp()
    {
        panelPopUp.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            jugador = other.gameObject;
            interaccionJugador = jugador.GetComponent<InteraccionJugador>();

            if (interaccionJugador != null && interaccionJugador.puntoCarga != null)
            {
                puntoCarga = interaccionJugador.puntoCarga;
            }
            else
            {
                Debug.LogWarning("⚠️ interaccionJugador o su puntoCarga es null");
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
            OcultarPopUp();
        }
    }

    public bool EstaListoConPlatosLimpios() => estadoActual == 2;

    public void SacarPlatosLimpios()
    {
        if (estadoActual == 2 && platoslimpios != null && interaccionJugador != null)
        {
            InstanciarYAsignarPlatos(platoslimpios, puntoCarga, interaccionJugador);
            estadoActual = 0;
            ActualizarEstados();
        }
    }

    private void InstanciarYAsignarPlatos(GameObject prefab, Transform puntoDeCarga, InteraccionJugador jugador)
    {
        if (prefab == null || puntoDeCarga == null || jugador == null)
        {
            Debug.LogWarning("❌ No se puede instanciar platos: faltan referencias.");
            return;
        }

        GameObject platos = Instantiate(prefab, puntoDeCarga.position, Quaternion.identity);
        platos.transform.SetParent(puntoDeCarga);
        platos.transform.localPosition = Vector3.zero;
        platos.transform.localRotation = Quaternion.identity;
        platos.transform.localScale = prefab.transform.localScale;
        platos.tag = "PlatosLimpios";

        jugador.RecogerObjeto(platos);
    }
}
