using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class SceneLoadDebugger : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == "MenuPrincipal")
            {
                UnityEngine.Debug.LogError("🛑 MenuPrincipal fue cargado. Mostrando rastro:");

                StackTrace stackTrace = new StackTrace(true);
                UnityEngine.Debug.Log(stackTrace.ToString());
            }
        };
    }
}
