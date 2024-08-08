using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class ItemsManager : MonoBehaviour
{
    [SerializeReference] Dictionary<string, Item> items = new Dictionary<string, Item>();
    private void Awake()
    {
        StartCoroutine(LoadItems());
    }
    public Item GetItem(IResourceLocation location)
    {
        return null;
    }
    private IEnumerator LoadItems()
    {
        var loadResourceLocationsHandle
            = Addressables.LoadResourceLocationsAsync("item", typeof(Item));

        if (!loadResourceLocationsHandle.IsDone)
            yield return loadResourceLocationsHandle;

        List<AsyncOperationHandle> opList = new List<AsyncOperationHandle>();

        foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
        {
            AsyncOperationHandle<Item> loadAssetHandle
                = Addressables.LoadAssetAsync<Item>(location);
            loadAssetHandle.Completed +=
            obj =>
            {
                //Debug.Log($"Loaded Item {location.PrimaryKey} | {obj.Result.DisplayName}");
                items.Add(location.PrimaryKey, obj.Result); 
            };
            opList.Add(loadAssetHandle);
        }

        var groupOp = Addressables.ResourceManager.CreateGenericGroupOperation(opList);

        if (!groupOp.IsDone)
            yield return groupOp;

        loadResourceLocationsHandle.Release();
    }
}
