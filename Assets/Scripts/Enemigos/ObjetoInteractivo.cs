using UnityEngine;

public class ObjetoInteractivo : MonoBehaviour
{
    [TextArea(3, 10)] // Para que el texto sea más fácil de editar en el Inspector
    public string popUpMessage = "¡Has interactuado con el Objeto!";
    public bool hideAfterInteraction = true; // Si quieres que el objeto se oculte después de interactuar

    // --- Opción 1: Detección por Click del Mouse (para objetos sin Rigidbody o si el click es suficiente) ---
    void OnMouseDown()
    {
        // Solo se activa si el mouse hace click directamente sobre el collider de este objeto
        Debug.Log($"Click en {gameObject.name}");
        PopUpManager.ShowPopUp(popUpMessage); // Mostrar el pop-up
        if (hideAfterInteraction)
        {
            gameObject.SetActive(false); // Ocultar el objeto después de la interacción
        }
    }

    // --- Opción 2: Detección por Colisión o Trigger (para interacción con el jugador, etc.) ---
    // Asegúrate de que tanto este objeto como el objeto que colisiona tengan Colliders y al menos uno tenga Rigidbody.
    // Si este collider es 'Is Trigger', usa OnTriggerEnter. Si no, usa OnCollisionEnter.

    void OnTriggerEnter(Collider other) // Para 3D. Si es 2D, usa OnTriggerEnter2D(Collider2D other)
    {
        // Puedes filtrar qué objeto lo activó (ej. solo el "Player")
        if (other.CompareTag("Player")) // Asegúrate de que tu Player GameObject tenga el tag "Player"
        {
            Debug.Log($"Colisión con {gameObject.name} por {other.name}");
            PopUpManager.ShowPopUp(popUpMessage);
            if (hideAfterInteraction)
            {
                gameObject.SetActive(false);
            }
        }
    }

    
     void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag("Player"))
         {
             Debug.Log($"Colisión 2D con {gameObject.name} por {other.name}");
             PopUpManager.ShowPopUp(popUpMessage);
             if (hideAfterInteraction)
             {
                 gameObject.SetActive(false);
             }
         }
     }
}