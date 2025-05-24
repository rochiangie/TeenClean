using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemigo : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 3f;
    public float distanciaDeteccion = 10f;
    public float distanciaAtaque = 2f;

    [Header("Salud")]
    public int saludMaxima = 50;
    private int saludActual;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 2f;
    public int dañoAtaque = 10;

    [Header("Referencias")]
    public Animator animator;
    public Transform jugador;
    public AudioClip sonidoMuerte;
    public AudioSource audioSource;

    private float tiempoUltimoAtaque;
    private bool jugadorEnRango = false;
    private bool atacando = false;
    private bool puedeSerEliminado = false;
    private bool estaMuerto = false;

    void Awake()
    {
        // Asignación automática de componentes si están en el mismo GameObject
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        saludActual = saludMaxima;

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                jugador = playerObj.transform;
        }

        tiempoUltimoAtaque = -tiempoEntreAtaques;
    }

    void Update()
    {
        if (jugador == null || estaMuerto) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaDeteccion && distancia > distanciaAtaque)
        {
            PerseguirJugador();
            jugadorEnRango = false;
            atacando = false;
        }
        else if (distancia <= distanciaAtaque)
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
        transform.position += direccion * velocidadMovimiento * Time.deltaTime;

        // Volteo del sprite si se mueve a izquierda o derecha
        if (direccion.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direccion.x), 1, 1);
    }

    void Atacar()
    {
        atacando = true;
        tiempoUltimoAtaque = Time.time;

        // Aquí podés aplicar daño al jugador si tenés el script de salud del jugador
        // jugador.GetComponent<SaludJugador>()?.RecibirDaño(dañoAtaque);

        puedeSerEliminado = true;
        Invoke("DesactivarEliminacion", 0.5f);
    }

    void DesactivarEliminacion()
    {
        puedeSerEliminado = false;
    }

    public void RecibirDaño(int cantidad)
    {
        if (estaMuerto) return;

        saludActual -= cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        Debug.Log($"Enemigo recibió {cantidad} de daño. Salud actual: {saludActual}");

        if (animator != null) animator.SetTrigger("Daño");

        if (saludActual <= 0) Morir();
    }

    void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        Debug.Log("¡Enemigo eliminado!");
        if (animator != null) animator.SetTrigger("Morir");
        if (sonidoMuerte != null && audioSource != null)
            audioSource.PlayOneShot(sonidoMuerte);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            if (s != this) s.enabled = false;
        }

        Destroy(gameObject, 2f);
    }

    void OnTriggerStay(Collider other)
    {
        if (estaMuerto) return;

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && puedeSerEliminado)
        {
            RecibirDaño(saludMaxima); // Mata al enemigo directo
        }
    }

    // BONUS: Si instanciás enemigos en runtime y están muy pequeños, podés usar este helper
    public static GameObject InstanciarEscalaNormal(GameObject prefab, Vector3 posicion)
    {
        GameObject clon = Instantiate(prefab, posicion, Quaternion.identity);
        clon.transform.localScale = new Vector3(20f, 20f, 20f);
        return clon;
    }
}
