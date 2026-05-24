/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
*/
using UnityEngine;

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

        movingCar.Setup(endPoint, carSpeed, damageAmount, carHitClip);
    }
}

class MovingCar : MonoBehaviour
{
    private Transform endPoint;
    private float speed;
    private int damageAmount;
    private AudioClip hitClip;
    private bool hasHitPlayer = false;

    public void Setup(Transform targetEndPoint, float moveSpeed, int damage, AudioClip clip)
    {
        endPoint = targetEndPoint;
        speed = moveSpeed;
        damageAmount = damage;
        hitClip = clip;
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
            GameManager.Instance.DamagePlayer(damageAmount);
        }

        Destroy(gameObject, 0.2f);
    }
}