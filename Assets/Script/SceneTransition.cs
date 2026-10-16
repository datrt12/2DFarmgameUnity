using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string targetScene;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning)
            return;

        if (!collision.CompareTag("Player"))
            return;

        isTransitioning = true;

        LoadTargetScene();
    }

    private void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning(
                "SceneTransition: Chưa nhập tên Scene!"
            );

            isTransitioning = false;
            return;
        }

        Debug.Log(
            $"Đang chuyển sang Scene: {targetScene}"
        );

        SceneManager.LoadScene(targetScene);
    }
}