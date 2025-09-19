using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 0.2f;

    [Header("Movement Settings")]
    public float speed = 0.125f;
    public float lifeTime = 30f;

    [Header("Visual Effects")]
    public float rotationSpeed = 360f; // Degrees per second

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private Rigidbody rb;
    private bool hasHitTarget = false;
    private Vector3 lastPosition;

    void Start()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("ArrowProjectile: No Rigidbody component found on " + gameObject.name);
        }

        // Store initial position for movement tracking
        lastPosition = transform.position;

        // Auto-destroy after lifetime
        Destroy(gameObject, lifeTime);

        if (enableDebugLogs)
        {
            Debug.Log("Flecha creada - Daño: " + damage + ", Velocidad: " + speed + ", Vida: " + lifeTime + "s");
        }
    }

    void Update()
    {
        // Rotate the arrow for visual effect
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Track movement for debugging
        if (enableDebugLogs && Vector3.Distance(lastPosition, transform.position) > 0.1f)
        {
            lastPosition = transform.position;
        }
    }

    /// <summary>
    /// Sets the arrow's velocity towards a target
    /// </summary>
    /// <param name="direction">Direction to shoot the arrow</param>
    public void SetDirection(Vector3 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;

            // Orient the arrow to face its movement direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (enableDebugLogs)
            {
                Debug.Log("Flecha lanzada en dirección: " + direction.normalized);
            }
        }
    }

    /// <summary>
    /// Launches the arrow towards a specific target position
    /// </summary>
    /// <param name="targetPosition">Position to aim at</param>
    public void LaunchTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        SetDirection(direction);
    }

    /// <summary>
    /// Launches the arrow towards a target transform
    /// </summary>
    /// <param name="target">Transform to aim at</param>
    public void LaunchTowards(Transform target)
    {
        if (target != null)
        {
            LaunchTowards(target.position + Vector3.up * 1.0f); // Aim slightly higher for better hit detection
        }
    }

    /// <summary>
    /// Detects collision with the player or other objects
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    void OnTriggerEnter(Collider other)
    {
        // Prevent multiple hits
        if (hasHitTarget)
        {
            return;
        }

        // Check if we hit the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Flecha impactó al jugador!");
            HandlePlayerHit(other.gameObject);
        }
        else if (other.gameObject != gameObject) // Ignore self
        {
            // Hit something else (wall, terrain, etc.)
            HandleEnvironmentHit(other.gameObject);
        }
    }

    /// <summary>
    /// Handles what happens when the arrow hits the player
    /// </summary>
    /// <param name="player">The player GameObject that was hit</param>
    private void HandlePlayerHit(GameObject player)
    {
        hasHitTarget = true;

        // Get player's health and invulnerability systems
        PlayerHealthSystem healthSystem = player.GetComponent<PlayerHealthSystem>();
        InvulnerabilitySystem invulnerabilitySystem = player.GetComponent<InvulnerabilitySystem>();

        if (healthSystem == null)
        {
            Debug.LogError("ArrowProjectile: No se encontró PlayerHealthSystem en el jugador " + player.name);
            DestroyArrow();
            return;
        }

        // Check if player is invulnerable
        if (invulnerabilitySystem != null && invulnerabilitySystem.isInvulnerable)
        {
            Debug.Log("Flecha impactó - Sin daño (invulnerable)");
        }
        else
        {
            // Apply damage
            healthSystem.TakeDamage(damage);
            Debug.Log("Flecha impactó - Daño aplicado: " + damage.ToString("F1"));
        }

        // Destroy the arrow after hitting
        DestroyArrow();
    }

    /// <summary>
    /// Handles what happens when the arrow hits the environment
    /// </summary>
    /// <param name="hitObject">The object that was hit</param>
    private void HandleEnvironmentHit(GameObject hitObject)
    {
        hasHitTarget = true;

        if (enableDebugLogs)
        {
            Debug.Log("Flecha impactó objeto: " + hitObject.name);
        }

        // Stop the arrow's movement
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Destroy after a short delay to show impact
        Destroy(gameObject, 1f);
    }

    /// <summary>
    /// Destroys the arrow immediately
    /// </summary>
    private void DestroyArrow()
    {
        if (enableDebugLogs)
        {
            Debug.Log("Flecha destruida tras impacto");
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Handle collision with non-trigger colliders
    /// </summary>
    /// <param name="collision">The collision data</param>
    void OnCollisionEnter(Collision collision)
    {
        if (!hasHitTarget)
        {
            HandleEnvironmentHit(collision.gameObject);
        }
    }

    /// <summary>
    /// For debugging - draws a line showing the arrow's trajectory
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 velocity = rb != null ? rb.linearVelocity : Vector3.forward;
        Gizmos.DrawLine(transform.position, transform.position + velocity.normalized * 2f);
    }
}