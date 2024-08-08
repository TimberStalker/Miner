using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Resources/Item")]
public class Item : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; set; }
    [field: SerializeField] public Texture2D Image { get; set; }
}
