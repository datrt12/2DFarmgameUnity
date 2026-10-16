using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [Header("Drop Settings")]
    [SerializeField] private float dropDistance = 1f;

    private int selectedSlot = -1;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
    }

    private void Update()
    {
        // TEST:
        // Q = drop item ở slot đang chọn

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropSelectedItem();
        }
    }

    public void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;
    }

    public void DropSelectedItem()
    {
        if (selectedSlot < 0)
        {
            Debug.Log("Chưa chọn item để drop!");
            return;
        }

        Vector3 dropPosition =
            transform.position +
            transform.up * dropDistance;

        inventory.DropItem(
            selectedSlot,
            1,
            dropPosition
        );
    }
}