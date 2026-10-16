using UnityEngine;
using System;

public class PlayerCurrency : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private int money = 500;

    public int Money => money;

    public event Action<int> OnMoneyChanged;

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        money += amount;

        OnMoneyChanged?.Invoke(money);
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0)
            return false;

        if (money < amount)
            return false;

        money -= amount;

        OnMoneyChanged?.Invoke(money);

        return true;
    }

    public bool HasEnoughMoney(int amount)
    {
        return money >= amount;
    }

    public void SetMoney(int amount)
    {
        money = Mathf.Max(0, amount);

        OnMoneyChanged?.Invoke(money);
    }
}