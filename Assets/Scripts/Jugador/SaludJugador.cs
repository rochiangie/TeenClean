using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private bool isAlive = true;
    private Rigidbody2D rb;
    public AudioClip sonidoDaño;
    private AudioSource audioSource;

    private bool yaMurio = false;

    public bool EstaMuerto => saludActual <= 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

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

        if (sonidoDaño != null && audioSource != null)
            audioSource.PlayOneShot(sonidoDaño);

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

    public void Morir()
    {
        if (!isAlive) return;
        isAlive = false;

        if (animator != null)
        {
            animator.SetBool("isAlive", false);
            animator.SetTrigger("Morir");
            animator.SetTrigger("Die");
        }

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        if (panelDerrota != null)
        {
            panelDerrota.transform.SetAsLastSibling();
            panelDerrota.SetActive(true);
        }

        Debug.Log("☠️ El jugador ha muerto por SaludJugador.");
        StartCoroutine(CargarMenuDerrotaTrasDelay());
    }




    public IEnumerator CargarMenuDerrotaTrasDelay()
    {
        yield return new WaitForSeconds(2f);

        // Mostrar panel de derrota
        if (panelDerrota != null)
            panelDerrota.SetActive(true);

        // Opcional: esperar un poco más antes de cargar el menú
        yield return new WaitForSeconds(3f);

        // Cargar menú principal
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
