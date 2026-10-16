using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class FarmPlantingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private HotbarUI hotbarUI;
    [SerializeField] private FarmCropManager farmCropManager;
    [SerializeField] private Tilemap farmableTilemap;

    [Header("Planting Settings")]
    [SerializeField] private float maxPlantDistance = 1.5f;

    [Header("Crop")]
    [SerializeField] private CropData carrotCrop;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (inventory == null ||
            hotbarUI == null ||
            farmCropManager == null ||
            farmableTilemap == null)
        {
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Không trồng khi click vào UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        TryPlantSelectedSeed();
    }

    private void TryPlantSelectedSeed()
    {
        ItemData selectedItem =
            hotbarUI.GetSelectedItem();

        if (selectedItem == null)
            return;

        // Chỉ cho phép Seed
        if (selectedItem.itemCategory != ItemCategory.Seed)
            return;

        // Phiên bản đầu tiên:
        // nếu chọn đúng CarrotSeed → trồng Carrot
        if (selectedItem != carrotCrop.seedItem)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

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

        Vector3Int cell =
            farmableTilemap.WorldToCell(
                mouseWorldPosition
            );

        Vector3 cellCenter =
            farmableTilemap.GetCellCenterWorld(cell);

        // Kiểm tra khoảng cách
        float distance =
            Vector2.Distance(
                transform.position,
                cellCenter
            );

        if (distance > maxPlantDistance)
        {
            Debug.Log("Ô đất quá xa để trồng.");
            return;
        }

        // Kiểm tra ô đã có cây chưa
        if (farmCropManager.HasCrop(cell))
        {
            Debug.Log("Ô này đã có cây.");
            return;
        }

        // Quan trọng:
        // Trừ hạt giống trước khi trồng
        int selectedSlot =
            hotbarUI.GetSelectedIndex();

        bool removed =
            inventory.RemoveItemFromSlot(
                selectedSlot,
                1
            );

        if (!removed)
        {
            Debug.Log("Không có hạt giống để trồng.");
            return;
        }

        bool planted =
            farmCropManager.TryPlant(
                cell,
                carrotCrop
            );

        if (!planted)
        {
            // Nếu trồng thất bại,
            // trả lại hạt giống
            inventory.AddItem(
                selectedItem,
                1
            );

            return;
        }

        Debug.Log(
            $"Đã gieo {selectedItem.itemName}"
        );
    }
}