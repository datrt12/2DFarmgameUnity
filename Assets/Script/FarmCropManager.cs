using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmCropManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap farmableTilemap;
    [SerializeField] private FarmSoilManager farmSoilManager;

    [Header("Crop")]
    [SerializeField] private GameObject cropPrefab;

    private readonly Dictionary<Vector3Int, CropPlant> plantedCrops =
        new Dictionary<Vector3Int, CropPlant>();

    public bool TryPlant(
        Vector3Int cell,
        CropData cropData)
    {
        if (cropData == null)
        {
            Debug.LogWarning(
                "FarmCropManager: CropData đang NULL!"
            );

            return false;
        }

        if (cropPrefab == null)
        {
            Debug.LogWarning(
                "FarmCropManager: Chưa gán Crop Prefab!"
            );

            return false;
        }

        if (farmableTilemap == null)
        {
            Debug.LogWarning(
                "FarmCropManager: Chưa gán Farmable Tilemap!"
            );

            return false;
        }

        if (farmSoilManager == null)
        {
            Debug.LogWarning(
                "FarmCropManager: Chưa gán FarmSoilManager!"
            );

            return false;
        }

        // Ô phải tồn tại
        if (!farmableTilemap.HasTile(cell))
        {
            Debug.Log("Ô này không phải đất.");
            return false;
        }

        // Ô phải được cuốc
        if (!farmSoilManager.IsTilled(cell))
        {
            Debug.Log("Ô đất chưa được cuốc.");
            return false;
        }

        // Không được trồng chồng cây
        if (plantedCrops.ContainsKey(cell))
        {
            Debug.Log("Ô này đã có cây.");
            return false;
        }

        // Vị trí giữa ô
        Vector3 plantPosition =
            farmableTilemap.GetCellCenterWorld(cell);

        plantPosition.z = 0f;

        GameObject cropObject =
            Instantiate(
                cropPrefab,
                plantPosition,
                Quaternion.identity
            );

        CropPlant cropPlant =
            cropObject.GetComponent<CropPlant>();

        if (cropPlant == null)
        {
            Debug.LogError(
                "Crop Prefab không có CropPlant!"
            );

            Destroy(cropObject);
            return false;
        }

        plantedCrops.Add(
            cell,
            cropPlant
        );

        // Rất quan trọng:
        // cây đã chiếm ô đất này,
        // không cho đất tự trở lại bình thường.
        farmSoilManager.CancelRevertTimer(cell);

        cropPlant.Initialize(
            cropData,
            this,
            cell
        );

        Debug.Log(
            $"Đã trồng {cropData.cropName} tại {cell}"
        );

        return true;
    }

    public bool HasCrop(Vector3Int cell)
    {
        return plantedCrops.ContainsKey(cell);
    }

    public void RemoveCrop(Vector3Int cell)
    {
        plantedCrops.Remove(cell);
    }
}