using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    [Header("Configuración")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] public string tagObjetoRequerido = "PlatosLimpios";
    [SerializeField] private GameObject prefabPlatosLimpios;
    [SerializeField] private AudioClip sonidoGuardar;

    private bool estaLleno = false;

    public bool EstaLleno() => estaLleno;
    public string TagObjetoRequerido => tagObjetoRequerido;

    private void Awake()
    {
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        if (estaLleno || objeto.tag != tagObjetoRequerido) return false;

        estaLleno = true;
        estadoVacio.SetActive(false);
        estadoLleno.SetActive(true);

        if (sonidoGuardar) AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);

        Destroy(objeto);
        return true;
    }

    // ✅ Nuevo método que recibe la mano del jugador y el script del jugador
    public void SacarObjeto(Transform puntoDeCarga, InteraccionJugador jugador)
    {
        if (!estaLleno || prefabPlatosLimpios == null || puntoDeCarga == null || jugador == null)
        {
            Debug.LogWarning("❌ No se puede sacar objeto del cabinet: faltan referencias.");
            return;
        }

        GameObject platos = Instantiate(prefabPlatosLimpios, puntoDeCarga.position, Quaternion.identity);
        platos.transform.SetParent(puntoDeCarga);
        platos.transform.localPosition = Vector3.zero;
        platos.transform.localRotation = Quaternion.identity;
        platos.transform.localScale = Vector3.one * 0.5f;
        platos.tag = "PlatosLimpios";

        jugador.RecogerObjeto(platos);

        estaLleno = false;
        estadoVacio.SetActive(true);
        estadoLleno.SetActive(false);
    }
}
