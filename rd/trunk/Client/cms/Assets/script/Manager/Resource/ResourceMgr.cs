using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PathologicalGames;

//---------------------------------------------------------------------------------------------
// call backs
public delegate void AssetLoadedCallBack(GameObject instance, System.EventArgs args, AssetBundle bundle);

// asynchronous request
public class AssetRequest
{
    //public bool isScene = false;
    //public SceneLoadedCallBack sceneCallBack = null;
    public string name = null;
    public string bundleName = null;

    public System.EventArgs args = null;
    public AssetLoadedCallBack assetCallBack = null;

    public AssetRequest(string bundleName, string name)
    {
        this.bundleName = bundleName;
        this.name = name;
    }

    public AssetRequest(string bundleName, string name, AssetLoadedCallBack callBack, System.EventArgs args = null)
    {
        this.bundleName = bundleName;
        this.name = name;
        this.assetCallBack = callBack;
        this.args = args;
    }
}
//---------------------------------------------------------------------------------------------
public class LoadedAssetBundle
{
    public AssetBundle assetBundle;
    public int referencedCount;

    public LoadedAssetBundle(AssetBundle assetBundle)
    {
        this.assetBundle = assetBundle;
        referencedCount = 1;
    }
}
//---------------------------------------------------------------------------------------------
public class ResourceMgr : MonoBehaviour
{
    private string[] m_Variants = { };
    private AssetBundleManifest manifest;
    private AssetBundle assetbundle;
    private Dictionary<string, LoadedAssetBundle> loadedAssetBundleList = new Dictionary<string, LoadedAssetBundle>();
    private Dictionary<string, string[]> dependenceList = new Dictionary<string, string[]>();

    private List<string> abRequestToRemoveList = new List<string>();
    private Dictionary<string, AssetBundleCreateRequest> abRequestList = new Dictionary<string, AssetBundleCreateRequest>();

    static ResourceMgr mInst = null;
    public SpawnPool objectPool = null;
    //pooled prefab
    private Dictionary<string, GameObject> battlePoolList = new Dictionary<string,GameObject>();

