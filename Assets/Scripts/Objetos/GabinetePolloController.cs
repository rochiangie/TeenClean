using UnityEngine;

public class GabinetePolloController : MonoBehaviour
{
    [SerializeField] private string tagObjetoRequerido = "Pollo";
    [SerializeField] private TareasManager tareasManager;
    private bool tareaCompletada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (tareaCompletada) return;

        if (other.CompareTag(tagObjetoRequerido))
        {
            Destroy(other.gameObject); // o lo que hagas normalmente
            tareasManager?.CompletarTarea("Pollo");
            tareaCompletada = true;
        }
    }
}
