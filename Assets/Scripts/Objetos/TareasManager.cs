using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class TareasManager : MonoBehaviour
{
    public static TareasManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoTareaCompletada;

    [Header("FX visual")]
    [SerializeField] private GameObject particulasTareaCompletada;
    [SerializeField] private Transform jugador;

    [Header("UI")]
    [SerializeField] private GameObject panelTasks;
    [SerializeField] private GameObject canvasHappyEnding;

    [Header("Toggles de Tasks")]
    [SerializeField] private Toggle RopaToggle;
    [SerializeField] private Toggle PlatosToggle;
    [SerializeField] private Toggle TareaToggle;
    [SerializeField] private Toggle CamaToggle;
    [SerializeField] private Toggle PolloToggle;

    private bool ropaCompletada = false;
    private bool platosCompletados = false;
    private bool tareaCompletada = false;
    private bool camaCompletada = false;
    private bool polloCompletado = false;

    private int ropaContador = 0;
    private int platosContador = 0;
    public int tareaContador = 0;

    private const int tareasNecesariasRopa = 2;
    private const int tareasNecesariasPlatos = 2;
    private const int tareasNecesariasTarea = 1;

    public enum Dificultad { Easy, Medium, Hard }
    [Header("Dificultad del juego")]
    public Dificultad dificultadActual = Dificultad.Medium;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuPrincipal")
        {
            Debug.Log("🎮 Se cargó el menú principal. Reiniciando progreso...");
            ReiniciarTareas();
            PlayerPrefs.DeleteAll();
        }
    }

    void Start()
    {
        if (RopaToggle != null) { RopaToggle.interactable = false; RopaToggle.isOn = false; }
        if (PlatosToggle != null) { PlatosToggle.interactable = false; PlatosToggle.isOn = false; }
        if (TareaToggle != null) { TareaToggle.interactable = false; TareaToggle.isOn = false; }
        if (CamaToggle != null) { CamaToggle.interactable = false; CamaToggle.isOn = false; }

        if (panelTasks != null) panelTasks.SetActive(false);
        if (canvasHappyEnding != null) canvasHappyEnding.SetActive(false);
    }

    public float ObtenerVelocidadMadre()
    {
        switch (dificultadActual)
        {
            case Dificultad.Easy: return 2f;
            case Dificultad.Medium: return 3f;
            case Dificultad.Hard: return 4f;
            default: return 3f;
        }
    }

    public void CompletarTarea(string tarea)
    {
        if (audioSource != null && sonidoTareaCompletada != null)
        {
            audioSource.PlayOneShot(sonidoTareaCompletada);
        }

        switch (tarea)
        {
            case "Ropa":
                ropaContador++;
                if (ropaContador >= tareasNecesariasRopa && !ropaCompletada)
                {
                    ropaCompletada = true;
                    if (RopaToggle != null) RopaToggle.isOn = true;
                    Debug.Log("✅ Tarea de ropa completada.");
                }
                break;

            case "Platos":
                platosContador++;
                if (platosContador >= tareasNecesariasPlatos && !platosCompletados)
                {
                    platosCompletados = true;
                    if (PlatosToggle != null) PlatosToggle.isOn = true;
                    Debug.Log("✅ Tarea de platos completada.");
                }
                break;

            case "Tarea":
                if (!tareaCompletada)
                {
                    tareaCompletada = true;
                    if (TareaToggle != null) TareaToggle.isOn = true;
                    Debug.Log("✅ Tarea académica completada.");
                }
                break;

            case "Cama":
                if (!camaCompletada)
                {
                    camaCompletada = true;
                    if (CamaToggle != null) CamaToggle.isOn = true;
                    Debug.Log("✅ Cama hecha.");
                }
                break;

            case "Pollo":
                PolloToggle.isOn = true;
                polloCompletado = true;
                break;

            default:
                Debug.LogWarning($"⚠️ Tarea '{tarea}' no reconocida.");
                break;
        }

        if (particulasTareaCompletada != null && jugador != null)
        {
            Vector3 pos = jugador.position + new Vector3(0, 1.5f, 0);
            GameObject fx = Instantiate(particulasTareaCompletada, pos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        VerificarVictoria();
    }

    private void VerificarVictoria()
    {
        if (ropaCompletada && platosCompletados && tareaCompletada && camaCompletada && polloCompletado)
        {
            Debug.Log("✅ Todas las tareas completadas.");
        }
    }

    public void MostrarFinalFelizYCreditos()
    {
        MostrarCanvasHappyEnding();
        StartCoroutine(CargarCreditosFinalesTrasDelay(5f));
    }


    private IEnumerator CargarCreditosFinalesTrasDelay(float delay)
    {
        if (canvasHappyEnding != null)
        {
            Debug.Log("🎉 Mostrando cartel de happy ending");
            canvasHappyEnding.SetActive(true);

            // Partículas de celebración
            if (particulasTareaCompletada != null && jugador != null)
            {
                var fx = Instantiate(particulasTareaCompletada, jugador.position + Vector3.up * 1.5f, Quaternion.identity);
                Destroy(fx, 3f);
            }

            yield return new WaitForSeconds(delay);
        }

        SceneManager.LoadScene("CreditosFinales");
    }


    public void MostrarCanvasHappyEnding()
    {
        if (canvasHappyEnding != null)
        {
            Debug.Log("🟢 Activando el canvasHappyEnding...");
            canvasHappyEnding.SetActive(true);
            Debug.Log($"✅ Activo en jerarquía después de SetActive: {canvasHappyEnding.activeInHierarchy}");
        }
        else
        {
            Debug.LogWarning("⚠️ canvasHappyEnding está null.");
        }
    }

    public bool TodasLasTareasCompletadasParaMadre()
    {
        return ropaCompletada && platosCompletados && tareaCompletada && camaCompletada && polloCompletado;
    }

    private HashSet<GabineteRopa> gabinetesConRopa = new HashSet<GabineteRopa>();

    public void RegistrarGabineteRopa(GabineteRopa gabinete)
    {
        if (!gabinetesConRopa.Contains(gabinete))
        {
            gabinetesConRopa.Add(gabinete);
            Debug.Log($"👕 Gabinete registrado ({gabinetesConRopa.Count}/{tareasNecesariasRopa})");

            if (gabinetesConRopa.Count >= tareasNecesariasRopa)
            {
                CompletarTarea("Ropa");
            }
        }
    }

    public void ReiniciarTareas()
    {
        ropaContador = 0;
        platosContador = 0;
        tareaContador = 0;

        ropaCompletada = false;
        platosCompletados = false;
        tareaCompletada = false;
        camaCompletada = false;
        polloCompletado = false;

        if (RopaToggle != null) RopaToggle.isOn = false;
        if (PlatosToggle != null) PlatosToggle.isOn = false;
        if (TareaToggle != null) TareaToggle.isOn = false;
        if (CamaToggle != null) CamaToggle.isOn = false;

        if (canvasHappyEnding != null) canvasHappyEnding.SetActive(false);

        Debug.Log("🔄 Tareas reiniciadas.");
    }

    public void CompletarTodasLasTareasDebug()
    {
        Debug.Log("🧪 Completando todas las tareas desde DEBUG.");

        ropaContador = tareasNecesariasRopa;
        platosContador = tareasNecesariasPlatos;
        tareaContador = tareasNecesariasTarea;

        ropaCompletada = true;
        platosCompletados = true;
        tareaCompletada = true;
        camaCompletada = true;
        polloCompletado = true;

        if (RopaToggle != null) RopaToggle.isOn = true;
        if (PlatosToggle != null) PlatosToggle.isOn = true;
        if (TareaToggle != null) TareaToggle.isOn = true;
        if (CamaToggle != null) CamaToggle.isOn = true;
        if (PolloToggle != null) PolloToggle.isOn = true;

        VerificarVictoria();
    }
}
