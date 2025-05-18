using UnityEngine;

public class SeguirJugadorUI : MonoBehaviour
{
    public Transform objetivo; // El jugador

    public Vector3 offset = new Vector3(0, -2.5f, 0);


    void LateUpdate()
    {
        if (objetivo != null)
            transform.position = objetivo.position + offset;
    }
}
