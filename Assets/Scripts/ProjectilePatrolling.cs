using UnityEngine;

public class PatrollingProjectile : MonoBehaviour
{
    public enum StartDirection { Forward, Backward, Right, Left, Up, Down }

    [Header("Movement Settings")]
    public StartDirection startingDirection = StartDirection.Forward;
    public float speed = 5f;
    public float travelTime = 3f;
    public float pauseTime = 0.5f;
    public float rotationSpeed = 180f;

    [Header("Hitbox Reference")]
    [Tooltip("Drag the Child object with the Collider here. Leave empty to auto-find on children.")]
    public Collider hitbox;

    private Vector3 moveDirection;
    private float timer = 0f;
    private int state = 0; 

    private Transform childTransform;
    private Quaternion childStartLocalRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Auto-find child and rotation
        if (transform.childCount > 0)
        {
            childTransform = transform.GetChild(0);
            childStartLocalRotation = childTransform.localRotation;
        }

        // Auto-find the collider if not assigned
        if (hitbox == null)
        {
            hitbox = GetComponentInChildren<Collider>();
        }

        // Make sure the collider is set as a trigger
        if (hitbox != null) hitbox.isTrigger = true;

        // Set starting direction
        switch (startingDirection)
        {
            case StartDirection.Forward:  moveDirection = Vector3.forward; break;
            case StartDirection.Backward: moveDirection = Vector3.back; break;
            case StartDirection.Right:    moveDirection = Vector3.right; break;
            case StartDirection.Left:     moveDirection = Vector3.left; break;
            case StartDirection.Up:       moveDirection = Vector3.up; break;
            case StartDirection.Down:     moveDirection = Vector3.down; break;
        }

        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    void Update()
    {
        if (state == 0)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= travelTime) { state = 1; timer = 0f; }
        }
        else if (state == 1)
        {
            timer += Time.deltaTime;
            if (timer >= pauseTime)
            {
                state = 2; timer = 0f;
                targetRotation = Quaternion.LookRotation(-moveDirection);
            }
        }
        else if (state == 2)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                transform.rotation = targetRotation;
                moveDirection = -moveDirection; 
                state = 0; timer = 0f;
            }
        }

        // Lock the child's visual rotation
        if (childTransform != null) childTransform.localRotation = childStartLocalRotation;
    }

    // --- Trigger detection handled by the Parent ---
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerRespawn>();
        if (player != null)
        {
            player.Kill();
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}