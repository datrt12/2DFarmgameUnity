using System.Collections;
using UnityEngine;

public class CropPlant : MonoBehaviour
{
    private CropData cropData;
    private FarmCropManager cropManager;
    private Vector3Int cell;

    private SpriteRenderer spriteRenderer;

    private int currentStage;
    private Coroutine growthCoroutine;

    public void Initialize(
        CropData data,
        FarmCropManager manager,
        Vector3Int plantedCell)
    {
        cropData = data;
        cropManager = manager;
        cell = plantedCell;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError(
                "CropPlant: Không tìm thấy SpriteRenderer!"
            );

            return;
        }

        if (cropData == null)
        {
            Debug.LogError(
                "CropPlant: CropData đang NULL!"
            );

            return;
        }

        currentStage = 0;

        UpdateSprite();

        growthCoroutine =
            StartCoroutine(GrowCrop());
    }

    private IEnumerator GrowCrop()
    {
        while (currentStage < 5)
        {
            yield return new WaitForSeconds(
                cropData.stageDuration
            );

            currentStage++;

            UpdateSprite();
        }

        // Cây đã chín
        if (cropData.matureDropDelay > 0f)
        {
            yield return new WaitForSeconds(
                cropData.matureDropDelay
            );
        }

        DropHarvest();
    }

    private void UpdateSprite()
    {
        if (cropData == null)
            return;

        if (cropData.growthSprites == null)
            return;

        if (currentStage < 0 ||
            currentStage >= cropData.growthSprites.Length)
        {
            return;
        }

        Sprite sprite =
            cropData.growthSprites[currentStage];

        spriteRenderer.sprite = sprite;
    }

    private void DropHarvest()
    {
        if (cropData.harvestItem == null)
        {
            Debug.LogWarning(
                $"Crop {cropData.cropName} chưa có Harvest Item!"
            );

            DestroyCrop();
            return;
        }

        if (cropData.harvestItem.collectablePrefab == null)
        {
            Debug.LogWarning(
                $"Item {cropData.harvestItem.itemName} " +
                "chưa có Collectable Prefab!"
            );

            DestroyCrop();
            return;
        }

        for (int i = 0; i < cropData.harvestAmount; i++)
        {
            Vector3 dropPosition =
                transform.position;

            Instantiate(
                cropData.harvestItem.collectablePrefab,
                dropPosition,
                Quaternion.identity
            );
        }

        Debug.Log(
            $"{cropData.cropName} đã chín " +
            $"→ rơi {cropData.harvestItem.itemName} " +
            $"x{cropData.harvestAmount}"
        );

        DestroyCrop();
    }

    private void DestroyCrop()
    {
        if (cropManager != null)
        {
            cropManager.RemoveCrop(cell);
        }

        Destroy(gameObject);
    }
}