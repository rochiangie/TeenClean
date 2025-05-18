using UnityEngine;

public class PopUpSeguidor : MonoBehaviour
{
    public Transform objetivo; // El jugador
    public Vector3 offset;
    public RectTransform panelPopUp;
    public Camera camara;

    void Update()
    {
        if (objetivo != null && panelPopUp != null && camara != null)
        {
            Vector3 posicionPantalla = camara.WorldToScreenPoint(objetivo.position + offset);
            panelPopUp.position = posicionPantalla;
        }
    }

    public void MostrarPopUp()
    {
        if (panelPopUp != null)
            panelPopUp.gameObject.SetActive(true);
    }

    public void OcultarPopUp()
    {
        if (panelPopUp != null)
            panelPopUp.gameObject.SetActive(false);
    }
}
