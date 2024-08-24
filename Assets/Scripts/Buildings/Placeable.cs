using UnityEngine;
public class Placeable
{
    [field:SerializeField] public GameObject Object { get; }
    [field:SerializeField] public Building Building { get; }
    [field:SerializeField] public Vector2Int CenterTile { get; set; }
    public Placeable(GameObject @object, Building building, Vector2Int centerTile)
    {
        Object = @object;
        Building = building;
        CenterTile = centerTile;
    }
}
