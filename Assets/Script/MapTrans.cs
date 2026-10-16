using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    [Header("Kéo Camera của khu vực này vào đây")]
    public CinemachineCamera roomCamera;

    // Khi Player BƯỚC VÀO khu vực map này
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            roomCamera.Priority = 20;
        }
    }

    // Khi Player ĐI RA KHỎI khu vực map này
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            roomCamera.Priority = 0;
        }
    }
}