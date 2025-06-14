using UnityEngine;

public class GabineteRopa : MonoBehaviour
{
    [Header("Estados Visuales")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;
    public GameObject prefabObjetoLleno;

    private bool estaLleno = false;
    private TareasManager tareasManager;

    void Awake()
    {
        tareasManager = FindObjectOfType<TareasManager>();
        if (tareasManager == null)
            Debug.LogError("🚨 No se encontró el TareasManager en la escena.");

        estadoVacio?.SetActive(true);
        estadoLleno?.SetActive(false);
    }

    public bool IntentarGuardar(GameObject objeto)
    {
        Debug.Log("🧪 Intentando guardar objeto...");

        if (estaLleno)
        {
            Debug.Log("❌ No se puede guardar: gabinete lleno.");
            return false;
        }

        if (!objeto.CompareTag("RopaLimpia"))
        {
            Debug.Log($"❌ Tag incorrecto: {objeto.tag} (esperado: RopaLimpia)");
            return false;
        }

        Destroy(objeto);
        estaLleno = true;

        if (estadoVacio != null && estadoLleno != null)
        {
            estadoVacio.SetActive(false);
            estadoLleno.SetActive(true);
            Debug.Log("✅ Estado del gabinete cambiado a 'Lleno'");
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignaron estadoVacio o estadoLleno en el Inspector.");
        }

        if (tareasManager != null)
        {
            tareasManager.CompletarTarea("Ropa");
            Debug.Log("🎯 Tarea de ropa completada");
        }

        return true;
    }


    public void SacarObjeto(Transform puntoDeCarga, InteraccionJugador jugador)
    {
        if (!estaLleno || prefabObjetoLleno == null || jugador == null)
        {
            Debug.LogWarning("⚠️ No se puede sacar objeto: condiciones inválidas.");
            return;
        }

        GameObject nuevo = Instantiate(prefabObjetoLleno, puntoDeCarga.position, Quaternion.identity);
        nuevo.transform.SetParent(puntoDeCarga);
        nuevo.transform.localPosition = Vector3.zero;
        nuevo.transform.localRotation = Quaternion.identity;
        nuevo.transform.localScale = Vector3.one * 0.5f;
        nuevo.tag = "RopaLimpia";

        jugador.RecogerObjeto(nuevo);

        estaLleno = false;
        estadoVacio?.SetActive(true);
        estadoLleno?.SetActive(false);

        Debug.Log("🧺 Ropa sacada del gabinete");
    }

    public bool EstaLleno()
    {
        return estaLleno;
    }

}
