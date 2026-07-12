/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 10 July 2026
*/
using UnityEngine;

// One ScriptableObject asset per unique fish (16 total).
// Acts as the data for both the world pickup and the collection log.
[CreateAssetMenu(fileName = "FishData", menuName = "Paws/Fish Data")]
public class FishData : ScriptableObject
{
    public int fishId;
    public string fishName;
    public Sprite fishSprite;

    [TextArea]
    public string description;
}
