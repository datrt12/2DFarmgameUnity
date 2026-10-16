using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmSoilManager : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap farmableTilemap;
    [SerializeField] private Tilemap soilStateTilemap;

    [Header("Soil Tiles")]
    [SerializeField] private TileBase tilledSoilTile;

    [Header("Settings")]
    [SerializeField] private float revertTime = 10f;

    [Header("Tool Range")]
    [SerializeField, Min(0f)]
    private float maxHoeDistance = 1.5f;

    public float MaxHoeDistance => maxHoeDistance;

    private readonly Dictionary<Vector3Int, Coroutine> revertTasks =
        new Dictionary<Vector3Int, Coroutine>();

    public bool TryHoe(Vector3Int cell)
    {
        // ==============================
        // Kiểm tra Reference
        // ==============================

        if (farmableTilemap == null)
        {
            Debug.LogWarning(
                "FarmSoilManager: Chưa gán Farmable Tilemap!"
            );

            return false;
        }

        if (soilStateTilemap == null)
        {
            Debug.LogWarning(
                "FarmSoilManager: Chưa gán Soil State Tilemap!"
            );

            return false;
        }

        if (tilledSoilTile == null)
        {
            Debug.LogWarning(
                "FarmSoilManager: Chưa gán Tilled Soil Tile!"
            );

            return false;
        }

        // ==============================
        // Kiểm tra ô có đất hay không
        // ==============================

        if (!farmableTilemap.HasTile(cell))
        {
            Debug.Log("Ô này không thể cuốc.");
            return false;
        }

        // ==============================
        // Kiểm tra đã cuốc chưa
        // ==============================

        if (IsTilled(cell))
        {
            Debug.Log("Ô đất này đã được cuốc.");
            return false;
        }

        // ==============================
        // Đặt tile đất đã cuốc
        // ==============================

        soilStateTilemap.SetTile(
            cell,
            tilledSoilTile
        );

        Debug.Log(
            $"Đã cuốc đất tại {cell}"
        );

        // ==============================
        // Bắt đầu đếm thời gian
        // ==============================

        StartRevertTimer(cell);

        return true;
    }

    public bool IsTilled(Vector3Int cell)
    {
        if (soilStateTilemap == null)
            return false;

        return soilStateTilemap.GetTile(cell) == tilledSoilTile;
    }

    private void StartRevertTimer(Vector3Int cell)
    {
        // Nếu ô này đã có timer
        // thì dừng timer cũ trước
        if (revertTasks.TryGetValue(
            cell,
            out Coroutine oldCoroutine))
        {
            if (oldCoroutine != null)
            {
                StopCoroutine(oldCoroutine);
            }
        }

        Coroutine newCoroutine =
            StartCoroutine(
                RevertAfterDelay(cell)
            );

        revertTasks[cell] = newCoroutine;
    }

    private IEnumerator RevertAfterDelay(
        Vector3Int cell)
    {
        // Chờ số giây được thiết lập
        yield return new WaitForSeconds(revertTime);

        // Xóa tile đất đã cuốc
        soilStateTilemap.SetTile(
            cell,
            null
        );

        // Xóa timer khỏi Dictionary
        revertTasks.Remove(cell);

        Debug.Log(
            $"Đất tại {cell} đã trở lại bình thường."
        );
    }

    // Hàm này sẽ dùng sau này
    // khi người chơi trồng cây trên đất đã cuốc.
    public void CancelRevertTimer(
        Vector3Int cell)
    {
        if (revertTasks.TryGetValue(
            cell,
            out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            revertTasks.Remove(cell);
        }
    }
}