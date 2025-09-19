using UnityEngine;
using System.Linq;

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
    public float spreadAngle = 10f;

    [Header("Audio")]
    public AudioClip launchSound;

    [Header("Debug")]
    public bool enableDebugLogs = true;
    public bool showGizmosInScene = true;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && launchSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (launchPoints == null || launchPoints.Length == 0 || launchPoints.All(p => p == null))
        {
            AutoFindLaunchPoints();
        }

        ValidateConfiguration();

        if (enableDebugLogs)
        {
            Debug.Log("ArrowLauncher inicializado en: " + gameObject.name);
        }
    }

    private void AutoFindLaunchPoints()
    {
        launchPoints = new Transform[3];
        launchPoints[0] = transform.Find("LaunchPoint_1");
        launchPoints[1] = transform.Find("LaunchPoint_2");
        launchPoints[2] = transform.Find("LaunchPoint_3");

        if (enableDebugLogs)
        {
            int found = 0;
            for (int i = 0; i < launchPoints.Length; i++)
            {
                if (launchPoints[i] != null) found++;
            }
            Debug.Log("ArrowLauncher: Auto-found " + found + " launch points on " + gameObject.name);
        }
    }

    private void ValidateConfiguration()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowLauncher: No arrow prefab assigned on " + gameObject.name);
        }

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

    public void LaunchArrows(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("ArrowLauncher: Target is null, cannot launch arrows");
            return;
        }

        LaunchArrows(target.position + Vector3.up * 1.0f);
    }

    public void LaunchArrows(Vector3 targetPosition)
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowLauncher: Cannot launch arrows, no prefab assigned");
            return;
        }

        Debug.Log("¡Flechas disparadas!");
        PlayLaunchSound();

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

    private void LaunchArrowFromPoint(Transform launchPoint, Vector3 targetPosition, int arrowIndex)
    {
        Vector3 baseDirection = (targetPosition - launchPoint.position).normalized;
        Vector3 direction = ApplySpread(baseDirection, arrowIndex);

        GameObject arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.LookRotation(direction));

        ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(direction);
        }
        else
        {
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

    private Vector3 ApplySpread(Vector3 baseDirection, int arrowIndex)
    {
        if (spreadAngle <= 0 || arrowIndex == 1)
        {
            return baseDirection;
        }

        float spreadOffset = 0f;
        if (arrowIndex == 0)
        {
            spreadOffset = -spreadAngle;
        }
        else if (arrowIndex == 2)
        {
            spreadOffset = spreadAngle;
        }

        Quaternion spreadRotation = Quaternion.AngleAxis(spreadOffset, Vector3.up);
        return spreadRotation * baseDirection;
    }

    private void PlayLaunchSound()
    {
        if (audioSource != null && launchSound != null)
        {
            audioSource.PlayOneShot(launchSound);
        }
    }

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

    void OnDrawGizmosSelected()
    {
        if (!showGizmosInScene) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < launchPoints.Length; i++)
        {
            if (launchPoints[i] != null)
            {
                Gizmos.DrawWireSphere(launchPoints[i].position, 0.1f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(launchPoints[i].position, launchPoints[i].position + transform.forward * 2f);
                Gizmos.color = Color.yellow;
            }
        }
    }
}
