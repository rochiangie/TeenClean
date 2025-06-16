using UnityEngine;

public class DificultadManager : MonoBehaviour
{
    public enum NivelDificultad { Facil, Media, Dificil }
    public NivelDificultad dificultadActual = NivelDificultad.Facil;

    public float TiempoLimite => dificultadActual switch
    {
        NivelDificultad.Facil => 300f,
        NivelDificultad.Media => 180f,
        NivelDificultad.Dificil => 120f,
        _ => 300f
    };

    public float VelocidadMadre => dificultadActual switch
    {
        NivelDificultad.Facil => 1.5f,
        NivelDificultad.Media => 2.5f,
        NivelDificultad.Dificil => 4f,
        _ => 2f
    };

    public int CantidadBichos => dificultadActual switch
    {
        NivelDificultad.Facil => 2,
        NivelDificultad.Media => 4,
        NivelDificultad.Dificil => 6,
        _ => 3
    };

    public static DificultadManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
