using UnityEngine;
using System.Collections;

public class CreatePlayerModule : ModuleBase 
{

	void Start()
	{
	}

	void BindListener()
	{
        GameEventMgr.Instance.AddListener<string> (GameEventList.createPlayerClick, OnCreatePlayerClick);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_CREATE_S.GetHashCode().ToString(), OnNetCreatePlayerFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_CREATE_C.GetHashCode().ToString(), OnNetCreatePlayerFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.LOGIN_S.GetHashCode().ToString(), OnNetLoginFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.LOGIN_C.GetHashCode().ToString(), OnNetLoginFinished);
    }
	
	void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<string>(GameEventList.createPlayerClick, OnCreatePlayerClick);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_CREATE_S.GetHashCode().ToString(), OnNetCreatePlayerFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_CREATE_C.GetHashCode().ToString(), OnNetCreatePlayerFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.LOGIN_S.GetHashCode().ToString(), OnNetLoginFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.LOGIN_C.GetHashCode().ToString(), OnNetLoginFinished);
    }
	
	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI_(UILoginDetail.ViewName);
	}
	
	public override void OnEnter(object param)
	{
		BindListener();
	}

    void OnCreatePlayerClick(string nickname)
    {
        Logger.Log("create button down");
        PB.HSPlayerCreate requestParam = new PB.HSPlayerCreate();
        requestParam.puid = GameDataMgr.Instance.UserDataAttr.guid;
        requestParam.nickname = nickname;
        requestParam.career = 0;
        requestParam.gender = 0;
        requestParam.eye = 0;
        requestParam.hair = 0;
        requestParam.hairColor = 0;
        GameApp.Instance.netManager.SendMessage(ProtocolMessage.Create(PB.code.PLAYER_CREATE_C.GetHashCode(), requestParam));
    }

    void OnNetCreatePlayerFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int) PB.sys.ERROR_CODE)
        {
            Logger.LogError("创建账号失败");
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();

            if (error.errCode == (int)PB.PlayerError.PLAYER_NICKNAME_EXIST)
            {
                Logger.LogError("昵称重复");
            }
            else if (error.errCode == (int)PB.PlayerError.PUID_EXIST)
            {
                Logger.LogError("账号已存在");
            }

            return;
        }

        Logger.Log("create player finish");
        BuildModule.needSyncInfo = true;
        UIServer.ResetUserData(GameDataMgr.Instance.UserDataAttr.guid);
        PlayerPrefs.SetString("testGuid", GameDataMgr.Instance.UserDataAttr.guid);       
        PB.HSPlayerCreateRet response = msg.GetProtocolBody<PB.HSPlayerCreateRet>();
        GameDataMgr.Instance.PlayerDataAttr.playerId = response.palyerID;
        UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UILoginDetail.ViewName));
        GameMain.Instance.ChangeModule<BuildModule>();
    }

    void OnNetLoginFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }

        BuildModule.needSyncInfo = true;
        PB.HSLoginRet loginS = msg.GetProtocolBody<PB.HSLoginRet>();   
        if (loginS.playerId > 0)
        {
            UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UILoginDetail.ViewName));
            GameDataMgr.Instance.PlayerDataAttr.playerId = loginS.playerId;
            GameMain.Instance.ChangeModule<BuildModule>();
            UIServer.ResetUserData(GameDataMgr.Instance.UserDataAttr.guid);
            PlayerPrefs.SetString("testGuid",GameDataMgr.Instance.UserDataAttr.guid);
        }
        else
        {
            Logger.Log("账号不存在");
        }
    }

	public override void OnExecute()
	{
		
	}
	
	public override void OnExit()
	{
		UnBindListener();
	}
}
