using UnityEngine;

public class TrapWallDetector : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("Time in seconds before the trap can be triggered again")]
    public float cooldownTime = 2f;
    
    [Header("References")]
    [Tooltip("Reference to the ArrowLauncher component")]
    public ArrowLauncher arrowLauncher;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private bool isOnCooldown = false;
    private float lastTriggerTime = 0f;

    void Start()
    {
        if (arrowLauncher == null)
        {
            arrowLauncher = GetComponentInParent<ArrowLauncher>();
            if (arrowLauncher != null && enableDebugLogs)
            {
                Debug.Log("TrapWallDetector: Auto-found ArrowLauncher on " + arrowLauncher.gameObject.name);
            }
        }
        
        if (arrowLauncher == null)
        {
            Debug.LogError("TrapWallDetector: No ArrowLauncher found on " + gameObject.name + " or parent objects!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOnCooldown)
        {
            if (enableDebugLogs)
            {
                Debug.Log("TrapWallDetector: Player detected! Triggering trap...");
            }
            
            TriggerTrap(other.transform);
        }
    }

    private void TriggerTrap(Transform target)
    {
        if (arrowLauncher == null)
        {
            Debug.LogError("TrapWallDetector: Cannot trigger trap, no ArrowLauncher assigned!");
            return;
        }

        isOnCooldown = true;
        lastTriggerTime = Time.time;

        arrowLauncher.LaunchArrows(target);

        if (enableDebugLogs)
        {
            Debug.Log("TrapWallDetector: Trap triggered! Cooldown for " + cooldownTime + " seconds");
        }

        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    private void ResetCooldown()
    {
        isOnCooldown = false;
        
        if (enableDebugLogs)
        {
            Debug.Log("TrapWallDetector: Trap ready again!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isOnCooldown ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
