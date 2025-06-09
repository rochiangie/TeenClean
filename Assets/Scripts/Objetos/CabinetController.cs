using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    [Header("Configuración")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] private string tagObjetoRequerido = "PlatosLimpios"; // o "Tarea"
    [SerializeField] private GameObject prefabObjetoLleno;
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
        if (estaLleno || objeto.tag != tagObjetoRequerido)
        {
            Debug.Log($"❌ No se pudo guardar: {objeto.name} no coincide con el tag requerido ({tagObjetoRequerido}).");
            return false;
        }

        estaLleno = true;
        estadoVacio.SetActive(false);
        estadoLleno.SetActive(true);

        if (sonidoGuardar) AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);

        Destroy(objeto);
        Debug.Log($"✅ {objeto.name} guardado correctamente en el gabinete.");
        return true;
    }

    public void SacarObjeto(Transform puntoDeCarga, InteraccionJugador jugador)
    {
        if (!estaLleno || prefabObjetoLleno == null || puntoDeCarga == null || jugador == null)
        {
            Debug.LogWarning("❌ No se puede sacar objeto del gabinete: faltan referencias.");
            return;
        }

        // Calcular la posición delante del jugador
        Vector3 posicionDelante = puntoDeCarga.position + puntoDeCarga.right * (jugador.transform.localScale.x > 0 ? 1f : -1f);

        GameObject objeto = Instantiate(prefabObjetoLleno, posicionDelante, Quaternion.identity);
        objeto.transform.SetParent(puntoDeCarga);
        objeto.transform.localPosition = Vector3.zero;
        objeto.transform.localRotation = Quaternion.identity;

        // Ajustar la escala según el tag
        if (tagObjetoRequerido == "Tarea")
        {
            objeto.transform.localScale = Vector3.one * 10f;
        }
        else if (tagObjetoRequerido == "PlatosLimpios")
        {
            objeto.transform.localScale = Vector3.one * 0.5f;
        }

        objeto.tag = tagObjetoRequerido;

        jugador.RecogerObjeto(objeto);

        estaLleno = false;
        estadoVacio.SetActive(true);
        estadoLleno.SetActive(false);
        Debug.Log($"✅ {tagObjetoRequerido} entregado al jugador desde el gabinete.");
    }
}
