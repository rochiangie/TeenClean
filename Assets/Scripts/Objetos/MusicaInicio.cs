using UnityEngine;

public class MusicaInicio : MonoBehaviour
{
    [SerializeField] private AudioSource musicaFondo;

    public static MusicaInicio Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Si querés que siga sonando entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (musicaFondo != null && !musicaFondo.isPlaying)
        {
            musicaFondo.loop = true;
            musicaFondo.Play();
        }
    }

    public void PausarMusica()
    {
        if (musicaFondo != null && musicaFondo.isPlaying)
        {
            musicaFondo.Pause();
        }
    }

    public void ReanudarMusica()
    {
        if (musicaFondo != null && !musicaFondo.isPlaying)
        {
            musicaFondo.Play();
        }
    }

    public void DetenerMusica()
    {
        if (musicaFondo != null)
        {
            musicaFondo.Stop();
        }
    }
}
