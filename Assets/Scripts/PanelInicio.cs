using UnityEngine;
using TMPro;

public class PanelInicio : MonoBehaviour
{
    [Header("Panel de Informaci�n")]
    [SerializeField] private GameObject panelInfo;
    [SerializeField] private TextMeshProUGUI textoInfo;
    [TextArea]
    [SerializeField]
    private string mensaje =
        "Presiona ENTER para acceder al men� principal\n" +
        "Presiona R para abrir el men� de tareas\n" +
        "Presiona F para atacar\n" +
        "Presiona la barra espaciadora para activar el escudo";

    [Header("Configuraci�n")]
    [SerializeField] private float tiempoEnPantalla = 5f; // en segundos

    private void Start()
    {
        MostrarPanel();
    }

    private void MostrarPanel()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(true);
            if (textoInfo != null)
            {
                textoInfo.text = mensaje;
            }
            // Ocultar despu�s de cierto tiempo
            Invoke(nameof(OcultarPanel), tiempoEnPantalla);
        }
    }

    private void OcultarPanel()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(false);
        }
    }
}
