using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogoInteractivo : MonoBehaviour
{
    public TextMeshProUGUI textoDialogo;

    public Button botonSi;
    public Button botonNo;
    public Button botonEntendi;
    public Button botonNoEntendi;
    public Button botonCerrar;

    private void Start()
    {
        // Asignamos eventos a botones
        botonSi.onClick.AddListener(CerrarDialogo);
        botonNo.onClick.AddListener(MostrarDialogo2);
        botonEntendi.onClick.AddListener(CerrarDialogo);
        botonNoEntendi.onClick.AddListener(MostrarDialogo3);
        botonCerrar.onClick.AddListener(CerrarDialogo);

        MostrarDialogo1(); // lo mostramos al iniciar para probar
    }

    public void MostrarDialogo1()
    {
        textoDialogo.text = "¡Hija! ¿Cómo estás? ¿Hiciste los quehaceres de la casa?";

        botonSi.gameObject.SetActive(true);
        botonNo.gameObject.SetActive(true);
        botonEntendi.gameObject.SetActive(false);
        botonNoEntendi.gameObject.SetActive(false);
        botonCerrar.gameObject.SetActive(true);
    }

    public void MostrarDialogo2()
    {
        textoDialogo.text = "Tenés que limpiar tu cuarto, lavar los platos y hacer la tarea.";

        botonSi.gameObject.SetActive(false);
        botonNo.gameObject.SetActive(false);
        botonEntendi.gameObject.SetActive(true);
        botonNoEntendi.gameObject.SetActive(true);
        botonCerrar.gameObject.SetActive(true);
    }

    public void MostrarDialogo3()
    {
        textoDialogo.text = "Primero levantá tu ropa, después lavá los platos y por último hacé la tarea. ¿Ahora sí?";

        botonSi.gameObject.SetActive(false);
        botonNo.gameObject.SetActive(false);
        botonEntendi.gameObject.SetActive(true);
        botonNoEntendi.gameObject.SetActive(false);
        botonCerrar.gameObject.SetActive(true);
    }

    public void CerrarDialogo()
    {
        gameObject.SetActive(false);
    }
}
