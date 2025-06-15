using UnityEngine;

public class CorazonPorSprite : MonoBehaviour
{
    [Header("Sprites de corazones (de 0 = lleno a 6 = vac�o)")]
    [SerializeField] private GameObject[] spritesVida; // sprite_0 a sprite_6

    [Header("Salud")]
    [SerializeField] private int saludMaxima = 100;
    private int saludActual;

    private int nivelActual = 0; // 0 = lleno, 6 = vac�o

    private void Start()
    {
        saludActual = saludMaxima;
        ActualizarSprites();
    }

    public void RecibirDa�o(int da�o)
    {
        saludActual = Mathf.Clamp(saludActual - da�o, 0, saludMaxima);
        nivelActual = CalcularNivelDeDa�o();
        ActualizarSprites();
    }

    private int CalcularNivelDeDa�o()
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
        nivelActual = CalcularNivelDeDa�o();
        ActualizarSprites();
    }
}
