using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class GridLayer : MonoBehaviour
{
    [field: SerializeField] CustomCollider2D HardCollider { get; set; }
    [field: SerializeField] CustomCollider2D PlacementCollider { get; set; }

    List<BuildingTile> tiles = new();
    bool chunkChanged;
    private void Update()
    {
        if (chunkChanged)
        {
            BuildColliders();
            chunkChanged = false;
        }
    }

    public bool ContainsBuilding(Vector2Int position) => ContainsBuilding(position.x, position.y);
    public bool ContainsBuilding(int x, int y)
    {
        return tiles.Any(t => t.Position == new Vector2Int(x, y));
    }
    public void PlaceBuildingAt(Vector2Int position, Placeable placeable)
    {
        if (ContainsBuilding(position))
        {
            Debug.LogError($"Unable to place building tile {placeable.Building.name} for chunk as {position} is already occupied.", this);
            return;
        }
        tiles.Add(new BuildingTile()
        {
            Position = position,
            Placeable = placeable
        });
        chunkChanged = true;
    }
    public void ClearTile(Vector2Int position)
    {
        tiles.RemoveAll(t => t.Position == position);
        chunkChanged = true;
    }
    void BuildColliders()
    {
        PhysicsShapeGroup2D shapeGroup = new PhysicsShapeGroup2D(7, 60);
        List<Vector2Int> sidePositions = new();
        foreach(var tile in tiles)
        {
            int x = tile.Position.x;
            int y = tile.Position.y;

            shapeGroup.AddBox(new Vector2(x + 0.5f, y + 0.5f), Vector2.one);

            if (!sidePositions.Contains(new Vector2Int(x - 1, y - 1)) && ContainsBuilding(x - 1, y - 1))
            {
                sidePositions.Add(new Vector2Int(x - 1, y - 1));
            }
            if (!sidePositions.Contains(new Vector2Int(x + 1, y - 1)) && ContainsBuilding(x + 1, y - 1))
            {
                sidePositions.Add(new Vector2Int(x + 1, y - 1));
            }
            if (!sidePositions.Contains(new Vector2Int(x - 1, y + 1)) && ContainsBuilding(x - 1, y + 1))
            {
                sidePositions.Add(new Vector2Int(x - 1, y + 1));
            }
            if (!sidePositions.Contains(new Vector2Int(x + 1, y + 1)) && ContainsBuilding(x + 1, y + 1))
            {
                sidePositions.Add(new Vector2Int(x + 1, y + 1));
            }
        }
        HardCollider.SetCustomShapes(shapeGroup);
        PhysicsShapeGroup2D placementGroup = new PhysicsShapeGroup2D(7, 60);
        foreach (var item in sidePositions)
        {
            placementGroup.AddBox(new Vector2(item.x + 0.5f, item.y + 0.5f), Vector2.one);
        }
        PlacementCollider.SetCustomShapes(placementGroup);
    }
}
[System.Serializable]
struct BuildingTile
{
    public Vector2Int Position { get; set; }
    public Building Building => Placeable?.Building;
    public Placeable Placeable { get; set; }
}