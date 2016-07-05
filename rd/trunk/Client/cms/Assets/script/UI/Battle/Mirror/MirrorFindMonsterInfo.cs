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
	void Start ()
	{

	}

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
		WpInfoGroup.Instance.Clear ();
		List<MirrorFindWpInfo> listWpInfo = WpInfoGroup.Instance.listWpInfo;
		int findIndex = 0;

		foreach (MirrorTarget subTarget in listTarget)
		{
			WeakPointData wpData = StaticDataMgr.Instance.GetWeakPointData(subTarget.WeakPointIDAttr);
			if(null == wpData || wpData.tipType == (int)TipInfoType.NoTip)
				continue;

			if(subTarget.isSelf)
			{
				if(wpData.tipType == (int)TipInfoType.TipSelf)
				{
					GameUnit unit = WeakPointController.Instance.getGameUnit(subTarget);
					propertyText.text = subTarget.WeakPointIDAttr;
					friendShipText.text = unit.friendship.ToString();
					friendShipText.gameObject.SetActive(true);
					propertyText.gameObject.SetActive(true);
					//likeFoodText.text = "subTarget";

					int property = WeakPointController.Instance.GetProperty(subTarget);
					var image = ResourceMgr.Instance.LoadAssetType<Sprite>("ui/property", "property_" + property) as Sprite;
					if(image != null)
					{
						propertyImage.enabled = true;
						propertyImage.sprite = image;
					}

				}
			}
			else
			{
				if(wpData.tipType == (int)TipInfoType.TipWeakPoint)
				{
					if(findIndex < listWpInfo.Count)
					{
						listWpInfo[findIndex].Show(subTarget, wpData.tipOffsetX, wpData.tipOffsetY);
						findIndex ++ ;
					}
				}
			}
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
