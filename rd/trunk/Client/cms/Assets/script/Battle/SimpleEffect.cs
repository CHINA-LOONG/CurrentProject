using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EffectList
{
	public static	string 	dazhaoPreprare = "dazhaoPrepare";
	public static	string	dazhaoReady = "dazhaoReady";
}

public	class EffectMap
{
	public	class EffectData
	{
		public string assetName;
		public string prefabName;
		
		public EffectData(string assetName, string prefabName)
		{
			this.assetName = assetName;
			this.prefabName = prefabName;
		}
	}

	public Dictionary<string,EffectData> mapData = new Dictionary<string, EffectData>();

	private static EffectMap instance = null;
	public	static	EffectMap Instance
	{
		get
		{
			if(null == instance)
			{
				instance = new EffectMap();
				instance.Init();
			}
			return instance;
		}
	}
	public	void Init()
	{
		mapData [EffectList.dazhaoPreprare] = new EffectData ("effect/particle","dazhao_prepare");
		mapData [EffectList.dazhaoReady] = new EffectData ("effect/particle","dazhao_ready");
	}
}



public class SimpleEffect : MonoBehaviour 
{
	// Use this for initialization
	void Start ()
	{
	
	}

	public void ShowEffectWithKey (string effectKey)
	{
		EffectMap.EffectData effectData = null;
		if (EffectMap.Instance.mapData.TryGetValue (effectKey, out effectData))
		{
			ShowEffect(effectData.assetName,effectData.prefabName);
		}
	}

	public void ShowEffect(string assetName, string prefabName)
	{
		DeleteEffect (prefabName);
		GameObject prefab = ResourceMgr.Instance.LoadAsset(assetName,prefabName) as GameObject;
		
		if(prefab != null)
		{
			GameObject go = Instantiate (prefab) as GameObject;
			go.name = prefabName;
			go.transform.SetParent(transform);
			go.transform.localPosition = Vector3.zero;
		}
	}

	public void HideEffectWithKey(string effectKey)
	{
		EffectMap.EffectData effectData = null;
		if (EffectMap.Instance.mapData.TryGetValue (effectKey, out effectData))
		{
			HideEffect(effectData.prefabName);
		}
	}

	public void HideEffect(string prefabName)
	{
		DeleteEffect (prefabName);
	}

	private void DeleteEffect(string prefabName)
	{
		GameObject oldEffect = Util.FindChildByName (gameObject, prefabName);
		if(null != oldEffect)
		{
			Destroy(oldEffect);
		}
	}

}
