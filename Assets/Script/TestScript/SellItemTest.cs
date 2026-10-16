using UnityEngine;
using UnityEngine.InputSystem;

public class SellItemTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerCurrency playerCurrency;

    [Header("Test")]
    [SerializeField] private int sellAmount = 1;

    private void Start()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();

        if (playerCurrency == null)
            playerCurrency = GetComponent<PlayerCurrency>();

        if (inventory == null)
        {
            Debug.LogWarning(
                "SellItemTest: Không tìm thấy Inventory!",
                this
            );
        }

        if (playerCurrency == null)
        {
            Debug.LogWarning(
                "SellItemTest: Không tìm thấy PlayerCurrency!",
                this
            );
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            SellFirstItem();
        }
    }

    private void SellFirstItem()
    {
        if (inventory == null || playerCurrency == null)
            return;

        var items = inventory.GetItems();

        for (int i = 0; i < items.Count; i++)
        {
            ItemStack stack = items[i];

            if (stack == null)
                continue;

            if (stack.item == null)
                continue;

            if (stack.quantity <= 0)
                continue;

            ItemData item = stack.item;

            if (item.SellPrice <= 0)
            {
                Debug.Log(
                    item.itemName + " không có giá bán!"
                );

                return;
            }

            if (stack.quantity < sellAmount)
            {
                Debug.Log(
                    "Không đủ "
                    + item.itemName
                    + " để bán."
                );

                return;
            }

            int totalMoney =
                item.SellPrice * sellAmount;

            bool removed =
                inventory.RemoveItem(
                    item,
                    sellAmount
                );

            if (!removed)
            {
                Debug.LogWarning(
                    "Không thể lấy vật phẩm khỏi Inventory!"
                );

                return;
            }

            playerCurrency.AddMoney(totalMoney);

            Debug.Log(
                "Đã bán "
                + item.itemName
                + " x"
                + sellAmount
                + " → +"
                + totalMoney
                + " Xu"
            );

            return;
        }

        Debug.Log("Inventory không có vật phẩm để bán.");
    }
}