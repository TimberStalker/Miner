using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Recipie", menuName = "Resources/Recipie")]
public class Recipe : ScriptableObject
{
    [field:SerializeField] public List<RecipeItem> RecipeItems { get; private set; }
    [field: SerializeField] public RecipeItem Result { get; private set; }
}
[System.Serializable]
public class RecipeItem
{
    [field:SerializeField] public Item Item { get; private set; }
    [field: SerializeField] public int Count { get; private set; } = 1;
}
