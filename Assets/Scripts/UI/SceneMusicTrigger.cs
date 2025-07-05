using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicTrigger : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<SceneMusicTrigger>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        Debug.Log("🎬 Escena activa: " + escenaActual);


        switch (escenaActual)
        {
            case "SplashScreen":
                AudioManager.Instance?.ReproducirMusicaLogo();
                break;
            case "MenuPrincipal":
                AudioManager.Instance?.ReproducirMusicaMenu();
                break;
            case "Juego1":
                AudioManager.Instance?.ReproducirMusicaJuego();
                break;
            case "Ending":
            case "CreditosFinales":
                AudioManager.Instance?.ReproducirMusicaFinal();
                break;
            default:
                Debug.Log("🎵 No hay música asignada para esta escena: " + escenaActual);
                break;
        }
    }
}
