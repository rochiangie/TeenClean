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

    public void SacarObjeto()
    {
        if (!estaLleno || prefabPlatosLimpios == null) return;

        Instantiate(prefabPlatosLimpios, transform.position + Vector3.right, Quaternion.identity);
        estaLleno = false;
        estadoVacio.SetActive(true);
        estadoLleno.SetActive(false);
    }
}
