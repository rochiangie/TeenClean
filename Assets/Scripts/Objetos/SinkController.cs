using UnityEngine;

public class SinkController : MonoBehaviour
{
    public GameObject estadoVacio;
    public GameObject estadoSucio;
    public GameObject estadoLimpio;

    public KeyCode teclaInteraccion = KeyCode.E;
    public string tagPlatosSucios = "Platos";
    public GameObject prefabPlatosLimpios;

    private int estadoActual = 0; // 0: vacío, 1: sucio, 2: limpio
    private bool jugadorEnRango = false;
    private GameObject jugador;
    private InteraccionJugador interaccionJugador;

    private void Start()
    {
        ActualizarEstados();
    }

    private void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(teclaInteraccion) && interaccionJugador != null)
        {
            // Si está vacío y el jugador tiene platos sucios → los deja
            if (estadoActual == 0 && interaccionJugador.LlevaObjetoConTag(tagPlatosSucios))
            {
                interaccionJugador.EliminarObjetoTransportado();
                estadoActual = 1;
                ActualizarEstados();
            }
            // Si está sucio → se limpia
            else if (estadoActual == 1)
            {
                estadoActual = 2;
                ActualizarEstados();
                Debug.Log("🧼 El sink ahora está limpio y listo para entregar platos limpios.");
            }
            // Si está limpio → entrega platos limpios si el jugador no lleva nada
            else if (estadoActual == 2 && !interaccionJugador.EstaLlevandoObjeto())
            {
                GameObject platos = Instantiate(prefabPlatosLimpios, transform.position + Vector3.right, Quaternion.identity);
                if (platos != null)
                {
                    interaccionJugador.RecogerObjeto(platos); // Esto asume que tenés este método público
                    Debug.Log("🍽️ Se entregaron platos limpios al jugador.");
                    estadoActual = 0;
                    ActualizarEstados();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            jugador = other.gameObject;
            interaccionJugador = jugador.GetComponent<InteraccionJugador>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            jugador = null;
            interaccionJugador = null;
        }
    }

    // Para sacar platos limpios desde otro script
    public bool EstaListoConPlatosLimpios()
    {
        return estadoActual == 2;
    }

    public void SacarPlatosLimpios()
    {
        if (estadoActual == 2 && prefabPlatosLimpios != null)
        {
            Instantiate(prefabPlatosLimpios, transform.position + Vector3.right, Quaternion.identity);
            estadoActual = 0;
            ActualizarEstados();
        }
    }
}
