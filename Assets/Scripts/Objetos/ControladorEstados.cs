using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ControladorEstados : MonoBehaviour
{
    [Header("Referencias de Estados")]
    [Tooltip("Arrastra aqu� el GameObject que representa el estado vac�o")]
    [SerializeField] private GameObject estadoVacio;
    [Tooltip("Arrastra aqu� el GameObject que representa el estado lleno")]
    [SerializeField] private GameObject estadoLleno;

    [Header("Configuraci�n")]
    [Tooltip("Nombre que se mostrar� en la UI")]
    public string nombreMostrado = "Objeto";

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
            Debug.LogError($"[ControladorEstados] Error en {gameObject.name}: Ambos estados (Vacio/Lleno) deben ser asignados en el inspector");
            enabled = false;
            return;
        }

        if (estadoVacio == estadoLleno)
        {
            Debug.LogError($"[ControladorEstados] Error en {gameObject.name}: Los estados Vacio y Lleno no pueden ser el mismo objeto");
            enabled = false;
        }
    }

    private void InicializarEstados()
    {
        estadoVacio.SetActive(!estaLleno);
        estadoLleno.SetActive(estaLleno);

        if (debugLogs)
        {
            Debug.Log($"[ControladorEstados] {gameObject.name} inicializado. Estado inicial: {(estaLleno ? "Lleno" : "Vac�o")}");
        }
    }

    public void AlternarEstado()
    {
        if (!enabled) return;

        estaLleno = !estaLleno;
        ActualizarEstados();

        if (debugLogs)
        {
            Debug.Log($"[ControladorEstados] {gameObject.name} alternado a {(estaLleno ? "Lleno" : "Vac�o")}", gameObject);
        }
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

    // M�todo para forzar un estado espec�fico (opcional)
    public void SetEstado(bool lleno)
    {
        estaLleno = lleno;
        ActualizarEstados();
    }

    void OnValidate()
    {
        if (estadoVacio != null && estadoLleno != null && estadoVacio != estadoLleno)
        {
            ActualizarEstados();
        }
    }
}