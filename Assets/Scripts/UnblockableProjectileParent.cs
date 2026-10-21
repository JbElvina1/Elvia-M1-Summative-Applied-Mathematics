using UnityEngine;

/// <summary>
/// Attach to the empty Parent object that holds the visual projectile.
/// Handles movement, lifespan, and destruction.
/// </summary>
public class UnblockableProjectileParent : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 6f;

    private Vector3 direction = Vector3.forward;

    void Start()
    {
        // Handles its own lifespan
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector3 dir, float spd = -1f)
    {
        direction = dir.normalized;
        if (spd > 0f) speed = spd;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}