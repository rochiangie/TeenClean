using UnityEngine;

public class SaludJugador : MonoBehaviour
{
    [Header("Salud del Jugador")]
    public int saludMaxima = 100;
    public int saludActual;

    [Header("Referencias")]
    public Animator animator; // Opcional para animaciones de daño o muerte

    public bool EstaMuerto => saludActual <= 0;

    void Start()
    {
        saludActual = saludMaxima;
    }

    // Llamar cuando recibe daño
    public void RecibirDaño(int cantidad)
    {
        Debug.Log($"🔥 SaludJugador.RecibirDaño llamado con cantidad: {cantidad}");

        saludActual -= cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        Debug.Log($"❤️ Salud actual: {saludActual}");

        if (animator != null)
        {
            animator.SetTrigger("Daño");
        }

        if (saludActual <= 0)
        {
            Morir();
        }
    }


    // Llamar cuando toma una poción u objeto de curación
    public void Curar(int cantidad)
    {
        saludActual += cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);
        Debug.Log($"Jugador se curó {cantidad}. Salud actual: {saludActual}");
    }

    void Morir()
    {
        Debug.Log("El jugador ha muerto.");
        if (animator != null)
        {
            animator.SetTrigger("Morir");
        }

        // Aquí podrías desactivar controles, reproducir sonidos, etc.
        GetComponent<InteraccionJugador>().enabled = false;
        // También podrías llamar a un GameManager para reiniciar nivel, mostrar UI, etc.
    }
}
