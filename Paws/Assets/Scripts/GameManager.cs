/*
Author: Muqrie Rahimi
Student ID: YOUR_STUDENT_ID
Date Created: 23 May 2026
*/

using UnityEngine;
using TMPro;

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

    private int burgerCollected = 0;
    private float currentTime;
    private bool gameEnded = false;
    private string endMessage = "";

    public int BurgerCollected => burgerCollected;
    public int BurgerTarget => burgerTarget;
    public int PlayerHealth => playerHealth;

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

        Debug.Log("Game started. Student IDs: Ahmad Aliff - 1221309548, Muqrie Rahimi - 1211109977");
    }

    private void Update()
    {
        if (gameEnded) return;

        UpdateTimer();
        HandleRaycastInteraction();
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
        if (gameEnded) return;

        burgerCollected++;
        Debug.Log("Burger collected: " + burgerCollected + "/" + burgerTarget);
    }

    public void DamagePlayer(int amount)
    {
        if (gameEnded) return;

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
        if (gameEnded) return;

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
        {
            interactionText.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;
        Debug.Log(endMessage);
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
        Debug.Log(endMessage);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), "Ahmad Aliff 1221309548 ");
        GUI.Label(new Rect(10, 35, 300, 30), "Muqrie Rahimi 1211109977 ");
        GUI.Label(new Rect(10, 65, 300, 30), "Burger: " + burgerCollected + "/" + burgerTarget);
        GUI.Label(new Rect(10, 90, 300, 30), "Health: " + playerHealth);
        GUI.Label(new Rect(10, 115, 300, 30), "Time: " + Mathf.CeilToInt(currentTime));

        if (gameEnded)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2, 400, 40), endMessage);
        }
    }
}