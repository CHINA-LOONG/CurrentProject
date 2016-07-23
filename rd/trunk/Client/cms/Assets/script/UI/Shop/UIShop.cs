using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIShop : UIBase
{
	public static string ViewName = "UIShop";

	public	CoinButton	jinbiCoinBtn;
	public	CoinButton	zuanshiCoinBtn;
	public	CoinButton	shopCoinBtn;

	public	ScrollView	shopItemsScrollView;
	public	Text		shopName;
	public	Text		nextRefreshText;

	public	Button	refreshButton;
	public	Button	closeButton;
	public	Button	leftButton;
	public	Button	rightButton;

	private bool	isInit  =false;
	List<ShopItem> listShopItem = new List<ShopItem>();
	ShopDataMgr	shopDataMgr = null;
	int	curShopType = -1;	

	TimeStaticData	timeNextRefresh = null;

	// Use this for initialization
	void Start ()
	{
	
	}
		
	#region ------------ override method---------------

	public override void Init()
	{
		if (!isInit) 
		{
			FistInit();
		}
	}
	
	public override void Clean()
	{
		for (int i = 0; i<listShopItem.Count; ++i) 
		{
			ResourceMgr.Instance.DestroyAsset(listShopItem[i].gameObject);
		}
		listShopItem.Clear ();
		UnBindListener ();
	}

	#endregion

	void FistInit()
	{
		if (listShopItem.Count < 1)
		{
			for(int i =0;i<ShopConst.shopItemsMax;++i)
			{
				ShopItem subItem = ShopItem.CreateShopItem();
				listShopItem.Add(subItem);
				shopItemsScrollView.AddElement(subItem.gameObject);
				subItem.gameObject.SetActive(false);
			}
		}
		shopDataMgr = GameDataMgr.Instance.ShopDataMgrAttr;
		
		EventTriggerListener.Get (refreshButton.gameObject).onClick = OnRefreshButtonClilck;
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseButtonClick ;
		EventTriggerListener.Get (leftButton.gameObject).onClick = OnLeftButtonClick ;
		EventTriggerListener.Get (rightButton.gameObject).onClick = OnRightButtonClick ;

		BindListener ();
		
		isInit = true;
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener (GameEventList.RefreshShopUi, OnRefreshShopUi);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener (GameEventList.RefreshShopUi, OnRefreshShopUi);
	}

	int count = 0;
	void Update()
	{
		count ++;
		if (count < 10)
			return;

		count = 0;
		TimeStaticData tNow = GameTimeMgr.Instance.GetTime ();
		if (tNow > timeNextRefresh)
		{
			RefreshShopData(curShopType);
		}
	}

	void OnRefreshButtonClilck(GameObject go)
	{
		int hasRefreshCount = 0;
		ShopDataMgr.ShopDescWithLevel desc = shopDataMgr.GetShopDesc (GameDataMgr.Instance.PlayerDataAttr.level, curShopType);
		if (hasRefreshCount == desc.maxRefreshTimes) 
		{
			//MsgBox.PromptMsg.Open("提示","今天刷新次数已经用完","确定");
			MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,"今天刷新次数已经用完");
			return;
		}

		if (GameDataMgr.Instance.PlayerDataAttr.gold < 5)
		{
			//MsgBox.PromptMsg.Open("提示","钻石不足","确定");
			MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,"钻石不足");
			return;
		}

		shopDataMgr.RefreshShopWithDiamond(curShopType);
	}

	void OnCloseButtonClick(GameObject go)
	{
		UIMgr.Instance.CloseUI_ (this);
	}

	void OnLeftButtonClick(GameObject go)
	{
		int preShopType = GetPreShopType ();
		curShopType = preShopType;
		//OnRefreshShopUi ();
		RefreshShopData (curShopType);
	}

	void OnRightButtonClick(GameObject go)
	{
		int nextShopType = GetNextShopType ();
		curShopType = nextShopType;
		RefreshShopData (curShopType);
	}

	public	void	RefreshShopData(int shopType,bool forceRefresh = false)
	{
		curShopType = shopType;
		if (shopDataMgr.IsNeedUpdateShopData ())
		{
			shopDataMgr.RequestShopData();
		}
		else 
		{
			OnRefreshShopUi();
		}
		timeNextRefresh = shopDataMgr.GetNextFreeRefreshTime (curShopType);
	}

	void	OnRefreshShopUi()
	{
		PB.ShopData shopData = shopDataMgr.GetShopData (curShopType);
		if (null == shopData)
		{
			Logger.LogError("Error for Get shopData....");
			return;
		}
		//shop info
		int gold = GameDataMgr.Instance.PlayerDataAttr.gold;//钻石
		long coin = GameDataMgr.Instance.PlayerDataAttr.coin;//金币
		jinbiCoinBtn.coinCount.text = gold.ToString ();
		zuanshiCoinBtn.coinCount.text = coin.ToString ();
		shopName.text = GetShopName (curShopType);

		TimeStaticData nextRefTime = shopDataMgr.GetNextFreeRefreshTime (curShopType);
		string day = "今天";
		if (nextRefTime.dayOfMonth > 1)
		{
			day = "明天";
		}
		string nextRefDes = string.Format ("下次更新时间:{0}:{1:00}:{2:00}",day,nextRefTime.hour,nextRefTime.minute);

		nextRefreshText.text = nextRefDes;

		if ((int)PB.shopType.NORMALSHOP == shopData.type) 
		{
			shopCoinBtn.gameObject.SetActive(false);
		}
		else 
		{
			shopCoinBtn.gameObject.SetActive(true);
		}


		//shop items
		List<PB.ShopItem> itemsInfo = shopData.itemInfos;


		ShopItem shopItemUi = null;
		PB.ShopItem shopItemData = null;
		for (int i =0; i<listShopItem.Count; ++ i)
		{
			shopItemData = null;
			shopItemUi = listShopItem[i];
			if(i < itemsInfo.Count)
			{
				shopItemData = itemsInfo[i];
				shopItemUi.gameObject.SetActive(true);
				shopItemUi.RefreshShopItem(shopItemData,shopData.shopId,shopData.type);
			}
			else
			{
				shopItemUi.gameObject.SetActive(false);
			}
		}
	}


	int GetPreShopType()
	{
		if (curShopType == (int)PB.shopType.NORMALSHOP) 
		{
			return (int)PB.shopType.ALLIANCESHOP;
		}
		else
		{
			return (curShopType - 1);
		}
	}

	int GetNextShopType()
	{
		if (curShopType == (int)PB.shopType.ALLIANCESHOP)
		{
			return (int) PB.shopType.NORMALSHOP;
		}
		else
		{
			return (curShopType + 1);
		}
	}

	string GetShopName(int stype)
	{
		switch (stype) 
		{
		case (int)PB.shopType.NORMALSHOP:
			return "普通商店";
		case (int)PB.shopType.ALLIANCESHOP:
			return "工会商店";
		default:
			return "";
		}
	}
}
