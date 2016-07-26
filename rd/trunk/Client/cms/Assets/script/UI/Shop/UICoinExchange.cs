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
	}
	
	public override void Init()
	{
		if (isFirst)
		{
			firstInit();
		}
	}
	
	private	void	firstInit()
	{
		isFirst = false;
		EventTriggerListener.Get (cancelButton.gameObject).onClick = OnCancelButtonClick;
		EventTriggerListener.Get (buyButton.gameObject).onClick = OnBuyButtonClick;
		BindListener ();
	}
	
	public override void Clean()
	{
		UnBindListener ();
	}
	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ShopGold2CoinC.GetHashCode ().ToString(), OnExchangeFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ShopGold2CoinS.GetHashCode().ToString(), OnExchangeFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ShopGold2CoinC.GetHashCode ().ToString (), OnExchangeFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ShopGold2CoinS.GetHashCode().ToString(), OnExchangeFinished);

	}
	
	void OnCancelButtonClick(GameObject go)
	{
		UIMgr.Instance.DestroyUI (this);
	}
	
	void OnBuyButtonClick(GameObject go)
	{
		PB.HSShopGold2Coin param = new PB.HSShopGold2Coin ();
		GameApp.Instance.netManager.SendMessage (PB.code.ShopGold2CoinC.GetHashCode (), param);
	}

	void OnExchangeFinished(ProtocolMessage msg)
	{
		UINetRequest.Close ();
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
		{
			PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();

			if(error.errCode == (int)PB.PlayerError.GOLD_NOT_ENOUGH)
			{
				//MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,"钻石不足");
				GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
			}

			return;
		}

		PB.HSShopGold2CoinRet msgret = msg.GetProtocolBody<PB.HSShopGold2CoinRet> ();
		//msgret.changeCount 已经兑换次数

		UIMgr.Instance.DestroyUI (this);
	}
}
