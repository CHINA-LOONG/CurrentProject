using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WpInfoGroup :MonoBehaviour
{
	static	WpInfoGroup instance = null;
	public	static WpInfoGroup Instance
	{
		get
		{
			return instance;
		}
	}

	private	static int MaxWpCount = 5;
	public List<MirrorFindWpInfo> listWpInfo = new List<MirrorFindWpInfo>();

	void Start()
	{
		instance = this;
	}

	public void InitWithParent(Transform parentTrans)
	{
		GameObject prefab = ResourceMgr.Instance.LoadAsset ("ui/findwpinfo", "MirrorFindWpInfo");

		for (int i = 0; i < MaxWpCount; ++i)
		{
			GameObject go = Instantiate (prefab) as GameObject;
			go.transform.SetParent (parentTrans,false);

			MirrorFindWpInfo subWpInfo = go.GetComponent<MirrorFindWpInfo>();
			subWpInfo.Clear();
			listWpInfo.Add(subWpInfo);
		}
	}

	public	void Clear()
	{
		foreach (MirrorFindWpInfo subWpInfo in listWpInfo) 
		{
			subWpInfo.Clear();
		}
	}
}
