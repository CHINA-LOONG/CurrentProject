using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIShop : UIBase
{
	public static string ViewName = "UIShop";

	public	CoinButton	jinbiCoinBtn;//金币，公会币 通用按钮
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
    int maxOpenShopIndex = 0;

    TimeStaticData	timeNextRefresh = null;
		
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

		BindListener ();
		
		isInit = true;
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener (GameEventList.RefreshShopUi, OnRefreshShopUi);
		GameEventMgr.Instance.AddListener (GameEventList.RefreshShopUiAfterBuy, OnRefreshUIAfterBuy);
        GameEventMgr.Instance.AddListener<long>(GameEventList.CoinChanged, OnCoinChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.ZuanshiChanged, OnZuanshiChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.TowerCoinChanged, OnTowerCoinChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.GonghuiCoinChanged, OnGonghuibiChanged);
    }
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener (GameEventList.RefreshShopUi, OnRefreshShopUi);
		GameEventMgr.Instance.RemoveListener (GameEventList.RefreshShopUiAfterBuy, OnRefreshUIAfterBuy);
        GameEventMgr.Instance.RemoveListener<long>(GameEventList.CoinChanged, OnCoinChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.ZuanshiChanged, OnZuanshiChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.TowerCoinChanged, OnTowerCoinChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.GonghuiCoinChanged, OnGonghuibiChanged);
    }

	void OnRefreshButtonClilck(GameObject go)
	{
		PB.ShopData shopData = shopDataMgr.GetShopData (curShopType);
		if (null == shopData)
		{
			return;
		}
        int maxRefreshTimes = shopDataMgr.GetMaxRefreshTimesWithShopType(curShopType);
        if(maxRefreshTimes == -1)
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("shop_refreshtips1"), OnRefreshConformDlgClick);
            return;
        }

		if (shopData.refreshTimesLeft < 1)
		{
			UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("shop_refresh_error"),
			                              (int)PB.ImType.PROMPT);
			return;
		}
		string msg = string.Format (StaticDataMgr.Instance.GetTextByID ("shop_refreshtips"), shopData.refreshTimesLeft);
		MsgBox.PromptMsg.Open (MsgBox.MsgBoxType.Conform_Cancel, msg, OnRefreshConformDlgClick);
	
	}
	void OnRefreshConformDlgClick(MsgBox.PrompButtonClick param)
	{
		if (param == MsgBox.PrompButtonClick.OK) 
		{
			if (GameDataMgr.Instance.PlayerDataAttr.gold < 20)
			{
				shopDataMgr.ZuanshiNoEnough();
				return;
			}
			
			shopDataMgr.RefreshShopWithDiamond(curShopType);
		}
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
        CheckOpendShop();
		curShopType = shopType;
        if (curShopType > maxOpenShopIndex)
        {
            curShopType = maxOpenShopIndex;
        }

		if (forceRefresh || shopDataMgr.IsNeedUpdateShopData ())
		{
			shopDataMgr.RequestShopData();
		}
		else 
		{
			OnRefreshShopUi();
		}
		timeNextRefresh = shopDataMgr.GetNextFreeRefreshTime (curShopType);
        zuanshiCoinBtn.gameObject.SetActive(shopType == (int)PB.shopType.NORMALSHOP);
    }

    void CheckOpendShop()
    {
        leftButton.gameObject.SetActive(true);
        if (GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.OpenLevelForGonghui)
        {
            maxOpenShopIndex = (int)PB.shopType.ALLIANCESHOP;
        }
        else if(GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.OpenLevelForTower)
        {
            maxOpenShopIndex = (int)PB.shopType.TOWERSHOP;
        }
        else
        {
            leftButton.gameObject.SetActive(false);
            maxOpenShopIndex = (int)PB.shopType.NORMALSHOP;
        }
    }

	void	OnRefreshShopUi()
	{
		RefreshUIWithNormalizedScrollPosition ();
	}

	void	OnRefreshUIAfterBuy()
	{
		RefreshUIWithNormalizedScrollPosition (false);
	}
    void OnCoinChanged(long coin)
    {
        RefreshUIWithNormalizedScrollPosition(false);
    }
    void OnZuanshiChanged(int zuanshi)
    {
        RefreshUIWithNormalizedScrollPosition(false);
    }
    void OnGonghuibiChanged(int gonghuibi)
    {
        RefreshUIWithNormalizedScrollPosition(false);
    }
    void OnTowerCoinChanged(int towerCoin)
    {
        RefreshUIWithNormalizedScrollPosition(false);
    }

	void	RefreshUIWithNormalizedScrollPosition(bool normallized = true)
	{
		PB.ShopData shopData = shopDataMgr.GetShopData (curShopType);
		if (null == shopData)
		{
			Logger.LogError("Error for Get shopData....");
			return;
		}
		
		shopName.text = GetShopName (curShopType);
		
		TimeStaticData nextRefTime = shopDataMgr.GetNextFreeRefreshTime (curShopType);
        if(null == nextRefTime)
        {
            nextRefreshText.text = "";
        }
        else
        {
            string day = StaticDataMgr.Instance.GetTextByID("shop_today"); ;
            if (nextRefTime.dayOfMonth > 0)
            {
                day = StaticDataMgr.Instance.GetTextByID("shop_tommorow");
            }
            string nextRefDes = string.Format("{0}{1} {2:00}:{3:00}", StaticDataMgr.Instance.GetTextByID("shop_nextrefresh"),
                                               day, nextRefTime.hour, nextRefTime.minute);

            nextRefreshText.text = nextRefDes;
        }
		
		jinbiCoinBtn.CoinTypeAttr = GetCoinType (shopData.type);
		
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
		if (normallized)
		{
			ScrollRect sr = shopItemsScrollView.GetComponent<ScrollRect> ();
			if (sr != null) 
			{
				sr.horizontalNormalizedPosition = 0.0f;
			}
		}
	}


	int GetPreShopType()
	{
		if (curShopType == (int)PB.shopType.NORMALSHOP) 
		{
			return maxOpenShopIndex;
		}
		else
		{
			return (curShopType - 1);
		}
	}

	int GetNextShopType()
	{
		if (curShopType == maxOpenShopIndex)
		{
			return (int) PB.shopType.NORMALSHOP;
		}
		else
		{
			return (curShopType + 1);
		}
	}
	
	CoinButton.CoinType GetCoinType(int stype)
	{
		switch (stype) 
		{
		case (int)PB.shopType.NORMALSHOP:
			return CoinButton.CoinType.Jinbi;
		case (int)PB.shopType.ALLIANCESHOP:
			return CoinButton.CoinType.GonghuiBi;
            case (int)PB.shopType.TOWERSHOP:
                return CoinButton.CoinType.TowerCoin;
		default:
			return CoinButton.CoinType.Jinbi;
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
            case (int)PB.shopType.TOWERSHOP:
                return StaticDataMgr.Instance.GetTextByID("towerBoss_instance_shop"); 
		default:
			return "";
		}
	}
}
