using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginModule : ModuleBase 
{
	void Start()
	{
		
	}
	void BindListener()
	{
		GameEventMgr.Instance.AddListener (GameEventList.LoginClick, OnLoginClick);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.LOGIN_S.GetHashCode ().ToString(), OnNetLoginFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener (GameEventList.LoginClick, OnLoginClick);	
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.LOGIN_S.GetHashCode ().ToString (), OnNetLoginFinished);
	}

	void OnLoginClick()
	{
		PB.HSLogin hsLogin = new PB.HSLogin ();
		hsLogin.puid = GameDataMgr.Instance.UserDataAttr.guid;
		hsLogin.token = GameDataMgr.Instance.UserDataAttr.token;

		GameApp.Instance.netManager.SendMessage (PB.code.LOGIN_C.GetHashCode(), hsLogin);
	}

	void OnNetLogin(object param)
	{
		GameMain.Instance.ChangeModule<BuildModule>();
	}

	void OnNetLoginFinished(ProtocolMessage  msg)
	{
		if (msg.GetMessageType () != PB.code.LOGIN_S.GetHashCode ())
		{
			Debug.LogError("Error msgType " + msg.GetMessageType());
			return;
		}

        BuildModule.needSyncInfo = true;
		PB.HSLoginRet loginS = msg.GetProtocolBody<PB.HSLoginRet> ();
		UIMgr.Instance.CloseUI (UILogin.ViewName);
		if (loginS.playerId > 0) 
		{
			//GameDataMgr.Instance.PlayerDataAttr.playerId = loginS.playerId;
			GameMain.Instance.ChangeModule<BuildModule>();
		}
		else
		{
			GameMain.Instance.ChangeModule<CreatePlayerModule>();
        }
	}

	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI (UILogin.AssertName, UILogin.ViewName);
	}
	
	public override void OnEnter(ModuleBase prevModule, object param)
	{
		BindListener();
	}
	
	public override void OnExecute()
	{
		
	}
	
	public override void OnExit(ModuleBase nextModule)
	{
		UnBindListener();
	}	
}
