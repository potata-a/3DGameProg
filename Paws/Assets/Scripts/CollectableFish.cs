/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
Updated: 10 July 2026 - repurposed from the burger counter to add the fish to the player's inventory instead.
Updated: 11 July 2026 - plays the shared pickup particle effect on collect.
*/
using UnityEngine;

// Fish pickup. Uses the same raycast + E interaction as the burger, but instead of
// counting toward the win condition it is added to the player's fish inventory
// (a carried fish that can later be eaten to heal, plus a permanent collection entry).
public class CollectableFish : MonoBehaviour
{
    [Header("Fish Data")]
    [Tooltip("Which of the 16 fish this pickup represents.")]
    [SerializeField] private FishData fishData;

    [Header("Collectable Settings")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 1.5f;

    private bool collected = false;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Spin and gently bob so the pickup reads as collectable.
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }

    private void CollectFish()
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

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.CollectFish(fishData);
        }

        gameObject.SetActive(false);
    }

    // Called by GameManager's raycast interaction (SendMessage "Interact").
    public void Interact()
    {
        if (!collected)
        {
            CollectFish();
        }
    }
}
