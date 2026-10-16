using UnityEngine;

[CreateAssetMenu(
    fileName = "Crop",
    menuName = "FarmGame/Crop"
)]
public class CropData : ScriptableObject
{
    [Header("Crop Info")]
    public string cropName;

    [Header("Seed")]
    public ItemData seedItem;

    [Header("Growth Stages")]
    [Tooltip("0 = seed, 1-4 = growth, 5 = mature")]
    public Sprite[] growthSprites = new Sprite[6];

    [Header("Growth Settings")]
    [Min(0.1f)]
    public float stageDuration = 10f;

    [Header("Harvest")]
    public ItemData harvestItem;

    [Min(1)]
    public int harvestAmount = 1;

    [Min(0f)]
    public float matureDropDelay = 0f;
}