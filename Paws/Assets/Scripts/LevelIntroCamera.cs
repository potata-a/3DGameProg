/*
Author: Ahmad Aliff
Student ID: 1221309548
Date Created: 12 July 2026
*/
using System.Collections;
using UnityEngine;

public class LevelIntroCamera : MonoBehaviour
{
    [Header("Cinematic Points")]
    [SerializeField] private Transform introStartPoint;
    [SerializeField] private Transform introEndPoint;
    [SerializeField] private float introDuration = 5f;

    [Header("References")]
    [SerializeField] private PlayerManager playerManager;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        if (playerManager != null)
            playerManager.enabled = false;

        transform.position = introStartPoint.position;
        transform.rotation = introStartPoint.rotation;

        float elapsed = 0f;
        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / introDuration;

            transform.position = Vector3.Lerp(introStartPoint.position, introEndPoint.position, t);
            transform.rotation = Quaternion.Slerp(introStartPoint.rotation, introEndPoint.rotation, t);

            yield return null;
        }

        if (playerManager != null)
            playerManager.enabled = true;
    }
}