using UnityEngine;
using UnityEngine.SceneManagement;

public class FinDelJuego : MonoBehaviour
{
    // Esta funci�n se llama al presionar el bot�n OK en el panel de victoria
    public void IrAEscenaEnding()
    {
        SceneManager.LoadScene("Ending");
    }
}
