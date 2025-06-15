using UnityEngine;
using TMPro;

public class PanelInicio : MonoBehaviour
{
    [Header("Panel de Información")]
    [SerializeField] private GameObject panelInfo;
    [SerializeField] private TextMeshProUGUI textoInfo;
    [TextArea]
    [SerializeField]
    private string mensaje =
        "Presiona ENTER para acceder al menú principal\n" +
        "Presiona R para abrir el menú de tareas\n" +
        "Presiona F para atacar\n" +
        "Presiona la barra espaciadora para activar el escudo";

    [Header("Configuración")]
    [SerializeField] private float tiempoEnPantalla = 5f; // en segundos

    // Ya no mostramos el panel automáticamente en Start
    private void Start()
    {
        // MostrarPanel(); // 🔕 Desactivado
    }

    // Este método puede llamarse desde un botón o evento
    public void MostrarPanel()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(true);
            if (textoInfo != null)
            {
                textoInfo.text = mensaje;
            }
            // Ocultar después de cierto tiempo
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
