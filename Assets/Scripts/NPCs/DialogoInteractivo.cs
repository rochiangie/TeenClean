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
        textoDialogo.text = "Hola hija! Espero estés bien\n¿Hiciste tus tasks de hoy?";

        botonSi.gameObject.SetActive(true);
        botonNo.gameObject.SetActive(true);
        botonEntendi.gameObject.SetActive(false);
        botonNoEntendi.gameObject.SetActive(false);
        botonCerrar.gameObject.SetActive(true);
    }

    public void MostrarDialogo2()
    {
        textoDialogo.text = "Tienes que lavar la ropa y los platos para luego guardarlos en su lugar, hacé la tarea y guardala.\nTerminá haciendo la cama.";

        botonSi.gameObject.SetActive(false);
        botonNo.gameObject.SetActive(false);
        botonEntendi.gameObject.SetActive(true);
        botonNoEntendi.gameObject.SetActive(true);
        botonCerrar.gameObject.SetActive(true);
    }

    public void MostrarDialogo3()
    {
        textoDialogo.text = "Placeholder: más detalles de las tareas que debe hacer.";

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
