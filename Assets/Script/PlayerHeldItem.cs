using UnityEngine;

public class PlayerHeldItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HotbarUI hotbarUI;
    [SerializeField] private SpriteRenderer heldItemRenderer;

    private void Start()
    {
        if (hotbarUI == null)
        {
            Debug.LogWarning("PlayerHeldItem: Chưa gán HotbarUI!");
            return;
        }

        if (heldItemRenderer == null)
        {
            Debug.LogWarning("PlayerHeldItem: Chưa gán SpriteRenderer!");
            return;
        }

        // Đăng ký lắng nghe khi Hotbar đổi item
        hotbarUI.OnSelectedItemChanged += UpdateHeldItem;

        // Cập nhật ngay lúc bắt đầu
        UpdateHeldItem(hotbarUI.GetSelectedItem());
    }

    private void OnDestroy()
    {
        if (hotbarUI != null)
        {
            hotbarUI.OnSelectedItemChanged -= UpdateHeldItem;
        }
    }

    private void UpdateHeldItem(ItemData item)
    {
        if (item == null)
        {
            heldItemRenderer.enabled = false;
            heldItemRenderer.sprite = null;
            return;
        }

        heldItemRenderer.enabled = true;
        heldItemRenderer.sprite = item.icon;
    }
}