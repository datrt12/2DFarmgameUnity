using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "SceneSpawnManager: Chưa gán Spawn Point!"
            );

            return;
        }

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "SceneSpawnManager: Không tìm thấy Player!"
            );

            return;
        }

        player.transform.position =
            spawnPoint.position;
    }
}
