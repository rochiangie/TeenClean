using UnityEngine;

public class SinkLimpiezaController : MonoBehaviour
{
    public GameObject estadoVacio;
    public GameObject estadoSucio;
    public GameObject estadoLimpio;

    [Header("Configuración")]
    [SerializeField] private string tagObjetoSucio = "Platos";
    [SerializeField] private string tagObjetoLimpio = "PlatosLimpios";
    [SerializeField] private GameObject prefabObjetoLimpio;
    [SerializeField] private Transform puntoSpawnLimpio;
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] private float tiempoLavado = 2f;

    private enum EstadoSink { Vacio, Sucio, Limpio }
    private EstadoSink estadoActual = EstadoSink.Vacio;

    private void Awake()
    {
        CambiarEstado(EstadoSink.Vacio);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(teclaInteraccion)) return;

        InteraccionJugador jugador = GameObject.FindWithTag("Player")?.GetComponent<InteraccionJugador>();
        if (jugador == null) return;

        if (estadoActual == EstadoSink.Vacio && jugador.EstaLlevandoObjeto())
        {
            GameObject obj = jugador.ObjetoTransportado;
            if (obj != null && obj.CompareTag(tagObjetoSucio))
            {
                jugador.SoltarYDestruirObjeto();
                CambiarEstado(EstadoSink.Sucio);
                Invoke(nameof(TerminarLavado), tiempoLavado);
            }
        }
        else if (estadoActual == EstadoSink.Limpio && !jugador.EstaLlevandoObjeto())
        {
            InstanciarObjetoLimpio();
            CambiarEstado(EstadoSink.Vacio);
        }
    }

    private void TerminarLavado()
    {
        CambiarEstado(EstadoSink.Limpio);
    }

    private void InstanciarObjetoLimpio()
    {
        if (prefabObjetoLimpio == null) return;

        Vector3 posicion = puntoSpawnLimpio != null ? puntoSpawnLimpio.position : transform.position + Vector3.right;
        Instantiate(prefabObjetoLimpio, posicion, Quaternion.identity);
    }

    private void CambiarEstado(EstadoSink nuevoEstado)
    {
        estadoVacio?.SetActive(nuevoEstado == EstadoSink.Vacio);
        estadoSucio?.SetActive(nuevoEstado == EstadoSink.Sucio);
        estadoLimpio?.SetActive(nuevoEstado == EstadoSink.Limpio);
        estadoActual = nuevoEstado;
    }
}
