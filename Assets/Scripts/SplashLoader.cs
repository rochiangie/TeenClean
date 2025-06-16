using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoader : MonoBehaviour
{
    public float duracion = 3f; // segundos antes de pasar al menú
    public string escenaMenu = "MenuPrincipal"; // cambialo si tu escena tiene otro nombre

    private void Start()
    {
        Invoke("CargarMenu", duracion);
    }

    void CargarMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }
}
