/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 23 May 2026
Updated: 10 July 2026 - added max health + heal and eat-fish-to-heal (F key); moved the on-screen HUD out to GameHUD (removed the old OnGUI drawing).
Updated: 11 July 2026 - hooked up ScoreManager (points + end-of-game finalize) and the damage/heal particle effects.
Updated: 12 July 2026 - added knockback + invulnerability on damage, and the victory "Next Level" flow with clear/survival time.
*/
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;


// Core game loop: burger objective, health, timer, pause, and raycast interaction, plus
// win/lose handling. Coordinates the score, save, effects, HUD, and hit-reaction systems.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    [SerializeField] private int playerHealth = 3;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int burgerTarget;
    [SerializeField] private float timeLimit = 600f;

    [Header("Fish Eating")]
    [SerializeField] private KeyCode eatFishKey = KeyCode.F;

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

    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;

    private int burgerCollected = 0;
    private float currentTime;
    private bool gameEnded = false;
    private bool isVictory = false;
    private bool isPaused = false;
    private string endMessage = "";
    private PlayerHitReaction hitReaction;

    public int BurgerCollected => burgerCollected;
    public int BurgerTarget => burgerTarget;
    public int PlayerHealth => playerHealth;
    public int MaxHealth => maxHealth;
    public bool IsPaused => isPaused;
    public bool IsGameEnded => gameEnded;
    public string EndMessage => endMessage;
    public float TimeRemaining => currentTime;
    public float TimeElapsed => Mathf.Max(0f, timeLimit - currentTime);
    public bool IsVictory => isVictory;
    public bool HasNextLevel => !string.IsNullOrEmpty(nextSceneName);
    public string NextSceneName => nextSceneName;

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
        DynamicGI.UpdateEnvironment();
        Time.timeScale = 1f;
        currentTime = timeLimit;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        hitReaction = FindObjectOfType<PlayerHitReaction>();

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
        HandleEatFishInput();
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

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddBurgerPoints();
        }
    }

    private void HandleEatFishInput()
    {
        if (Input.GetKeyDown(eatFishKey))
        {
            TryEatFish();
        }
    }

    // Eats one carried fish to restore 1 health. A fish is only spent if the player
    // actually has one AND is below max health (so it is never wasted).
    private void TryEatFish()
    {
        if (PlayerInventory.Instance == null) return;

        if (playerHealth >= maxHealth)
        {
            Debug.Log("Already at full health - fish not eaten.");
            return;
        }

        if (!PlayerInventory.Instance.HasFish)
        {
            Debug.Log("No fish to eat.");
            return;
        }

        if (PlayerInventory.Instance.ConsumeFish())
        {
            HealPlayer(1);
        }
    }

    public void HealPlayer(int amount)
    {
        if (gameEnded) return;

        playerHealth = Mathf.Min(playerHealth + amount, maxHealth);
        Debug.Log("Player healed. HP: " + playerHealth + "/" + maxHealth);

        if (EffectsManager.Instance != null && player != null)
        {
            EffectsManager.Instance.PlayHeal(player);
        }
    }

    // Legacy entry point (no knockback direction).
    public void DamagePlayer(int amount)
    {
        DamagePlayer(amount, Vector3.zero, 0f);
    }

    // Full entry point: knockbackDirection is the hazard's travel direction (the cat is
    // flung this way). During the invulnerability window further hits are ignored.
    public void DamagePlayer(int amount, Vector3 knockbackDirection, float knockbackForce)
    {
        if (gameEnded || isPaused) return;
        if (hitReaction != null && hitReaction.IsInvulnerable) return;

        playerHealth -= amount;
        Debug.Log("Player damaged. HP left: " + playerHealth);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ApplyDamagePenalty();
        }

        if (EffectsManager.Instance != null && player != null)
        {
            EffectsManager.Instance.PlayDamage(player);
        }

        if (hitReaction != null)
        {
            hitReaction.TakeHit(knockbackDirection, knockbackForce);
        }

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
        isVictory = true;
        endMessage = "YOU WIN! The cat reached the shelter safely.";

        FinalizeScore(true);

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Advancing to the next level is handled by the "Next" button on the victory screen.
    }

    private void FinalizeScore(bool victory)
    {
        if (ScoreManager.Instance == null) return;

        int uniqueFish = PlayerInventory.Instance != null ? PlayerInventory.Instance.UniqueCollected : 0;
        int totalFish = PlayerInventory.Instance != null ? PlayerInventory.Instance.TotalFishTypes : 0;

        ScoreManager.Instance.EndGame(victory, currentTime, uniqueFish, totalFish);
    }

    private void LoseGame(string reason)
    {
        gameEnded = true;
        endMessage = "GAME OVER! " + reason;

        FinalizeScore(false);

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(endMessage);
    }

}