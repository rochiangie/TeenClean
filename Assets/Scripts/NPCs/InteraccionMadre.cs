using UnityEngine;
using TMPro;

public class InteraccionMadre : MonoBehaviour
{
    private bool yaInteractuo = false;
    private bool jugadorEnRango = false;
    private Madre madre;

    [Header("UI de interacción")]
    public GameObject panelInteraccion;
    public TextMeshProUGUI textoInteraccion;
    public KeyCode teclaInteraccion = KeyCode.E;

    void Start()
    {
        madre = GetComponent<Madre>();
        if (madre == null)
        {
            Debug.LogError("❌ No se encontró el componente Madre en el objeto.");
        }

        OcultarCartel();
    }

    void Update()
    {
        if (jugadorEnRango)
        {
            if (yaInteractuo)
            {
                MostrarCartel("Presioná E para hablar con Mamá");

                if (Input.GetKeyDown(teclaInteraccion))
                {
                    madre?.IniciarDialogo();
                }
            }
        }
        else
        {
            OcultarCartel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;

            if (!yaInteractuo && madre != null)
            {
                madre.IniciarDialogo();
                yaInteractuo = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            OcultarCartel();
        }
    }

    private void MostrarCartel(string mensaje)
    {
        if (panelInteraccion != null && textoInteraccion != null)
        {
            textoInteraccion.text = mensaje;
            panelInteraccion.SetActive(true);
        }
    }

    private void OcultarCartel()
    {
        if (panelInteraccion != null)
        {
            panelInteraccion.SetActive(false);
        }
    }
}
