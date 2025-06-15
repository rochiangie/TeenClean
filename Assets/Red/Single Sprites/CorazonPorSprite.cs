using UnityEngine;

public class CorazonPorSprite : MonoBehaviour
{
    [Header("Sprites de corazones (de 0 = lleno a 6 = vacío)")]
    [SerializeField] private GameObject[] spritesVida; // sprite_0 a sprite_6

    [Header("Salud")]
    [SerializeField] private int saludMaxima = 100;
    private int saludActual;

    private int nivelActual = 0; // 0 = lleno, 6 = vacío

    private void Start()
    {
        saludActual = saludMaxima;
        ActualizarSprites();
    }

    public void RecibirDaño(int daño)
    {
        saludActual = Mathf.Clamp(saludActual - daño, 0, saludMaxima);
        nivelActual = CalcularNivelDeDaño();
        ActualizarSprites();
    }

    private int CalcularNivelDeDaño()
    {
        float porcentaje = 1f - ((float)saludActual / saludMaxima);
        int totalNiveles = spritesVida.Length - 1;
        return Mathf.Clamp(Mathf.RoundToInt(porcentaje * totalNiveles), 0, totalNiveles);
    }

    private void ActualizarSprites()
    {
        for (int i = 0; i < spritesVida.Length; i++)
        {
            if (spritesVida[i] != null)
                spritesVida[i].SetActive(i == nivelActual);
        }
    }

    public void Curar(int cantidad)
    {
        saludActual = Mathf.Clamp(saludActual + cantidad, 0, saludMaxima);
        nivelActual = CalcularNivelDeDaño();
        ActualizarSprites();
    }
}
