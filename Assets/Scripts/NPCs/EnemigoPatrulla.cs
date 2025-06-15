using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemigoPatrulla : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform[] waypoints;
    public float velocidad = 2f;
    public float distanciaMinima = 0.1f;
    private int indiceActual = 0;

    [Header("Ataque")]
    public float radioDeAtaque = 2f;
    public int daño = 10;
    public LayerMask capaJugador;
    public int dañoAlJugador = 10;

    private Rigidbody2D rb;

    [Header("Instanciación")]
    public bool instanciarAlInicio = false;
    public GameObject prefabDelNPC;
    public Transform puntoSpawn;

    [Header("Ataque al Jugador")]
    public float tiempoEntreAtaques = 2f;
    private float tiempoUltimoAtaque;
    private Animator animator;

    [Header("Efecto de Ataque")]
    public GameObject prefabHumo;
    public Transform puntoEfecto;
    public float duracionHumo = 2f;

    [Header("Salud")]
    public int saludMaxima = 3;
    private int saludActual;
    private bool estaMuerto = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Si está activado instanciar al inicio, crear copia y destruir el original
        if (instanciarAlInicio)
        {
            if (prefabDelNPC != null && puntoSpawn != null)
            {
                Instantiate(prefabDelNPC, puntoSpawn.position, Quaternion.identity);
                Debug.Log("📦 NPC instanciado desde sí mismo.");
                Destroy(gameObject); // Destruye este objeto "fantasma"
                return; // Evita seguir ejecutando Start en este objeto
            }
            else
            {
                Debug.LogError("⚠️ Prefab o punto de spawn no asignado.");
            }
        }

        // Inicialización normal del NPC real
        rb = GetComponent<Rigidbody2D>();

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("🚨 No hay waypoints asignados para el enemigo.");
            enabled = false;
        }
    }



    void Update()
    {
        Patrullar();
        VerificarJugadorEnZona();
    }

    void Patrullar()
    {
        Transform objetivo = waypoints[indiceActual];
        Vector2 direccion = (objetivo.position - transform.position).normalized;
        rb.velocity = direccion * velocidad;

        if (Vector2.Distance(transform.position, objetivo.position) < distanciaMinima)
        {
            indiceActual = (indiceActual + 1) % waypoints.Length;
        }
    }

    void VerificarJugadorEnZona()
    {
        Collider2D jugador = Physics2D.OverlapCircle(transform.position, radioDeAtaque, capaJugador);

        if (jugador != null)
        {
            float distanciaWaypoint = Vector2.Distance(transform.position, waypoints[indiceActual].position);

            // Solo daña si está suficientemente cerca del waypoint actual
            if (distanciaWaypoint <= 1f)
            {
                if (jugador.TryGetComponent(out SaludJugador vida))
                {
                    vida.RecibirDaño(daño);
                    Debug.Log("💥 Enemigo atacó al jugador.");
                }
            }
        }
    }

    void AtacarAlJugador(GameObject jugador)
    {
        if (Time.time < tiempoUltimoAtaque + tiempoEntreAtaques) return;
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        var saludJugador = jugador.GetComponent<SaludJugador>();
        var defensaJugador = jugador.GetComponent<DefensaJugador>();
        bool estaDefendiendo = defensaJugador != null && defensaJugador.EstaDefendiendo();

        if (saludJugador != null && !saludJugador.EstaMuerto)
        {
            if (!estaDefendiendo)
            {
                saludJugador.RecibirDaño(dañoAlJugador);
                Debug.Log("💥 Patrullero hizo daño al jugador.");
            }
            else
            {
                Debug.Log("🛡️ Jugador se está defendiendo, no recibe daño.");
            }
        }

        if (prefabHumo != null && puntoEfecto != null)
        {
            GameObject humo = Instantiate(prefabHumo, puntoEfecto.position, Quaternion.identity);
            Destroy(humo, duracionHumo);
            Debug.Log("🌫️ Humo generado por patrullero.");
        }

        tiempoUltimoAtaque = Time.time;
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeAtaque);
    }
    /*void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🚪 Entró al trigger con el jugador.");
            AtacarAlJugador(other.gameObject);
        }
    }*/
    public void RecibirDaño(int cantidad)
    {
        if (estaMuerto) return;

        saludActual -= cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        if (animator != null)
            animator.SetTrigger("Daño"); // si tenés animación de impacto

        if (saludActual <= 0)
            Morir();
    }
    void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        if (animator != null)
            animator.SetTrigger("Morir");

        // desactivar colisión
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // detener movimiento
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        // destruir el objeto después de un tiempo
        Destroy(gameObject, 2f);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AtacarAlJugador(other.gameObject);
        }
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("🗡️ El jugador atacó al NPC");
            RecibirDaño(1); // o el daño que quieras
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            AtacarAlJugador(collision.gameObject);
        }
    }


}
