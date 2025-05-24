using UnityEngine;

public class DefensaJugador : MonoBehaviour
{
    public KeyCode teclaDefensa = KeyCode.Space;
    public GameObject prefabEscudo;
    public Transform puntoDeSpawn; // Donde se instancia el escudo (por ejemplo, un hijo del jugador)

    private GameObject escudoInstanciado;

    void Update()
    {
        if (Input.GetKeyDown(teclaDefensa))
        {
            ActivarDefensa();
        }
        else if (Input.GetKeyUp(teclaDefensa))
        {
            DesactivarDefensa();
        }
    }

    public bool EstaDefendiendo()
    {
        return escudoInstanciado != null;
    }

    void ActivarDefensa()
    {
        if (prefabEscudo != null && escudoInstanciado == null)
        {
            escudoInstanciado = Instantiate(prefabEscudo, puntoDeSpawn.position, Quaternion.identity, puntoDeSpawn);
            Debug.Log("🛡️ Escudo activado");
        }
    }

    void DesactivarDefensa()
    {
        if (escudoInstanciado != null)
        {
            Destroy(escudoInstanciado);
            escudoInstanciado = null;
            Debug.Log("🧼 Escudo destruido");
        }
    }
}
