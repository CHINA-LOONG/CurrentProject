using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIShop : UIBase
{
	public static string ViewName = "UIShop";

	public	CoinButton	jinbiCoinBtn;
	public	CoinButton	zuanshiCoinBtn;

	public	ScrollView	shopItemsScrollView;
	public	Text		shopName;
	public	Text		nextRefreshText;
	public 	Text 		refreshText;

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

		refreshText.text = StaticDataMgr.Instance.GetTextByID ("shop_refresh");
		shopDataMgr = GameDataMgr.Instance.ShopDataMgrAttr;
		
		EventTriggerListener.Get (refreshButton.gameObject).onClick = OnRefreshButtonClilck;
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseButtonClick ;
		EventTriggerListener.Get (leftButton.gameObject).onClick = OnLeftButtonClick ;
		EventTriggerListener.Get (rightButton.gameObject).onClick = OnRightButtonClick ;
		EventTriggerListener.Get (jinbiCoinBtn.addCoinButton.gameObject).onClick = OnJinbiCoinButtonClick;
		EventTriggerListener.Get (zuanshiCoinBtn.addCoinButton.gameObject).onClick = OnZuanshiCoinbuttonClick;

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
			MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,"今天刷新次数已经用完");//todo:liws 禁用按钮 
			return;
		}

		if (GameDataMgr.Instance.PlayerDataAttr.gold < 5)
		{
			//MsgBox.PromptMsg.Open("提示","钻石不足","确定");
			shopDataMgr.ZuanshiNoEnough();
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

	void	OnJinbiCoinButtonClick(GameObject go)
	{
		shopDataMgr.JinbiNoEnough ();
	}
	void 	OnZuanshiCoinbuttonClick(GameObject go)
	{
		UIMgr.Instance.OpenUI_ (UIMall.ViewName, false);
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
		string day =  StaticDataMgr.Instance.GetTextByID("shop_today");;
		if (nextRefTime.dayOfMonth > 1)
		{
			day = StaticDataMgr.Instance.GetTextByID("shop_tommorow");
		}
		string nextRefDes = string.Format ("{0}{1}:{2:00}:{3:00}",StaticDataMgr.Instance.GetTextByID("shop_nextrefresh"),
		                                   day,nextRefTime.hour,nextRefTime.minute);

		nextRefreshText.text = nextRefDes;

		Sprite coinSp = ResourceMgr.Instance.LoadAssetType<Sprite>(GetCoinImageName(shopData.type)) as Sprite;
		if(null != coinSp)
		{
			jinbiCoinBtn.coinImage.sprite = coinSp;
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

	string GetCoinImageName(int stype)
	{
		switch (stype) 
		{
		case (int)PB.shopType.NORMALSHOP:
			return "icon_jinbi";
		case (int)PB.shopType.ALLIANCESHOP:
			return "icon_gonghuibi";
		default:
			return "";
		}
	}

	string GetShopName(int stype)
	{
		switch (stype) 
		{
		case (int)PB.shopType.NORMALSHOP:
			return StaticDataMgr.Instance.GetTextByID("shop_putong");
		case (int)PB.shopType.ALLIANCESHOP:
			 return StaticDataMgr.Instance.GetTextByID("shop_gonghui");
		default:
			return "";
		}
	}
}
