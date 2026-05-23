/*
Author: Muqrie Rahimi
Student ID: YOUR_STUDENT_ID
Date Created: 23 May 2026
*/

using UnityEngine;

public class CollectableFish : MonoBehaviour
{
    [Header("Collectable Settings")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private float rotateSpeed = 90f;

    private bool collected = false;

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is near collectable. Press E to pick up.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left collectable area.");
        }
    }

    private void CollectFish()
    {
        collected = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(collectClip);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddFish();
        }

        gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (!collected)
        {
            CollectFish();
        }
    }
}