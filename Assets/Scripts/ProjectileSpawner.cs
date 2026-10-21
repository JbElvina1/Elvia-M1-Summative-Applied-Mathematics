using System.Collections;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab; // Drag the PARENT empty object here
    public float spawnInterval = 2f;
    public float spreadAngle = 0f;

    void Start()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned on " + gameObject.name);
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Quaternion spawnRotation = transform.rotation;
            if (spreadAngle > 0f)
            {
                float randomYaw = Random.Range(-spreadAngle, spreadAngle);
                float randomPitch = Random.Range(-spreadAngle, spreadAngle);
                spawnRotation *= Quaternion.Euler(randomPitch, randomYaw, 0f);
            }

            // Spawn the Parent object
            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, spawnRotation);

            // Tell the parent script which way to fly
            UnblockableProjectileParent ubp = newProjectile.GetComponent<UnblockableProjectileParent>();
            if (ubp != null)
            {
                ubp.Launch(spawnRotation * Vector3.forward);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 5f);
    }
}