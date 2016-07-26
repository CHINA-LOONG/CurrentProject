using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdjustCountForSell : UIBase 
{
	public static string ViewName = "AdjustCountForSell";
	public	static void	OpenWith(int maxValue,int defaultValue,RectTransform sellItemRt,AdjustCountForSellDelegate callBack )
	{
		AdjustCountForSell ad = (AdjustCountForSell)UIMgr.Instance.OpenUI_ (ViewName, false);
		ad.SetParam (maxValue, defaultValue, sellItemRt, callBack);
	}
	public delegate void AdjustCountForSellDelegate(int sellCount);
	public	Button	minButton;
	public	Button	maxButton;
	public	Button	okButton;
	public	Button	cancelButton;

	public	Image	leftImage;
	public	Image	rightImage;
	public	Text	countText;
	public	RectTransform	rootPanel;

	private	ChangeValueByHand	chgValue = null;

	private AdjustCountForSellDelegate callBack;
	private	int	maxCount = 0;
	private	int curCount = 0;
	private int defaultValue = 0;
	private	int CurCountAttr
	{
		get{ return curCount;}
		set
		{
			curCount = value;
			if(curCount < 1) 
			{
				curCount = 1;
			}
			if(curCount > maxCount)
			{
				curCount = maxCount;
			}
			countText.text = curCount.ToString ();
		}
	}
	
	bool isFirst = true;
	public override void Init()
	{
		if (isFirst)
		{
			isFirst = true;
			FirsInit();
		}
	}
	
	void FirsInit()
	{
		EventTriggerListener.Get (minButton.gameObject).onClick = OnMinButtonClicked;
		EventTriggerListener.Get (maxButton.gameObject).onClick = OnMaxButtonClicked;
		EventTriggerListener.Get (okButton.gameObject).onClick = OnOKButtonClicked;
		EventTriggerListener.Get (cancelButton.gameObject).onClick = OnCancelButtonClicked;

		chgValue = GetComponent<ChangeValueByHand> ();
		chgValue.callback = OnChangedValueByHand;
	}

	public override void Clean()
	{
	}

	public void SetParam(int maxValue, int defaultValue, RectTransform sellItemRt, AdjustCountForSellDelegate callBack )
	{
		this.maxCount = maxValue;
		this.callBack = callBack;
		this.defaultValue = defaultValue;
		CurCountAttr = defaultValue;

		chgValue.maxValue = maxCount;
		chgValue.curValue = defaultValue;

		float fscale = UIMgr.Instance.CanvasAttr.scaleFactor;
		Vector3 spacePos = UIUtil.GetSpacePos (sellItemRt, UIMgr.Instance.CanvasAttr, UICamera.Instance.CameraAttr);
		rootPanel.anchoredPosition = new Vector2(spacePos.x/fscale,spacePos.y/fscale);
	}

	void OnChangedValueByHand(int value)
	{
		CurCountAttr = value;
	}
	
	void	OnMinButtonClicked(GameObject go)
	{
		CurCountAttr = 1;
		chgValue.curValue = 1;
	}

	void	OnMaxButtonClicked(GameObject go)
	{
		CurCountAttr = maxCount;
		chgValue.curValue = maxCount;
	}

	void 	OnOKButtonClicked(GameObject go)
	{
		if (callBack != null) 
		{
			callBack(curCount);
		}
		UIMgr.Instance.CloseUI_ (this);
	}

	void 	OnCancelButtonClicked(GameObject go)
	{
		curCount = 0;
		chgValue.curValue = 0;
		OnOKButtonClicked (null);
	}
	
}
