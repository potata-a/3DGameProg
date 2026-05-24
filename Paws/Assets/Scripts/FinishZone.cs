/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
*/
using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TryWinGame();
        }
    }
}