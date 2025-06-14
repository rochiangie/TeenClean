using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ControladorEstados : MonoBehaviour
{
    [Header("Referencias de Estados")]
    [SerializeField] private GameObject estadoVacio;
    [SerializeField] private GameObject estadoLleno;

    [Header("Configuraci�n")]
    public string nombreMostrado = "Ba�era";

    [Header("Debug")]
    [SerializeField] private bool estaLleno = false;
    [SerializeField] private bool debugLogs = true;

    void Start()
    {
        ValidarReferencias();
        InicializarEstados();
    }

    private void ValidarReferencias()
    {
        if (estadoVacio == null || estadoLleno == null)
        {
            //Debug.LogError($"[ControladorEstados] Error en {gameObject.name}: Ambos estados deben ser asignados.");
            enabled = false;
            return;
        }

        if (estadoVacio == estadoLleno)
        {
            //Debug.LogError($"[ControladorEstados] Error en {gameObject.name}: Los estados no pueden ser el mismo objeto.");
            enabled = false;
        }
    }

    private void InicializarEstados()
    {
        estadoVacio.SetActive(!estaLleno);
        estadoLleno.SetActive(estaLleno);

        /*if (debugLogs)
            Debug.Log($"[ControladorEstados] {nombreMostrado} inicializado como {(estaLleno ? "Lleno" : "Vac�o")}");*/
    }

    public void AlternarEstado()
    {
        if (!enabled) return;

        estaLleno = !estaLleno;
        ActualizarEstados();

        /*if (debugLogs)
            Debug.Log($"[ControladorEstados] {nombreMostrado} ahora est� {(estaLleno ? "Lleno" : "Vac�o")}");*/
    }

    private void ActualizarEstados()
    {
        estadoVacio.SetActive(!estaLleno);
        estadoLleno.SetActive(estaLleno);
    }


    public string ObtenerNombreEstado()
    {
        return $"{nombreMostrado} ({(estaLleno ? "Lleno" : "Vac�o")})";
    }

    void OnValidate()
    {
        if (estadoVacio != null && estadoLleno != null && estadoVacio != estadoLleno)
        {
            ActualizarEstados();
        }
    }
}
