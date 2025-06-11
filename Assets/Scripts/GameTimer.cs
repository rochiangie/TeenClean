using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;

    private float timeRemaining;
    private bool timerIsRunning = false;

    void Start()
    {
        int dificultad = PlayerPrefs.GetInt("Dificultad", 1);

        switch (dificultad)
        {
            case 0: timeRemaining = 10 * 60; break;
            case 1: timeRemaining = 5 * 60; break;
            case 2: timeRemaining = 1 * 60; break;
        }

        timerText.gameObject.SetActive(true);
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                Debug.Log("¡Tiempo terminado!");

                // Mostrar cartel
                gameOverPanel.SetActive(true);

                // Matar al jugador con animación
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    var jugador = player.GetComponent<InteraccionJugador>();
                    if (jugador != null)
                    {
                        jugador.Die();
                        // Iniciar la corutina para destruir al jugador después de la animación
                        StartCoroutine(DestruirJugadorDespuesDeAnimacion(player));
                    }
                }
                else
                {
                    // Si no hay player, pasamos directo a menú principal
                    StartCoroutine(VolverAlMenuPrincipal());
                }
            }
        }
    }

    IEnumerator DestruirJugadorDespuesDeAnimacion(GameObject player)
    {
        // Ajustá este tiempo a la duración de tu animación de muerte
        yield return new WaitForSeconds(1.5f);

        if (player != null)
        {
            Destroy(player);
        }

        // Ir al menú principal después de un tiempo extra (opcional)
        StartCoroutine(VolverAlMenuPrincipal());
    }

    IEnumerator VolverAlMenuPrincipal()
    {
        yield return new WaitForSeconds(2f); // espera 2 segundos después de destruir al player
        SceneManager.LoadScene("MenuPrincipal");
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
