using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeValueByHand : MonoBehaviour 
{

	public	delegate	void	ChangeValueDelegate(int value);
	public	ChangeValueDelegate	callback = null;

	public	Image	changeSmaller;
	public	Image	changeBigger;

	public	int	minValue = 1;
	public	int maxValue = 1;
	public	int curValue = 1;

	// Use this for initialization
	void Start ()
	{
		EventTriggerListener.Get (changeSmaller.gameObject).onDown = OnLeftImageDown;
		EventTriggerListener.Get (changeSmaller.gameObject).onUp = OnLeftImageUp;
		
		EventTriggerListener.Get (changeBigger.gameObject).onDown = OnRightImageDown;
		EventTriggerListener.Get (changeBigger.gameObject).onUp = OnRightImageUp;
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
			curValue += changeStep;
			if(curValue <= minValue || curValue >= maxValue)
			{
				changeStep = 0;
			}
			if(null != callback)
			{
				callback(curValue);
			}
		}
		else
		{
			updateFrame --;
		}
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
