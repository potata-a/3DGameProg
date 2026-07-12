/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 12 July 2026
*/
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One slot in the fish collection panel. Shows the fish sprite + name when caught,
// or a darkened silhouette + "???" when still missing.
public class FishSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;

    public void Set(FishData fish, bool caught)
    {
        if (fish == null) return;

        if (icon != null)
        {
            icon.sprite = fish.fishSprite;
            icon.enabled = fish.fishSprite != null;
            icon.color = caught ? Color.white : new Color(0f, 0f, 0f, 0.55f);
        }

        if (label != null)
        {
            label.text = caught ? fish.fishName : "???";
        }
    }
}
