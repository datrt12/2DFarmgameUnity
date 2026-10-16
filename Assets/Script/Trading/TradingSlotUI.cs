using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradingSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Button button;

    private int slotIndex;
    private TradingUI tradingUI;

    public void Setup(
        TradingUI owner,
        int index,
        ItemStack stack)
    {
        tradingUI = owner;
        slotIndex = index;

        if (stack == null ||
            stack.item == null ||
            stack.quantity <= 0)
        {
            Clear();
            return;
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = stack.item.icon;
            itemIcon.enabled = stack.item.icon != null;
        }

        if (quantityText != null)
        {
            quantityText.text =
                stack.quantity.ToString();
        }

        if (button != null)
        {
            button.interactable = true;

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(
                OnSlotClicked
            );
        }
    }

    private void OnSlotClicked()
    {
        if (tradingUI == null)
            return;

        tradingUI.SelectItem(slotIndex);
    }

    private void Clear()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = "";
        }

        if (button != null)
        {
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }
    }
}