using TMPro;
using UnityEngine;

public class MostrarHoraAlTocarReloj : MonoBehaviour
{
    public TextMeshProUGUI textoHora;
    public float tiempoVisible = 3f;

    private float tiempoOcultar = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textoHora.text = System.DateTime.Now.ToString("HH:mm");
            textoHora.gameObject.SetActive(true);
            tiempoOcultar = Time.time + tiempoVisible;
        }
    }

    void Update()
    {
        if (textoHora != null && Time.time >= tiempoOcultar)
        {
            textoHora.gameObject.SetActive(false);
        }
    }
}
