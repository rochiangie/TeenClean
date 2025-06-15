using UnityEngine;

public class CorazonTransparente : MonoBehaviour
{
    [Header("Transparencia de da�o")]
    [SerializeField] private Transform tapaDa�o; // hijo que tapa el coraz�n
    [SerializeField] private int saludMaxima = 100;
    [SerializeField] private int saludActual = 100;

    private Vector3 escalaOriginal;

    private void Start()
    {
        if (tapaDa�o != null)
            escalaOriginal = tapaDa�o.localScale;
    }

    public void RecibirDa�o(int da�o)
    {
        saludActual = Mathf.Clamp(saludActual - da�o, 0, saludMaxima);
        ActualizarTransparencia();
    }

    private void ActualizarTransparencia()
    {
        if (tapaDa�o != null)
        {
            float porcentaje = 1f - ((float)saludActual / saludMaxima);
            tapaDa�o.localScale = new Vector3(porcentaje * escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        }
    }
}
