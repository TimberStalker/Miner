using UnityEngine;

public abstract class MineableObject : MonoBehaviour
{
    public virtual bool IsSameTarget(DigSettings drillInfo, DrillProgress progress) => progress.Target == this;
    public virtual DrillProgress GetDrillProgress(DigSettings drillInfo, DrillProgress progress) => new DrillProgress(this);
    public DigResult Dig(DigSettings drillInfo, DrillProgress progress)
    {
        if(progress == null || !IsSameTarget(drillInfo, progress))
        {
            progress = GetDrillProgress(drillInfo, progress);
        }
        return DigInternal(drillInfo, progress);
    }
    protected abstract DigResult DigInternal(DigSettings drillInfo, DrillProgress progress);
}
public class DigSettings
{
    public float DigPower { get; set; }
    public Vector2 DigPosition { get; set; }
    public float DigDirection { get; set; }
    public DigSettings(float digPower, Vector2 digPosition, float digDirection)
    {
        DigPower = digPower;
        DigPosition = digPosition;
        DigDirection = digDirection;
    }

}
public class DigResult
{
    public ItemGroup ItemTransfer { get; }
    public bool Impact { get; set; }
    public DrillProgress Progress { get; }
    public DigResult(DrillProgress progress, bool impact, ItemGroup itemTransfer)
    {
        ItemTransfer = itemTransfer;
        Impact = impact;
        Progress = progress;
    }
    public DigResult(DrillProgress progress, bool impact)
    {
        Impact = impact;
        Progress = progress;
    }
}
public class DrillProgress
{
    public float Progress { get; set; }
    public MineableObject Target { get; }
    public DrillProgress(MineableObject target)
    {
        Target = target;
    }
}