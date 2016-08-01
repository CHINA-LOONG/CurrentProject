using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PathologicalGames;

//---------------------------------------------------------------------------------------------
// call backs
public delegate void AssetLoadedCallBack(GameObject instance, System.EventArgs args);

// asynchronous request
public class AssetRequest
{
    public bool isAdditive = false;//
    //public SceneLoadedCallBack sceneCallBack = null;
    public string assetName = null;

    public System.EventArgs args = null;
    public AssetLoadedCallBack assetCallBack = null;
    public bool loaded = false;

    public AssetRequest(string name)
    {
        this.assetName = name;
        loaded = false;
    }

    public AssetRequest(string name, AssetLoadedCallBack callBack, System.EventArgs args = null, bool additive = false)
    {
        this.assetName = name;
        this.assetCallBack = callBack;
        this.args = args;
        this.isAdditive = additive;
        loaded = false;
    }
}
//---------------------------------------------------------------------------------------------
public class AbRequestData
{
   public AssetBundleCreateRequest abRequest;
   public int referencedCount;
}
//---------------------------------------------------------------------------------------------
public class LoadedAssetBundle
{
    public AssetBundle assetBundle;
    public int referencedCount;
    public bool ignoreReferenceCount;

    public LoadedAssetBundle(AssetBundle assetBundle)
    {
        this.assetBundle = assetBundle;
        referencedCount = 1;
        ignoreReferenceCount = false;
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
    private Dictionary<string, AbRequestData> abRequestList = new Dictionary<string, AbRequestData>();

    private List<AssetBundleLoadOperation> inProgressLoadList = new List<AssetBundleLoadOperation>();
    static ResourceMgr mInst = null;
    //public SpawnPool objectPool = null;
    //pooled prefab
    private Dictionary<string, GameObject> battlePoolList = new Dictionary<string,GameObject>();
    public Dictionary<string, AssetRequest> assetRequestList = new Dictionary<string, AssetRequest>();

    void Awake()
    {
    }

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
            AssetBundleCreateRequest abRequest = keyValue.Value.abRequest;
            if (abRequest.isDone)
            {
                LoadedAssetBundle loadedBundle = new LoadedAssetBundle(abRequest.assetBundle);
                loadedBundle.referencedCount = keyValue.Value.referencedCount;
                loadedAssetBundleList.Add(keyValue.Key, loadedBundle);

                abRequestToRemoveList.Add(keyValue.Key);
            }
        }

        // Remove the finished WWWs.
        foreach (var key in abRequestToRemoveList)
        {
            abRequestList.Remove(key);
        }

        abRequestToRemoveList.Clear();

        for (int i = 0; i < inProgressLoadList.Count; )
        {
            if (!inProgressLoadList[i].Update())
            {
                inProgressLoadList.RemoveAt(i);
            }
            else
                i++;
        }

        UpdateAssetRequest();
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        if (assetbundle != null)
        {
            Logger.LogError("warning, resource manager is inited already, destroy may failed last time!");
            return;
        }

