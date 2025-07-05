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
    public GameObject prefabMusicaDerrota;
    public GameObject prefabMusicaCreditos; // 🎵 NUEVO

    private AudioSource musicaLogo;
    private AudioSource musicaMenu;
    private AudioSource musicaJuego;
    private AudioSource musicaFinal;
    private AudioSource musicaDerrota;
    private AudioSource musicaCreditos; // 🎵 NUEVO

    public bool mantenerMusicaActual = false;

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
        if (prefabMusicaDerrota != null)
            musicaDerrota = Instantiate(prefabMusicaDerrota).GetComponent<AudioSource>();
        if (prefabMusicaCreditos != null)
            musicaCreditos = Instantiate(prefabMusicaCreditos).GetComponent<AudioSource>();

        DontDestroyIfNotNull(musicaLogo);
        DontDestroyIfNotNull(musicaMenu);
        DontDestroyIfNotNull(musicaJuego);
        DontDestroyIfNotNull(musicaFinal);
        DontDestroyIfNotNull(musicaDerrota);
        DontDestroyIfNotNull(musicaCreditos); // 🎵 NUEVO

        float volumenGuardado = PlayerPrefs.GetFloat("volumenMusica", 1f);
        CambiarVolumen(volumenGuardado);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void DontDestroyIfNotNull(AudioSource source)
    {
        if (source != null)
            DontDestroyOnLoad(source.gameObject);
    }

    private string escenaAnterior = "";
    private string musicaActual = "";

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string nombreEscena = scene.name;
        Debug.Log($"🎬 Escena cargada: {nombreEscena}");

        if (nombreEscena != "Menu-opciones" && nombreEscena != "CreditosFinales")
        {
            DetenerTodaMusica();
        }

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
                ReproducirMusicaFinal();
                break;
            case "CreditosFinales":
                if (!mantenerMusicaActual)
                {
                    Debug.Log("🎵 Reproduciendo música de créditos.");
                    ReproducirMusicaCreditos();
                }
                else
                {
                    Debug.Log("🎵 Manteniendo música de victoria en créditos.");
                }
                break;
            case "Menu-opciones":
                Debug.Log("🎵 Música de menú sigue sonando en opciones.");
                break;
            default:
                Debug.Log("🎵 No hay música asignada para esta escena.");
                break;
        }

        // Siempre reiniciar el flag al cambiar de escena
        mantenerMusicaActual = false;
    }

    public void CambiarVolumen(float nuevoVolumen)
    {
        if (musicaLogo != null) musicaLogo.volume = nuevoVolumen;
        if (musicaMenu != null) musicaMenu.volume = nuevoVolumen;
        if (musicaJuego != null) musicaJuego.volume = nuevoVolumen;
        if (musicaFinal != null) musicaFinal.volume = nuevoVolumen;
        if (musicaDerrota != null) musicaDerrota.volume = nuevoVolumen;
        if (musicaCreditos != null) musicaCreditos.volume = nuevoVolumen;
    }

    public void DetenerTodaMusica()
    {
        musicaLogo?.Stop();
        musicaMenu?.Stop();
        musicaJuego?.Stop();
        musicaFinal?.Stop();
        musicaDerrota?.Stop();
        musicaCreditos?.Stop();
    }

    public void ReproducirMusicaDerrota()
    {
        DetenerTodaMusica();
        musicaDerrota?.Play();
        musicaActual = "Derrota";
    }

    public void ReproducirMusicaFinal()
    {
        if (musicaActual == "Derrota") return; // No sobreescribas si ya está la música triste
        DetenerTodaMusica();
        musicaFinal?.Play();
        musicaActual = "Final";
    }

    public void ReproducirMusicaCreditos()
    {
        DetenerTodaMusica();
        musicaCreditos?.Play();
        musicaActual = "Creditos";
    }

    public void ReproducirMusicaLogo() => musicaLogo?.Play();
    public void ReproducirMusicaMenu() => musicaMenu?.Play();
    public void ReproducirMusicaJuego() => musicaJuego?.Play();
}
