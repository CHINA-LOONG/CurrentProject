using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeValueByHand : MonoBehaviour 
{

	public	delegate	void	ChangeValueDelegate(int value);
	public	ChangeValueDelegate	callback = null;

	public	ArrowButton	changeSmaller;
	public	ArrowButton	changeBigger;

	private	int	minValue = 1;
	private	int maxValue = 1;
	private	int curValue = 1;
	public	int	CurValueAttr
	{
		get
		{
			return curValue;
		}
		set
		{
			ResetValue(value,maxValue,minValue);
		}
	}

	// Use this for initialization
	void Start ()
	{
		EventTriggerListener.Get (changeSmaller.gameObject).onDown = OnLeftImageDown;
		EventTriggerListener.Get (changeSmaller.gameObject).onUp = OnLeftImageUp;
		
		EventTriggerListener.Get (changeBigger.gameObject).onDown = OnRightImageDown;
		EventTriggerListener.Get (changeBigger.gameObject).onUp = OnRightImageUp;
	}
	
	int  changeStep = 0;
	int	 updateNeedFrames = 10;
	int  curFrame = 0;

	public	void ResetValue(int	curVal,int maxVal,int minMal = 1)
	{
		curValue = curVal;
		maxValue = maxVal;
		minValue = minMal;

		changeSmaller.IsEnable = (curValue > minValue);
		changeBigger.IsEnable = (curValue < maxValue);
	}

	void Update()
	{
		if (changeStep == 0)
			return;
		
		if(curFrame == 0)
		{
			curFrame ++;
			curValue += changeStep;
			if(curValue <= minValue )
			{
				curValue = minValue;
				changeStep = 0;
			}
			if( curValue >= maxValue)
			{
				curValue = maxValue;
				changeStep = 0;
			}
			if(null != callback)
			{
				callback(curValue);
			}
			changeSmaller.IsEnable = (curValue > minValue);
			changeBigger.IsEnable = (curValue < maxValue);
		}
		else
		{
			curFrame ++;
			if(curFrame >= updateNeedFrames)
			{
				updateNeedFrames = 5;
				curFrame = 0;
			}
		}
	}

	void OnLeftImageDown(GameObject go)
	{
		if (changeSmaller.IsEnable) 
		{
			curFrame = 0;
			updateNeedFrames = 20;
			changeStep = -1;
		}
	}
	
	void OnLeftImageUp(GameObject go)
	{
		if (changeSmaller.IsEnable) 
		{
			changeStep = 0;
		}
	}
	
	void OnRightImageDown(GameObject go)
	{
		if (changeBigger.IsEnable)
		{
			curFrame = 0;
			updateNeedFrames = 60;
			changeStep = 1;
		}
	}
	
	void OnRightImageUp(GameObject go)
	{
		if (changeBigger.IsEnable)
		{
			changeStep = 0;
		}
	}
}
