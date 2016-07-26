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
        GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.SYNCINFO_C.GetHashCode().ToString(), OnRequestPlayerSyncInfoFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ASSEMBLE_FINISH_S.GetHashCode ().ToString (), OnRequestPlayerSyncInfoFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.SYNCINFO_S.GetHashCode().ToString (), OnRequestPlayerSyncInfoFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SYNCINFO_C.GetHashCode().ToString(), OnRequestPlayerSyncInfoFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ASSEMBLE_FINISH_S.GetHashCode().ToString(), OnRequestPlayerSyncInfoFinished);
	}

	void RequestPlayerData()
	{
		PB.HSSyncInfo param = new PB.HSSyncInfo ();
		param.deviceId = GameDataMgr.Instance.UserDataAttr.deviceID;
		param.platform = Const.platform;
		param.version = Const.versionName;

		GameApp.Instance.netManager.SendMessage (ProtocolMessage.Create (PB.code.SYNCINFO_C.GetHashCode (), param));
	}


	public override void OnInit(object param)
	{
		UIBuild uiBuild = UIMgr.Instance.OpenUI_(UIBuild.ViewName) as UIBuild;
        UIMgr.Instance.OpenUI_(UIIm.ViewName);
        if (param != null)
        {
            int initState = System.Convert.ToInt32(param);
            UIInstance uiInstance = uiBuild.OpenInstanceUI();
            switch (initState)
            {
                case 1:
                    {
                        EnterInstanceParam curInstance = BattleController.Instance.GetCurrentInstance();
                        if (curInstance != null)
                        {
                            uiInstance.OpenNextInstance(curInstance.instanceData.instanceId);
                        }
                    }
                    break;
                case 2:
                    {
                        EnterInstanceParam curInstance = BattleController.Instance.GetCurrentInstance();
                        if (curInstance != null)
                        {
                            uiInstance.ReOpenCurrentInstance(curInstance.instanceData.instanceId);
                        }
                    }
                    break;
            }
        }
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
        UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UIBuild.ViewName));
	}


	//net message
	void OnRequestPlayerSyncInfoFinished(ProtocolMessage msg)
	{

        if (msg.GetMessageType() == (int) PB.sys.ERROR_CODE)
        {
            UINetRequest.Close();
            return;
        }

		int msgType = msg.GetMessageType ();
		if (msgType == PB.code.SYNCINFO_S.GetHashCode ())
		{
			PB.HSSyncInfoRet  response = msg.GetProtocolBody<PB.HSSyncInfoRet>();
		}
		else if ( msgType == PB.code.ASSEMBLE_FINISH_S.GetHashCode() )
		{           
            UINetRequest.Close();

			//消息同步完成
			PB.HSAssembleFinish finishState = msg.GetProtocolBody<PB.HSAssembleFinish>();
            //GameDataMgr.Instance.PlayerDataAttr.InitMainUnitList();
			Debug.LogWarning("player info sync finished!");

            StatisticsDataMgr.Instance.BeginHeartBreak();
		}
	}
}


