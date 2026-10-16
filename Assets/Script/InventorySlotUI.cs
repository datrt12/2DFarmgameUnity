using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    public void SetItem(ItemStack itemStack)
    {
        if (itemStack == null || itemStack.item == null)
        {
            icon.enabled = false;
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = itemStack.item.icon;

        if (itemStack.quantity > 1)
            amountText.text = itemStack.quantity.ToString();
        else
            amountText.text = "";
    }
}