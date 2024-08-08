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

    public override ItemGroup GetDrillItems()
    {
        return new ItemGroup(new ItemSet() { Item = Resource, Count = Mathf.CeilToInt(ResourceDensity) });
    }
}