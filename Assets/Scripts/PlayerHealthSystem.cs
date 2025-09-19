using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float currentHealth = 1.0f;
    
    [Header("References")]
    private InvulnerabilitySystem invulnerabilitySystem;
    
    void Start()
    {
        // Get reference to InvulnerabilitySystem on the same GameObject
        invulnerabilitySystem = GetComponent<InvulnerabilitySystem>();
        
        if (invulnerabilitySystem == null)
        {
            Debug.LogError("PlayerHealthSystem: InvulnerabilitySystem not found on " + gameObject.name);
        }
        
        // Log initial health
        Debug.Log("Vida actual: " + currentHealth.ToString("F1"));
    }
    
    /// <summary>
    /// Applies damage to the player if not invulnerable
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    public void TakeDamage(float damage)
    {
        // Check if player is invulnerable
        if (invulnerabilitySystem != null && invulnerabilitySystem.isInvulnerable)
        {
            Debug.Log("Jugador es invulnerable - Daño bloqueado: " + damage.ToString("F1"));
            return;
        }
        
        // Apply damage
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth); // Prevent negative health
        
        // Log health change
        Debug.Log("Vida actual: " + currentHealth.ToString("F1"));
        
        // Check if player died
        if (currentHealth <= 0f)
        {
            Debug.Log("¡Jugador murió! Vida llegó a 0");
            OnPlayerDeath();
        }
    }
    
    /// <summary>
    /// Checks if the player is still alive
    /// </summary>
    /// <returns>True if health is greater than 0</returns>
    public bool IsAlive()
    {
        return currentHealth > 0f;
    }
    
    /// <summary>
    /// Gets the current health value
    /// </summary>
    /// <returns>Current health as float</returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    /// <summary>
    /// Resets health to maximum
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = 1.0f;
        Debug.Log("Vida restaurada: " + currentHealth.ToString("F1"));
    }
    
    /// <summary>
    /// Called when player dies
    /// </summary>
    private void OnPlayerDeath()
    {
        // You can add death behavior here (disable movement, show death UI, etc.)
        Debug.Log("Ejecutando comportamiento de muerte del jugador");
    }
    
    /// <summary>
    /// For debugging - display health in inspector
    /// </summary>
    void Update()
    {
        // Optional: You can remove this in production
        // Just for real-time monitoring in inspector
    }
}