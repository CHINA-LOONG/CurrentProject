using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Funplus;

public class UILogin : UIBase
{
	public static	string	ViewName = "UILogin";
	public Text testTipInfo = null;

	public Button m_LoginButton;
    public Button m_FunplusButton;
    public InputField playerIDFileld;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<string>(GameEventList.funplusPuid, OnFunPlusGetPuid);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<string>(GameEventList.funplusPuid, OnFunPlusGetPuid);
    }

    public override void Init()
    {
        BindListener();
    }

    public override void Clean()
    {
        UnBindListener();
    }

	IEnumerator Start()
	{
		EventTriggerListener.Get(m_LoginButton.gameObject).onClick = onLoginButtonClicked;
        EventTriggerListener.Get(m_FunplusButton.gameObject).onClick = OnFunPlusButtonClicked;
       
        if (playerIDFileld != null && string.IsNullOrEmpty(PlayerPrefs.GetString("testGuid")) == false)
        {
            playerIDFileld.text = PlayerPrefs.GetString("testGuid");
        }

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
	}
	
	void  onLoginButtonClicked(GameObject go)
	{
		//Logger.Log ("Login Button Click!!");

        if (string.IsNullOrEmpty(playerIDFileld.text) == true)
        {
            Logger.Log("请输入用户名");
            return;
        }
        UIServer.ResetUserData(playerIDFileld.text);
        PlayerPrefs.SetString("testGuid", playerIDFileld.text);
        GameEventMgr.Instance.FireEvent(GameEventList.LoginClick);
	}

    void OnFunPlusButtonClicked(GameObject go)
    {
        FunplusAccount.GetInstance().OpenSession();
    }

    void OnFunPlusGetPuid(string funplusPuid) {
        if (string.IsNullOrEmpty(funplusPuid) == false)
        {
            UIServer.ResetUserData(funplusPuid);
            PlayerPrefs.SetString("testGuid", funplusPuid);
            GameEventMgr.Instance.FireEvent(GameEventList.LoginClick);
        }
    }
}
