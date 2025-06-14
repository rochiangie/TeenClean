using UnityEngine;

public class GabinetePlatos : MonoBehaviour
{
    public GameObject estadoVacio;
    public GameObject estadoLleno;
    public GameObject prefabObjetoLleno;

    private bool estaLleno = false;
    private TareasManager tareasManager;



    void Awake()
    {
        tareasManager = FindObjectOfType<TareasManager>();
        estadoVacio?.SetActive(true);
        estadoLleno?.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        if (estaLleno || !objeto.CompareTag("PlatosLimpios")) return false;

        Destroy(objeto);
        estaLleno = true;
        estadoVacio.SetActive(false);
        estadoLleno.SetActive(true);

        tareasManager?.CompletarTarea("Platos");
        return true;
    }

    /*public void SacarObjeto(Transform puntoDeCarga, InteraccionJugador jugador)
    {
        if (!estaLleno || prefabObjetoLleno == null) return;

        GameObject nuevo = Instantiate(prefabObjetoLleno, puntoDeCarga.position, Quaternion.identity);
        nuevo.transform.SetParent(puntoDeCarga);
        nuevo.transform.localPosition = Vector3.zero;
        nuevo.transform.localRotation = Quaternion.identity;
        nuevo.transform.localScale = Vector3.one * 0.5f;
        nuevo.tag = "PlatosLimpios";

        jugador.RecogerObjeto(nuevo);

        estaLleno = false;
        estadoVacio.SetActive(true);
        estadoLleno.SetActive(false);
    }

    public bool EstaLleno()
    {
        return estaLleno;
    }
    */
}