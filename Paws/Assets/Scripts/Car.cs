/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
Updated: 12 July 2026 - on hit, flings the cat along the car's travel direction (knockback) with brief invulnerability.
*/
using UnityEngine;

// Trigger volume that spawns a car which drives across the road; on collision the car
// damages and knocks the cat back, then despawns.
public class Car : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float carSpeed = 8f;
    [SerializeField] private float spawnCooldown = 2f;

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 9f;
    [SerializeField] private AudioClip carHitClip;

    private float nextSpawnTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SpawnCar();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnCar();
        }
    }

    public void SpawnCar()
    {
        if (carPrefab == null || spawnPoint == null || endPoint == null) return;
        if (Time.time < nextSpawnTime) return;

        nextSpawnTime = Time.time + spawnCooldown;

        GameObject spawnedCar = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);

        MovingCar movingCar = spawnedCar.GetComponent<MovingCar>();

        if (movingCar == null)
        {
            movingCar = spawnedCar.AddComponent<MovingCar>();
        }

        movingCar.Setup(endPoint, carSpeed, damageAmount, carHitClip, knockbackForce);
    }
}

class MovingCar : MonoBehaviour
{
    private Transform endPoint;
    private float speed;
    private int damageAmount;
    private AudioClip hitClip;
    private float knockbackForce;
    private bool hasHitPlayer = false;

    public void Setup(Transform targetEndPoint, float moveSpeed, int damage, AudioClip clip, float knockback)
    {
        endPoint = targetEndPoint;
        speed = moveSpeed;
        damageAmount = damage;
        hitClip = clip;
        knockbackForce = knockback;
    }

    private void Update()
    {
        if (endPoint == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, endPoint.position) <= 0.3f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHitPlayer) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        hasHitPlayer = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hitClip);
        }

        if (GameManager.Instance != null)
        {
            // Fling the cat along the car's travel direction (toward its endpoint).
            Vector3 travelDir = endPoint != null
                ? (endPoint.position - transform.position)
                : transform.forward;
            GameManager.Instance.DamagePlayer(damageAmount, travelDir, knockbackForce);
        }

        Destroy(gameObject, 0.2f);
    }
}