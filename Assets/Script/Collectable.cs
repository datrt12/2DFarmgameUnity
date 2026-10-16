using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private ItemData itemData;

    [Header("Quantity")]
    [SerializeField] private int quantity = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Inventory inventory = collision.GetComponent<Inventory>();

        if (inventory == null)
        {
            Debug.LogWarning("Player chưa có Inventory!");
            return;
        }

        bool added = inventory.AddItem(itemData, quantity);

        if (added)
        {
            Destroy(gameObject);
        }
    }
}