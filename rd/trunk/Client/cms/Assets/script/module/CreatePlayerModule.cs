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
	}
	
	void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<string>(GameEventList.createPlayerClick, OnCreatePlayerClick);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_CREATE_S.GetHashCode().ToString(), OnNetCreatePlayerFinished);
	}
	
	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI (UICreatePlayer.AssertName, UICreatePlayer.ViewName);
	}
	
	public override void OnEnter(ModuleBase prevModule, object param)
	{
		BindListener();
	}

    void OnCreatePlayerClick(string nickname)
    {
        Debug.Log("create button down");
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
        Debug.Log("create player finish");

        PB.HSPlayerCreateRet response = msg.GetProtocolBody<PB.HSPlayerCreateRet>();
        GameDataMgr.Instance.PlayerDataAttr.playerId = response.palyerID;
        UIMgr.Instance.CloseUI(UICreatePlayer.ViewName);
        GameMain.Instance.ChangeModule<BuildModule>();
    }

	public override void OnExecute()
	{
		
	}
	
	public override void OnExit(ModuleBase nextModule)
	{
		UnBindListener();
	}
}
