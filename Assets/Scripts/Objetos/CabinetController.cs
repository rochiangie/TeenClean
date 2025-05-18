// CabinetController.cs
using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    [Header("Platos")]
    public GameObject prefabPlatos;

    [Header("Configuración")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] public string tagObjetoRequerido = "Platos";
    [SerializeField] private AudioClip sonidoGuardar;

    private bool estaLleno = false;

    public bool EstaLleno() => estaLleno;

    private void Awake()
    {
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);
    }

    public void IntentarGuardarPlatos(InteraccionJugador jugador)
    {
        if (estaLleno || jugador == null) return;

        GameObject obj = jugador.ObjetoTransportado;
        if (obj == null || !obj.CompareTag(tagObjetoRequerido)) return;

        jugador.SoltarYDestruirObjeto();

        estaLleno = true;
        if (estadoVacio != null) estadoVacio.SetActive(false);
        if (estadoLleno != null) estadoLleno.SetActive(true);

        if (sonidoGuardar != null)
            AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);
    }

    public void SacarPlatosDelGabinete(InteraccionJugador jugador)
    {
        if (!estaLleno || jugador == null || jugador.EstaLlevandoObjeto() || prefabPlatos == null) return;

        Transform puntoCarga = jugador.puntoDeCarga;
        if (puntoCarga == null) return;

        GameObject nuevosPlatos = Instantiate(prefabPlatos, puntoCarga.position, Quaternion.identity);
        jugador.RecogerObjeto(nuevosPlatos);

        estaLleno = false;
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);
    }

    public string TagObjetoRequerido => tagObjetoRequerido;
    public GameObject PrefabPlatos => prefabPlatos;
}
