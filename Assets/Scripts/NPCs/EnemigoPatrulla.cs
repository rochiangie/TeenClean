using UnityEngine;

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
    public int daño = 1;
    public LayerMask capaJugador;

    private Rigidbody2D rb;

    [Header("Instanciación")]
    public bool instanciarAlInicio = false;
    public GameObject prefabDelNPC;
    public Transform puntoSpawn;

    void Start()
    {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeAtaque);
    }
}
