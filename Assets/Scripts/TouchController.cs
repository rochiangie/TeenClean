using UnityEngine;

public class TouchController : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0) // Si hay al menos un toque
        {
            Touch touch = Input.GetTouch(0); // Primer toque

            if (touch.phase == TouchPhase.Began)
            {
                //Debug.Log("Toque iniciado en: " + touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // Mover un objeto (ej: un personaje)
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                transform.position = new Vector3(touchPosition.x, touchPosition.y, 0);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                //Debug.Log("Toque finalizado");
            }
        }
    }
}