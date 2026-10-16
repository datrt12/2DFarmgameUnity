using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public System.Action OnInventoryChanged;

    [Header("Inventory Settings")]
    [SerializeField] private int slotCount = 12;

    [Header("Inventory Slots")]
    [SerializeField] private List<ItemStack> items = new List<ItemStack>();

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        // Nếu chưa đủ slot thì thêm slot rỗng
        while (items.Count < slotCount)
        {
            items.Add(null);
        }

        // Nếu thừa slot thì xóa phần dư
        while (items.Count > slotCount)
        {
            items.RemoveAt(items.Count - 1);
        }
    }

    // =========================================================
    // ADD ITEM
    // =========================================================

    public bool AddItem(ItemData item, int amount)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "Inventory: ItemData đang bị NULL!"
            );

            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning(
                "Inventory: Quantity phải lớn hơn 0!"
            );

            return false;
        }

        int maxStack = Mathf.Max(1, item.maxStack);
        int remaining = amount;

        // =====================================================
        // 1. Thêm vào stack đã có
        // =====================================================

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null ||
                items[i].item == null)
            {
                continue;
            }

            if (items[i].item != item)
            {
                continue;
            }

            if (items[i].quantity >= maxStack)
            {
                continue;
            }

            int space =
                maxStack - items[i].quantity;

            int amountToAdd =
                Mathf.Min(space, remaining);

            items[i].quantity += amountToAdd;
            remaining -= amountToAdd;

            Debug.Log(
                $"Thêm vào slot {i}: " +
                $"{item.itemName} +{amountToAdd}"
            );

            if (remaining <= 0)
            {
                Debug.Log(
                    $"Nhặt thành công: " +
                    $"{item.itemName} x{amount}"
                );

                OnInventoryChanged?.Invoke();

                return true;
            }
        }

        // =====================================================
        // 2. Tạo stack mới
        // =====================================================

        while (remaining > 0)
        {
            int emptySlot = FindEmptySlot();

            if (emptySlot == -1)
            {
                Debug.Log(
                    $"Không đủ chỗ để nhặt " +
                    $"{item.itemName} x{remaining}!"
                );

                return false;
            }

            int amountToAdd =
                Mathf.Min(maxStack, remaining);

            items[emptySlot] =
                new ItemStack(item, amountToAdd);

            remaining -= amountToAdd;

            Debug.Log(
                $"Tạo stack mới tại slot {emptySlot}: " +
                $"{item.itemName} x{amountToAdd}"
            );
        }

        Debug.Log(
            $"Nhặt thành công: " +
            $"{item.itemName} x{amount}"
        );

        OnInventoryChanged?.Invoke();

        return true;
    }

    // =========================================================
    // REMOVE ITEM
    // =========================================================

    public bool RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        int remaining = amount;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                continue;
            }

            if (items[i].item != item)
            {
                continue;
            }

            int removeAmount =
                Mathf.Min(
                    items[i].quantity,
                    remaining
                );

            items[i].quantity -= removeAmount;
            remaining -= removeAmount;

            if (items[i].quantity <= 0)
            {
                items[i] = null;
            }

            if (remaining <= 0)
            {
                // Báo cho Inventory UI refresh
                OnInventoryChanged?.Invoke();

                return true;
            }
        }

        return false;
    }

    // =========================================================
    // DROP ITEM
    // =========================================================

    public bool DropItem(
        int slotIndex,
        int amount,
        Vector3 dropPosition)
    {
        if (slotIndex < 0 ||
            slotIndex >= items.Count)
        {
            return false;
        }

        if (items[slotIndex] == null)
        {
            return false;
        }

        ItemData item =
            items[slotIndex].item;

        if (item == null)
        {
            return false;
        }

        if (item.collectablePrefab == null)
        {
            Debug.LogWarning(
                $"Item {item.itemName} " +
                $"chưa có Collectable Prefab!"
            );

            return false;
        }

        int amountToDrop =
            Mathf.Min(
                amount,
                items[slotIndex].quantity
            );

        // Trừ item khỏi Inventory
        items[slotIndex].quantity -= amountToDrop;

        if (items[slotIndex].quantity <= 0)
        {
            items[slotIndex] = null;
        }

        // Tạo item ngoài thế giới
        for (int i = 0; i < amountToDrop; i++)
        {
            Instantiate(
                item.collectablePrefab,
                dropPosition,
                Quaternion.identity
            );
        }

        Debug.Log(
            $"Đã drop " +
            $"{item.itemName} x{amountToDrop}"
        );

        // Refresh Inventory UI
        OnInventoryChanged?.Invoke();

        return true;
    }

    // =========================================================
    // REMOVE ITEM FROM SLOT
    // =========================================================

    public bool RemoveItemFromSlot(
        int slotIndex,
        int amount)
    {
        if (slotIndex < 0 ||
            slotIndex >= items.Count)
        {
            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        ItemStack stack =
            items[slotIndex];

        if (stack == null ||
            stack.item == null ||
            stack.quantity <= 0)
        {
            return false;
        }

        if (stack.quantity < amount)
        {
            return false;
        }

        stack.quantity -= amount;

        if (stack.quantity <= 0)
        {
            items[slotIndex] = null;
        }

        OnInventoryChanged?.Invoke();

        return true;
    }

    // =========================================================
    // FIND EMPTY SLOT
    // =========================================================

    private int FindEmptySlot()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null ||
                items[i].item == null ||
                items[i].quantity <= 0)
            {
                return i;
            }
        }

        return -1;
    }

    // =========================================================
    // GET ITEMS
    // =========================================================

    public List<ItemStack> GetItems()
    {
        return items;
    }

    // =========================================================
    // GET SLOT COUNT
    // =========================================================

    public int GetSlotCount()
    {
        return slotCount;
    }
}
