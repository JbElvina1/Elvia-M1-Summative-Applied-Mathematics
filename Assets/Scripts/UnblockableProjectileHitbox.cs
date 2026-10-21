using UnityEngine;

/// <summary>
/// Attach this to the CHILD object that has the Collider.
/// It detects the hit and destroys the whole parent hierarchy.
/// </summary>
public class UnblockableHitbox : MonoBehaviour
{
    void Awake()
    {
        // Ensure the collider is set up correctly
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other) => TryHit(other);
    void OnTriggerStay(Collider other) => TryHit(other);

    void TryHit(Collider other)
    {
        // 1. Did we hit the player? Kill them.
        var player = other.GetComponentInParent<PlayerRespawn>();
        if (player != null)
        {
            player.Kill();
            Destroy(transform.parent.gameObject); // Destroy the parent (which destroys the child)
            return;
        }

        // 2. Did we hit a wall? Destroy.
        if (other.CompareTag("Wall"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}