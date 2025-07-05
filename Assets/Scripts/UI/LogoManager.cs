using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoManager : MonoBehaviour
{
    [SerializeField] private float duracionLogo = 10f;

    void Start()
    {
        // Reproduce la música del logo desde el AudioManager
        AudioManager.Instance?.ReproducirMusicaLogo();

        // Inicia la cuenta regresiva para cambiar de escena
        StartCoroutine(CambiarASiguienteEscena());
    }

    private System.Collections.IEnumerator CambiarASiguienteEscena()
    {
        yield return new WaitForSeconds(duracionLogo);
        SceneManager.LoadScene("MenuPrincipal");
    }
}
