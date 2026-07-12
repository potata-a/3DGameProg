/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 12 July 2026
*/
using System.Collections;
using UnityEngine;

// Handles the cat's reaction to being hit by a hazard (car / dog):
//  - Knockback: flings the cat along the hazard's travel direction. The knockback is
//    an additive, decaying velocity (see CurrentKnockback) that PlayerController layers
//    on top of movement, so the fling still works even while the player holds an input.
//  - Hit-stun: a short window where the player can't control the cat at all (the punch).
//  - Invulnerability + flash: brief i-frames so overlapping hazards can't chain-drain
//    health, with the cat blinking to signal it.
public class PlayerHitReaction : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField] private float upwardForce = 3f;
    [SerializeField] private float staggerDuration = 0.1f;    // short "can't act" punch
    [SerializeField] private float knockbackDuration = 0.45f; // how long the fling pushes/decays

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private float flashInterval = 0.1f;

    private Rigidbody rb;
    private Renderer[] renderers;

    private Vector3 knockbackVelocity;   // horizontal fling velocity at full strength
    private float knockbackTimer = 0f;
    private float staggerTimer = 0f;
    private float invulnTimer = 0f;
    private Coroutine flashRoutine;

    public bool IsStaggered => staggerTimer > 0f;
    public bool IsInvulnerable => invulnTimer > 0f;

    // Decaying knockback that PlayerController adds on top of input movement.
    public Vector3 CurrentKnockback
    {
        get
        {
            if (knockbackTimer <= 0f) return Vector3.zero;
            return knockbackVelocity * (knockbackTimer / knockbackDuration);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Update()
    {
        if (staggerTimer > 0f) staggerTimer -= Time.deltaTime;
        if (knockbackTimer > 0f) knockbackTimer -= Time.deltaTime;
        if (invulnTimer > 0f) invulnTimer -= Time.deltaTime;
    }

    // direction = the hazard's travel direction (the cat is flung this way).
    public void TakeHit(Vector3 direction, float force)
    {
        if (force > 0f && rb != null)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) direction = -transform.forward; // fallback
            direction.Normalize();

            knockbackVelocity = direction * force;
            knockbackTimer = knockbackDuration;
            staggerTimer = staggerDuration;

            // Initial impulse (horizontal fling + upward pop).
            rb.velocity = knockbackVelocity + Vector3.up * upwardForce;
        }

        invulnTimer = invulnerabilityDuration;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        bool visible = true;
        while (invulnTimer > 0f)
        {
            visible = !visible;
            SetRenderersEnabled(visible);
            yield return new WaitForSeconds(flashInterval);
        }
        SetRenderersEnabled(true);
        flashRoutine = null;
    }

    private void SetRenderersEnabled(bool value)
    {
        if (renderers == null) return;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null) renderers[i].enabled = value;
        }
    }
}
