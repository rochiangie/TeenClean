using UnityEngine;

public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    public float velocidadMovimiento = 3f;
    public float distanciaDeteccion = 10f;
    public float distanciaAtaque = 2f;

    [Header("Referencias")]
    public Animator animator;
    public Transform jugador;

    [Header("Configuraci�n de Ataque")]
    public float tiempoEntreAtaques = 2f;
    public int da�oAtaque = 10;

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

        // Perseguir al jugador si est� en rango de detecci�n pero no en rango de ataque
        if (distanciaAlJugador <= distanciaDeteccion && distanciaAlJugador > distanciaAtaque)
        {
            PerseguirJugador();
            jugadorEnRango = false;
            atacando = false;
        }
        // Atacar si est� en rango de ataque
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

        // Aqu� podr�as a�adir l�gica para hacer da�o al jugador
        // Ejemplo: jugador.GetComponent<SaludJugador>().RecibirDa�o(da�oAtaque);

        // Activar el estado de "puede ser eliminado" durante un breve periodo
        puedeSerEliminado = true;
        Invoke("DesactivarEliminacion", 0.5f); // El jugador tiene 0.5 segundos para presionar F
    }

    void DesactivarEliminacion()
    {
        puedeSerEliminado = false;
    }

    // M�todo llamado desde la animaci�n de ataque cuando termina
    public void FinAtaque()
    {
        atacando = false;
    }

    void OnTriggerStay(Collider other)
    {
        // Si el jugador presiona F mientras el enemigo est� atacando y en rango
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && puedeSerEliminado)
        {
            Morir();
        }
    }

    void Morir()
    {
        // Aqu� podr�as a�adir efectos de muerte, sonidos, etc.
        animator.SetTrigger("Morir");

        // Deshabilitar el collider y el script para que no siga funcionando
        GetComponent<Collider>().enabled = false;
        this.enabled = false;

        // Destruir el objeto despu�s de un tiempo (para que termine la animaci�n)
        Destroy(gameObject, 2f);
    }
}