/*
Author: Ahmad Aliff
Student ID: 1221309548
Date Created: 8 July 2026
*/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float displayDuration = 3f;

    [Header("Scene Flow")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    private void Start()
    {
        StartCoroutine(ShowSplashThenLoadMenu());
    }

    private IEnumerator ShowSplashThenLoadMenu()
    {
        yield return new WaitForSeconds(displayDuration);
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}