    public static ResourceMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("ResourceMgr");
                mInst = go.AddComponent<ResourceMgr>();
            }
            return mInst;
        }
    }

    //---------------------------------------------------------------------------------------------
    void Update()
    {
        //update bundle request
        foreach (var keyValue in abRequestList)
        {
            AssetBundleCreateRequest abRequest = keyValue.Value;
            if (abRequest.isDone)
            {
                LoadedAssetBundle loadedBundle = new LoadedAssetBundle(abRequest.assetBundle);
                loadedAssetBundleList.Add(keyValue.Key, loadedBundle);

                abRequestToRemoveList.Add(keyValue.Key);
            }
        }

        // Remove the finished WWWs.
        foreach (var key in abRequestToRemoveList)
        {
            abRequestList.Remove(key);
        }

        abRequestList.Clear();
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if (assetbundle != null)
        {
            Logger.LogError("warning, resource manager is inited already, destroy may failed last time!");
            return;
        }

        byte[] stream = null;
        string uri = Path.Combine(Util.AssetBundlePath, Const.AssetDirname);
        objectPool = PoolManager.Pools.Create("ObjectPool");

        if (File.Exists(uri))
        {
            stream = File.ReadAllBytes(uri);
            assetbundle = AssetBundle.CreateFromMemoryImmediate(stream);
            manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        else
        {
            Logger.LogWarning("Assets file not exists!!! Ignoring...");
        }

        //资源初始化完成，回调游戏管理器，执行后续操作 
        GameApp.Instance.OnResourceInited();
    }
    //---------------------------------------------------------------------------------------------
    public void LoadAssetAsyn(string abname, string name, AssetLoadedCallBack callBack = null, System.EventArgs args = null)
    {
        GameObject obj;
        if ((obj = GetPoolObject(name)) != null)
        {
            if (callBack != null)
            {
                callBack(obj, args, null);
            }
            return;
        }

        StartCoroutine(LoadAssetRequest(new AssetRequest(abname, name, callBack, args)));
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator LoadAssetRequest(AssetRequest request)
    {
        LoadAssetBundleAsync(request.bundleName);
        AssetBundleLoadAssetOperationFull loadAssetOperate = new AssetBundleLoadAssetOperationFull(
            request.bundleName,
            request.name,
            typeof(GameObject)
            );

        if (loadAssetOperate == null)
            yield break;

        yield return StartCoroutine(loadAssetOperate);

        GameObject prefab = loadAssetOperate.GetAsset<GameObject>();
        CreatePoolObject(prefab);
        if (request.assetCallBack != null)
        {
            request.assetCallBack(Instantiate(prefab), request.args, GetLoadedAssetBundle(request.bundleName).assetBundle);
        }

        UnloadAssetBundle(request.bundleName);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 载入素材
    /// </summary>
    /// TODO: remove cache
    public GameObject LoadAsset(string assetname, bool cache = false)
    {
        assetname = StaticDataMgr.Instance.GetRealName(assetname);

        GameObject obj = GetPoolObject(assetname);
        if (obj != null)
        {
            return obj;
        }
        string abname = StaticDataMgr.Instance.GetBundleName(assetname).ToLower();
        AssetBundle bundle = LoadAssetBundle(abname);
        if (bundle == null)
        {
            Logger.LogErrorFormat("Load bundle{0} faild", abname);
            return null;
        }
        GameObject prefab = bundle.LoadAsset<GameObject>(assetname);
        if (prefab == null)
        {
            Logger.LogErrorFormat("Load asset{0} faild", assetname);
            return null;
        }

        //obj = Instantiate(prefab);
        //obj.name = assetname;
        //return obj;
        GameObject go = null;
        if (cache == true)
        {
            CreatePoolObject(prefab);
            //go = objectPool.Spawn(prefab.transform, prefab.transform.localPosition, prefab.transform.localRotation).gameObject;
            go = GetPoolObject(prefab.name);
        }
        else
        {
            CreatePoolObject(prefab);
            //go = objectPool.Spawn(prefab.transform, prefab.transform.localPosition, prefab.transform.localRotation).gameObject;
            go = GetPoolObject(prefab.name);
            //go = Instantiate(prefab);
        }

        UnloadAssetBundle(abname);
        //Resources.UnloadUnusedAssets();
        System.GC.Collect();

        return go;
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator LoadAssetBatch(List<KeyValuePair<string, string>> assetList)
    {
        int count = assetList.Count;
        //List<string> requestBundleList = new List<string>();
        string bundleName;
        string assetName;
        for (int i = 0; i < count; ++i)
        {
            assetName = assetList[i].Key;
            bundleName = assetList[i].Value;
            
            //GameObject obj = GetPoolObject(assetName);
            //if (obj != null)
            //{
            //    continue;
            //}

            bundleName = bundleName.ToLower();
            LoadAssetBundleAsync(bundleName);
            AssetBundleLoadAssetOperationFull loadAssetOperate = new AssetBundleLoadAssetOperationFull(
                bundleName,
                assetName,
                typeof(GameObject)
                );

            if (loadAssetOperate == null)
                yield break;

            yield return StartCoroutine(loadAssetOperate);

            GameObject prefab = loadAssetOperate.GetAsset<GameObject>();
            CreatePoolObject(prefab);
            //yield return StartCoroutine(LoadAssetRequest(new AssetRequest(bundleName, name)));
            //CreatePoolObject(prefab);
        }
        for (int i = 0; i < count; ++i)
        {
            bundleName = assetList[i].Value;
            UnloadAssetBundle(bundleName);
            //Resources.UnloadUnusedAssets();
        }
        System.GC.Collect();

        //for (int i = 0; i < count; ++i)
        //{
        //    UnloadAssetBundle(assetList[i].Value);
        //    Resources.UnloadUnusedAssets();
        //}
        //System.GC.Collect();
    }
    //---------------------------------------------------------------------------------------------
    public T LoadAssetType<T>(string assetname) where T:Object
    {
        //assetname = StaticDataMgr.Instance.GetRealName(assetname);
        string abname = StaticDataMgr.Instance.GetBundleName(assetname).ToLower();
        abname = abname.ToLower();
        AssetBundle bundle = LoadAssetBundle(abname);

        return bundle.LoadAsset<T>(assetname);

        //TODO: pool object too
        //T prefab = bundle.LoadAsset<T>(assetname);
        //T obj = Object.Instantiate(prefab);

        //UnloadAssetBundle(abname);
        //Resources.UnloadUnusedAssets();
        //System.GC.Collect();

        //return obj;
    }
    //---------------------------------------------------------------------------------------------
    public void DestroyAsset(GameObject target, bool cache = false)
    {
        if (cache == true)
        {
            Destroy(target);
            //objectPool.Despawn(target.transform);
            //float defaultZ = target.transform.localPosition.z;
            //target.transform.SetParent(objectPool.transform, false);
            ////target.transform.parent = objectPool.transform;
            ////target.transform.localPosition = new Vector3(0, 0, defaultZ);
        }
        else
        {
            Destroy(target);
        }

        //Resources.UnloadUnusedAssets();
        //System.GC.Collect();
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetPoolObject(string name)
    {
        //if (objectPool.prefabs.ContainsKey(name))
        //{
        //    Transform trans = objectPool.Spawn(name);
        //    if (trans != null)
        //        return trans.gameObject;
        //}

        GameObject go;
        if (battlePoolList.TryGetValue(name, out go))
        {
            return Instantiate(go);
        }

        return null;
    }
    //---------------------------------------------------------------------------------------------
    public void CreatePoolObject(GameObject srcObj)
    {
        if (battlePoolList.ContainsKey(srcObj.name) == false)
        {
            battlePoolList.Add(srcObj.name, srcObj);
        }
        //PrefabPool prefab = new PrefabPool(srcObj.transform);
        //prefab.preloadAmount = 0;
        //prefab.limitFIFO = false;
        //prefab.limitInstances = true;
        //prefab.cullAbove = 10;
        ////prefab.cullDespawned = true;
        ////prefab.cullAbove = 0;
        ////prefab.cullDelay = 60;
        //objectPool._perPrefabPoolOptions.Add(prefab);
        //objectPool.CreatePrefabPool(prefab);
    }
    //---------------------------------------------------------------------------------------------
    bool LoadAssetBundleAsync(string abname)
    {
        if (!abname.EndsWith(Const.ExtName))
        {
            abname += Const.ExtName;
        }

        LoadedAssetBundle loadedBundle;
        if (loadedAssetBundleList.TryGetValue(abname, out loadedBundle) == true)
        {
            ++loadedBundle.referencedCount;
            return true;
        }

        byte[] stream = null;
        string uri = Path.Combine(Util.AssetBundlePath, abname);
        Logger.Log("LoadFile::>> " + uri);
        LoadDependencies(abname, true);

        stream = File.ReadAllBytes(uri);
        AssetBundleCreateRequest abRequest = AssetBundle.CreateFromMemory(stream);
        abRequestList.Add(abname, abRequest);

        return false;
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 载入AssetBundle
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>
    AssetBundle LoadAssetBundle(string abname)
    {
        if (!abname.EndsWith(Const.ExtName))
        {
            abname += Const.ExtName;
        }

        LoadedAssetBundle loadedBundle = null;
        if (!loadedAssetBundleList.ContainsKey(abname))
        {
            byte[] stream = null;
            string uri = Path.Combine(Util.AssetBundlePath, abname);
            Logger.Log("LoadFile::>> " + uri);
            LoadDependencies(abname);
            stream = File.ReadAllBytes(uri);
            AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(stream); //关联数据的素材绑定
            loadedBundle = new LoadedAssetBundle(bundle);
            loadedAssetBundleList.Add(abname, loadedBundle);
        }
        else
        {
            loadedAssetBundleList.TryGetValue(abname, out loadedBundle);
            ++loadedBundle.referencedCount;
        }

        return loadedBundle.assetBundle;
    }
    //---------------------------------------------------------------------------------------------
    public LoadedAssetBundle GetLoadedAssetBundle(string bundleName)
    {
        LoadedAssetBundle bundle;
        if (loadedAssetBundleList.TryGetValue(bundleName, out bundle) == true)
        {
            return bundle;
        }

        return null;
    }
    //---------------------------------------------------------------------------------------------
    void UnloadAssetBundle(string abname)
    {
        UnloadAssetBundleInternal(abname);
        UnloadDependencies(abname);
    }
    //---------------------------------------------------------------------------------------------
    void UnloadAssetBundleInternal(string assetBundleName)
    {
        LoadedAssetBundle bundle;
        if (loadedAssetBundleList.TryGetValue(assetBundleName, out bundle))
        {
            if (--bundle.referencedCount == 0)
            {
                bundle.assetBundle.Unload(false);
                loadedAssetBundleList.Remove(assetBundleName);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void CreateBattleObjPool()
    {

    }
    //---------------------------------------------------------------------------------------------
    public void DespawnAllBattleObj()
    {
        //TODO: seprate pool
        objectPool.DespawnAll();
        Destroy(objectPool);
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 载入依赖
    /// </summary>
    /// <param name="name"></param>
    void LoadDependencies(string name, bool async = false)
    {
        if (manifest == null)
        {
            Logger.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            return;
        }
        // Get dependecies from the AssetBundleManifest object..
        string[] dependencies = manifest.GetAllDependencies(name);
        if (dependencies.Length == 0) return;

        for (int i = 0; i < dependencies.Length; i++){
            dependencies[i] = RemapVariantName(dependencies[i]);}

        dependenceList.Add(name, dependencies);
        // Record and load all dependencies.
        if (async == false)
        {
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundle(dependencies[i]);
            }
        }
        else
        {
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundleAsync(dependencies[i]);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void UnloadDependencies(string assetBundleName)
    {
        string[] dependencies = null;
        if (!dependenceList.TryGetValue(assetBundleName, out dependencies))
            return;

        // Loop dependencies.
        foreach (var dependency in dependencies)
        {
            UnloadAssetBundleInternal(dependency);
        }

        dependenceList.Remove(assetBundleName);
    }
    //---------------------------------------------------------------------------------------------
    // Remaps the asset bundle name to the best fitting asset bundle variant.
    string RemapVariantName(string assetBundleName)
    {
        string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
        // If the asset bundle doesn't have variant, simply return.
        if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
            return assetBundleName;

        string[] split = assetBundleName.Split('.');

        int bestFit = int.MaxValue;
        int bestFitIndex = -1;
        // Loop all the assetBundles with variant to find the best fit variant assetBundle.
        for (int i = 0; i < bundlesWithVariant.Length; i++)
        {
            string[] curSplit = bundlesWithVariant[i].Split('.');
            if (curSplit[0] != split[0])
                continue;

            int found = System.Array.IndexOf(m_Variants, curSplit[1]);
            if (found != -1 && found < bestFit)
            {
                bestFit = found;
                bestFitIndex = i;
            }
        }
        if (bestFitIndex != -1)
            return bundlesWithVariant[bestFitIndex];
        else
            return assetBundleName;
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 销毁资源
    /// </summary>
    void OnDestroy()
    {
        if (manifest != null) manifest = null;

        //var itor = battlePoolList.GetEnumerator();
        //while (itor.MoveNext())
        //{
        //    Destroy(itor.Current.Value);
        //}
        battlePoolList.Clear();
        Destroy(gameObject);
		Logger.Log("~ResourceManager was destroy!");
    }
    //---------------------------------------------------------------------------------------------
}
