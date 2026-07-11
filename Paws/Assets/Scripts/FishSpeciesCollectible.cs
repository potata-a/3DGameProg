/*
Author: Muqrie Rahimi
Student ID: 1211109977
*/
using UnityEngine;

public class FishSpeciesCollectible : MonoBehaviour
{
    [Header("Species Settings")]
    [SerializeField] private int speciesId;
    [SerializeField] private string speciesName;

    [Header("Collectable Settings")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private GameObject collectEffectPrefab;
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 1.5f;

    private bool collected = false;
    private bool playerInRange = false;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (collected) return;

        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
    }

    // Called by GameManager's raycast interaction system via SendMessage when the player looks at this object and presses E.
    public void Interact()
    {
        if (collected || !playerInRange) return;
        if (GameManager.Instance == null) return;

        bool isNewSpecies = GameManager.Instance.CollectFishSpecies(speciesId);
        if (!isNewSpecies) return;

        collected = true;
        playerInRange = false;

        SpawnCollectEffect();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(collectClip);
        }

        gameObject.SetActive(false);
    }

    private void SpawnCollectEffect()
    {
        if (collectEffectPrefab == null) return;

        GameObject fx = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            ps.Play();
        }

        Destroy(fx, 2f);
    }
}
