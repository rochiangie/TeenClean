using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Prefabs de música")]
    public GameObject prefabMusicaLogo;
    public GameObject prefabMusicaMenu;
    public GameObject prefabMusicaJuego;
    public GameObject prefabMusicaFinal;

    private AudioSource musicaLogo;
    private AudioSource musicaMenu;
    private AudioSource musicaJuego;
    private AudioSource musicaFinal;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Instanciar los prefabs de música
        if (prefabMusicaLogo != null)
            musicaLogo = Instantiate(prefabMusicaLogo).GetComponent<AudioSource>();
        if (prefabMusicaMenu != null)
            musicaMenu = Instantiate(prefabMusicaMenu).GetComponent<AudioSource>();
        if (prefabMusicaJuego != null)
            musicaJuego = Instantiate(prefabMusicaJuego).GetComponent<AudioSource>();
        if (prefabMusicaFinal != null)
            musicaFinal = Instantiate(prefabMusicaFinal).GetComponent<AudioSource>();

        DontDestroyIfNotNull(musicaLogo);
        DontDestroyIfNotNull(musicaMenu);
        DontDestroyIfNotNull(musicaJuego);
        DontDestroyIfNotNull(musicaFinal);

        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void DontDestroyIfNotNull(AudioSource source)
    {
        if (source != null)
            DontDestroyOnLoad(source.gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string nombreEscena = scene.name;
        Debug.Log($"🎬 Escena cargada: {nombreEscena}");

        DetenerTodaMusica();

        switch (nombreEscena)
        {
            case "SplashScreen":
                ReproducirMusicaLogo();
                break;
            case "MenuPrincipal":
                ReproducirMusicaMenu();
                break;
            case "Juego1":
                ReproducirMusicaJuego();
                break;
            case "Ending":
            case "CreditosFinales":
                ReproducirMusicaFinal();
                break;
            default:
                Debug.Log("🎵 No hay música asignada para esta escena.");
                break;
        }
    }

    public void DetenerTodaMusica()
    {
        musicaLogo?.Stop();
        musicaMenu?.Stop();
        musicaJuego?.Stop();
        musicaFinal?.Stop();
    }

    public void ReproducirMusicaLogo() => musicaLogo?.Play();
    public void ReproducirMusicaMenu() => musicaMenu?.Play();
    public void ReproducirMusicaJuego() => musicaJuego?.Play();
    public void ReproducirMusicaFinal() => musicaFinal?.Play();
}
