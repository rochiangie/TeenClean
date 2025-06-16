using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashToMenu : MonoBehaviour
{
    public float duracionSplash = 3f;

    private void Start()
    {
        Invoke("VolverAlMenu", duracionSplash);
    }

    private void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
