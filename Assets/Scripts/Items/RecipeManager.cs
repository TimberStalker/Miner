using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class RecipeManager : MonoBehaviour
{
    public Dictionary<string, Recipe> Recipes { get; private set; } = new Dictionary<string, Recipe>();
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
            = Addressables.LoadResourceLocationsAsync("recipe", typeof(Recipe));

        if (!loadResourceLocationsHandle.IsDone)
            yield return loadResourceLocationsHandle;

        List<AsyncOperationHandle> opList = new List<AsyncOperationHandle>();

        foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
        {
            AsyncOperationHandle<Recipe> loadAssetHandle
                = Addressables.LoadAssetAsync<Recipe>(location);
            loadAssetHandle.Completed +=
            obj =>
            {
                //Debug.Log($"Loaded Recipe {location.PrimaryKey}");
                Recipes.Add(location.PrimaryKey, obj.Result);
            };
            opList.Add(loadAssetHandle);
        }

        var groupOp = Addressables.ResourceManager.CreateGenericGroupOperation(opList);

        if (!groupOp.IsDone)
            yield return groupOp;

        loadResourceLocationsHandle.Release();
    }
}
