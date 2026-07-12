/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 12 July 2026
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Drives the on-screen HUD using real UGUI elements (replaces the old GameManager
// OnGUI/IMGUI drawing). Reads live state from GameManager and PlayerInventory, and
// shows the Game Over / Victory screen with the saved data (high score + total fish).
public class GameHUD : MonoBehaviour
{
    [Header("Live Stats (top-left)")]
    [SerializeField] private TMP_Text statsText;

    [Header("Pause Message")]
    [SerializeField] private TMP_Text centerMessageText;

    [Header("Game Over / Victory Screen")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverTitle;
    [SerializeField] private TMP_Text gameOverStats;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    [Header("Fish Collection Panel (hold Tab)")]
    [SerializeField] private KeyCode collectionKey = KeyCode.Tab;
    [SerializeField] private GameObject collectionPanel;
    [SerializeField] private Transform collectionGrid;
    [SerializeField] private FishSlotUI fishSlotPrefab;
    [SerializeField] private TMP_Text collectionTitle;

    private FishSlotUI[] slots;
    private bool endScreenShown = false;

    private void Start()
    {
        BuildCollectionSlots();

        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (centerMessageText != null) centerMessageText.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (nextButton != null) nextButton.onClick.AddListener(GoToNextLevel);
        if (retryButton != null) retryButton.onClick.AddListener(Retry);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void BuildCollectionSlots()
    {
        if (fishSlotPrefab == null || collectionGrid == null || PlayerInventory.Instance == null) return;

        FishData[] all = PlayerInventory.Instance.AllFishTypes;
        if (all == null) return;

        slots = new FishSlotUI[all.Length];
        for (int i = 0; i < all.Length; i++)
        {
            slots[i] = Instantiate(fishSlotPrefab, collectionGrid);
        }
    }

    private void Update()
    {
        UpdateStats();
        UpdatePauseMessage();
        UpdateEndScreen();
        HandleCollectionPanel();
    }

    private void UpdateStats()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || statsText == null) return;

        string fishLine = "Fish: 0   (Collection 0/0)";
        if (PlayerInventory.Instance != null)
        {
            fishLine = "Fish: " + PlayerInventory.Instance.CarriedFish +
                       "   (Collection " + PlayerInventory.Instance.UniqueCollected +
                       "/" + PlayerInventory.Instance.TotalFishTypes + ")";
        }

        string scoreLine = ScoreManager.Instance != null
            ? "Score: " + ScoreManager.Instance.Score + "\n"
            : "";

        statsText.text =
            scoreLine +
            "Burger: " + gm.BurgerCollected + "/" + gm.BurgerTarget + "\n" +
            "Health: " + gm.PlayerHealth + "/" + gm.MaxHealth + "\n" +
            "Time: " + Mathf.CeilToInt(gm.TimeRemaining) + "\n" +
            fishLine;
    }

    private void UpdatePauseMessage()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || centerMessageText == null) return;

        bool showPause = gm.IsPaused && !gm.IsGameEnded;
        if (centerMessageText.gameObject.activeSelf != showPause)
        {
            centerMessageText.gameObject.SetActive(showPause);
        }
        if (showPause)
        {
            centerMessageText.text = "PAUSED\n<size=60%>Press ESC to resume</size>";
        }
    }

    private void UpdateEndScreen()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gameOverPanel == null) return;

        if (gm.IsGameEnded && !endScreenShown)
        {
            PopulateGameOver(gm);
            gameOverPanel.SetActive(true);
            endScreenShown = true;
        }
    }

    private void PopulateGameOver(GameManager gm)
    {
        if (gameOverTitle != null)
        {
            gameOverTitle.text = gm.EndMessage;
        }

        if (gameOverStats != null)
        {
            gameOverStats.text = BuildGameOverStats();
        }

        // "Next" only makes sense on a victory that has a level after it.
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(gm.IsVictory && gm.HasNextLevel);
        }
    }

    private string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return (total / 60).ToString("0") + ":" + (total % 60).ToString("00");
    }

    // Score breakdown plus the two saved data points (high score + lifetime fish).
    private string BuildGameOverStats()
    {
        ScoreManager sm = ScoreManager.Instance;
        GameManager gm = GameManager.Instance;
        string s = "";

        // How long the run took (label adapts to win vs lose).
        if (gm != null)
        {
            bool victory = sm != null && sm.WasVictory;
            string timeLabel = victory ? "Clear Time" : "Survival Time";
            s += timeLabel + "   " + FormatTime(gm.TimeElapsed) + "\n";
        }

        if (sm != null)
        {
            if (sm.WasVictory && sm.LastTimeBonus > 0)
            {
                s += "Time Bonus   +" + sm.LastTimeBonus + "\n";
            }
            if (sm.WasVictory && sm.LastAllFishBonus > 0)
            {
                s += "All Fish Bonus   +" + sm.LastAllFishBonus + "\n";
            }
            s += "FINAL SCORE   " + sm.Score + "\n\n";
            s += "High Score   " + sm.HighScore;
            if (sm.NewHighScore)
            {
                s += "   <color=#FFD24A>NEW!</color>";
            }
            s += "\n";
        }

        // 2nd saved data point, loaded from SaveManager.
        s += "Total Fish Collected   " + SaveManager.LoadTotalFish();
        return s;
    }

    public void GoToNextLevel()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.HasNextLevel) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(gm.NextSceneName);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HandleCollectionPanel()
    {
        if (collectionPanel == null) return;

        bool ended = GameManager.Instance != null && GameManager.Instance.IsGameEnded;
        bool show = Input.GetKey(collectionKey) && !ended;

        if (collectionPanel.activeSelf != show)
        {
            collectionPanel.SetActive(show);
        }

        if (show) RefreshCollection();
    }

    private void RefreshCollection()
    {
        PlayerInventory inv = PlayerInventory.Instance;
        if (inv == null || slots == null) return;

        FishData[] all = inv.AllFishTypes;
        for (int i = 0; i < slots.Length && i < all.Length; i++)
        {
            if (slots[i] != null) slots[i].Set(all[i], inv.IsDiscovered(all[i].fishId));
        }

        if (collectionTitle != null)
        {
            collectionTitle.text = "FISH COLLECTION  " + inv.UniqueCollected + "/" + inv.TotalFishTypes;
        }
    }
}
