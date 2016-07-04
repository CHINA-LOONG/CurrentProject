using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BuildModule : ModuleBase 
{	
	void Start()
	{
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<PbStartBattle>(GameEventList.BattleBtnClick, OnBattleBtnClick);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.SYNCINFO_S.GetHashCode().ToString (), OnRequestPlayerSyncInfoFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ASSEMBLE_FINISH_S.GetHashCode ().ToString (), OnRequestPlayerSyncInfoFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<PbStartBattle>(GameEventList.BattleBtnClick, OnBattleBtnClick);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.SYNCINFO_S.GetHashCode().ToString (), OnRequestPlayerSyncInfoFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ASSEMBLE_FINISH_S.GetHashCode ().ToString (), OnRequestPlayerSyncInfoFinished);
	}

	private void OnBattleBtnClick(PbStartBattle proto)
	{
		GameMain.Instance.ChangeModule<BattleModule>();
		Logger.Log("Enter Battle");
		UIMgr.Instance.CloseUI(UIBuild.ViewName);
		GameEventMgr.Instance.FireEvent(GameEventList.StartBattle, proto);
	}


	void RequestPlayerData()
	{
		PB.HSSyncInfo param = new PB.HSSyncInfo ();
		param.deviceId = GameDataMgr.Instance.UserDataAttr.deviceID;
		param.platform = GameDataMgr.Instance.UserDataAttr.platform;
		param.version = GameDataMgr.Instance.UserDataAttr.version;

		GameApp.Instance.netManager.SendMessage (ProtocolMessage.Create (PB.code.SYNCINFO_C.GetHashCode (), param));
	}


	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI (UIBuild.AssertName, UIBuild.ViewName);

	}
	
	public override void OnEnter(ModuleBase prevModule, object param)
	{
		BindListener();
		RequestPlayerData ();
	}
	
	public override void OnExecute()
	{
		
	}
	
	public override void OnExit(ModuleBase nextModule)
	{
		UnBindListener();
	}


	//net message

	void OnRequestPlayerSyncInfoFinished(ProtocolMessage msg)
	{
		int msgType = msg.GetMessageType ();
		if (msgType == PB.code.SYNCINFO_S.GetHashCode ())
		{
			PB.HSSyncInfoRet  response = msg.GetProtocolBody<PB.HSSyncInfoRet>();
		}
		else if ( msgType == PB.code.ASSEMBLE_FINISH_S.GetHashCode() )
		{
			//消息同步完成
			PB.HSAssembleFinish finishState = msg.GetProtocolBody<PB.HSAssembleFinish>();
			//
			Debug.LogWarning("player info sync finished!");
		}
	}
}


