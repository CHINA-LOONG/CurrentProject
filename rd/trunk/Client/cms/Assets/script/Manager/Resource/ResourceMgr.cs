using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResourceMgr : MonoBehaviour
{
    private string[] m_Variants = { };
    private AssetBundleManifest manifest;
    private AssetBundle assetbundle;
    private Dictionary<string, AssetBundle> bundles;

    static ResourceMgr mInst = null;
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

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        byte[] stream = null;
        string uri = Path.Combine(Util.AssetBundlePath, Const.AssetDirname);

        bundles = new Dictionary<string, AssetBundle>();

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

    /// <summary>
    /// 载入素材
    /// </summary>
    public GameObject LoadAsset(string abname, string assetname)
    {
        abname = abname.ToLower();
        AssetBundle bundle = LoadAssetBundle(abname);
        return bundle.LoadAsset<GameObject>(assetname);
    }

    public T LoadAssetType<T>(string abname, string assetname) where T:Object
    {
        abname = abname.ToLower();
        AssetBundle bundle = LoadAssetBundle(abname);
        return bundle.LoadAsset<T>(assetname);
    }

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
        AssetBundle bundle = null;
        if (!bundles.ContainsKey(abname))
        {
            byte[] stream = null;
            string uri = Path.Combine(Util.AssetBundlePath, abname);
            Debug.Log("LoadFile::>> " + uri);
            LoadDependencies(abname);

            stream = File.ReadAllBytes(uri);
            bundle = AssetBundle.CreateFromMemoryImmediate(stream); //关联数据的素材绑定
            bundles.Add(abname, bundle);
        }
        else
        {
            bundles.TryGetValue(abname, out bundle);
        }
        return bundle;
    }

    /// <summary>
    /// 载入依赖
    /// </summary>
    /// <param name="name"></param>
    void LoadDependencies(string name)
    {
        if (manifest == null)
        {
            Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            return;
        }
        // Get dependecies from the AssetBundleManifest object..
        string[] dependencies = manifest.GetAllDependencies(name);
        if (dependencies.Length == 0) return;

        for (int i = 0; i < dependencies.Length; i++)
            dependencies[i] = RemapVariantName(dependencies[i]);

        // Record and load all dependencies.
        for (int i = 0; i < dependencies.Length; i++)
        {
            LoadAssetBundle(dependencies[i]);
        }
    }

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

    /// <summary>
    /// 销毁资源
    /// </summary>
    void OnDestroy()
    {
        if (manifest != null) manifest = null;
        Debug.Log("~ResourceManager was destroy!");
    }
}
