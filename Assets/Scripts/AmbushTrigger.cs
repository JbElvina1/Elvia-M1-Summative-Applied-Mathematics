using UnityEngine;

/// <summary>
/// When the player steps into this invisible trigger, it activates the placed bird.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AmbushTrigger : MonoBehaviour
{
    [Tooltip("Drag the bird that is already in your scene here")]
    public HomingProjectile projectileToActivate;

    private bool triggered = false;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Is it the player?
        bool isPlayer = other.GetComponentInParent<PlayerController>() != null || 
                        other.GetComponentInParent<SlipperyPlayerController>() != null;

        if (!isPlayer) return;

        triggered = true;

        // Activate the bird!
        if (projectileToActivate != null)
        {
            projectileToActivate.Activate();
        }
        else
        {
            Debug.LogError("No projectile assigned to the Ambush Trigger!");
        }
    }
}