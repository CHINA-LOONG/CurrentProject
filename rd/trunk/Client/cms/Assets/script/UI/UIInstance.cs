using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIInstance : UIBase,TabButtonDelegate
{
	public static string ViewName = "UIInstance";

	public Text		chapterText;
	public	Button	closeButton;
	public	Button	leftButton;
	public	Button	rightButton;
	public	Transform	chapterTranform;
	public	Transform	infoLayer;//副本信息

	//[HideInInspector]
	public	InstanceInfo	instanceInfo;
    //[HideInInspector]
    public InstanceChapter insChapter;
	//[HideInInspector]
	public int difficulty;


    private TabButtonGroup tabGroup;

	// Use this for initialization
	void Start () 
	{
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseInstance;
		EventTriggerListener.Get (leftButton.gameObject).onClick = OnLeftButtonClick;
		EventTriggerListener.Get (rightButton.gameObject).onClick = OnRightButtonClcked;
	}

    public override void Init()
    {
        if (insChapter != null)
        {
            ResourceMgr.Instance.DestroyAsset(insChapter.gameObject);
        }
        if (tabGroup == null)
        {
            tabGroup = GetComponentInChildren<TabButtonGroup>();
            tabGroup.InitWithDelegate(this);
        }
		InitInstanceInfo ();
		RefreshInstance ();
    }
    public override void Clean()
    {
        if (instanceInfo!=null)
        {
            ResourceMgr.Instance.DestroyAsset(instanceInfo.gameObject);
        }
        if (insChapter!=null)
        {
            ResourceMgr.Instance.DestroyAsset(insChapter.gameObject);
        }
    }


	void InitInstanceInfo()
	{
        if (instanceInfo == null)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("InstanceInfo");
            go.transform.SetParent(infoLayer, false);
            instanceInfo = go.GetComponent<InstanceInfo>();
        }
        instanceInfo.SetShow(false);

        //test only(重置副本)
        PB.HSInstanceResetCount resetCount = new PB.HSInstanceResetCount();
        resetCount.instanceId = "demo";
        GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_RESET_COUNT_C.GetHashCode(), resetCount);
	}


	void RefreshInstance()
	{
		int difficulty = 0;
		int chapter = 1;

		List<InstanceEntryRuntimeData> listEntry = InstanceMapService.Instance.GetRuntimeInstance (InstanceDifficulty.Normal, chapter);
		
		string name = string.Format ("instanceChapter_{0}_{1}", difficulty, chapter);
		
		GameObject go = ResourceMgr.Instance.LoadAsset (name);
		
		go.transform.SetParent (chapterTranform, false);

		insChapter = go.GetComponent<InstanceChapter> ();

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
			subButton.SetName(subRuntimeButton.staticData.NameAttr);
			subButton.instanceId = subRuntimeButton.instanceId;
		}
	}

	void OnCloseInstance(GameObject go)
	{
		UIMgr.Instance.CloseUI_(this);
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
