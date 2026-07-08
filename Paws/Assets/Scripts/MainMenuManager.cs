using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Flow")]
    [SerializeField] private string firstLevelSceneName = "Level 1";

    [Header("Panels")]
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Options")]
    [SerializeField] private Slider masterVolumeSlider;

    private const string VolumePrefKey = "MasterVolume";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        optionsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);

        // Load saved volume, default to full volume if none saved yet
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        masterVolumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenOptions()
    {
        mainButtonsPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }

    public void OnVolumeSliderChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(VolumePrefKey, value);
    }

    private void ApplyVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }
}