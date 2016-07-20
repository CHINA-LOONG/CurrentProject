using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILogin : UIBase
{
	public static	string	ViewName = "UILogin";
	public Text testTipInfo = null;

	public Button m_LoginButton;
    public Button resetGuuidButton;

	IEnumerator Start()
	{
		EventTriggerListener.Get(m_LoginButton.gameObject).onClick = onLoginButtonClicked;
		EventTriggerListener.Get(resetGuuidButton.gameObject).onClick = OnGuuidButtonClicked;
		
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
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
}
