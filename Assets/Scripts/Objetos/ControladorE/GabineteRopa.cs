using UnityEngine;

public class GabineteRopa : MonoBehaviour
{
    public GameObject estadoVacio;
    public GameObject estadoLleno;
    public string tagRequerido = "RopaLimpia";
    private int ropaGuardada = 0;
    public int cantidadParaCompletar = 2;

    private TareasManager tareasManager;
    private bool estaLleno = false;

    [SerializeField] private GameObject prefabObjetoLleno;

    private void Awake()
    {
        tareasManager = FindObjectOfType<TareasManager>();
        if (estadoVacio) estadoVacio.SetActive(true);
        if (estadoLleno) estadoLleno.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        if (!objeto.CompareTag(tagRequerido)) return false;

        ropaGuardada++;
        Destroy(objeto);
        if (ropaGuardada >= cantidadParaCompletar && tareasManager != null)
        {
            tareasManager.CompletarTarea("Ropa");
        }

        return true;
    }

    public void SacarObjeto(Transform puntoDeCarga, InteraccionJugador jugador)
    {
        if (!estaLleno || prefabObjetoLleno == null || puntoDeCarga == null || jugador == null)
        {
            Debug.LogWarning("❌ No se puede sacar objeto del gabinete de ropa: faltan referencias.");
            return;
        }

        Vector3 posicionDelante = puntoDeCarga.position + puntoDeCarga.right * (jugador.transform.localScale.x > 0 ? 1f : -1f);

        GameObject objeto = Instantiate(prefabObjetoLleno, posicionDelante, Quaternion.identity);
        objeto.transform.SetParent(puntoDeCarga);
        objeto.transform.localPosition = Vector3.zero;
        objeto.transform.localRotation = Quaternion.identity;

        objeto.transform.localScale = Vector3.one; // escala normal
        objeto.tag = "RopaLimpia";

        jugador.RecogerObjeto(objeto);

        estaLleno = false;
        estadoVacio.SetActive(true);
        estadoLleno.SetActive(false);

        Debug.Log($"✅ Ropa entregada al jugador desde el gabinete.");
    }

}
