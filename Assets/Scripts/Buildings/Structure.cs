using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuildingGrid))]
public class Structure : MonoBehaviour
{
    [field: SerializeField] public List<Placement> Placements { get; private set; }

    BuildingGrid grid;
    private void Awake()
    {
        grid = GetComponent<BuildingGrid>();
    }
    void Start()
    {
        foreach (var placement in Placements)
        {
            if(!grid.PlaceBuilding(placement.Position, placement.Building))
            {
                Debug.LogWarning($"Unable to place {placement.Building.name} at {placement.Position}", this);
            }
        }
    }

    void Update()
    {
        
    }

    [System.Serializable]
    public struct Placement
    {
        [field: SerializeField] public GridPosition Position { get; private set; }
        [field: SerializeField] public Building Building { get; private set; }
    }
}