using UnityEngine;
using UnityEngine.UI;

public class ControladorVolumen : MonoBehaviour
{
    [SerializeField] private Slider sliderVolumen;
    [SerializeField] private AudioSource musica;

    private void Start()
    {
        if (sliderVolumen != null && musica != null)
        {
            sliderVolumen.value = musica.volume; // inicia con el volumen actual
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    private void CambiarVolumen(float valor)
    {
        musica.volume = valor;
    }
}
