using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UICreatePlayer : UIBase
{
	public static	string	ViewName = "UICreatePlayer";
	public static string AssertName = "ui/createplayer";

	public Button 		createButton;
	public InputField   nameInputFileld;

	// Use this for initialization
	void Start ()
	{
		EventTriggerListener.Get (createButton.gameObject).onClick = OnCreateButtonClick;
		BindListener ();
	}

	void OnDestroy()
	{
		UnBindListener ();
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.PLAYER_CREATE_S.GetHashCode ().ToString(), OnRequestCreatePlayerFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.PLAYER_CREATE_S.GetHashCode ().ToString (), OnRequestCreatePlayerFinished);
	}

	void OnCreateButtonClick(GameObject go)
	{
		string name = nameInputFileld.text;
		if (string.IsNullOrEmpty (name))
		{
			Debug.LogError("input name is empty!");
			return;
		}
		PB.HSPlayerCreate requestParam = new PB.HSPlayerCreate ();

		requestParam.puid = GameDataMgr.Instance.UserDataAttr.guid;
		requestParam.nickname = name;
		requestParam.career = 0;
		requestParam.gender = 0;
		requestParam.eye = 0;
		requestParam.hair = 0;
		requestParam.hairColor = 0;

		GameApp.Instance.netManager.SendMessage (ProtocolMessage.Create (PB.code.PLAYER_CREATE_C.GetHashCode(), requestParam));

	}

	void OnRequestCreatePlayerFinished(ProtocolMessage msg)
	{
		PB.HSPlayerCreateRet response = msg.GetProtocolBody<PB.HSPlayerCreateRet> ();
		GameDataMgr.Instance.PlayerDataAttr.playerId = response.palyerID;

		GameEventMgr.Instance.FireEvent (GameEventList.CreatePlayerFinished);
	}
}
