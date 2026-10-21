using UnityEngine;

/// <summary>
/// Moves back and forth instantly (no turning, no pause).
/// Attach to the PARENT object of the projectile.
/// </summary>
public class PingPongProjectile : MonoBehaviour
{
    public enum StartDirection
    {
        Forward,    // +Z
        Backward,   // -Z
        Right,      // +X
        Left,       // -X
        Up,         // +Y
        Down        // -Y
    }

    [Header("Movement Settings")]
    public StartDirection startingDirection = StartDirection.Forward;
    public float speed = 5f;

    [Tooltip("How long it moves in one direction before reversing")]
    public float travelTime = 3f;

    private Vector3 moveDirection;
    private float timer = 0f;

    // Child rotation lock (so the model doesn't spin)
    private Transform childTransform;
    private Quaternion childStartWorldRotation;

    void Start()
    {
        // Grab the child so we can keep its visual rotation locked
        if (transform.childCount > 0)
        {
            childTransform = transform.GetChild(0);
            childStartWorldRotation = childTransform.rotation;
        }

        // Set starting direction from the dropdown
        switch (startingDirection)
        {
            case StartDirection.Forward:  moveDirection = Vector3.forward; break;
            case StartDirection.Backward: moveDirection = Vector3.back; break;
            case StartDirection.Right:    moveDirection = Vector3.right; break;
            case StartDirection.Left:     moveDirection = Vector3.left; break;
            case StartDirection.Up:       moveDirection = Vector3.up; break;
            case StartDirection.Down:     moveDirection = Vector3.down; break;
        }
    }

    void Update()
    {
        // Move forward
        transform.position += moveDirection * speed * Time.deltaTime;

        // Count down the travel time
        timer += Time.deltaTime;
        if (timer >= travelTime)
        {
            // Instant 180 flip — no rotation, no pause
            moveDirection = -moveDirection;
            timer = 0f;
        }

        // Keep the child's visual rotation locked so the model doesn't spin
        if (childTransform != null)
        {
            childTransform.rotation = childStartWorldRotation;
        }
    }
}