/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
Updated: 12 July 2026 - added knockback on hit and removed the self-destruct so the dog keeps patrolling.
*/
using UnityEngine;

// Patrols between waypoints; on contact it damages and knocks the cat back, and
// re-damages on a cooldown while the cat stays in contact.
public class Dog : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float reachDistance = 0.3f;

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float damageCooldown = 1.5f;
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private AudioClip dogHitClip;

    private int currentWaypointIndex = 0;
    private float nextDamageTime = 0f;

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0f;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            patrolSpeed * Time.deltaTime
        );

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) <= reachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamagePlayer(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        DamagePlayer(collision.gameObject);
    }

    private void DamagePlayer(GameObject other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextDamageTime) return;

        nextDamageTime = Time.time + damageCooldown;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(dogHitClip);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DamagePlayer(damageAmount, transform.forward, knockbackForce);
        }
    }
}