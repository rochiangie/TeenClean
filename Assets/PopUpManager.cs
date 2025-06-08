using UnityEngine;
using TMPro; // Necesario para TextMeshProUGUI

public class PopUpManager : MonoBehaviour
{
    public GameObject popUpPanel; // Referencia al PanelPopUp en la UI
    public TextMeshProUGUI popUpText; // Referencia al TextoPopUp dentro del panel
    public float displayDuration = 3f; // Duraci�n del pop-up en segundos (si es temporal)

    private static PopUpManager instance; // Singleton para f�cil acceso
    private Coroutine hideCoroutine; // Para controlar la corrutina de ocultar

    void Awake()
    {
        // Implementaci�n de Singleton
        if (instance == null)
        {
            instance = this;
            // Optional: DontDestroyOnLoad(gameObject); // Si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
        }

        // Aseg�rate de que el pop-up est� oculto al inicio
        if (popUpPanel != null)
        {
            popUpPanel.SetActive(false);
        }
    }

    // M�todo para mostrar el pop-up
    public static void ShowPopUp(string message, bool temporary = true)
    {
        if (instance == null)
        {
            Debug.LogError("PopUpManager instance is not found. Make sure it's in the scene.");
            return;
        }

        if (instance.popUpPanel != null && instance.popUpText != null)
        {
            // Detiene cualquier corrutina de ocultamiento previa para el nuevo pop-up
            if (instance.hideCoroutine != null)
            {
                instance.StopCoroutine(instance.hideCoroutine);
            }

            instance.popUpText.text = message; // Asigna el texto
            instance.popUpPanel.SetActive(true); // Hace visible el panel

            if (temporary)
            {
                // Inicia una corrutina para ocultar el pop-up despu�s de un tiempo
                instance.hideCoroutine = instance.StartCoroutine(instance.HidePopUpAfterDelay(instance.displayDuration));
            }
        }
        else
        {
            Debug.LogError("PopUpPanel or PopUpText is not assigned in PopUpManager.");
        }
    }

    // M�todo para ocultar el pop-up manualmente
    public static void HidePopUp()
    {
        if (instance == null) return;

        if (instance.hideCoroutine != null)
        {
            instance.StopCoroutine(instance.hideCoroutine);
            instance.hideCoroutine = null;
        }

        if (instance.popUpPanel != null)
        {
            instance.popUpPanel.SetActive(false);
        }
    }

    // Corrutina para ocultar el pop-up despu�s de un retraso
    private System.Collections.IEnumerator HidePopUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePopUp();
    }
}