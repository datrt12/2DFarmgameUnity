using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class FarmCursor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HotbarUI hotbarUI;
    [SerializeField] private FarmSoilManager farmSoilManager;

    [SerializeField] private Tilemap farmableTilemap;
    [SerializeField] private Tilemap cursorTilemap;

    [Header("Cursor Tile")]
    [SerializeField] private TileBase cursorTile;

    

    private Camera mainCamera;

    private Vector3Int currentCell;
    private bool hasCurrentCell;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (hotbarUI == null ||
            farmSoilManager == null ||
            farmableTilemap == null ||
            cursorTilemap == null ||
            cursorTile == null)
        {
            ClearCursor();
            return;
        }

        // Không hiện cursor khi đang rê chuột lên UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            ClearCursor();
            return;
        }

        UpdateCursor();
    }

    private void UpdateCursor()
    {
        // Chỉ hiện cursor khi đang chọn Hoe
        ItemData selectedItem = hotbarUI.GetSelectedItem();

        if (selectedItem == null)
        {
            ClearCursor();
            return;
        }

        if (selectedItem.itemCategory != ItemCategory.Tool)
        {
            ClearCursor();
            return;
        }

        if (selectedItem.toolType != ToolType.Hoe)
        {
            ClearCursor();
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            ClearCursor();
            return;
        }

        // Lấy vị trí chuột
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    0f
                )
            );

        mouseWorldPosition.z = 0f;

        // Đổi World Position thành Cell
        Vector3Int cell =
            farmableTilemap.WorldToCell(mouseWorldPosition);

        // Kiểm tra ô có phải đất cuốc được không
        if (!farmableTilemap.HasTile(cell))
        {
            ClearCursor();
            return;
        }

        // Không hiện nếu đất đã cuốc
        if (farmSoilManager.IsTilled(cell))
        {
            ClearCursor();
            return;
        }

        // Lấy tâm ô
        Vector3 cellCenter =
            farmableTilemap.GetCellCenterWorld(cell);

        // Kiểm tra khoảng cách
        float distance =
            Vector2.Distance(
            transform.position,
             cellCenter
    );

        if (distance > farmSoilManager.MaxHoeDistance)
        {
            ClearCursor();
            return;
        }

        // Hiển thị cursor
        ShowCursor(cell);
    }

    private void ShowCursor(Vector3Int cell)
    {
        // Nếu đang ở cùng ô thì không cần cập nhật lại
        if (hasCurrentCell && currentCell == cell)
            return;

        ClearCursor();

        cursorTilemap.SetTile(
            cell,
            cursorTile
        );

        currentCell = cell;
        hasCurrentCell = true;
    }

    private void ClearCursor()
    {
        if (!hasCurrentCell)
            return;

        cursorTilemap.SetTile(
            currentCell,
            null
        );

        hasCurrentCell = false;
    }

    private void OnDisable()
    {
        ClearCursor();
    }
}