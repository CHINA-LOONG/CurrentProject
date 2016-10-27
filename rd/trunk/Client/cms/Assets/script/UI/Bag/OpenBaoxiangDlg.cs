using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpenBaoxiangDlg : UIBase {

	public static string ViewName = "OpenBaoxiangDlg";

	public	static	void	OpenWith(ItemData	baoxiangData, int maxKeyValue)
	{
		OpenBaoxiangDlg dlg = (OpenBaoxiangDlg)UIMgr.Instance.OpenUI_ (ViewName, false);
		dlg.RefreshWith (baoxiangData, maxKeyValue);
	}
	//----------------------------------------------------------------------------
	public	ItemIcon	itemIcon;
	public	Text	useCountDescText;
	public	Text	usecountValueText;
	public	Text	boxNameText;

	public	Button	cancelButton;
	public	Button	OKButton;

	private ChangeValueByHand chgValue;
	private	ItemData	itemData;
	private	int	useKeyValue = 0;
	private	int	UseKeyValueAttr
	{
		get{ return useKeyValue;}
		set
		{
			useKeyValue = value;
			usecountValueText.text = useKeyValue.ToString();

		}
	}
	//-----------------------------------------------------------------------------
	bool isFirst = true;
	public override void Init(bool forbidGuide = false)
	{
		if (isFirst)
		{
			isFirst = false;
			FirsInit();
		}
		
	}
	
	void FirsInit()
	{
		EventTriggerListener.Get (cancelButton.gameObject).onClick = OnCancelButtonClicked;
		EventTriggerListener.Get (OKButton.gameObject).onClick = OnOKButtonClicked;
		
		chgValue = GetComponent<ChangeValueByHand> ();
		chgValue.callback = OnUseKeysValueChanged;
		useCountDescText.text = StaticDataMgr.Instance.GetTextByID ("bag_usenum");
		cancelButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("ui_quxiao");
		OKButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("ui_queding");

	}
	
	public override void Clean()
	{
		
	}

	public	void	RefreshWith(ItemData	baoxiangData, int maxKeyValue)
	{
		itemData = baoxiangData;
		UseKeyValueAttr = maxKeyValue;

		chgValue.ResetValue (maxKeyValue, maxKeyValue);
		ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData(baoxiangData.itemId);
		if (null != itemStaticData)
		{
			UIUtil.SetStageColor(boxNameText,itemStaticData);
		}

		itemIcon.RefreshWithItemInfo (itemData);
	}

	void OnUseKeysValueChanged(int value)
	{
		UseKeyValueAttr = value;
	}

	void	OnCancelButtonClicked(GameObject go)
	{
		UIMgr.Instance.CloseUI_ (this);
	}

	void 	OnOKButtonClicked(GameObject go)
	{
		////开宝箱
		Logger.LogError ("开宝箱了。。。。");
		RequestOpenBox ();
	}

	void RequestOpenBox()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ITEM_BOX_USE_BATCH_C.GetHashCode ().ToString(), OnOpenBoxFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ITEM_BOX_USE_BATCH_S.GetHashCode ().ToString (), OnOpenBoxFinished);

		PB.HSItemBoxUseBatch param = new PB.HSItemBoxUseBatch ();
		param.itemId = itemData.itemId;
		param.itemCount = useKeyValue;

		GameApp.Instance.netManager.SendMessage (PB.code.ITEM_BOX_USE_BATCH_C.GetHashCode (), param);
	}

	void OnOpenBoxFinished(ProtocolMessage msg)
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ITEM_BOX_USE_BATCH_C.GetHashCode ().ToString(), OnOpenBoxFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ITEM_BOX_USE_BATCH_S.GetHashCode ().ToString (), OnOpenBoxFinished);

		UINetRequest.Close ();
		
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
		{
			Logger.LogError("open box Error.....");
			return;
		}
		GameEventMgr.Instance.FireEvent (GameEventList.OpenBoxFinished);
		UIMgr.Instance.CloseUI_ (this);
		//PB.HSItemBoxUseBatchRet msgRet = msg.GetProtocolBody<PB.HSItemBoxUseBatchRet> ();
	}

}
