using UnityEngine;

public class DetectorDeTriggers : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("👤 El jugador entró en: " + other.name);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("👤 El jugador sigue dentro de: " + other.name);
    }
}
