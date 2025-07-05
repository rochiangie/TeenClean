using UnityEngine;
using UnityEngine.UI;

public class ControladorVolumen : MonoBehaviour
{
    [SerializeField] private Slider sliderVolumen;

    void Start()
    {
        if (sliderVolumen != null)
        {
            float volumenGuardado = PlayerPrefs.GetFloat("volumenMusica", 1f); // mismo nombre que en AudioManager
            sliderVolumen.value = volumenGuardado;

            // Aplicar volumen inicial al AudioManager
            AudioManager.Instance?.CambiarVolumen(volumenGuardado);

            sliderVolumen.onValueChanged.AddListener((valor) =>
            {
                AudioManager.Instance?.CambiarVolumen(valor);
                PlayerPrefs.SetFloat("volumenMusica", valor);
                PlayerPrefs.Save();
            });
        }
    }
}
