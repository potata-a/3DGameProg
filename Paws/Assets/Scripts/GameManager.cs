/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
*/
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    [SerializeField] private int playerHealth = 3;
    [SerializeField] private int burgerTarget = 15;
    [SerializeField] private float timeLimit = 600f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    [Header("Interaction UI")]
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Raycast Interaction")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Level Transition")]
    [SerializeField] private string nextSceneName; // leave empty on the last map
    [SerializeField] private float winDelay = 2f;

    private int burgerCollected = 0;
    private float currentTime;
    private bool gameEnded = false;
    private bool isPaused = false;
    private string endMessage = "";

    public int BurgerCollected => burgerCollected;
    public int BurgerTarget => burgerTarget;
    public int PlayerHealth => playerHealth;
    public bool IsPaused => isPaused;

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

    private void Start()
    {
        Time.timeScale = 1f;
        currentTime = timeLimit;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }

        LockCursor();

        Debug.Log("Game started. Student IDs: Ahmad Aliff - 1221309548, Muqrie Rahimi - 1211109977");
    }

    private void Update()
    {
        HandlePauseInput();

        if (gameEnded || isPaused) return;

        UpdateTimer();
        HandleRaycastInteraction();
    }

    private void HandlePauseInput()
    {
        if (gameEnded) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Paused");
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        LockCursor();

        Debug.Log("Game Resumed");
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            LoseGame("Time ran out!");
        }
    }

    private void HandleRaycastInteraction()
    {
        if (player == null || mainCamera == null) return;

        Vector3 rayOrigin = player.position + Vector3.up * 0.5f;
        Vector3 rayDirection = mainCamera.transform.forward;

        bool hitInteractable = Physics.Raycast(
            rayOrigin,
            rayDirection,
            out RaycastHit hit,
            interactDistance,
            interactableLayer
        );

        if (hitInteractable)
        {
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(true);
                interactionText.text = "Press E to pick up";
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interacted with: " + hit.collider.name);
                hit.collider.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
    }

    public void AddBurger()
    {
        if (gameEnded || isPaused) return;

        burgerCollected++;
        Debug.Log("Burger collected: " + burgerCollected + "/" + burgerTarget);
    }

    public void DamagePlayer(int amount)
    {
        if (gameEnded || isPaused) return;

        playerHealth -= amount;
        Debug.Log("Player damaged. HP left: " + playerHealth);

        if (playerHealth <= 0)
        {
            playerHealth = 0;
            LoseGame("The cat was hit too many times!");
        }
    }

    public bool HasEnoughBurger()
    {
        return burgerCollected >= burgerTarget;
    }

    public void TryWinGame()
    {
        if (gameEnded || isPaused) return;

        if (HasEnoughBurger())
        {
            WinGame();
        }
        else
        {
            Debug.Log("Need more burger before reaching shelter!");
        }
    }

    private void WinGame()
    {
        gameEnded = true;
        endMessage = "YOU WIN! The cat reached the shelter safely.";

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!string.IsNullOrEmpty(nextSceneName))
            StartCoroutine(LoadNextLevelAfterDelay());
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        // WaitForSecondsRealtime because Time.timeScale is 0 right now
        yield return new WaitForSecondsRealtime(winDelay);

        Time.timeScale = 1f; // IMPORTANT — otherwise the next scene loads still paused
        SceneManager.LoadScene(nextSceneName);
    }

    private void LoseGame(string reason)
    {
        gameEnded = true;
        endMessage = "GAME OVER! " + reason;

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(endMessage);
    }

    private void OnGUI()
    {
        GUIStyle hudStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        GUIStyle missionStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        GUIStyle controlsStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft,
            normal = { textColor = Color.white }
        };

        GUIStyle centerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 36,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        GUI.Label(new Rect(Screen.width / 2 - 250, 10, 500, 50), "MISSION: Find all 15 burgers", missionStyle);
        GUI.Label(new Rect(Screen.width / 2 - 250, 40, 500, 50), "and reach the shelter safely", missionStyle);

        GUI.Label(new Rect(10, 10, 500, 35), "Ahmad Aliff 1221309548", hudStyle);
        GUI.Label(new Rect(10, 45, 500, 35), "Muqrie Rahimi 1211109977", hudStyle);
        GUI.Label(new Rect(10, 80, 500, 35), "Burger: " + burgerCollected + "/" + burgerTarget, hudStyle);
        GUI.Label(new Rect(10, 115, 500, 35), "Health: " + playerHealth, hudStyle);
        GUI.Label(new Rect(10, 150, 500, 35), "Time: " + Mathf.CeilToInt(currentTime), hudStyle);

        string controls =
            "CONTROLS\n" +
            "W/A/S/D - Player Movement\n" +
            "Left Shift - Sprint\n" +
            "Spacebar - Jump\n" +
            "E - Interact";

        GUI.Label(
            new Rect(Screen.width - 260, Screen.height - 120, 250, 110),
            controls,
            controlsStyle
        );

        if (isPaused && !gameEnded)
        {
            GUI.Label(
                new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 400, 60),
                "PAUSED",
                centerStyle
            );

            GUI.Label(
                new Rect(Screen.width / 2 - 250, Screen.height / 2 + 5, 500, 50),
                "Press ESC to resume",
                missionStyle
            );
        }

        if (gameEnded)
        {
            GUI.Label(
                new Rect(Screen.width / 2 - 350, Screen.height / 2, 700, 70),
                endMessage,
                centerStyle
            );
        }
    }
}