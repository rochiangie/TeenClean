using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class CreditosFinales : MonoBehaviour
{
    public float duracion = 5f; // segundos antes de ir al menú

    private void Start()
    {
        StartCoroutine(CargarMenuTrasDelay());
    }

    private IEnumerator CargarMenuTrasDelay()
    {
        yield return new WaitForSeconds(duracion);
        SceneManager.LoadScene("MenuPrincipal"); // Cambiá el nombre si es otro
    }
}
