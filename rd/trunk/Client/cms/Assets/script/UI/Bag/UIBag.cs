using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public	enum BagType:int
{
	BaoXiang = 0,
	Xiaohao,
	Baoshi,
	Cailiao,
	Count,
	Unknow
}

public	enum BagState
{
	Norml = 0,
	Sell
}

public class UIBag : UIBase,TabButtonDelegate
{
    public static string ViewName = "UIBag";
	public	static	UIBag Instance = null;

    public Button m_closeButton;
	public	TabButtonGroup	tabBtnGroup;
	public	ScrollView	scrollView;
	public	Button	cancelSellButton;
	public	Button	conformSellButton;
	public	Button	sellButton;

	private BagType curBagType = BagType.BaoXiang;

	private BagState curBagState = BagState.Norml;
	public BagState  BagStateAttr 
	{
		get{ return curBagState;}
		set
		{
			if(value != curBagState)
			{
				curBagState = value;
				RefreshBag();
			}
		}
	}

	private	Dictionary<string,int>  sellItemsDic = new Dictionary<string, int>();

    void Start()
    {
		Instance = this;
        
    }
	bool isFirst = true;
    public override void Init()
    {
		if (isFirst)
		{
			isFirst = true;
			FirsInit();
		}
		curBagState = BagState.Norml;
		RefreshBag ();
    }

	void FirsInit()
	{
		EventTriggerListener.Get(m_closeButton.gameObject).onClick = CloseBagButtonDown;
		EventTriggerListener.Get (cancelSellButton.gameObject).onClick = OnSellCancleClick;
		EventTriggerListener.Get (conformSellButton.gameObject).onClick = OnSellConformClick;
		EventTriggerListener.Get (sellButton.gameObject).onClick = OnSellClick;
		tabBtnGroup.InitWithDelegate (this);
	}
    public override void Clean()
    {
    }

    void CloseBagButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

	public void OnTabButtonChanged (int index)
	{
		if ((int)curBagType == index)
			return;

		curBagType = (BagType)index;
		RefreshBag ();
	}

	private void RefreshBag()
	{
		cancelSellButton.gameObject.SetActive (curBagState == BagState.Sell);
		conformSellButton.gameObject.SetActive (curBagState == BagState.Sell);
		sellButton.gameObject.SetActive (curBagState == BagState.Norml);

		scrollView.ClearAllElement ();

		List<ItemData> items = BagDataMgr.Instance.GetBagItemsWithType (curBagType);
		ItemData subItemData;
		for (int i =0; i< items.Count; ++i)
		{
			subItemData = items[i];
 			BagItem subBagItme = BagItem.Create(subItemData,curBagType,curBagState);
		
			scrollView.AddElement(subBagItme.gameObject);
		}
	}

	void OnSellCancleClick(GameObject go)
	{
		BagStateAttr = BagState.Norml;
		sellItemsDic.Clear ();
	}

	void OnSellConformClick(GameObject go)
	{
		if (sellItemsDic.Count < 1) 
		{
			//todo:msg
			Logger.LogError("未选择出售物品....");
			UIIm.Instance.ShowSystemHints("未选择出售物品....",(int)PB.ImType.PROMPT);
			return;
		}
	}

	void OnSellClick(GameObject go)
	{
		BagStateAttr = BagState.Sell;
		sellItemsDic.Clear ();
	}

	public	void UpdateSellItems(string itemId, int itemCount)
	{
		if (itemCount == 0 )
		{
			if(sellItemsDic.ContainsKey (itemId))
			{
				sellItemsDic.Remove(itemId);
			}
			return;
		}
		if (sellItemsDic.ContainsKey (itemId))
		{
			sellItemsDic[itemId] = itemCount;
		}
		else 
		{
			sellItemsDic.Add(itemId,itemCount);
		}
	}
}

