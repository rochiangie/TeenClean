using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VolverAlMenu : MonoBehaviour
{
    public float delay = 30f;
    public string nombreMenu = "MenuPrincipal";

    private void Start()
    {
        StartCoroutine(VolverAlMenuTrasDelay());
    }

    private IEnumerator VolverAlMenuTrasDelay()
    {
        yield return new WaitForSeconds(delay);

        if (TareasManager.Instance != null)
        {
            TareasManager.Instance.ReiniciarTareas(); // ✅ resetea el progreso
        }

        SceneManager.LoadScene(nombreMenu);
    }
}
