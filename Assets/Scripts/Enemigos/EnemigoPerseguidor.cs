using UnityEngine;

public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadMovimiento = 3f;
    public float distanciaDeteccion = 10f;
    public float distanciaAtaque = 2f;

    [Header("Referencias")]
    public Animator animator;
    public Transform jugador;

    [Header("Configuración de Ataque")]
    public float tiempoEntreAtaques = 2f;
    public int dañoAtaque = 10;

    private bool jugadorEnRango = false;
    private bool atacando = false;
    private float tiempoUltimoAtaque;
    private bool puedeSerEliminado = false;
    private bool estaMuerto = false;

    void Start()
    {
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                jugador = playerObj.transform;
            }
        }

        tiempoUltimoAtaque = -tiempoEntreAtaques;
    }

    void Update()
    {
        if (jugador == null || estaMuerto) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaDeteccion && distanciaAlJugador > distanciaAtaque)
        {
            PerseguirJugador();
            jugadorEnRango = false;
            atacando = false;
        }
        else if (distanciaAlJugador <= distanciaAtaque)
        {
            if (!atacando && Time.time > tiempoUltimoAtaque + tiempoEntreAtaques)
            {
                Atacar();
            }
            jugadorEnRango = true;
        }
        else
        {
            jugadorEnRango = false;
            atacando = false;
        }

        animator.SetBool("JugadorEnRango", jugadorEnRango);
        animator.SetBool("Atacando", atacando);
    }

    void PerseguirJugador()
    {
        Vector3 direccion = (jugador.position - transform.position).normalized;
        Quaternion rotacionDeseada = Quaternion.LookRotation(Vector3.forward, direccion); // 2D fix
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * 10f);
        transform.position = Vector3.MoveTowards(transform.position, jugador.position, velocidadMovimiento * Time.deltaTime);
    }

    void Atacar()
    {
        atacando = true;
        tiempoUltimoAtaque = Time.time;

        // Aquí podrías añadir lógica de daño
        // Ejemplo: jugador.GetComponent<SaludJugador>()?.RecibirDaño(dañoAtaque);

        puedeSerEliminado = true;
        Invoke("DesactivarEliminacion", 0.5f); // Solo se puede eliminar durante esta ventana
    }

    void DesactivarEliminacion()
    {
        puedeSerEliminado = false;
    }

    public void FinAtaque()
    {
        atacando = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (estaMuerto) return;

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && puedeSerEliminado)
        {
            Morir();
        }
    }

    void Morir()
    {
        estaMuerto = true;
        animator.SetTrigger("Morir");

        // Opcional: reproducir sonido o partículas de muerte aquí
        // AudioSource.PlayClipAtPoint(clipMuerte, transform.position);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        this.enabled = false;

        Destroy(gameObject, 2f); // espera 2 segundos para que la animación se reproduzca
    }
}
