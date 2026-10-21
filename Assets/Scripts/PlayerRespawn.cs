using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Leave empty to use the position the player starts at")]
    public Transform spawnPoint;
    public float invulnerableSeconds = 1f;

    [Header("Death Settings")]
    [Tooltip("How long to wait after dying before restarting the scene")]
    public float deathDelay = 1f;

    public int HitCount { get; private set; }
    public event Action<int> OnHit;

    CharacterController controller;
    MonoBehaviour motion; 
    Vector3 spawnPos;
    float invulnerableUntil;
    bool isDead = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        
        motion = GetComponent<PlayerController>() as MonoBehaviour;
        if (motion == null) motion = GetComponent<SlipperyPlayerController>() as MonoBehaviour;

        spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
    }

    public void Kill()
    {
        if (Time.time < invulnerableUntil || isDead) return;
        
        isDead = true;
        HitCount++;
        OnHit?.Invoke(HitCount);

        // Restart the scene instead of respawning
        StartCoroutine(RestartScene());
    }

    IEnumerator RestartScene()
    {
        // Freeze the player in place during the delay
        if (motion is PlayerController normal) normal.ResetMotion();
        else if (motion is SlipperyPlayerController slippery) slippery.ResetMotion();
        
        controller.enabled = false;

        // Wait so the player can see they died
        yield return new WaitForSeconds(deathDelay);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Keep this public in case you ever want to force a respawn without restarting
    public void Respawn()
    {
        controller.enabled = false;
        transform.position = spawnPos;
        controller.enabled = true;
        
        if (motion is PlayerController normal) normal.ResetMotion();
        else if (motion is SlipperyPlayerController slippery) slippery.ResetMotion();

        invulnerableUntil = Time.time + invulnerableSeconds;
    }
}