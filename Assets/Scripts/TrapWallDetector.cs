using UnityEngine;

public class TrapWallDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Cooldown time between trap activations in seconds")]
    public float cooldownTime = 2.0f;
    
    [Header("References")]
    [Tooltip("Reference to the ArrowLauncher component (usually on parent)")]
    public ArrowLauncher arrowLauncher;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private bool isOnCooldown = false;
    private float lastActivationTime = 0f;
    
    void Start()
    {
        // Auto-find ArrowLauncher if not assigned
        if (arrowLauncher == null)
        {
            // First try to get it from the parent
            arrowLauncher = GetComponentInParent<ArrowLauncher>();
            
            // If not found in parent, try to get it from the same object
            if (arrowLauncher == null)
            {
                arrowLauncher = GetComponent<ArrowLauncher>();
            }
        }
        
        // Validate configuration
        ValidateSetup();
        
        if (enableDebugLogs)
        {
            Debug.Log("TrapWallDetector inicializado en: " + gameObject.name);
        }
    }
    
    /// <summary>
    /// Validates that the detector is properly configured
    /// </summary>
    private void ValidateSetup()
    {
        // Check if we have a trigger collider
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogError("TrapWallDetector: No Collider found on " + gameObject.name);
        }
        else if (!triggerCollider.isTrigger)
        {
            Debug.LogError("TrapWallDetector: Collider on " + gameObject.name + " must be set as Trigger");
        }
        
        // Check if we have ArrowLauncher reference
        if (arrowLauncher == null)
        {
            Debug.LogError("TrapWallDetector: No ArrowLauncher reference found on " + gameObject.name + " or its parent");
        }
    }
    
    /// <summary>
    /// Detects when the player enters the trigger zone
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            if (enableDebugLogs)
            {
                Debug.Log("¡Jugador detectado cerca de la pared trampa!");
            }
            
            // Try to activate the trap
            TryActivateTrap(other.gameObject);
        }
    }
    
    /// <summary>
    /// Attempts to activate the trap if not on cooldown
    /// </summary>
    /// <param name="player">The player GameObject that triggered the trap</param>
    private void TryActivateTrap(GameObject player)
    {
        // Check if we're on cooldown
        if (IsOnCooldown())
        {
            if (enableDebugLogs)
            {
                float remainingCooldown = cooldownTime - (Time.time - lastActivationTime);
                Debug.Log("Trampa en cooldown. Tiempo restante: " + remainingCooldown.ToString("F1") + "s");
            }
            return;
        }
        
        // Check if ArrowLauncher is available
        if (arrowLauncher == null)
        {
            Debug.LogError("TrapWallDetector: No se puede disparar, ArrowLauncher no encontrado");
            return;
        }
        
        // Activate the trap
        ActivateTrap(player);
    }
    
    /// <summary>
    /// Activates the trap and launches arrows
    /// </summary>
    /// <param name="player">The player who triggered the trap</param>
    private void ActivateTrap(GameObject player)
    {
        // Log activation
        Debug.Log("¡Jugador detectado! Disparando flechas!");
        
        // Set cooldown
        lastActivationTime = Time.time;
        isOnCooldown = true;
        
        // Launch arrows through the ArrowLauncher
        arrowLauncher.LaunchArrows(player.transform);
        
        // Start cooldown coroutine
        StartCoroutine(CooldownCoroutine());
        
        if (enableDebugLogs)
        {
            Debug.Log("Trampa activada. Próxima activación disponible en " + cooldownTime + " segundos");
        }
    }
    
    /// <summary>
    /// Checks if the trap is currently on cooldown
    /// </summary>
    /// <returns>True if on cooldown, false otherwise</returns>
    private bool IsOnCooldown()
    {
        return isOnCooldown && (Time.time - lastActivationTime < cooldownTime);
    }
    
    /// <summary>
    /// Coroutine to handle cooldown timing
    /// </summary>
    private System.Collections.IEnumerator CooldownCoroutine()
    {
        // Wait for cooldown time
        yield return new WaitForSeconds(cooldownTime);
        
        // Reset cooldown
        isOnCooldown = false;
        
        if (enableDebugLogs)
        {
            Debug.Log("Cooldown de trampa terminado. Lista para activar nuevamente");
        }
    }
    
    /// <summary>
    /// Manually reset the cooldown (useful for testing)
    /// </summary>
    public void ResetCooldown()
    {
        isOnCooldown = false;
        lastActivationTime = 0f;
        StopAllCoroutines();
        
        if (enableDebugLogs)
        {
            Debug.Log("Cooldown de trampa reiniciado manualmente");
        }
    }
    
    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    /// <returns>Remaining cooldown time in seconds</returns>
    public float GetRemainingCooldown()
    {
        if (!isOnCooldown) return 0f;
        return Mathf.Max(0f, cooldownTime - (Time.time - lastActivationTime));
    }
    
    /// <summary>
    /// For debugging - draws the trigger area in the scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}