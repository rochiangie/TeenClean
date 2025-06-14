using UnityEngine;

public class GabinetePlatos : MonoBehaviour
{
    [Header("Estados")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    [Header("Configuración")]
    public string tagRequerido = "PlatosLimpios";
    public AudioClip sonidoGuardar;

    private bool estaLleno = false;
    private TareasManager tareasManager;

    [SerializeField] private GameObject prefabObjetoLleno;

    private void Awake()
    {
        tareasManager = FindObjectOfType<TareasManager>();
        if (estadoVacio) estadoVacio.SetActive(true);
        if (estadoLleno) estadoLleno.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        if (estaLleno || !objeto.CompareTag(tagRequerido)) return false;

        Destroy(objeto);
        estaLleno = true;

        if (estadoVacio) estadoVacio.SetActive(false);
        if (estadoLleno) estadoLleno.SetActive(true);

        if (sonidoGuardar)
            AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);

        if (tareasManager != null)
            tareasManager.CompletarTarea("Platos");

        return true;
    }

    public void SacarObjeto(Transform puntoDeCarga, InteraccionJugador jugador)
    {
        if (!estaLleno || prefabObjetoLleno == null || puntoDeCarga == null || jugador == null)
        {
            Debug.LogWarning("❌ No se puede sacar objeto del gabinete de platos: faltan referencias.");
            return;
        }

        Vector3 posicionDelante = puntoDeCarga.position + puntoDeCarga.right * (jugador.transform.localScale.x > 0 ? 1f : -1f);

        GameObject objeto = Instantiate(prefabObjetoLleno, posicionDelante, Quaternion.identity);
        objeto.transform.SetParent(puntoDeCarga);
        objeto.transform.localPosition = Vector3.zero;
        objeto.transform.localRotation = Quaternion.identity;

        objeto.transform.localScale = Vector3.one * 0.5f;
        objeto.tag = "PlatosLimpios";

        jugador.RecogerObjeto(objeto);

        estaLleno = false;
        estadoVacio.SetActive(true);
        estadoLleno.SetActive(false);

        Debug.Log($"✅ Platos entregados al jugador desde el gabinete.");
    }

}
