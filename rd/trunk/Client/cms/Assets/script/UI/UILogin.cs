using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILogin : UIBase, TabButtonDelegate
{
	public static	string	ViewName = "UILogin";
	public static string AssertName = "ui/login";
	public Text testTipInfo = null;

	public Button m_LoginButton;
	public Button resetGuuidButton;
	IEnumerator Start()
	{
		EventTriggerListener.Get (m_LoginButton.gameObject).onClick = onLoginButtonClicked;

		EventTriggerListener.Get (resetGuuidButton.gameObject).onClick = OnGuuidButtonClicked;

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		GetComponent<TabButtonGroup> ().InitWithDelegate (this);
	}
	

	void  onLoginButtonClicked(GameObject go)
	{
		//Debug.Log ("Login Button Click!!");
        GameEventMgr.Instance.FireEvent(GameEventList.LoginClick);
	}

	void  OnGuuidButtonClicked(GameObject go)
	{
		//Debug.Log ("Login Button Click!!");
		PlayerPrefs.DeleteKey ("testGuid");
	}

	public void OnTabButtonChanged(int index)
	{
		testTipInfo.text = "changeItemIndex: " + index.ToString();
	}
}
