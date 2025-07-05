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
        if (TareasManager.Instance != null)
            TareasManager.Instance.ResetearTareas();

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
                Debug.Log("⏰ ¡Tiempo terminado!");

                // Mostrar cartel
                if (gameOverPanel != null) gameOverPanel.SetActive(true);

                // Intentar matar al jugador (esto ya dispara panelDerrota y muerte)
                SaludJugador salud = FindObjectOfType<SaludJugador>();
                if (salud != null)
                {
                    salud.Morir(); // NO debe llamar a créditos felices
                }
                else
                {
                    Debug.LogWarning("⚠️ No se encontró SaludJugador. Volviendo al menú principal.");
                    StartCoroutine(VolverAlMenuPrincipal());
                }

                // Ejecutar animación de muerte si existe el jugador
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    var jugador = player.GetComponent<InteraccionJugador>();
                    if (jugador != null)
                    {
                        jugador.Die(); // ← si tenés un método Die() para animar
                        StartCoroutine(DestruirJugadorDespuesDeAnimacion(player));
                    }
                }
            }
        }
    }

    IEnumerator DestruirJugadorDespuesDeAnimacion(GameObject player)
    {
        yield return new WaitForSeconds(1.5f);
        if (player != null) Destroy(player);
        // ❌ Eliminado: TareasManager.Instance.MostrarFinalFelizYCreditos();
        // Si querés volver al menú después de morir por tiempo:
        // StartCoroutine(VolverAlMenuPrincipal());
    }

    IEnumerator VolverAlMenuPrincipal()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MenuPrincipal");
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
