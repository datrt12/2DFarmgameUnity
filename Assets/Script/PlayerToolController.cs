using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerToolController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HotbarUI hotbarUI;
    [SerializeField] private FarmSoilManager farmSoilManager;
    [SerializeField] private Tilemap farmableTilemap;

  

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (hotbarUI == null)
            return;

        if (farmSoilManager == null)
            return;

        // Chỉ xử lý khi vừa click chuột trái
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Nếu đang click vào UI thì không sử dụng công cụ
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        UseSelectedTool();
    }

    private void UseSelectedTool()
    {
        ItemData selectedItem = hotbarUI.GetSelectedItem();

        // Không có item trong ô đang chọn
        if (selectedItem == null)
        {
            return;
        }

        // Không phải Tool
        if (selectedItem.itemCategory != ItemCategory.Tool)
        {
            return;
        }

        // Kiểm tra loại Tool
        switch (selectedItem.toolType)
        {
            case ToolType.Hoe:
                TryHoe();
                break;

            default:
                break;
        }
    }

    private void TryHoe()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogWarning(
                "PlayerToolController: Không tìm thấy Main Camera!"
            );

            return;
        }

        if (farmableTilemap == null)
        {
            Debug.LogWarning(
                "PlayerToolController: Chưa gán Farmable Tilemap!"
            );

            return;
        }

        // ==============================
        // 1. Lấy vị trí chuột trên màn hình
        // ==============================

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        // ==============================
        // 2. Đổi sang vị trí trong World
        // ==============================

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    0f
                )
            );

        mouseWorldPosition.z = 0f;

        // ==============================
        // 3. Đổi World Position → Cell
        // ==============================

        Vector3Int clickedCell =
            farmableTilemap.WorldToCell(
                mouseWorldPosition
            );

        // ==============================
        // 4. Lấy tâm của ô đó
        // ==============================

        Vector3 cellCenter =
            farmableTilemap.GetCellCenterWorld(
                clickedCell
            );

        // ==============================
        // 5. Kiểm tra khoảng cách
        // ==============================

        float distance =
    Vector2.Distance(
        transform.position,
        cellCenter
    );

        if (distance > farmSoilManager.MaxHoeDistance)
        {
            Debug.Log(
                $"Ô đất quá xa! " +
                $"Khoảng cách: {distance:F2} / " +
                $"Tối đa: {farmSoilManager.MaxHoeDistance:F2}"
            );

            return;
        }

        // ==============================
        // 6. Gửi yêu cầu cuốc đất
        // ==============================

        bool success =
            farmSoilManager.TryHoe(
                clickedCell
            );

        if (success)
        {
            Debug.Log(
                $"Cuốc thành công tại ô {clickedCell}"
            );
        }
    }
}