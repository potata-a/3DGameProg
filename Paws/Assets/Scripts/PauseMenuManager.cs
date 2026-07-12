/*
Author: Ahmad Aliff
Student ID: 1221309548
Date Created: 12 July 2026
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pauseButtonsPanel;
    [SerializeField] private GameObject pauseOptionsPanel;

    [Header("Options")]
    [SerializeField] private Slider masterVolumeSlider;

    [Header("Scene Flow")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    private const string VolumePrefKey = "MasterVolume";

    private void OnEnable()
    {
        pauseOptionsPanel.SetActive(false);
        pauseButtonsPanel.SetActive(true);

        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        masterVolumeSlider.value = savedVolume;
    }

    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenOptions()
    {
        pauseButtonsPanel.SetActive(false);
        pauseOptionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        pauseOptionsPanel.SetActive(false);
        pauseButtonsPanel.SetActive(true);
    }

    public void OnVolumeSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }

        PlayerPrefs.SetFloat(VolumePrefKey, value);
    }
}