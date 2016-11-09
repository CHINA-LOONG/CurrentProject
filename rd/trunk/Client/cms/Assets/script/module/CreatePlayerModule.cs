using UnityEngine;
using System.Collections;

public class CreatePlayerModule : ModuleBase 
{
	void BindListener()
	{
        GameEventMgr.Instance.AddListener<string> (GameEventList.completePlayerClick, OnCompletePlayerClick);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_COMPLETE_S.GetHashCode().ToString(), OnNetCompletePlayerFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_COMPLETE_C.GetHashCode().ToString(), OnNetCompletePlayerFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.LOGIN_S.GetHashCode().ToString(), OnNetLoginFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.LOGIN_C.GetHashCode().ToString(), OnNetLoginFinished);
    }
	
	void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<string>(GameEventList.completePlayerClick, OnCompletePlayerClick);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_COMPLETE_S.GetHashCode().ToString(), OnNetCompletePlayerFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_COMPLETE_C.GetHashCode().ToString(), OnNetCompletePlayerFinished);
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

    void OnCompletePlayerClick(string nickname)
    {
        Logger.Log("create button down");
        PB.HSPlayerComplete requestParam = new PB.HSPlayerComplete();
        requestParam.nickname = nickname;
        requestParam.portraitId = 0;
        GameApp.Instance.netManager.SendMessage(ProtocolMessage.Create(PB.code.PLAYER_COMPLETE_C.GetHashCode(), requestParam));
    }

    void OnNetCompletePlayerFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int) PB.sys.ERROR_CODE)
        {
            Logger.LogError("完善账号失败");
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();

            if (error.errCode == (int)PB.PlayerError.NICKNAME_EXIST)
            {
                Logger.LogError("昵称重复");
            }

            return;
        }

        Logger.Log("complete player finish");

        UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UILoginDetail.ViewName));

        BuildModule.needSyncInfo = true;
        UIServer.ResetUserData(GameDataMgr.Instance.UserDataAttr.guid);
        PlayerPrefs.SetString("testGuid", GameDataMgr.Instance.UserDataAttr.guid);
      
       // GameDataMgr.Instance.PlayerDataAttr.playerId = response.palyerID;
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
