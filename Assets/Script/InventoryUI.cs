using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Slots")]
    [SerializeField] private InventorySlotUI[] slots;

    private void Start()
    {
        if (inventory == null)
        {
            Debug.LogWarning("InventoryUI: Chưa gán Inventory!");
            return;
        }

        inventory.OnInventoryChanged += RefreshUI;

        RefreshUI();

        // Ẩn Inventory khi bắt đầu game
        inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        bool isOpen = inventoryPanel.activeSelf;

        inventoryPanel.SetActive(!isOpen);
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        if (inventory == null)
            return;

        var items = inventory.GetItems();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].SetItem(items[i]);
            }
            else
            {
                slots[i].SetItem(null);
            }
        }
    }
}