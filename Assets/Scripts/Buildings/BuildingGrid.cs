using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Grid))]
public class BuildingGrid : MonoBehaviour
{
    [field: SerializeField] public List<LayerNode> Layers { get; private set; } = new List<LayerNode>();

    [field: SerializeField] public GridLayer ChunkPrefab { get; private set; }

    new Rigidbody2D rigidbody;
    public Grid Grid { get; private set; }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        Grid = GetComponent<Grid>();
    }

    public bool CanPlaceBuilding(GridPosition position, Building building)
    {
        var positions = building.GetPositions();
        var layer = GetOrCreateLayer(position.Layer);
        if (positions.Any(p => layer.Layer.ContainsBuilding(p + position)))
        {
            return false;
        }
        return true;
    }

    public bool PlaceBuilding(in GridPosition position, Building building)
    {
        if (building.Template == null)
        {
            Debug.LogError($"Building does not containt a template.", building);
            return false;
        }

        if (!CanPlaceBuilding(position, building)) return false;

        var layer = GetOrCreateLayer(position.Layer).Layer;

        var blockInstance = Instantiate(building.Template, layer.transform);

        var placeable = new Placeable(blockInstance, building, position);
        
        blockInstance.transform.localPosition = new Vector3(position.X, position.Y, 0);
        blockInstance.transform.localRotation = Quaternion.identity;


        foreach(var blockLocalposition in building.GetPositions())
        {
            layer.PlaceBuildingAt(blockLocalposition + position, placeable);
        }

        return true;
    }

    public Building RemoveBuilding(in GridPosition position)
    {
        Debug.Log($"Removing Building At ({position.X}, {position.Y}) [{position.Layer}]");
        var layer = GetOrCreateLayer(position.Layer).Layer;

        var placeable = layer.GetBuilding(position);
        if (placeable == null) return null;

        foreach (var blockLocalposition in placeable.Building.GetPositions())
        {
            layer.ClearTile(blockLocalposition + placeable.CenterTile);
        }
        var building = placeable.Building;
        Destroy(placeable.Object);

        return building;
    }

    public LayerNode GetLayer(int position)
    {
        return Layers.FirstOrDefault(l => l.Position == position);
    }
    public LayerNode GetOrCreateLayer(int position)
    {
        var layer = GetLayer(position);
        if(layer.Layer == null)
        {
            layer = new LayerNode(Instantiate(ChunkPrefab, transform), position);
            Layers.Add(layer);
        }
        return layer;
    }

    public struct LayerNode
    {
        public GridLayer Layer { get; }
        public int Position { get; }
        public LayerNode(GridLayer layer, int position)
        {
            Layer = layer;
            Position = position;
        }
    }
}
[Serializable]
public struct GridPosition
{
    [field:SerializeField] public int X { get; set; }
    [field:SerializeField] public int Y { get; set; }
    [field: SerializeField] public int Layer { get; set; }
    public GridPosition(int x, int y, int layer)
    {
        X = x;
        Y = y;
        Layer = layer;
    }
    public static implicit operator Vector3Int(GridPosition position)
    {
        return new Vector3Int(position.X, position.Y, position.Layer);
    }
    public static implicit operator GridPosition(Vector3Int position)
    {
        return new GridPosition(position.x, position.y, position.z);
    }
    public static implicit operator Vector2Int(GridPosition position)
    {
        return new Vector2Int(position.X, position.Y);
    }
}