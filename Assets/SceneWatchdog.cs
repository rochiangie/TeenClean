using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class SceneWatchdog : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuPrincipal")
        {
            UnityEngine.Debug.LogError("🛑 MenuPrincipal fue cargado. Mostrando rastro:");

            StackTrace stackTrace = new StackTrace(true);
            UnityEngine.Debug.Log(stackTrace.ToString());
        }
    }
}
