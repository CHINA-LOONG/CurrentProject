using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DazhaoTip : MonoBehaviour 
{
	public Text timeText;
	public Text attackText;
	private bool isShow = false;
	// Use this for initialization
	void Start () 
	{
	
	}

	public void SetTipInfo(int timeLeft,int useAttack,int allAttack)
	{
		timeText.text = timeLeft.ToString ();
		//attackText.text = useAttack.ToString () + "/" + allAttack.ToString ();
	}

	public void Show()
	{
		isShow = true;
		gameObject.SetActive (true);
	}

	public void Hide()
	{
		isShow = false;
		gameObject.SetActive (false);
	}
	public bool IsShow()
	{
		return  isShow;
	}
}
