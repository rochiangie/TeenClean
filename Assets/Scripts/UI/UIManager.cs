using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panelInstrucciones;

    public void MostrarInstrucciones()
    {
        panelInstrucciones.SetActive(true);
    }

    public void OcultarInstrucciones()
    {
        panelInstrucciones.SetActive(false);
    }
}
