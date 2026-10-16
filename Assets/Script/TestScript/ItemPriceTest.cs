using UnityEngine;

public class ItemPriceTest : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogWarning(
                "Chưa gán ItemData!",
                this
            );

            return;
        }

        Debug.Log(
            "Item: " + itemData.name
        );

        Debug.Log(
            "Giá mua: " + itemData.BuyPrice + " Xu"
        );

        Debug.Log(
            "Giá bán: " + itemData.SellPrice + " Xu"
        );
    }
}