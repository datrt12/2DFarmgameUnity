using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "FarmGame/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;

    public Sprite icon;

    [TextArea]
    public string description;

    public int maxStack = 99;

    [Header("Drop")]
    public GameObject collectablePrefab;

    public ItemCategory itemCategory;
    public ToolType toolType;

    [Header("Trading")]
    [SerializeField] private int buyPrice = 0;
    [SerializeField] private int sellPrice = 0;

    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
}