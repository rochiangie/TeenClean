﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class TareasManager : MonoBehaviour
{
    // === SINGLETON PATTERN ===
    public static TareasManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoTareaCompletada;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ========================

    [Header("Panel de Tasks")]
    [SerializeField] private GameObject panelTasks;
    [SerializeField] private GameObject panelWin;

    [Header("Toggles de Tasks")]
    [SerializeField] private Toggle RopaToggle;
    [SerializeField] private Toggle PlatosToggle;
    [SerializeField] private Toggle TareaToggle;
    [SerializeField] private Toggle CamaToggle;
    [SerializeField] private Toggle PolloToggle; // 👈 nuevo toggle

    private bool polloCompletado = false; // 👈 nueva flag

    private int ropaContador = 0;
    private int platosContador = 0;
    public  int tareaContador = 0;

    private const int tareasNecesariasRopa = 2;
    private const int tareasNecesariasPlatos = 2;
    private const int tareasNecesariasTarea = 1;

    private bool ropaCompletada = false;
    private bool platosCompletados = false;
    private bool tareaCompletada = false;
    private bool camaCompletada = false;

    void Start()
    {
        if (RopaToggle != null) { RopaToggle.interactable = false; RopaToggle.isOn = false; }
        if (PlatosToggle != null) { PlatosToggle.interactable = false; PlatosToggle.isOn = false; }
        if (TareaToggle != null) { TareaToggle.interactable = false; TareaToggle.isOn = false; }
        if (CamaToggle != null) { CamaToggle.interactable = false; CamaToggle.isOn = false; }

        if (panelTasks != null) panelTasks.SetActive(false);
        else Debug.LogError("🚨 TareasManager: 'panelTasks' no está asignado en el Inspector.");

        if (panelWin != null) panelWin.SetActive(false);
        else Debug.LogError("🚨 TareasManager: 'panelWin' no está asignado en el Inspector.");
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
            default:
                Debug.LogWarning($"⚠️ Tarea '{tarea}' no reconocida.");
                break;
            case "Pollo":
                PolloToggle.isOn = true;
                polloCompletado = true;
                break;
        }

        VerificarVictoria();
    }

    private void VerificarVictoria()
    {
        if (ropaCompletada && platosCompletados && tareaCompletada && camaCompletada && polloCompletado)
        {
            Debug.Log("✅ Todas las tareas completadas.");
            // Ya no activamos el panel de victoria aquí.
        }
    }


    public class FinDelJuego : MonoBehaviour
    {
        public void IrAEscenaEnding()
        {
            SceneManager.LoadScene("Ending");
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
                Debug.Log("✅ Tarea de ropa completada por llenar gabinetes.");
            }
        }
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); // Asegurate de usar el nombre correcto
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

        if (RopaToggle != null) RopaToggle.isOn = false;
        if (PlatosToggle != null) PlatosToggle.isOn = false;
        if (TareaToggle != null) TareaToggle.isOn = false;
        if (CamaToggle != null) CamaToggle.isOn = false;

        Debug.Log("🔄 Tareas reiniciadas.");

        if (panelWin != null) panelWin.SetActive(false);
    }

    // === Acceso desde Madre ===

    public GameObject PanelVictoria => panelWin;

    public IEnumerator CargarCreditosFinalesTrasDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("CreditosFinales");
    }

}
