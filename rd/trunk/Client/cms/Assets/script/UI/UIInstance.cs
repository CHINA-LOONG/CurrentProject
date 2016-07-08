using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIInstance : UIBase,TabButtonDelegate
{
	public static string ViewName = "UIInstance";
	public static string AssertName = "ui/instance";
	private	static	UIInstance instance;
	public	static	UIInstance Instance
	{
		get
		{
			return instance;
		}
	}

	public Text		chapterText;
	public	Button	closeButton;
	public	Button	leftButton;
	public	Button	rightButton;
	public	Transform	chapterTranform;
	public	Transform	infoLayer;//副本信息

	[HideInInspector]
	public	InstanceInfo	instanceInfo;

	[HideInInspector]
	public int difficulty;

	// Use this for initialization
	IEnumerator Start () 
	{
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseInstance;
		EventTriggerListener.Get (leftButton.gameObject).onClick = OnLeftButtonClick;
		EventTriggerListener.Get (rightButton.gameObject).onClick = OnRightButtonClcked;
		yield return new WaitForEndOfFrame ();
		InitInstanceInfo ();
		yield return new WaitForEndOfFrame ();
		GetComponent<TabButtonGroup> ().InitWithDelegate (this);
		RefreshInstance ();

		instance = this;
	}

	void InitInstanceInfo()
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("ui/instanceinfo", "InstanceInfo");
		go.transform.SetParent (infoLayer, false);
		instanceInfo = go.GetComponent<InstanceInfo> ();
		instanceInfo.SetShow (false);
	}


	void RefreshInstance()
	{
		int difficulty = 0;
		int chapter = 1;

		List<InstanceEntryRuntimeData> listEntry = InstanceMapService.Instance.GetRuntimeInstance (InstanceDifficulty.Normal, chapter);
		
		string name = string.Format ("instanceChapter_{0}_{1}", difficulty, chapter);
		
		GameObject go = ResourceMgr.Instance.LoadAsset ("ui/instancechapter", name);
		
		go.transform.SetParent (chapterTranform, false);

		InstanceChapter insChapter = go.GetComponent<InstanceChapter> ();

		List<InstanceButton> listInsButton = insChapter.instanceButtonList;

		if (listEntry.Count != listInsButton.Count)
		{
			return;
		}

		InstanceButton subButton = null;
		InstanceEntryRuntimeData subRuntimeButton = null;
		for (int i =0; i<listInsButton.Count; ++i) 
		{
			subButton = listInsButton[i];

			subRuntimeButton = listEntry[i];
			subButton.SetStar(subRuntimeButton.star);
			subButton.SetName(subRuntimeButton.staticData.name);
			subButton.instanceId = subRuntimeButton.instanceId;
		}
	}

	void OnCloseInstance(GameObject go)
	{
		UIMgr.Instance.CloseUI (this);
	}

	void OnLeftButtonClick(GameObject go)
	{
	}

	void OnRightButtonClcked(GameObject go)
	{
	}

	public	void OnTabButtonChanged (int index)
	{

	}
	
}
