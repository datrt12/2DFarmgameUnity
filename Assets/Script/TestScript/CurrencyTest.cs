using UnityEngine;
using UnityEngine.InputSystem;

public class CurrencyTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private int testAmount = 100;

    private PlayerCurrency playerCurrency;

    private void Start()
    {
        playerCurrency = GetComponent<PlayerCurrency>();

        if (playerCurrency == null)
        {
            Debug.LogWarning(
                "CurrencyTest: Player chưa có PlayerCurrency!",
                this
            );
        }
    }

    private void Update()
    {
        if (playerCurrency == null)
            return;

        if (Keyboard.current == null)
            return;

        // F1 = cộng tiền
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            AddTestMoney();
        }

        // F2 = trừ tiền
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            SpendTestMoney();
        }
    }

    private void AddTestMoney()
    {
        playerCurrency.AddMoney(testAmount);

        Debug.Log(
            "Đã cộng "
            + testAmount
            + " Xu. Tiền hiện tại: "
            + playerCurrency.Money
        );
    }

    private void SpendTestMoney()
    {
        bool success =
            playerCurrency.SpendMoney(testAmount);

        if (success)
        {
            Debug.Log(
                "Đã trừ "
                + testAmount
                + " Xu. Tiền hiện tại: "
                + playerCurrency.Money
            );
        }
        else
        {
            Debug.Log(
                "Không đủ tiền! Tiền hiện tại: "
                + playerCurrency.Money
            );
        }
    }
}   