        byte[] stream = null;
        string uri = Path.Combine(Util.AssetBundlePath, Const.AssetDirname);
        //objectPool = PoolManager.Pools.Create("ObjectPool");

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
    }
    public void LoadLevelAsyn(string levelName, bool isAdditive, AssetLoadedCallBack callBack = null, System.EventArgs args = null)
    {
        levelName = StaticDataMgr.Instance.GetRealName(levelName);
        StartCoroutine(LoadLevelRequest(new AssetRequest(levelName, callBack, args, isAdditive)));
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator LoadLevelRequest(AssetRequest request)
    {
        string abname = StaticDataMgr.Instance.GetBundleName(request.assetName);
        if (abname == null)
            yield return null;

        abname = abname.ToLower();
        LoadAssetBundleAsync(abname);
        AssetLevelLoadOperation loadLevelOperation = new AssetLevelLoadOperation(
            abname,
            request.assetName,
            typeof(GameObject),
            request.isAdditive
            );

        if (loadLevelOperation == null)
            yield break;

        inProgressLoadList.Add(loadLevelOperation);
        yield return StartCoroutine(loadLevelOperation);

        if (request.assetCallBack != null)
        {
            request.assetCallBack(null, request.args);
        }

        UnloadAssetBundle(abname);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    //---------------------------------------------------------------------------------------------
    public void UnloadCachedBundles(bool clearPoolList = false)
    {
        var itor = battlePoolList.GetEnumerator();
        string abname;
        while (itor.MoveNext())
        {
            abname = StaticDataMgr.Instance.GetBundleName(itor.Current.Key);
            if (string.IsNullOrEmpty(abname) == false)
                UnloadAssetBundle(abname);
        }
        if (clearPoolList == true)
        {
            battlePoolList.Clear();
            if (assetRequestList.Count > 0)
            {
                Logger.LogError("loading not finish!");
            }
            Resources.UnloadUnusedAssets();
        }

        System.GC.Collect();
    }
    //---------------------------------------------------------------------------------------------
    public GameObject LoadUI(string assetname)
    {
        assetname = StaticDataMgr.Instance.GetRealName(assetname);
        GameObject obj = GetPoolObject(assetname);
        if (obj != null)
        {
            return obj;
        }
        string abname = StaticDataMgr.Instance.GetBundleName(assetname);
        if (string.IsNullOrEmpty(abname))
        {
            //Logger.LogErrorFormat("Load asset   {0}  faild", assetname);
            return null;
        }
        abname = abname.ToLower();
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

        GameObject go = null;
        {
            CreatePoolObject(prefab);
            go = GetPoolObject(prefab.name);
        }

        UnloadAssetBundle(abname);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        return go;
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 载入素材
    /// </summary>
    /// TODO: remove cache
    public GameObject LoadAsset(string assetname, bool cache = false)
    {
        //Logger.LogWarningFormat("[#################]LoadAsset assetname={0}[##################]", assetname);
        assetname = StaticDataMgr.Instance.GetRealName(assetname);

        GameObject obj = GetPoolObject(assetname);
        if (obj != null)
        {
            return obj;
        }
		string abname = StaticDataMgr.Instance.GetBundleName (assetname);
		if (string.IsNullOrEmpty (abname))
		{
			//Logger.LogErrorFormat("Load asset   {0}  faild", assetname);
			return null;
		}
		abname = abname.ToLower();
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

        //UnloadAssetBundle(abname);
        //Resources.UnloadUnusedAssets();
        //System.GC.Collect();

        return go;
    }
    //---------------------------------------------------------------------------------------------
    public void AddAssetRequest(AssetRequest request)
    {
        string realName = StaticDataMgr.Instance.GetRealName(request.assetName);

        if (assetRequestList.ContainsKey(realName))
            return;

        if (battlePoolList.ContainsKey(realName))
            return;

        assetRequestList.Add(realName, request);
    }
    //---------------------------------------------------------------------------------------------
    public int GetAssetRequestCount()
    {
        return assetRequestList.Count;
    }
    //---------------------------------------------------------------------------------------------
    public void UpdateAssetRequest()
    {
        LoadAssetBatch(ref assetRequestList);
    }
    //---------------------------------------------------------------------------------------------
    private void LoadAssetBatch(ref Dictionary<string, AssetRequest> requestList)
    {
        var itr = requestList.GetEnumerator();
        while (itr.MoveNext())
        {
            LoadAssetAsyn(itr.Current.Value);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void LoadAssetAsyn(AssetRequest request)
    {
        if (request.loaded == true)
            return;

        request.loaded = true;
        string realName = StaticDataMgr.Instance.GetRealName(request.assetName);
        GameObject obj;
        if ((obj = GetPoolObject(realName)) != null)
        {
            if (request.assetCallBack != null)
            {
                request.assetCallBack(obj, request.args);
            }
            return;
        }

        StartCoroutine(LoadAssetRequest(request));
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator LoadAssetRequest(AssetRequest request)
    {
        string realName = StaticDataMgr.Instance.GetRealName(request.assetName);
        string abname = StaticDataMgr.Instance.GetBundleName(realName).ToLower();
        LoadAssetBundleAsync(abname);
        AssetBundleLoadAssetOperationFull loadAssetOperate = new AssetBundleLoadAssetOperationFull(
            abname,
            realName,
            typeof(GameObject)
            );

        if (loadAssetOperate == null)
            yield break;

        inProgressLoadList.Add(loadAssetOperate);
        yield return StartCoroutine(loadAssetOperate);

        GameObject prefab = loadAssetOperate.GetAsset<GameObject>();
        CreatePoolObject(prefab);
        if (request.assetCallBack != null)
        {
            request.assetCallBack(Instantiate(prefab), request.args);
        }

        if (assetRequestList.ContainsKey(realName))
        {
            assetRequestList.Remove(realName);
        }
        //UnloadAssetBundle(abname);
        //Resources.UnloadUnusedAssets();
        //System.GC.Collect();
    }
    //---------------------------------------------------------------------------------------------
    public T LoadAssetType<T>(string assetname) where T:Object
    {
        //assetname = StaticDataMgr.Instance.GetRealName(assetname);
        string abname = StaticDataMgr.Instance.GetBundleName (assetname);
		if (string.IsNullOrEmpty (abname))
		{
			Logger.LogError("Load assetType error: no asset : " + assetname);
			return null;
		}
        abname = abname.ToLower();
        AssetBundle bundle = LoadAssetBundle(abname, true);

        //T obj = bundle.LoadAsset<T>(assetname);

        return bundle.LoadAsset<T>(assetname);
        ////TODO: pool object too
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

        AbRequestData abRequestData = null;
        if (abRequestList.TryGetValue(abname, out abRequestData) == false)
        {
            byte[] stream = null;
            string uri = Path.Combine(Util.AssetBundlePath, abname);
            Logger.Log("LoadFile::>> " + uri);
            LoadDependencies(abname, true);
            stream = File.ReadAllBytes(uri);
            AssetBundleCreateRequest abRequest = AssetBundle.CreateFromMemory(stream);

            abRequestData = new AbRequestData();
            abRequestData.abRequest = abRequest;
            abRequestData.referencedCount = 1;
            abRequestList.Add(abname, abRequestData);
        }
        else 
        {
            abRequestData.referencedCount++;
        }

        return false;
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 载入AssetBundle
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>
    AssetBundle LoadAssetBundle(string abname, bool ignoreReference = false)
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
            loadedBundle.ignoreReferenceCount = ignoreReference;
            loadedAssetBundleList.Add(abname, loadedBundle);
        }
        else
        {
            loadedAssetBundleList.TryGetValue(abname, out loadedBundle);
            if (loadedBundle.ignoreReferenceCount == false)
            {
                ++loadedBundle.referencedCount;
            }
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
        //objectPool.DespawnAll();
        //Destroy(objectPool);
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
