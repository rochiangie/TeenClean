using UnityEngine;

public class CorazonTransparente : MonoBehaviour
{
    [Header("Transparencia de daño")]
    [SerializeField] private Transform tapaDaño; // hijo que tapa el corazón
    [SerializeField] private int saludMaxima = 100;
    [SerializeField] private int saludActual = 100;

    private Vector3 escalaOriginal;

    private void Start()
    {
        if (tapaDaño != null)
            escalaOriginal = tapaDaño.localScale;
    }

    public void RecibirDaño(int daño)
    {
        saludActual = Mathf.Clamp(saludActual - daño, 0, saludMaxima);
        ActualizarTransparencia();
    }

    private void ActualizarTransparencia()
    {
        if (tapaDaño != null)
        {
            float porcentaje = 1f - ((float)saludActual / saludMaxima);
            tapaDaño.localScale = new Vector3(porcentaje * escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        }
    }
}
