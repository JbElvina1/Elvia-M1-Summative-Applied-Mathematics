using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [Header("Homing Settings")]
    public float speed = 15f;
    public float lifetime = 8f; 

    private Transform playerTarget;
    private bool isActive = false;
    private Vector3 lockedTargetPosition;

    private Transform childTransform;
    private Quaternion childStartLocalRotation;
    
    // Store the Child's world rotation so we can restore it
    private Quaternion childStartWorldRotation;

    void Start()
    {
        var normalPlayer = FindObjectOfType<PlayerController>();
        var slipperyPlayer = FindObjectOfType<SlipperyPlayerController>();

        if (normalPlayer != null) playerTarget = normalPlayer.transform;
        else if (slipperyPlayer != null) playerTarget = slipperyPlayer.transform;

        if (transform.childCount > 0)
        {
            childTransform = transform.GetChild(0);
            childStartLocalRotation = childTransform.localRotation;
            childStartWorldRotation = childTransform.rotation;
        }
    }

    public void Activate()
    {
        if (playerTarget == null) return;

        lockedTargetPosition = playerTarget.position;

        Vector3 direction = (lockedTargetPosition - transform.position).normalized;
        
        // Rotate the parent, but we don't need to snap it. The child will handle the visuals.
        transform.rotation = Quaternion.LookRotation(direction);

        isActive = true;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isActive) return;

        transform.position += transform.forward * speed * Time.deltaTime;

        // --- THE FIX: Restore the child's WORLD rotation every frame ---
        // This ignores the parent's rotation entirely
        if (childTransform != null)
        {
            childTransform.rotation = childStartWorldRotation;
        }
    }

    public void Block()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerRespawn>();
        if (player != null)
        {
            player.Kill();
            Destroy(gameObject); 
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}