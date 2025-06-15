using UnityEngine;
using UnityEngine.SceneManagement;

public class SaludJugador : MonoBehaviour
{
    [Header("Salud del Jugador")]
    [SerializeField] private int saludMaxima = 100;
    private int saludActual;

    [Header("Corazones UI (Sprites)")]
    //[SerializeField] private SpriteRenderer[] corazonesSprites; // Asegurate de usar SpriteRenderer, NO Image

    [Header("Panel de derrota")]
    [SerializeField] private GameObject panelDerrota;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [SerializeField] private CorazonPorSprite corazonHUD;


    private bool yaMurio = false;

    public bool EstaMuerto => saludActual <= 0;

    void Start()
    {
        saludActual = saludMaxima;
        //ActualizarCorazones();
        if (panelDerrota != null) panelDerrota.SetActive(false);
    }

    public void RecibirDaño(int cantidad)
    {
        if (yaMurio) return;

        saludActual -= cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        if (animator != null)
        {
            animator.SetTrigger("Daño");
        }

        //ActualizarCorazones();
        if (corazonHUD != null)
        {
            corazonHUD.RecibirDaño(cantidad); // ← actualiza el sprite de vida visual
        }

        if (saludActual <= 0)
        {
            yaMurio = true;
            Morir();
        }
    }

    /*private void ActualizarCorazones()
    {
        int corazonesVisibles = Mathf.CeilToInt((float)saludActual / (saludMaxima / corazonesSprites.Length));

        for (int i = 0; i < corazonesSprites.Length; i++)
        {
            if (corazonesSprites[i] != null)
            {
                Color color = corazonesSprites[i].color;
                color.a = i < corazonesVisibles ? 1f : 0f;
                corazonesSprites[i].color = color;
            }
        }
    }*/

    private void Morir()
    {
        Debug.Log("☠️ El jugador ha muerto.");

        if (animator != null)
            animator.SetTrigger("Morir");

        if (panelDerrota != null)
            panelDerrota.SetActive(true);

        GetComponent<InteraccionJugador>().enabled = false;

        StartCoroutine(CargarMenuDerrotaTrasDelay());
    }

    private System.Collections.IEnumerator CargarMenuDerrotaTrasDelay()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void Curar(int cantidad)
    {
        if (yaMurio) return;

        saludActual += cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);
        //ActualizarCorazones();
        if (corazonHUD != null)
        {
            corazonHUD.Curar(cantidad);
        }

    }

    public int GetVidaActual()
    {
        return saludActual;
    }
}
