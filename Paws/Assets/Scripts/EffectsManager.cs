/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 11 July 2026
*/
using UnityEngine;

// Central spawner for one-shot particle effects, mirroring the AudioManager pattern.
// Effects auto-destroy (the prefabs use Stop Action = Destroy, with a timed fallback).
public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;

    [Header("Particle Prefabs")]
    [SerializeField] private ParticleSystem damageEffect;
    [SerializeField] private ParticleSystem pickupEffect;
    [SerializeField] private ParticleSystem healEffect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Player-centred effects follow the given transform.
    public void PlayDamage(Transform target)
    {
        SpawnOn(damageEffect, target, 0.5f);
    }

    public void PlayHeal(Transform target)
    {
        SpawnOn(healEffect, target, 0.3f);
    }

    // World-space effect at a fixed position for any "pickup" action (burger or fish).
    public void PlayPickup(Vector3 position)
    {
        SpawnAt(pickupEffect, position);
    }

    private void SpawnOn(ParticleSystem prefab, Transform target, float heightOffset)
    {
        if (prefab == null || target == null) return;

        Vector3 pos = target.position + Vector3.up * heightOffset;
        ParticleSystem ps = Instantiate(prefab, pos, Quaternion.identity, target);
        ps.Play();
        Destroy(ps.gameObject, LifetimeOf(ps));
    }

    private void SpawnAt(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null) return;

        ParticleSystem ps = Instantiate(prefab, position, Quaternion.identity);
        ps.Play();
        Destroy(ps.gameObject, LifetimeOf(ps));
    }

    private float LifetimeOf(ParticleSystem ps)
    {
        return ps.main.duration + ps.main.startLifetime.constantMax + 0.5f;
    }
}
