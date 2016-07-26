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
			RefreshCountText();
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}

	int  changeStep = 0;
	int  updateFrame = 10;
	void Update()
	{
		if (changeStep == 0)
			return;

		if(updateFrame <= 0)
		{
			updateFrame = 10;
			CurCountAttr += changeStep;
			if(curCount <= 1 || curCount >= maxCount)
			{
				changeStep = 0;
			}
		}
		else
		{
			updateFrame --;
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

		EventTriggerListener.Get (leftImage.gameObject).onDown = OnLeftImageDown;
		EventTriggerListener.Get (leftImage.gameObject).onUp = OnLeftImageUp;

		EventTriggerListener.Get (rightImage.gameObject).onDown = OnRightImageDown;
		EventTriggerListener.Get (rightImage.gameObject).onUp = OnRightImageUp;
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

		float fscale = UIMgr.Instance.CanvasAttr.scaleFactor;
		Vector3 spacePos = UIUtil.GetSpacePos (sellItemRt, UIMgr.Instance.CanvasAttr, UICamera.Instance.CameraAttr);
		rootPanel.anchoredPosition = new Vector2(spacePos.x/fscale,spacePos.y/fscale);
	}

	void	RefreshCountText()
	{
		countText.text = curCount.ToString ();
	}

	void	OnMinButtonClicked(GameObject go)
	{
		CurCountAttr = 1;
	}

	void	OnMaxButtonClicked(GameObject go)
	{
		CurCountAttr = maxCount;
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
		OnOKButtonClicked (null);
	}

	void OnLeftImageDown(GameObject go)
	{
		changeStep = -1;
	}

	void OnLeftImageUp(GameObject go)
	{
		changeStep = 0;
	}

	void OnRightImageDown(GameObject go)
	{
		changeStep = 1;
	}

	void OnRightImageUp(GameObject go)
	{
		changeStep = 0;
	}
}
