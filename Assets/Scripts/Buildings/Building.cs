using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Resources/Building")]
public class Building : ScriptableObject
{
    [field: SerializeField] public Texture2D Texture { get; private set; }
    [field: SerializeField] public Item Item { get; private set; }
    [field: SerializeField] public GameObject Template { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public Vector2Int CenterOffset { get; private set; } = Vector2Int.zero;
    [field: SerializeField] public Layer AlowedLayers { get; private set; } = Layer.Main;
    [field: SerializeField] public CustomShapeType CustomShape { get; private set; }
    [field: SerializeField] public List<Vector2Int> CustomIncludeExclude { get; private set; } = new List<Vector2Int>();

    public bool Contains(Vector2Int chunkPoint)
    {

        return true;
    }

    public IEnumerable<Vector2Int> GetPositions()
    {
        switch (CustomShape)
        {
            case CustomShapeType.None:
                for(int x = 0; x < Size.x; x++)
                {
                    for (int y = 0; y < Size.y; y++)
                    {
                        yield return new Vector2Int(x, y) - CenterOffset;
                    }
                }
                break;
            case CustomShapeType.Exclude:
                for (int x = 0; x < Size.x; x++)
                {
                    for (int y = 0; y < Size.y; y++)
                    {
                        var position = new Vector2Int(x, y);
                        if(!CustomIncludeExclude.Contains(position))
                        {
                            yield return position - CenterOffset;
                        }
                    }
                }
                break;
            case CustomShapeType.Include:
                foreach (var position in CustomIncludeExclude)
                {
                    yield return position - CenterOffset;
                }
                break;
            default:
                break;
        }
    }
}
public enum CustomShapeType
{
    None,
    Exclude,
    Include
}
[Flags]
public enum Layer
{
    Floor,
    Main,
}