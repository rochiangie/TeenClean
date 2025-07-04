using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VolverAlMenuTrasCreditos : MonoBehaviour
{
    public float delay = 30f;
    public string nombreMenu = "MenuPrincipal";

    private void Start()
    {
        StartCoroutine(VolverAlMenu());
    }

    private IEnumerator VolverAlMenu()
    {
        yield return new WaitForSeconds(delay);

        // ✅ Reiniciar progreso
        if (TareasManager.Instance != null)
        {
            TareasManager.Instance.ReiniciarTareas();
        }

        // 🧹 Limpiar PlayerPrefs si los usás
        PlayerPrefs.DeleteAll();

        // ✅ Volver al menú
        SceneManager.LoadScene(nombreMenu);
    }
}
