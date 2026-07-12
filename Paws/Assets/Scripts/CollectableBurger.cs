/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 10 July 2026
*/
using UnityEngine;

// Burger objective pickup. Collected with the same raycast + E interaction the
// GameManager drives. Counts toward the mandatory burger target (win condition).
public class CollectableBurger : MonoBehaviour
{
    [Header("Collectable Settings")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 1f;

    private bool collected = false;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }

    private void Collect()
    {
        collected = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(collectClip);
        }

        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.PlayPickup(transform.position);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddBurger();
        }

        gameObject.SetActive(false);
    }

    // Called by GameManager's raycast interaction (SendMessage "Interact").
    public void Interact()
    {
        if (!collected)
        {
            Collect();
        }
    }
}
