using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float timeRemaining;
    private bool timerIsRunning = false;

    void Start()
    {
        int dificultad = PlayerPrefs.GetInt("Dificultad", 1); // Default normal

        switch (dificultad)
        {
            case 0: // Fácil
                timeRemaining = 20 * 60;
                break;
            case 1: // Normal
                timeRemaining = 10 * 60;
                break;
            case 2: // Difícil
                timeRemaining = 5 * 60;
                break;
        }

        // ¡Activamos el texto y arrancamos el timer!
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
                // Aquí podrías mostrar Game Over o algo similar
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
