/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
*/
using UnityEngine;

public class CollectableFish : MonoBehaviour
{
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
        // Rotate
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

        // Float up and down
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
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
            GameManager.Instance.AddBurger();
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