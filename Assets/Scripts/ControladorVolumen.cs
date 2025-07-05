using UnityEngine;
using UnityEngine.UI;

public class ControladorVolumen : MonoBehaviour
{
    [SerializeField] private Slider sliderVolumen;
    [SerializeField] private AudioSource musica;

    void Start()
    {
        if (sliderVolumen != null)
        {
            float volumenGuardado = PlayerPrefs.GetFloat("Volumen", 1f); // valor por defecto: 1
            sliderVolumen.value = volumenGuardado;
            CambiarVolumen(volumenGuardado); // aplicar el volumen

            sliderVolumen.onValueChanged.AddListener((valor) =>
            {
                CambiarVolumen(valor);
                PlayerPrefs.SetFloat("Volumen", valor); // guardar
                PlayerPrefs.Save(); // muy importante para que persista en la build
            });
        }
    }


    private void CambiarVolumen(float valor)
    {
        musica.volume = valor;
    }
}
