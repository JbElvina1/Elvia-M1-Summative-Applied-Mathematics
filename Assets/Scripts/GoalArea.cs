using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// A stationary goal zone. When the player walks into the trigger, they win.
/// Attach to a GameObject with a Trigger Collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GoalArea : MonoBehaviour
{
    [Header("Level flow")]
    [Tooltip("Scene to load after winning. Leave empty if you don't want to load a new scene.")]
    public string nextSceneName = "Level3";
    public float loadDelay = 1f;
    public UnityEvent onCollected; 

    [Header("Feedback")]
    public AudioClip winSound;
    public ParticleSystem winParticles; // Optional: drag a particle system here to play on win

    private bool collected = false;

    void Awake()
    {
        // Ensure the collider is a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // Check for either player controller
        bool isPlayer = other.GetComponentInParent<PlayerController>() != null || 
                        other.GetComponentInParent<SlipperyPlayerController>() != null;

        if (!isPlayer) return;

        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        collected = true;

        // Optional visual/audio feedback
        if (winSound != null) AudioSource.PlayClipAtPoint(winSound, transform.position);
        if (winParticles != null) winParticles.Play();
        onCollected?.Invoke();

        // Wait for the delay
        yield return new WaitForSeconds(loadDelay);

        // Load the next scene if one is assigned
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Goal Area reached! (No next scene assigned)");
        }
    }
}