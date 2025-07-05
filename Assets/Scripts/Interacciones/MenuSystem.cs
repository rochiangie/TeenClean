using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    void Update()
    {
        // Detecta la tecla Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
    }

    public void Jugar()
    {
        SceneManager.LoadScene("Juego1");
    }

    public void Salir()
    {
        Debug.Log("Saliendo del Juego");
        Application.Quit();
    }

    public void MostrarOpciones()
    {
        SceneManager.LoadScene("Menu-opciones");
    }

    public void SetDificultadFacil()
    {
        PlayerPrefs.SetInt("Dificultad", 0);
        SceneManager.LoadScene("Juego1");
    }

    public void SetDificultadNormal()
    {
        PlayerPrefs.SetInt("Dificultad", 1);
        SceneManager.LoadScene("Juego1");
    }

    public void SetDificultadDificil()
    {
        PlayerPrefs.SetInt("Dificultad", 2);
        SceneManager.LoadScene("Juego1");
    }

    public void VolverAlMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
