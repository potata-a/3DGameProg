/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 10 July 2026
*/
using System.Collections.Generic;
using UnityEngine;

// Player's fish inventory.
// - Collecting a fish increases the carried count AND records it in a permanent
//   collection log (used for the 16-fish "collection" completion display).
// - Eating a fish (handled by GameManager on the F key) consumes one carried fish
//   and heals the player. The collection log never decreases.
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Fish Collection")]
    [Tooltip("Assign all 16 unique FishData assets here (drives the collection panel).")]
    [SerializeField] private FishData[] allFishTypes;

    // Permanent record of which fish types have ever been collected.
    private readonly HashSet<int> discoveredIds = new HashSet<int>();

    // Consumable stock available to eat for healing.
    private int carriedFish = 0;

    public int CarriedFish => carriedFish;
    public int UniqueCollected => discoveredIds.Count;
    public int TotalFishTypes => allFishTypes != null ? allFishTypes.Length : 0;
    public bool HasFish => carriedFish > 0;
    public FishData[] AllFishTypes => allFishTypes;

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

    // Called by CollectableFish when the player picks up a fish.
    public void CollectFish(FishData fish)
    {
        carriedFish++;

        if (fish != null)
        {
            discoveredIds.Add(fish.fishId);
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddFishPoints();
        }

        // Persist the lifetime total (2nd saved data point).
        SaveManager.AddFishToTotal(1);

        string name = fish != null ? fish.fishName : "Unknown fish";
        Debug.Log("Collected " + name + ". Carried: " + carriedFish +
                  ", Collection: " + UniqueCollected + "/" + TotalFishTypes);
    }

    // Removes one carried fish. Returns true if one was available.
    public bool ConsumeFish()
    {
        if (carriedFish <= 0)
        {
            return false;
        }

        carriedFish--;
        return true;
    }

    public bool IsDiscovered(int id)
    {
        return discoveredIds.Contains(id);
    }
}
