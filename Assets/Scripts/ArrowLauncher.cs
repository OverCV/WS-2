using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Prefab of the arrow to launch")]
    public GameObject arrowPrefab;
    
    [Header("Launch Points")]
    [Tooltip("Array of 3 transforms where arrows will be spawned")]
    public Transform[] launchPoints = new Transform[3];
    
    [Header("Launch Settings")]
    public float launchForce = 15f;
    public float spreadAngle = 10f; // Degrees of spread between arrows
    
    [Header("Audio")]
    public AudioClip launchSound;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    public bool showGizmosInScene = true;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && launchSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Validate setup
        ValidateConfiguration();
        
        if (enableDebugLogs)
        {
            Debug.Log("ArrowLauncher inicializado en: " + gameObject.name);
        }
    }
    
    /// <summary>
    /// Validates that the launcher is properly configured
    /// </summary>
    private void ValidateConfiguration()
    {
        // Check arrow prefab
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowLauncher: No arrow prefab assigned on " + gameObject.name);
        }
        
        // Check launch points
        int validLaunchPoints = 0;
        for (int i = 0; i < launchPoints.Length; i++)
        {
            if (launchPoints[i] != null)
            {
                validLaunchPoints++;
            }
            else
            {
                Debug.LogWarning("ArrowLauncher: Launch point " + i + " is not assigned on " + gameObject.name);
            }
        }
        
        if (validLaunchPoints == 0)
        {
            Debug.LogError("ArrowLauncher: No valid launch points found on " + gameObject.name);
        }
        else if (validLaunchPoints < 3)
        {
            Debug.LogWarning("ArrowLauncher: Only " + validLaunchPoints + " out of 3 launch points assigned on " + gameObject.name);
        }
    }
    
    /// <summary>
    /// Launches 3 arrows towards the target
    /// </summary>
    /// <param name="target">Target to aim at</param>
    public void LaunchArrows(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("ArrowLauncher: Target is null, cannot launch arrows");
            return;
        }
        
        LaunchArrows(target.position + Vector3.up * 1.0f); // Aim slightly higher for better hit detection
    }
    
    /// <summary>
    /// Launches 3 arrows towards a target position
    /// </summary>
    /// <param name="targetPosition">Position to aim at</param>
    public void LaunchArrows(Vector3 targetPosition)
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowLauncher: Cannot launch arrows, no prefab assigned");
            return;
        }
        
        Debug.Log("¡Flechas disparadas!");
        
        // Play launch sound
        PlayLaunchSound();
        
        // Launch from each valid launch point
        int arrowsLaunched = 0;
        for (int i = 0; i < launchPoints.Length; i++)
        {
            if (launchPoints[i] != null)
            {
                LaunchArrowFromPoint(launchPoints[i], targetPosition, i);
                arrowsLaunched++;
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("Lanzadas " + arrowsLaunched + " flechas hacia: " + targetPosition);
        }
    }
    
    /// <summary>
    /// Launches a single arrow from a specific launch point
    /// </summary>
    /// <param name="launchPoint">Point to launch from</param>
    /// <param name="targetPosition">Position to aim at</param>
    /// <param name="arrowIndex">Index of the arrow (for spread calculation)</param>
    private void LaunchArrowFromPoint(Transform launchPoint, Vector3 targetPosition, int arrowIndex)
    {
        // Calculate direction with spread
        Vector3 baseDirection = (targetPosition - launchPoint.position).normalized;
        Vector3 direction = ApplySpread(baseDirection, arrowIndex);
        
        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.LookRotation(direction));
        
        // Configure arrow
        ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(direction);
        }
        else
        {
            // Fallback: use Rigidbody directly
            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                arrowRb.linearVelocity = direction * launchForce;
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("Flecha " + (arrowIndex + 1) + " lanzada desde: " + launchPoint.name);
        }
    }
    
    /// <summary>
    /// Applies spread to the arrow direction based on its index
    /// </summary>
    /// <param name="baseDirection">Base direction to aim</param>
    /// <param name="arrowIndex">Index of the arrow (0, 1, 2)</param>
    /// <returns>Modified direction with spread applied</returns>
    private Vector3 ApplySpread(Vector3 baseDirection, int arrowIndex)
    {
        if (spreadAngle <= 0 || arrowIndex == 1) // Middle arrow goes straight
        {
            return baseDirection;
        }
        
        // Calculate spread offset
        float spreadOffset = 0f;
        if (arrowIndex == 0) // Left arrow
        {
            spreadOffset = -spreadAngle;
        }
        else if (arrowIndex == 2) // Right arrow
        {
            spreadOffset = spreadAngle;
        }
        
        // Apply horizontal spread
        Quaternion spreadRotation = Quaternion.AngleAxis(spreadOffset, Vector3.up);
        return spreadRotation * baseDirection;
    }
    
    /// <summary>
    /// Plays the launch sound effect
    /// </summary>
    private void PlayLaunchSound()
    {
        if (audioSource != null && launchSound != null)
        {
            audioSource.PlayOneShot(launchSound);
        }
    }
    
    /// <summary>
    /// Launches arrows in all directions (for testing)
    /// </summary>
    public void LaunchArrowsOmnidirectional()
    {
        Vector3[] directions = {
            transform.forward,
            transform.forward + transform.right,
            transform.forward - transform.right
        };
        
        for (int i = 0; i < launchPoints.Length && i < directions.Length; i++)
        {
            if (launchPoints[i] != null)
            {
                LaunchArrowFromPoint(launchPoints[i], launchPoints[i].position + directions[i] * 10f, i);
            }
        }
    }
    
    /// <summary>
    /// Creates 3 launch points as children if they don't exist
    /// </summary>
    [ContextMenu("Auto-Create Launch Points")]
    public void AutoCreateLaunchPoints()
    {
        for (int i = 0; i < 3; i++)
        {
            if (launchPoints[i] == null)
            {
                GameObject launchPoint = new GameObject("LaunchPoint_" + (i + 1));
                launchPoint.transform.SetParent(transform);
                
                // Position launch points in a horizontal line
                float xOffset = (i - 1) * 0.5f; // -0.5, 0, 0.5
                launchPoint.transform.localPosition = new Vector3(xOffset, 0, 0.5f);
                
                launchPoints[i] = launchPoint.transform;
                
                if (enableDebugLogs)
                {
                    Debug.Log("Created launch point: " + launchPoint.name);
                }
            }
        }
    }
    
    /// <summary>
    /// For debugging - draws gizmos showing launch points and directions
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!showGizmosInScene) return;
        
        // Draw launch points
        Gizmos.color = Color.yellow;
        for (int i = 0; i < launchPoints.Length; i++)
        {
            if (launchPoints[i] != null)
            {
                Gizmos.DrawWireSphere(launchPoints[i].position, 0.1f);
                
                // Draw launch direction
                Gizmos.color = Color.red;
                Gizmos.DrawLine(launchPoints[i].position, launchPoints[i].position + transform.forward * 2f);
                Gizmos.color = Color.yellow;
            }
        }
    }
}