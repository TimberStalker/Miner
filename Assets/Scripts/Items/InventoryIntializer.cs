using UnityEngine;

[RequireComponent(typeof(Inventory))]
[DisallowMultipleComponent]
public class InventoryIntializer : MonoBehaviour
{
    [Min(0)]
    [SerializeField] int inputSize;
    [Min(0)]
    [SerializeField] int storageSize;
    [Min(0)]
    [SerializeField] int outputSize;
    Inventory inventory;
    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }
    private void Start()
    {
        inventory.SetSlotSize(storageSize, ItemSlotType.Storage);
        inventory.SetSlotSize(inputSize, ItemSlotType.Input);
        inventory.SetSlotSize(outputSize, ItemSlotType.Output);
    }
}
