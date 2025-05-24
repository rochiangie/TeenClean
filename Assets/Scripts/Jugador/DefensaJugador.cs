using UnityEngine;

public class DefensaJugador : MonoBehaviour
{
    public KeyCode teclaDefensa = KeyCode.Space; // Cambialo si querés otra tecla

    public bool EstaDefendiendo()
    {
        bool defendiendo = Input.GetKey(teclaDefensa);
        if (defendiendo)
            Debug.Log("🛡️ Jugador está presionando defensa.");
        return defendiendo;
    }

}
