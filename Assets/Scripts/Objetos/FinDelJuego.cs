using UnityEngine;
using UnityEngine.SceneManagement;

public class FinDelJuego : MonoBehaviour
{
    // Esta función se llama al presionar el botón OK en el panel de victoria
    public void IrAEscenaEnding()
    {
        SceneManager.LoadScene("Ending");
    }
}
