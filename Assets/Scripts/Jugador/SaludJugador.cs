using UnityEngine;

public class SaludJugador : MonoBehaviour
{
    [Header("Salud del Jugador")]
    public int saludMaxima = 100;
    public int saludActual;

    [Header("Referencias")]
    public Animator animator; // Opcional para animaciones de da�o o muerte

    void Start()
    {
        saludActual = saludMaxima;
    }

    // Llamar cuando recibe da�o
    public void RecibirDa�o(int cantidad)
    {
        saludActual -= cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        Debug.Log($"Jugador recibi� {cantidad} de da�o. Salud actual: {saludActual}");

        if (animator != null)
        {
            animator.SetTrigger("Da�o");
        }

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    // Llamar cuando toma una poci�n u objeto de curaci�n
    public void Curar(int cantidad)
    {
        saludActual += cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);
        Debug.Log($"Jugador se cur� {cantidad}. Salud actual: {saludActual}");
    }

    void Morir()
    {
        Debug.Log("El jugador ha muerto.");
        if (animator != null)
        {
            animator.SetTrigger("Morir");
        }

        // Aqu� podr�as desactivar controles, reproducir sonidos, etc.
        GetComponent<InteraccionJugador>().enabled = false;
        // Tambi�n podr�as llamar a un GameManager para reiniciar nivel, mostrar UI, etc.
    }
}
