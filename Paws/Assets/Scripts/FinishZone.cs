/*
Author: Muqrie Rahimi
Student ID: YOUR_STUDENT_ID
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