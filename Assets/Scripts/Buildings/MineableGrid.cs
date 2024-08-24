using UnityEngine;
[RequireComponent(typeof(BuildingGrid))]
public class MineableGrid : MineableObject
{
    BuildingGrid buildingGrid;
    private void OnEnable()
    {
        buildingGrid = GetComponent<BuildingGrid>();
    }
    protected override DigResult DigInternal(DigSettings drillInfo, DrillProgress progress)
    {
        var dipPosition = buildingGrid.Grid.WorldToLocal(drillInfo.DigPosition);
        int x = Mathf.FloorToInt(dipPosition.x);
        int y = Mathf.FloorToInt(dipPosition.y);

        var xOffset = 0.5f - (dipPosition.x - x);
        var yOffset = 0.5f - (dipPosition.y - y);

        if(Mathf.Abs(xOffset) >= Mathf.Abs(yOffset))
        {
            x -= (int)Mathf.Sign(xOffset);
        }
        else
        {
            y -= (int)Mathf.Sign(yOffset);
        }


        progress.Progress += drillInfo.DigPower * Time.fixedDeltaTime;
        if(progress.Progress >= 1)
        {
            var removedBuilding = buildingGrid.RemoveBuilding(new GridPosition(x, y, 0));
            if (removedBuilding != null)
            {
                progress.Progress = 0;
                return new DigResult(progress, true, 
                    new ItemGroup(new ItemSet { Item = removedBuilding.Item, Count = 1}));
            }
        }

        return new DigResult(progress, false);
    }
}
