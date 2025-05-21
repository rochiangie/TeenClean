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

    void Start()
    {
        // Si no se asigna manualmente, busca al jugador por tag
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
        if (jugador == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        // Perseguir al jugador si está en rango de detección pero no en rango de ataque
        if (distanciaAlJugador <= distanciaDeteccion && distanciaAlJugador > distanciaAtaque)
        {
            PerseguirJugador();
            jugadorEnRango = false;
            atacando = false;
        }
        // Atacar si está en rango de ataque
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

        // Actualizar animaciones
        animator.SetBool("JugadorEnRango", jugadorEnRango);
        animator.SetBool("Atacando", atacando);
    }

    void PerseguirJugador()
    {
        // Rotar hacia el jugador
        Vector3 direccion = (jugador.position - transform.position).normalized;
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * 10f);

        // Moverse hacia el jugador
        transform.position = Vector3.MoveTowards(transform.position, jugador.position, velocidadMovimiento * Time.deltaTime);
    }

    void Atacar()
    {
        atacando = true;
        tiempoUltimoAtaque = Time.time;

        // Aquí podrías añadir lógica para hacer daño al jugador
        // Ejemplo: jugador.GetComponent<SaludJugador>().RecibirDaño(dañoAtaque);

        // Activar el estado de "puede ser eliminado" durante un breve periodo
        puedeSerEliminado = true;
        Invoke("DesactivarEliminacion", 0.5f); // El jugador tiene 0.5 segundos para presionar F
    }

    void DesactivarEliminacion()
    {
        puedeSerEliminado = false;
    }

    // Método llamado desde la animación de ataque cuando termina
    public void FinAtaque()
    {
        atacando = false;
    }

    void OnTriggerStay(Collider other)
    {
        // Si el jugador presiona F mientras el enemigo está atacando y en rango
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && puedeSerEliminado)
        {
            Morir();
        }
    }

    void Morir()
    {
        // Aquí podrías añadir efectos de muerte, sonidos, etc.
        animator.SetTrigger("Morir");

        // Deshabilitar el collider y el script para que no siga funcionando
        GetComponent<Collider>().enabled = false;
        this.enabled = false;

        // Destruir el objeto después de un tiempo (para que termine la animación)
        Destroy(gameObject, 2f);
    }
}