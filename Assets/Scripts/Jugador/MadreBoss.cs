using UnityEngine;
using TMPro;

public class MadreBoss : MonoBehaviour
{
    [Header("Configuración de diálogo")]
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private TextMeshProUGUI textoDialogo;
    [SerializeField] private string[] dialogos;
    [SerializeField] private KeyCode teclaContinuar = KeyCode.Space;

    private int indiceDialogo = 0;
    private bool enDialogo = false;

    private void Start()
    {
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }

    private void Update()
    {
        if (enDialogo && Input.GetKeyDown(teclaContinuar))
        {
            SiguienteDialogo();
        }
    }

    public void IniciarDialogo()
    {
        if (dialogos.Length == 0) return;

        indiceDialogo = 0;
        enDialogo = true;

        panelDialogo.SetActive(true);
        textoDialogo.text = dialogos[indiceDialogo];
    }

    private void SiguienteDialogo()
    {
        indiceDialogo++;
        if (indiceDialogo < dialogos.Length)
        {
            textoDialogo.text = dialogos[indiceDialogo];
        }
        else
        {
            FinalizarDialogo();
        }
    }

    private void FinalizarDialogo()
    {
        enDialogo = false;
        panelDialogo.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IniciarDialogo();
        }
    }
}
