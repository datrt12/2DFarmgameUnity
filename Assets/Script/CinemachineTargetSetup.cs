using UnityEngine;
using Unity.Cinemachine;

public class CinemachineTargetSetup : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;

    private void Start()
    {
        // Lấy Cinemachine Camera trên chính GameObject này
        cinemachineCamera =
            GetComponent<CinemachineCamera>();

        if (cinemachineCamera == null)
        {
            Debug.LogError(
                "CinemachineTargetSetup: " +
                "Không tìm thấy CinemachineCamera!"
            );

            return;
        }

        // Tìm Player persistent
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "CinemachineTargetSetup: " +
                "Không tìm thấy Player!"
            );

            return;
        }

        // Gán Player làm Follow Target
        cinemachineCamera.Follow =
            player.transform;

        Debug.Log(
            "Cinemachine Vùng 2 đã Follow Player!"
        );
    }
}