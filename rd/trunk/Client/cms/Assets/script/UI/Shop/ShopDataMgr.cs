using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShopDataMgr : MonoBehaviour 
{
	public	struct ShopDescWithLevel
	{
		public	int	maxRefreshTimes;
		public	int	minLevel;
		public	int	maxLevel;
	}

	//商城中数据
	public	int	monthCardLeft = 0;
	public	List<PB.RechargeState> listRechageState;


	public	Dictionary<int,PB.ShopData> shopDataDic = new Dictionary<int, PB.ShopData> ();
	TimeStaticData	lastRefreshTime;

	//buying
	PB.ShopItem curBuyItem = null;
	public	void	Init()
	{
		BindListener ();
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ShopDataInitC.GetHashCode ().ToString(), OnRequestShopDataFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ShopDataInitS.GetHashCode().ToString(), OnRequestShopDataFinished);
		
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ShopDataSynC.GetHashCode ().ToString(), OnRefreshShopWithFreeFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ShopDataSynS.GetHashCode().ToString(), OnRefreshShopWithFreeFinished);
		
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ShopRefreshC.GetHashCode ().ToString(), OnRefreshShopWithDiamondFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ShopRefreshS.GetHashCode ().ToString (), OnRefreshShopWithDiamondFinished);
		
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ShopItemBuyC.GetHashCode ().ToString(), OnBuyShopItemFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ShopItemBuyS.GetHashCode ().ToString (), OnBuyShopItemFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.INSTANCE_ENTER_S.GetHashCode ().ToString (), OnRequestShopDataFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestShopDataFinished);

		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ShopDataSynC.GetHashCode ().ToString(), OnRefreshShopWithFreeFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ShopDataSynS.GetHashCode().ToString(), OnRefreshShopWithFreeFinished);
		
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ShopRefreshC.GetHashCode ().ToString(), OnRefreshShopWithDiamondFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ShopRefreshS.GetHashCode ().ToString (), OnRefreshShopWithDiamondFinished);
		
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ShopItemBuyC.GetHashCode ().ToString(), OnBuyShopItemFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ShopItemBuyS.GetHashCode ().ToString (), OnBuyShopItemFinished);
	}

	public	bool	IsNeedUpdateShopData()
	{
		if (lastRefreshTime == null)
			return true;
		TimeStaticData tNow = GameTimeMgr.Instance.GetTime ();
		int delT = 60 * (tNow.hour - lastRefreshTime.hour) + (tNow.minute - lastRefreshTime.minute);
		if (delT > ShopConst.shopDataSynNeedMinute)
			return true;

		return false;
	}

	public	PB.ShopData GetShopData(int shopType)
	{
		PB.ShopData retData = null;
		shopDataDic.TryGetValue (shopType, out retData);
		return retData;
	}

	#region ------------------RequestShopData-----------------
	public	void	RequestShopData()
	{
		PB.HSShopDataInit param = new PB.HSShopDataInit ();
		GameApp.Instance.netManager.SendMessage (PB.code.ShopDataInitC.GetHashCode (), param);
	}

	void OnRequestShopDataFinished(ProtocolMessage msg)
	{
		UINetRequest.Close ();
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
		{
			PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
			Logger.LogError("RequestShopData Error errorCode: " + error.errCode);
			return;
		}
		PB.HSShopDataInitRet msgRet = msg.GetProtocolBody<PB.HSShopDataInitRet> ();
		shopDataDic.Clear ();
		PB.ShopData subShopData = null;
		for (int i =0; i< msgRet.shopDatas.Count; ++i)
		{
			subShopData = msgRet.shopDatas[i];
			shopDataDic.Add(subShopData.type,subShopData);
		}
		lastRefreshTime = GameTimeMgr.Instance.GetTime ();

		GameEventMgr.Instance.FireEvent (GameEventList.RefreshShopUi);
	}
	#endregion


	#region ----------------------RefreshShopWithFree--------------------------------
	public	void 	RefreshShopWithFree(int shopType)
	{
		PB.HSShopDataSyn param = new PB.HSShopDataSyn ();
		param.type = shopType;
		GameApp.Instance.netManager.SendMessage (PB.code.ShopDataSynC.GetHashCode (), param);
	}

	void	OnRefreshShopWithFreeFinished(ProtocolMessage msg)
	{
		UINetRequest.Close ();
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
		{
			PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
			Logger.LogError("RefreshShopWithFree Error errorCode: " + error.errCode);
			return;
		}
		PB.HSShopDataSynRet msgRet = msg.GetProtocolBody<PB.HSShopDataSynRet> ();

		shopDataDic [msgRet.shopData.type] = msgRet.shopData;
		lastRefreshTime = GameTimeMgr.Instance.GetTime ();
	}
	#endregion


	#region -----------RefreshShopWithDiamond----------
	public	void	RefreshShopWithDiamond(int shopType)
	{
		PB.HSShopRefresh param = new PB.HSShopRefresh ();
		param.type = shopType;
		GameApp.Instance.netManager.SendMessage (PB.code.ShopDataSynC.GetHashCode (), param);
	}

	void OnRefreshShopWithDiamondFinished(ProtocolMessage msg)
	{
		UINetRequest.Close ();
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
		{
			PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
			Logger.LogError("RefreshShopWithDiamond Error errorCode: " + error.errCode);
			return;
		}
		PB.HSShopRefreshRet msgRet = msg.GetProtocolBody<PB.HSShopRefreshRet> ();
		
		shopDataDic [msgRet.shopData.type] = msgRet.shopData;

		GameEventMgr.Instance.FireEvent (GameEventList.RefreshShopUi);
		lastRefreshTime = GameTimeMgr.Instance.GetTime ();
	}
	#endregion


	#region ------------------buyShopItem-----------------------------
	public void BuyShopItem(PB.ShopItem buyItem,int shopId,int shopType)
	{
		PB.HSShopItemBuy	param = new PB.HSShopItemBuy();
		param.type = shopType;
		param.slot = buyItem.slot;
		param.shopId = shopId;
		curBuyItem = buyItem;
		GameApp.Instance.netManager.SendMessage (PB.code.ShopItemBuyC.GetHashCode (), param);
	}

	void	OnBuyShopItemFinished(ProtocolMessage msg)
	{
		UINetRequest.Close ();
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
		{
			PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
			Logger.LogError( string.Format("BuyShopItem Error errorCode: {0:x}" ,error.errCode));

			if(error.errCode == (int)PB.shopError.SHOP_REFRESH_TIMEOUT ||
			   error.errCode == (int)PB.shopError.SHOP_ITEM_ALREADY_BUY)
			{
				MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,
				                      StaticDataMgr.Instance.GetTextByID("shop_timeout"),
				                      OnPrompButtonClick);
			}
			else if(error.errCode == (int)PB.itemError.GOLD_NOT_ENOUGH)
			{
				//MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,"钻石不足");
				ZuanshiNoEnough();
			}
			else if (error.errCode == (int)PB.itemError.COINS_NOT_ENOUGH)
			{
				JinbiNoEnough();
			}
			return;
		}
		curBuyItem.hasBuy = true;
		GameEventMgr.Instance.FireEvent (GameEventList.RefreshShopUi);
	}

    void OnPrompButtonClick(MsgBox.PrompButtonClick state)
	{
		if (state == MsgBox.PrompButtonClick.OK)
		{
			RequestShopData();
		}
	}
	#endregion

	#region -----钻石不足引导充值  --金币不足要兑换(兑换不足强提示)
	public	void	ZuanshiNoEnough()
	{
		MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,
		                      StaticDataMgr.Instance.GetTextByID("shop_zuanshinoenough"),
		                      OnChongzhiButtonSel);
	}
    void OnChongzhiButtonSel(MsgBox.PrompButtonClick state)
	{
		if (state == (int)MsgBox.PrompButtonClick.OK)
		{
			UIMgr.Instance.OpenUI_(UIMall.ViewName,false);
		}
	}

	public	void JinbiNoEnough()
	{
		bool isCanExchange = (UnityEngine.Random.Range (0, 2) == 1);

		if (isCanExchange) 
		{
			UICoinExchange.Open ();
		}
		else
		{
			MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,
			                      StaticDataMgr.Instance.GetTextByID("shop_nojinbi_noduihuan"));
		}
	}
	#endregion

	public ShopDescWithLevel GetShopDesc(int playerLevel, int shopType)
	{
		ShopDescWithLevel shopDesc = new ShopDescWithLevel() ;

		List<ShopStaticData> listShop = StaticDataMgr.Instance.GetShopStaticDataList ();
		List<ShopStaticData> listSpellShop = new List<ShopStaticData> ();
		for (int i =0; i< listShop.Count; ++i) 
		{
			if(listShop[i].type == shopType)
			{
				listSpellShop.Add(listShop[i]);
			}
		}

		ShopStaticData subShopData = null;
		for (int i =0; i<listSpellShop.Count; ++i) 
		{
			subShopData = listSpellShop[i];
			if(subShopData.maxLevel >= playerLevel)
			{
				shopDesc.maxRefreshTimes = subShopData.refreshMaxNumByHand;
				shopDesc.maxLevel =  subShopData.maxLevel;
				if( i ==0 )
				{
					shopDesc.minLevel = 1;
				}
				else
				{
					ShopStaticData lastData = listSpellShop[i - 1];
					shopDesc.minLevel = lastData.maxLevel;
				}
				break;
			}

			if(i == listSpellShop.Count -1)
			{
				shopDesc.maxRefreshTimes = subShopData.refreshMaxNumByHand;
				shopDesc.maxLevel = subShopData.maxLevel + 99;
				shopDesc.minLevel = subShopData.maxLevel;
			}
		}
		return shopDesc;
	}

	public	TimeStaticData	GetNextFreeRefreshTime(int shopType)
	{
		ShopAutoRefreshData refreshItem = StaticDataMgr.Instance.GetShopAutoRefreshData (shopType);

		ArrayList timesArray = MiniJsonExtensions.arrayListFromJson (refreshItem.times);
		string subitem = null;
		TimeStaticData tNow = GameTimeMgr.Instance.GetTime();
		TimeStaticData earlyRefresh = null;
		for (int i =0; i < timesArray.Count; ++ i)
		{
			subitem = timesArray[i] as string;
			TimeStaticData staticTime = StaticDataMgr.Instance.GetTimeData(int.Parse(subitem));
			if(i ==0 )
			{
				earlyRefresh = staticTime;
			}
			if(tNow < staticTime)
			{
				return staticTime;
			}
		}

		TimeStaticData tomorrow = new TimeStaticData ();
		tomorrow.hour = earlyRefresh.hour;
		tomorrow.minute = earlyRefresh.minute;
		tomorrow.dayOfMonth = 1;
		return tomorrow;
	}
}
