using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceSource : MineableObject
{
    [field:SerializeField] AssetReferenceT<Item> ResourceReference { get; set; }
    [field: SerializeField] public float ResourceDensity { get; private set; } = 1;

    public Item Resource
    {
        get
        {
            return (Item)ResourceReference.Asset;
        }
    }
    ItemsManager itemsManager;
    void Start()
    {
        ResourceReference.LoadAssetAsync();
        //ResourceReference.Asset
        //var handle = Addressables.LoadAssetAsync<Item>(ResourceReference);
        //handle.Completed += handle =>
        //{
        //    resource = handle.Result;
        //};
    }

    //public override ItemGroup GetDrillItems()
    //{
    //    return new ItemGroup(new ItemSet() { Item = Resource, Count = Mathf.CeilToInt(ResourceDensity) });
    //}

    protected override DigResult DigInternal(DigSettings drillInfo, DrillProgress progress)
    {
        progress.Progress += drillInfo.DigPower * Time.fixedDeltaTime;
        ItemGroup items = null;
        if (progress.Progress > 1)
        {
            progress.Progress = 0;
            items = new ItemGroup(new ItemSet() { Item = Resource, Count = Mathf.CeilToInt(ResourceDensity) });
        }
        return new DigResult(progress, Random.Range(0f, 1f) > .9f, items);
    }
}