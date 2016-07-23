using UnityEngine;
using System.IO;
#if UNITY_IOS || UNITY_ANDROID
//using Helpshift;
#endif
using System.Collections.Generic;
using HSMiniJSON;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
[System.Serializable]
public class FunSdkConfig : ScriptableObject
{
	private static FunSdkConfig instance;
	private const string funSdkConfigAssetName = "FunplusSdkConfig";
	private const string funSdkConfigPath = "FunSdk/Resources";

	[SerializeField]
	private bool mEnableFb;
	[SerializeField]
	private bool mEnableVk;
	[SerializeField]
	private bool mEnableWx;
	[SerializeField]
	private string mGameId;
	[SerializeField]
	private string mGameKey;
	[SerializeField]
	private string mFbAppId;
	[SerializeField]
	private string mFbAppDisName;
	[SerializeField]
	private string mVkAppId;
	[SerializeField]
	private string mWxAppId;
	[SerializeField]
	private string mWxAppKey;
	[SerializeField]
	private string mWxMsdkUrl;
	[SerializeField]
	private string mWxMsdkOfferId;
	[SerializeField]
	private bool iTunesfilesharing ;
	
	public static FunSdkConfig Instance
	{
		get
		{
			instance = Resources.Load(funSdkConfigAssetName) as FunSdkConfig;
			if (instance == null) {
				instance = CreateInstance<FunSdkConfig>();
				#if UNITY_EDITOR
				string properPath = Path.Combine(Application.dataPath, funSdkConfigPath);
				if (!Directory.Exists(properPath))
				{
					AssetDatabase.CreateFolder("Assets/FunSdk", "Resources");
				}
				
				string fullPath = Path.Combine(Path.Combine("Assets", funSdkConfigPath),
				                               funSdkConfigAssetName + ".asset"
				                               );
				AssetDatabase.CreateAsset(instance, fullPath);
				#endif
			}
			return instance;
		}
	}
	
	#if UNITY_EDITOR
	[MenuItem("FunSdk/Edit Config")]
	public static void Edit()
	{
		Selection.activeObject = Instance;
	}
	    
	[MenuItem("FunSdk/Developers Page")]
	public static void OpenAppPage()
	{
		string url = "http://developer.funplus.com";
		Application.OpenURL(url);
	}

	#endif


	public bool MEnableFb
	{
		get { return mEnableFb; }
		set
		{
			if (mEnableFb != value)
			{
				mEnableFb = value;
				DirtyEditor();
			}
		}
	}

	public bool MEnableVk
	{
		get { return mEnableVk; }
		set
		{
			if (mEnableVk != value)
			{
				mEnableVk = value;
				DirtyEditor();
			}
		}
	}

	public bool MEnableWx
	{
		get { return mEnableWx; }
		set
		{
			if (mEnableWx != value)
			{
				mEnableWx = value;
				DirtyEditor();
			}
		}
	}

	public string MGameId
	{
		get { return mGameId; }
		set
		{
			if (mGameId != value)
			{
				mGameId = value;
				DirtyEditor();
			}
		}
	}

	public string MGameKey
	{
		get { return mGameKey; }
		set
		{
			if (mGameKey != value)
			{
				mGameKey = value;
				DirtyEditor();
			}
		}
	}
	
	public string MFbAppId
	{
		get { return mFbAppId; }
		set
		{
			if (mFbAppId != value)
			{
				mFbAppId = value;
				DirtyEditor();
			}
		}
	}

	public string MFbAppDisName
	{
		get { return mFbAppDisName; }
		set
		{
			if (mFbAppDisName != value)
			{
				mFbAppDisName = value;
				DirtyEditor();
			}
		}
	}

	public string MVkAppId
	{
		get { return mVkAppId; }
		set
		{
			if (mVkAppId != value)
			{
				mVkAppId = value;
				DirtyEditor();
			}
		}
	}

	public string MWxAppId
	{
		get { return mWxAppId; }
		set
		{
			if (mWxAppId != value)
			{
				mWxAppId = value;
				DirtyEditor();
			}
		}
	}

	public string MWxAppKey
	{
		get { return mWxAppKey; }
		set
		{
			if (mWxAppKey != value)
			{
				mWxAppKey = value;
				DirtyEditor();
			}
		}
	}

	public string MWxMsdkUrl
	{
		get { return mWxMsdkUrl; }
		set
		{
			if (mWxMsdkUrl != value)
			{
				mWxMsdkUrl = value;
				DirtyEditor();
			}
		}
	}

	public string MWxMsdkOfferId
	{
		get { return mWxMsdkOfferId; }
		set
		{
			if (mWxMsdkOfferId != value)
			{
				mWxMsdkOfferId = value;
				DirtyEditor();
			}
		}
	}

	public bool ITunesfilesharing
	{
		get { return iTunesfilesharing;}
		set
		{
			if(iTunesfilesharing != value)
			{
				iTunesfilesharing = value;
				DirtyEditor();
			}
		}
	}
	
	public Dictionary<string, string> ApiConfig
	{
		get { return instance.getApiConfig(); }
	}
	
	private static void DirtyEditor()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty(Instance);
		instance.SaveConfig();
		#endif
	}
	
	public void SaveConfig () {
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets();
		string apiJson = Json.Serialize(instance.ApiConfig);
		string iosPath = Path.Combine(Application.dataPath, "FunSdk/Plugins/iOS/");
		if (Directory.Exists(iosPath)) {
			System.IO.File.WriteAllText (iosPath + "FunSdk.json", apiJson);
		}
		#endif
	}
	
	public Dictionary<string, string> getApiConfig () {
		Dictionary<string, string> configDictionary = new Dictionary<string, string>();

		configDictionary.Add("gameid", instance.mGameId);
		configDictionary.Add("gamekey", instance.mGameKey);
		configDictionary.Add("enablefb", instance.mEnableFb.ToString());
		configDictionary.Add("enablevk", instance.mEnableVk.ToString());
		configDictionary.Add("enablewx", instance.mEnableWx.ToString());

		configDictionary.Add("fbappid", instance.mFbAppId);
		configDictionary.Add("fbappdisname", instance.mFbAppDisName);
		configDictionary.Add("vkappid", instance.mVkAppId);
		configDictionary.Add("wxappid", instance.mWxAppId);
		configDictionary.Add("wxappkey", instance.mWxAppKey);
		configDictionary.Add("wxmsdkurl", instance.mWxMsdkUrl);
		configDictionary.Add("wxmsdkofferid", instance.mWxMsdkOfferId);

		if (instance.iTunesfilesharing)
		{
			configDictionary.Add("iTunesfilesharing", "YES");
		}
		else 
		{
			configDictionary.Add("iTunesfilesharing", "NO");
		}


		return configDictionary;
	}
}
