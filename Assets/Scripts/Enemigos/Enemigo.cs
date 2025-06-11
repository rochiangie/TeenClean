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


    [Header("Efecto de Ataque")]
    public GameObject prefabHumo;
    public Transform puntoEfecto;
    public float duracionHumo = 2f;

    void Awake()
    {
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
        transform.localScale = new Vector3(20f, 20f, 20f); // Forzamos escala
    }

    void Update()
    {
        if (jugador == null || estaMuerto)
            return;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        //Debug.Log($"📏 Distancia al jugador: {distancia}");

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

        if (direccion.x != 0)
            transform.localScale = new Vector3(20f * Mathf.Sign(direccion.x), 20f, 20f);
    }

    void Atacar()
    {
        atacando = true;
        tiempoUltimoAtaque = Time.time;

        if (jugador != null)
        {
            var saludJugador = jugador.GetComponent<SaludJugador>();
            var defensaJugador = jugador.GetComponent<DefensaJugador>();

            bool estaDefendiendo = defensaJugador != null && defensaJugador.EstaDefendiendo();

            if (saludJugador != null && !saludJugador.EstaMuerto)
            {
                if (!estaDefendiendo)
                {
                    Debug.Log("💥 Daño aplicado al jugador.");
                    saludJugador.RecibirDaño(dañoAtaque);
                }
                else
                {
                    Debug.Log("🛡️ Jugador se está defendiendo. No se aplica daño.");
                }
            }
            if (prefabHumo != null && puntoEfecto != null)
            {
                GameObject humo = Instantiate(prefabHumo, puntoEfecto.position, Quaternion.identity);
                humo.tag = "Humo"; // asegurarse por las dudas
                Destroy(humo, duracionHumo); // lo destruye luego de x segundos
                Debug.Log("🌫️ Se instanció el efecto de humo contaminante.");
            }

        }

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

        if (animator != null) animator.SetTrigger("Daño");

        if (saludActual <= 0) Morir();
    }

    void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (estaMuerto) return;

        if (collision.collider.CompareTag("Player") && Time.time > tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            Debug.Log("👊 Enemigo colisionando con el jugador, intenta atacar.");
            Atacar();
        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (estaMuerto) return;

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && puedeSerEliminado)
        {
            Debug.Log("🗡️ Jugador mató al enemigo con F");
            RecibirDaño(saludMaxima);
        }
    }

}
