using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BuildModule : ModuleBase 
{	
    public static bool needSyncInfo = false;

	void Start()
	{
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.SYNCINFO_S.GetHashCode().ToString (), OnRequestPlayerSyncInfoFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ASSEMBLE_FINISH_S.GetHashCode ().ToString (), OnRequestPlayerSyncInfoFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.SYNCINFO_S.GetHashCode().ToString (), OnRequestPlayerSyncInfoFinished);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ASSEMBLE_FINISH_S.GetHashCode ().ToString (), OnRequestPlayerSyncInfoFinished);
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
	
	public override void OnEnter(object param)
	{
		BindListener();
        if (needSyncInfo)
        {
            needSyncInfo = false;
            RequestPlayerData();
        }
	}
	
	public override void OnExecute()
	{
		
	}
	
	public override void OnExit()
	{
        UnBindListener();
        UIMgr.Instance.CloseUI(UIBuild.ViewName);
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
            //GameDataMgr.Instance.PlayerDataAttr.InitMainUnitList();
			//
			Debug.LogWarning("player info sync finished!");
		}
	}
}


