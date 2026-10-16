using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    private PlayerCurrency playerCurrency;

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "Không tìm thấy Player!",
                this
            );

            return;
        }

        playerCurrency =
            player.GetComponent<PlayerCurrency>();

        if (playerCurrency == null)
        {
            Debug.LogWarning(
                "Player chưa có PlayerCurrency!",
                this
            );

            return;
        }

        playerCurrency.OnMoneyChanged += UpdateMoneyUI;

        UpdateMoneyUI(playerCurrency.Money);
    }

    private void UpdateMoneyUI(int money)
    {
        if (moneyText == null)
            return;

        moneyText.text = money.ToString("N0") + " Xu";
    }

    private void OnDestroy()
    {
        if (playerCurrency != null)
        {
            playerCurrency.OnMoneyChanged -= UpdateMoneyUI;
        }
    }
}