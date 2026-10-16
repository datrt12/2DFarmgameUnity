using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TradingUI : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject tradingWindow;

    [Header("Money")]
    [SerializeField] private TMP_Text moneyText;

    [Header("Item Slots")]
    [SerializeField] private TradingSlotUI[] itemSlots;

    [Header("Selected Item")]
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedQuantityText;
    [SerializeField] private TMP_Text selectedPriceText;

    [Header("Buttons")]
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closeButton;

    private Inventory inventory;
    private PlayerCurrency playerCurrency;

    private int selectedSlotIndex = -1;

    private void Awake()
    {
        if (tradingWindow != null)
        {
            tradingWindow.SetActive(false);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(
                SellSelectedItem
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                CloseTrading
            );
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsOpen())
            {
                CloseTrading();
            }
        }
    }

    private void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "TradingUI: Không tìm thấy Player!",
                this
            );

            return;
        }

        inventory =
            player.GetComponent<Inventory>();

        playerCurrency =
            player.GetComponent<PlayerCurrency>();

        if (inventory == null)
        {
            Debug.LogWarning(
                "TradingUI: Player chưa có Inventory!",
                this
            );
        }

        if (playerCurrency == null)
        {
            Debug.LogWarning(
                "TradingUI: Player chưa có PlayerCurrency!",
                this
            );
        }
    }

    // =========================================================
    // OPEN
    // =========================================================

    public void OpenTrading()
    {
        if (inventory == null ||
            playerCurrency == null)
        {
            FindPlayer();
        }

        if (tradingWindow == null)
            return;

        tradingWindow.SetActive(true);

        selectedSlotIndex = -1;

        UpdateMoneyUI();
        RefreshItemSlots();
        ClearSelectedItem();
    }

    // =========================================================
    // CLOSE
    // =========================================================

    public void CloseTrading()
    {
        if (tradingWindow != null)
        {
            tradingWindow.SetActive(false);
        }

        selectedSlotIndex = -1;
    }

    // =========================================================
    // REFRESH SLOTS
    // =========================================================

    private void RefreshItemSlots()
    {
        if (inventory == null)
            return;

        List<ItemStack> items =
            inventory.GetItems();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
                continue;

            if (i < items.Count)
            {
                itemSlots[i].Setup(
                    this,
                    i,
                    items[i]
                );
            }
            else
            {
                itemSlots[i].Setup(
                    this,
                    i,
                    null
                );
            }
        }
    }

    // =========================================================
    // SELECT ITEM
    // =========================================================

    public void SelectItem(int slotIndex)
    {
        if (inventory == null)
            return;

        List<ItemStack> items =
            inventory.GetItems();

        if (slotIndex < 0 ||
            slotIndex >= items.Count)
        {
            return;
        }

        ItemStack stack =
            items[slotIndex];

        if (stack == null ||
            stack.item == null ||
            stack.quantity <= 0)
        {
            return;
        }

        selectedSlotIndex = slotIndex;

        ItemData item =
            stack.item;

        if (selectedItemIcon != null)
        {
            selectedItemIcon.sprite =
                item.icon;

            selectedItemIcon.enabled =
                item.icon != null;
        }

        if (selectedItemName != null)
        {
            selectedItemName.text =
                item.itemName;
        }

        if (selectedQuantityText != null)
        {
            selectedQuantityText.text =
                "Số lượng: "
                + stack.quantity;
        }

        if (selectedPriceText != null)
        {
            selectedPriceText.text =
                "Giá bán: "
                + item.SellPrice.ToString("N0")
                + " Xu";
        }

        if (sellButton != null)
        {
            sellButton.interactable =
                item.SellPrice > 0;
        }
    }

    // =========================================================
    // SELL
    // =========================================================

    private void SellSelectedItem()
    {
        if (inventory == null ||
            playerCurrency == null)
        {
            return;
        }

        if (selectedSlotIndex < 0)
        {
            return;
        }

        List<ItemStack> items =
            inventory.GetItems();

        if (selectedSlotIndex >= items.Count)
        {
            return;
        }

        ItemStack stack =
            items[selectedSlotIndex];

        if (stack == null ||
            stack.item == null ||
            stack.quantity <= 0)
        {
            ClearSelectedItem();
            return;
        }

        ItemData item =
            stack.item;

        if (item.SellPrice <= 0)
        {
            Debug.Log(
                item.itemName
                + " không có giá bán!"
            );

            return;
        }

        bool removed =
            inventory.RemoveItem(
                item,
                1
            );

        if (!removed)
        {
            Debug.LogWarning(
                "Không thể trừ vật phẩm khỏi Inventory!"
            );

            return;
        }

        playerCurrency.AddMoney(
            item.SellPrice
        );

        Debug.Log(
            "Đã bán "
            + item.itemName
            + " x1 → +"
            + item.SellPrice
            + " Xu"
        );

        UpdateMoneyUI();
        RefreshItemSlots();

        // Kiểm tra item còn tồn tại không
        items = inventory.GetItems();

        if (selectedSlotIndex < items.Count &&
            items[selectedSlotIndex] != null &&
            items[selectedSlotIndex].item != null)
        {
            SelectItem(selectedSlotIndex);
        }
        else
        {
            ClearSelectedItem();
        }
    }

    // =========================================================
    // MONEY UI
    // =========================================================

    private void UpdateMoneyUI()
    {
        if (moneyText == null ||
            playerCurrency == null)
        {
            return;
        }

        moneyText.text =
            playerCurrency.Money.ToString("N0")
            + " Xu";
    }

    // =========================================================
    // CLEAR SELECTED ITEM
    // =========================================================

    private void ClearSelectedItem()
    {
        if (selectedItemIcon != null)
        {
            selectedItemIcon.sprite = null;
            selectedItemIcon.enabled = false;
        }

        if (selectedItemName != null)
        {
            selectedItemName.text =
                "Chưa chọn vật phẩm";
        }

        if (selectedQuantityText != null)
        {
            selectedQuantityText.text =
                "Số lượng: 0";
        }

        if (selectedPriceText != null)
        {
            selectedPriceText.text =
                "Giá bán: 0 Xu";
        }

        if (sellButton != null)
        {
            sellButton.interactable = false;
        }
    }

    // =========================================================
    // CHECK OPEN
    // =========================================================

    public bool IsOpen()
    {
        return tradingWindow != null &&
               tradingWindow.activeSelf;
    }
}