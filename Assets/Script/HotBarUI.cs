using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarUI : MonoBehaviour
{
    public System.Action<ItemData> OnSelectedItemChanged;

    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject hotbarPanel;

    [Header("Hotbar Slots")]
    [SerializeField] private InventorySlotUI[] hotbarSlots;

    [Header("Highlight")]
    [SerializeField] private GameObject[] highlightOverlays;

    private int selectedIndex = 0;

    private void Awake()
    {
        // Hotbar luôn phải được bật
        if (hotbarPanel != null)
        {
            hotbarPanel.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += RefreshHotbarUI;
        }
    }

    private void Start()
    {
        if (inventory == null)
        {
            Debug.LogWarning("HotbarUI: Chưa gán Inventory!");
            return;
        }

        if (hotbarSlots == null || hotbarSlots.Length != 6)
        {
            Debug.LogWarning(
                $"HotbarUI: Cần đúng 6 slot. Hiện tại có " +
                $"{(hotbarSlots == null ? 0 : hotbarSlots.Length)}."
            );
        }

        RefreshHotbarUI();

        // Chọn slot đầu tiên
        SelectSlot(0);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        CheckNumberKeys();
    }

    private void CheckNumberKeys()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SelectSlot(0);

        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SelectSlot(1);

        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SelectSlot(2);

        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            SelectSlot(3);

        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            SelectSlot(4);

        else if (Keyboard.current.digit6Key.wasPressedThisFrame)
            SelectSlot(5);
    }

    private void SelectSlot(int index)
    {
        if (hotbarSlots == null)
            return;

        if (index < 0 || index >= hotbarSlots.Length)
            return;

        selectedIndex = index;

        UpdateHighlights();

        // Báo cho Player biết Item đang được chọn
        OnSelectedItemChanged?.Invoke(GetSelectedItem());
    }

    private void UpdateHighlights()
    {
        if (highlightOverlays == null)
            return;

        for (int i = 0; i < highlightOverlays.Length; i++)
        {
            if (highlightOverlays[i] != null)
            {
                highlightOverlays[i].SetActive(i == selectedIndex);
            }
        }
    }

    public void RefreshHotbarUI()
    {
        if (inventory == null || hotbarSlots == null)
            return;

        var items = inventory.GetItems();

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] == null)
                continue;

            if (i < items.Count)
            {
                hotbarSlots[i].SetItem(items[i]);
            }
            else
            {
                hotbarSlots[i].SetItem(null);
            }
        }

        // Giữ slot đang chọn hợp lệ
        if (selectedIndex >= hotbarSlots.Length)
        {
            selectedIndex = hotbarSlots.Length - 1;
        }

        UpdateHighlights();

        OnSelectedItemChanged?.Invoke(GetSelectedItem());
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public ItemData GetSelectedItem()
    {
        if (inventory == null)
            return null;

        var items = inventory.GetItems();

        if (selectedIndex < 0 || selectedIndex >= items.Count)
            return null;

        if (items[selectedIndex] == null)
            return null;

        return items[selectedIndex].item;
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= RefreshHotbarUI;
        }
    }
}