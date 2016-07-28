using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICoinExchange : UIBase
{
	public static string ViewName = "UICoinExchange";
	
	public 	Text	msgTitle;
	public	Text	msgTodayDesc;
	
	public	Text	msgBuyDesc1;
	public	Text	msgBuyDesc2;
	
	public	Text	zuanshiText;
	public	Text	jinbiText;
	
	public	Button	cancelButton;
	public	Button	buyButton;
	
	bool	isFirst = true;

	public	static	void	Open()
	{
		UIMgr.Instance.OpenUI_ (UICoinExchange.ViewName, false);
		if (StatisticsDataMgr.Instance.gold2coinExchargeTimes >= MaxExchangeCount)
		{
			MsgBox.PromptMsg.Open (MsgBox.MsgBoxType.Conform,
			                       StaticDataMgr.Instance.GetTextByID ("shop_noduihuantimes"));//shop_noduihuantimes shop_nojinbi_noduihuan
		}
	}

	public	static	int	MaxExchangeCount 
	{
		get
		{
			GoldChargeData	item = StaticDataMgr.Instance.GetGoldChageStaticData(GameConfig.Instance.GoldExchangeId);
			return item.maxTimes;
		}
	}

	public override void Init()
	{
		if (isFirst)
		{
			firstInit();
		}
		RefreshExchangeUI ();
	}
	
	private	void	firstInit()
	{
		isFirst = false;
		EventTriggerListener.Get (cancelButton.gameObject).onClick = OnCancelButtonClick;
		EventTriggerListener.Get (buyButton.gameObject).onClick = OnBuyButtonClick;
		BindListener ();
		msgTitle.text = StaticDataMgr.Instance.GetTextByID ("shop_duihuan");
		msgBuyDesc1.text = StaticDataMgr.Instance.GetTextByID ("shop_isbuy");
		msgBuyDesc2.text = StaticDataMgr.Instance.GetTextByID ("shop_now");

		cancelButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("ui_quxiao");
		buyButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("ui_queding");
	}
	
	public override void Clean()
	{
		UnBindListener ();
	}
	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.SHOP_GOLD2COIN_C.GetHashCode ().ToString(), OnExchangeFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SHOP_GOLD2COIN_S.GetHashCode().ToString(), OnExchangeFinished);
	}
	
	void UnBindListener()
	{
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SHOP_GOLD2COIN_C.GetHashCode().ToString(), OnExchangeFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SHOP_GOLD2COIN_S.GetHashCode().ToString(), OnExchangeFinished);
	}

	void	RefreshExchangeUI()
	{
		GoldChargeData	item = StaticDataMgr.Instance.GetGoldChageStaticData(GameConfig.Instance.GoldExchangeId);
		int curExchange = StatisticsDataMgr.Instance.gold2coinExchargeTimes + 1;
		int zuanshi = item.GetBaseZuanshiWithTime (curExchange);;
		int jinbi = item.GetBaseJinBiWithTime (curExchange);

		zuanshiText.text = zuanshi.ToString ();
		jinbiText.text =  jinbi.ToString ();

		msgTodayDesc.text = string.Format (StaticDataMgr.Instance.GetTextByID ("shop_duihuanTimes"),
		                                  curExchange,
		                                  MaxExchangeCount);
	}

	void OnCancelButtonClick(GameObject go)
	{
		UIMgr.Instance.DestroyUI (this);
	}
	
	void OnBuyButtonClick(GameObject go)
	{
		PB.HSShopGold2Coin param = new PB.HSShopGold2Coin ();
        GameApp.Instance.netManager.SendMessage(PB.code.SHOP_GOLD2COIN_C.GetHashCode(), param);
	}

	void OnExchangeFinished(ProtocolMessage msg)
	{
		UINetRequest.Close ();
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
		{
			PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();

			if(error.errCode == (int)PB.PlayerError.GOLD_NOT_ENOUGH)
			{
				GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
			}

			return;
		}

		PB.HSShopGold2CoinRet msgret = msg.GetProtocolBody<PB.HSShopGold2CoinRet> ();
		int getJinbi = msgret.changeCount * msgret.multiple;
		StatisticsDataMgr.Instance.gold2coinExchargeTimes = getJinbi;
		string immsg = null;
		if(msgret.multiple > 1)
		{
			immsg = string.Format(StaticDataMgr.Instance.GetTextByID("succbuy_baoji"),getJinbi);
		}
		else
		{
			immsg = string.Format(StaticDataMgr.Instance.GetTextByID("succbuy"),getJinbi);
		}
	
		UIIm.Instance.ShowSystemHints (immsg, (int) PB.ImType.PROMPT);
		UIMgr.Instance.DestroyUI (this);
	}
}
