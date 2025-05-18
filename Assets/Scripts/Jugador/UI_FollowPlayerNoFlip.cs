using UnityEngine;

public class UI_FollowPlayerNoFlip : MonoBehaviour
{
    public Transform objetivo; // Asign� al jugador en el inspector
    public Vector3 offset = new Vector3(0, 2f, 0); // Ajust� esto
    public bool seguirX = true;
    public bool seguirY = true;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 nuevaPos = transform.position;

        if (seguirX) nuevaPos.x = objetivo.position.x + offset.x;
        if (seguirY) nuevaPos.y = objetivo.position.y + offset.y;

        transform.position = nuevaPos;
    }
}
