using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public GameObject panelOpciones;

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
        SceneManager.LoadScene("Juego1"); // Carga la escena que se llama "Juego"
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

    public void OcultarOpciones()
    {
        // Esto solo se usaría si tienes un panel y no una escena, pero lo dejamos por si acaso
        panelOpciones.SetActive(false);
    }

    public void SetDificultadFacil()
    {
        Debug.Log("Dificultad facil");
        PlayerPrefs.SetInt("Dificultad", 0);
        SceneManager.LoadScene("MenuPrincipal"); // Vuelve automáticamente al menú
    }

    public void SetDificultadNormal()
    {

        Debug.Log("Dificultad normal");
        PlayerPrefs.SetInt("Dificultad", 1);
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void SetDificultadDificil()
    {

        Debug.Log("Dificultad dificil");
        PlayerPrefs.SetInt("Dificultad", 2);
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void VolverAlMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
