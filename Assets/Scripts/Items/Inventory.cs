using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.UI.Image;
[DisallowMultipleComponent]
public class Inventory : MonoBehaviour, IInventory
{
    List<ItemSlot> InputSlots { get; } = new List<ItemSlot>();
    List<ItemSlot> StorageSlots { get; } = new List<ItemSlot>();
    List<ItemSlot> OutputSlots { get; } = new List<ItemSlot>();
    public IEnumerable<ItemSlot> ItemSlots => OutputSlots.Concat(StorageSlots).Concat(InputSlots);
    public event EventHandler<ItemSet> ItemsAdded;
    public void Add(ItemGroup itemTransfer)
    {
        foreach (var item in itemTransfer.Items)
        {
            Add(item);
        }
    }
    public void Add(ItemSet itemTransfer)
    {
        int itemCount = itemTransfer.Count;
        foreach (var itemSlot in ItemSlots.Where(i => i.Item == itemTransfer.Item))
        {
            var original = itemSlot.ItemCount;
            itemSlot.ItemCount += itemCount;
            itemCount -= itemSlot.ItemCount - original;
            break;
        }
        if(itemCount > 0)
        {
            foreach (var itemSlot in ItemSlots.Where(i => i.ItemSet == null))
            {
                var itemSet = new ItemSet() { Item = itemTransfer.Item };
                itemSet.Count += itemTransfer.Count;
                itemSlot.ItemSet = itemSet;
                itemCount -= itemSlot.ItemCount;
                break;
            }
        }
        if(itemCount != itemTransfer.Count)
        {
            ItemsAdded?.Invoke(this, new ItemSet { Item = itemTransfer.Item, Count = itemTransfer.Count - itemCount});
        }
    }
    public int GetItemCount(Item item)
    {
        return ItemSlots.Where(s => s.Item == item).Sum(s => s.ItemCount);
    }
    public void SetSlotSize(int length, ItemSlotType type = ItemSlotType.Storage)
    {
        Assert.IsTrue(length >= 0);
        List <ItemSlot> list = type switch
        {
            ItemSlotType.Storage => StorageSlots,
            ItemSlotType.Input => InputSlots,
            ItemSlotType.Output => OutputSlots,
            _ => throw new System.NotImplementedException(),
        };
        if (list.Count == length) return;
        if(list.Count < length)
        {
            while(list.Count < length)
            {
                list.Add(new ItemSlot { SlotType = type });
            }
        }
        else
        {
            while(list.Count > length)
            {
                var removedItem = list[^1];
                list.RemoveAt(list.Count - 1);
            }
        }
    }

    public void Drop(ItemSet itemSet)
    {

    }
}
public interface IInventory
{
    public IEnumerable<ItemSlot> ItemSlots { get; }
    void Add(ItemSet itemTransfer);
}
[System.Serializable]
public sealed class ItemSet
{
    [field: SerializeField] public Item Item { get; set; }
    [field: SerializeField] public int Count { get; set; }
}
[System.Serializable]
public sealed class ItemGroup
{
    [field: SerializeField] public IReadOnlyList<ItemSet> Items { get; }
    public ItemGroup(params ItemSet[] items) => Items = items;
    public ItemGroup(List<ItemSet> items) => Items = items;
}
[System.Serializable]
public sealed class ItemSlot
{
    [field:SerializeField] public ItemSlotType SlotType { get; set; }
    public Item Item => ItemSet?.Item;
    public int ItemCount
    {
        get => ItemSet.Count;
        set => ItemSet.Count = value;
    }
    [field: SerializeField] public ItemSet ItemSet { get; set; }
}
public enum ItemSlotType
{
    Storage,
    Input,
    Output
}