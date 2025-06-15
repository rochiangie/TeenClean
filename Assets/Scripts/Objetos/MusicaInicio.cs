using UnityEngine;

public class MusicaInicio : MonoBehaviour
{
    [SerializeField] private AudioSource musicaFondo;

    private void Start()
    {
        if (musicaFondo != null && !musicaFondo.isPlaying)
        {
            musicaFondo.loop = true;
            musicaFondo.Play();
        }
    }
}
