using UnityEngine;

public class DebugActivador : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("🟢 Panel de instrucciones ACTIVADO por alguien", gameObject);
    }

    private void Start()
    {
        Debug.Log("🔁 Start del panel de instrucciones ejecutado", gameObject);
    }
}
