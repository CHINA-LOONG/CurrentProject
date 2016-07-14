using UnityEngine;
using System.Collections;

public abstract class AssetBundleLoadOperation : IEnumerator
{
	public object Current
	{
		get
		{
			return null;
		}
	}
	public bool MoveNext()
	{
		return !IsDone();
	}
	
	public void Reset()
	{
	}
	
	abstract public bool Update ();
	
	abstract public bool IsDone ();
}

public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
{
	public abstract T GetAsset<T>() where T : UnityEngine.Object;
}

public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
{
	protected string 				mAssetBundleName;
	protected string 				mAssetName;
	protected System.Type 			mType;
	protected AssetBundleRequest	mRequest = null;

	public AssetBundleLoadAssetOperationFull (string bundleName, string assetName, System.Type type)
	{
		mAssetBundleName = bundleName;
		mAssetName = assetName;
		mType = type;
	}
	
	public override T GetAsset<T>()
	{
		if (mRequest != null && mRequest.isDone)
			return mRequest.asset as T;
		else
			return null;
	}
	
	// Returns true if more Update calls are required.
	public override bool Update ()
	{
		if (mRequest != null)
			return false;

        LoadedAssetBundle bundle = ResourceMgr.Instance.GetLoadedAssetBundle(mAssetBundleName);
		if (bundle != null)
		{
			mRequest = bundle.assetBundle.LoadAssetAsync (mAssetName, mType);
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public override bool IsDone ()
	{
		return mRequest != null && mRequest.isDone;
	}
}

public class AssetLevelLoadOperation : AssetBundleLoadAssetOperation
{
    protected string mAssetBundleName;
    protected string mAssetName;
    protected bool mIsAdditive;
    protected System.Type mType;
    protected AsyncOperation mRequest = null;

    public override T GetAsset<T>()
    {
        return null;
    }
    public AssetLevelLoadOperation(string bundleName, string assetName, System.Type type, bool isAdditive)
    {
        mAssetBundleName = bundleName;
        mAssetName = assetName;
        mType = type;
        mIsAdditive = isAdditive;
    }

    // Returns true if more Update calls are required.
    public override bool Update()
    {
        if (mRequest != null)
            return false;

        LoadedAssetBundle bundle = ResourceMgr.Instance.GetLoadedAssetBundle(mAssetBundleName);
        if (bundle != null)
        {
            if (mIsAdditive)
                mRequest = Application.LoadLevelAdditiveAsync(mAssetName);
            else
                mRequest = Application.LoadLevelAsync(mAssetName);
            return false;
        }
        else
            return true;
    }

    public override bool IsDone()
    {
        return mRequest != null && mRequest.isDone;
    }
}
//public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
//{
//    public AssetBundleLoadManifestOperation (string bundleName, string assetName, System.Type type)
//        : base(bundleName, assetName, type)
//    {
//    }

//    public override bool Update ()
//    {
//        base.Update();
		
//        if (m_Request != null && m_Request.isDone)
//        {
//            AssetBundleManager.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();
//            return false;
//        }
//        else
//            return true;
//    }
//}

