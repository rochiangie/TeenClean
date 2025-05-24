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
    public GameObject platoslimpios; // Este es el prefab que se instancia

    [Header("UI")]
    public TextMeshProUGUI mensajeUI;

    [Header("Punto de carga del jugador")]
    public Transform puntoCarga; // Se asigna manualmente o desde el jugador

    private int estadoActual = 0; // 0: vacío, 1: sucio, 2: limpio
    private bool jugadorEnRango = false;
    private GameObject jugador;
    private InteraccionJugador interaccionJugador;

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
                    Debug.Log("🧼 Fregadero limpio. Listo para entregar platoslimpios.");
                }
                // Paso 3: entregar platoslimpios
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
            mensajeUI.text = $"Presiona {teclaInteraccion} para recoger platoslimpios";
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
            OcultarMensaje();
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
        if (prefab == null)
            Debug.LogWarning("❌ Error: prefabPlatosLimpios es null");
        if (puntoDeCarga == null)
            Debug.LogWarning("❌ Error: puntoDeCarga es null");
        if (jugador == null)
            Debug.LogWarning("❌ Error: interaccionJugador es null");

        if (prefab == null || puntoDeCarga == null || jugador == null)
        {
            Debug.LogWarning("❌ No se puede instanciar platos: faltan referencias.");
            return;
        }

        GameObject platos = Instantiate(prefab, puntoDeCarga.position, Quaternion.identity);
        platos.transform.SetParent(puntoDeCarga);
        platos.transform.localPosition = Vector3.zero;
        platos.transform.localRotation = Quaternion.identity;

        // ✅ usar la escala del prefab (ya seteada correctamente en Unity)
        platos.transform.localScale = prefab.transform.localScale;

        // ✅ establecer el tag correcto
        platos.tag = "PlatosLimpios";

        jugador.RecogerObjeto(platos);
    }

}
