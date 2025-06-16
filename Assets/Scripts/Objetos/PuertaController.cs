using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuertaController : MonoBehaviour
{
    [SerializeField] private GameObject puertaAbierta;
    [SerializeField] private GameObject puertaCerrada;
    [SerializeField] private AudioClip sonidoPuerta;
    [SerializeField] private AudioSource audioSource;
    private bool estaAbierta = false;

    public void AlternarEstado()
    {
        estaAbierta = !estaAbierta;
        ActualizarEstadoVisual();
        audioSource.PlayOneShot(sonidoPuerta);
    }

    private void ActualizarEstadoVisual()
    {
        puertaAbierta.SetActive(estaAbierta);
        puertaCerrada.SetActive(!estaAbierta);
    }

    public string ObtenerNombreEstado()
    {
        return $"Puerta ({(estaAbierta ? "Abierta" : "Cerrada")})";
    }
}
