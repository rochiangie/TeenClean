using TMPro;
using UnityEngine;

public class MostrarHoraAlTocarReloj : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelHora;  // El panel que contiene el texto
    public TextMeshProUGUI textoHora;  // El texto dentro del panel

    public float tiempoVisible = 3f;
    private float tiempoOcultar = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (textoHora != null)
            {
                textoHora.text = System.DateTime.Now.ToString("HH:mm");
            }

            if (panelHora != null)
            {
                panelHora.SetActive(true);
                tiempoOcultar = Time.time + tiempoVisible;
            }
        }
    }

    private void Update()
    {
        if (panelHora != null && panelHora.activeSelf && Time.time >= tiempoOcultar)
        {
            panelHora.SetActive(false);
        }
    }
}
