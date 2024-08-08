using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryUIModel", menuName = "UI/Models/InventoryUIModel")]
public class InventoryUIModel : ScriptableObject
{
    [field: SerializeField] public List<ItemSlot> Items = new List<ItemSlot>();
}
