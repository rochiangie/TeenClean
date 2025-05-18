// CabinetController.cs
using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    [Header("Platos")]
    public GameObject prefabPlatos1;

    [Header("Configuración")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] public string tagObjetoRequerido = "Platos";
    [SerializeField] private AudioClip sonidoGuardar;

    private bool estaLleno = false;

    public bool EstaLleno() => estaLleno;

    private void Awake()
    {
        ActualizarEstadoVisual(false);
    }

    public void IntentarGuardarPlatos(InteraccionJugador jugador)
    {
        if (estaLleno || jugador == null || prefabPlatos1 == null) return;

        GameObject obj = jugador.ObjetoTransportado;
        if (obj == null || !obj.CompareTag(tagObjetoRequerido)) return;

        Debug.Log("Guardando platos en el gabinete");

        jugador.SoltarYDestruirObjeto(); // destruye el objeto visual transportado

        estaLleno = true;
        ActualizarEstadoVisual(true);

        if (sonidoGuardar != null)
            AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);
    }


    public void SacarPlatosDelGabinete(InteraccionJugador jugador)
    {
        if (!estaLleno || jugador == null || jugador.EstaLlevandoObjeto() || prefabPlatos1 == null || jugador.puntoDeCarga == null) return;

        Debug.Log("Sacando platos del gabinete");

        GameObject nuevosPlatos = Instantiate(prefabPlatos1, jugador.puntoDeCarga.position, Quaternion.identity);
        jugador.RecogerObjeto(nuevosPlatos);

        estaLleno = false;
        ActualizarEstadoVisual(false);
    }


    private void ActualizarEstadoVisual(bool lleno)
    {
        if (estadoVacio != null) estadoVacio.SetActive(!lleno);
        if (estadoLleno != null) estadoLleno.SetActive(lleno);
    }

    public string TagObjetoRequerido => tagObjetoRequerido;
    public GameObject PrefabPlatos => prefabPlatos1;
}
