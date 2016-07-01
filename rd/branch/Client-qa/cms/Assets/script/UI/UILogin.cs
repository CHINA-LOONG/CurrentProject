using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILogin : UIBase
{
	public static	string	ViewName = "UILogin";
	public Button m_LoginButton;
	void Start()
	{
		EventTriggerListener.Get (m_LoginButton.gameObject).onClick = onLoginButtonClicked;
	}

	void  onLoginButtonClicked(GameObject go)
	{
		//Debug.Log ("Login Button Click!!");
		GameEventMgr.Instance.FireEvent (GameEventList.LoginClick);
	}
}
