using UnityEngine;

public class BotonDebugCompletarTareas : MonoBehaviour
{
    public void CompletarTodasLasTareas()
    {
        if (TareasManager.Instance != null)
        {
            TareasManager.Instance.CompletarTodasLasTareasDebug();
        }
        else
        {
            Debug.LogWarning("⚠️ TareasManager.Instance no encontrado.");
        }
    }
}
