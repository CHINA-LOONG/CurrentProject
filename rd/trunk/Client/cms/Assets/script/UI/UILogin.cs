using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILogin : UIBase, TabButtonDelegate
{
	public static	string	ViewName = "UILogin";
	public Text testTipInfo = null;

	public Button m_LoginButton;
    public Button resetGuuidButton;
    public Button changeServerButton;

	IEnumerator Start()
	{
		EventTriggerListener.Get(m_LoginButton.gameObject).onClick = onLoginButtonClicked;
		EventTriggerListener.Get(resetGuuidButton.gameObject).onClick = OnGuuidButtonClicked;
        EventTriggerListener.Get(changeServerButton.gameObject).onClick = OnChangeServerButtonClicked;
		
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

    void OnChangeServerButtonClicked(GameObject go)
    {
        Text changeButtonText = changeServerButton.GetComponentInChildren<Text>();
        if (changeServerButton != null)
        {
            if (Const.ServerType == Const.SERVERTYPE.LOCAL_SERVER_NORMAL)
            {
                Const.ServerType = Const.SERVERTYPE.LOCAL_SERVER_TEST;
                changeButtonText.text = "内网测试服务器";
            }
            else if (Const.ServerType == Const.SERVERTYPE.LOCAL_SERVER_TEST)
            {
                Const.ServerType = Const.SERVERTYPE.REMOTE_SERVER_NORMAL;
                changeButtonText.text = "外网服务器";
            }
            else if (Const.ServerType == Const.SERVERTYPE.REMOTE_SERVER_NORMAL)
            {
                Const.ServerType = Const.SERVERTYPE.LOCAL_SERVER_NORMAL;
                changeButtonText.text = "内网正式服务器";
            }
        }

    }

	public void OnTabButtonChanged(int index)
	{
		testTipInfo.text = "changeItemIndex: " + index.ToString();
	}
}
