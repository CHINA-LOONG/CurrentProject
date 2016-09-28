using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MirrorFindMonsterInfo : MonoBehaviour 
{
	public enum TipInfoType
	{
		NoTip = 0,
		TipSelf,
		TipWeakPoint
	}

	public	Text	propertyText;
	public  Image 	propertyImage;
	public	Text	friendShipText;
	public	Text	likeFoodText;

	// Use this for initialization
	public void Init()
	{
		BindListener ();
	}

	void OnDestroy()
	{
		UnBindListener ();
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<List<MirrorTarget>>(GameEventList.ShowFindMonsterInfo, OnShowFindMonsterInfo);
		GameEventMgr.Instance.AddListener (GameEventList.HideFindMonsterInfo, OnHideFindMonsterInfo);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<List<MirrorTarget>>(GameEventList.ShowFindMonsterInfo, OnShowFindMonsterInfo);
		GameEventMgr.Instance.RemoveListener (GameEventList.HideFindMonsterInfo, OnHideFindMonsterInfo);
	}

	void	OnShowFindMonsterInfo(List<MirrorTarget> listTarget)
	{
        //todo:弱点信息outtime
		WpInfoGroup.Instance.Clear ();
		List<MirrorFindWpInfo> listWpInfo = WpInfoGroup.Instance.listWpInfo;
		int findIndex = 0;

		foreach (MirrorTarget subTarget in listTarget)
		{
			BattleObject bo = subTarget.battleObject;
			if(null == bo)
				continue;

			WeakPointRuntimeData wpRuntimeData = null;
			bool isok = bo.wpGroup.allWpDic.TryGetValue(subTarget.WeakPointIDAttr,out wpRuntimeData);
			if(!isok)
				continue;
			
		}

	}

	void OnHideFindMonsterInfo()
	{
		propertyText.text = "";
		friendShipText.text = "";
		friendShipText.gameObject.SetActive (false);
		likeFoodText.text = "";
		propertyImage.enabled = false;
		propertyText.gameObject.SetActive (false);

		WpInfoGroup.Instance.Clear ();
	}
}
