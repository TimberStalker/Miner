using UnityEngine;

public abstract class MineableObject : MonoBehaviour
{
    //public DigResult Dig(float digPower)
    //{
    //    return Dig(digPower, new DrillProgress(this));
    //}
    //public DigResult Dig(float digPower, DrillProgress progress)
    //{
    //
    //}
    public abstract ItemGroup GetDrillItems();
}
public class DigResult
{
    public ItemSet ItemTransfer { get; }
    public bool Impact { get; set; }
    public DrillProgress Progress { get; }
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