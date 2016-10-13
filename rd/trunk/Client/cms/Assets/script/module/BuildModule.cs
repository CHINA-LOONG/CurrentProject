using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BuildModule : ModuleBase 
{	
    public static bool needSyncInfo = false;

    public int CurrentInitState
    {
        get { return mCurrentInitState; }
    }
    public bool isSummon = false;
    private int mCurrentInitState = -1;
    private object mParam = null;

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.SYNCINFO_S.GetHashCode().ToString (), OnRequestPlayerSyncInfoFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.SYNCINFO_C.GetHashCode().ToString(), OnRequestPlayerSyncInfoFinished);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ASSEMBLE_FINISH_S.GetHashCode ().ToString (), OnRequestPlayerSyncInfoFinished);
        GameEventMgr.Instance.AddListener(GameEventList.LogoutClick, OnLogoutClick);
    }
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.SYNCINFO_S.GetHashCode().ToString (), OnRequestPlayerSyncInfoFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SYNCINFO_C.GetHashCode().ToString(), OnRequestPlayerSyncInfoFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ASSEMBLE_FINISH_S.GetHashCode().ToString(), OnRequestPlayerSyncInfoFinished);
        GameEventMgr.Instance.RemoveListener(GameEventList.LogoutClick, OnLogoutClick);
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
        mParam = param;
    }
    public override void OnExecute()
    {

    }

    public override void OnEnter(object param)
	{
		BindListener();
        if (needSyncInfo)
        {
            needSyncInfo = false;
            RequestPlayerData();
        }
		//ResourceMgr.Instance.LoadLevelAsyn("mainstage", false, null);
		UILoading loading = UIMgr.Instance.OpenUI_(UILoading.ViewName) as UILoading;
        if (GameDataMgr.Instance.loadingNum == 0)
		{			
			loading.SetLoading(LoadingType.loadingLog);
            GameDataMgr.Instance.loadingNum++;
		}
       else 
		{
			loading.SetLoading(LoadingType.loadingDefault);
		}
        if (loading != null)
        {
            UIMgr.Instance.FixBrokenWord();
            //add resource request
            AddResRequest();
            loading.SetLoadingCallback(LoadResourceFinish);
            loading.UpdateTotalAssetCount();
        }
    }
    private void AddResRequest()
    {
        ResourceMgr resMgr = ResourceMgr.Instance;
        resMgr.AddAssetRequest(new AssetRequest(UIBuild.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIQuest.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIMail.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIInstance.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIMonsters.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIPetDetails.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIMonsterCompose.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIAdjustBattleTeam.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIShop.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIStore.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UICompose.ViewName));
        resMgr.AddAssetRequest(new AssetRequest(UIDecompose.ViewName));

    }
    public override IEnumerator LoadResourceFinish()
    {
        ResourceMgr.Instance.LoadLevelAsyn("mainstage", false, null);
        UIMgr.Instance.ClearUILayerList();
        //wait for ui
        yield return new WaitForFixedUpdate();
        UIMgr.Instance.CloseUI_(UILoading.ViewName);

        UIBuild uiBuild = UIMgr.Instance.OpenUI_(UIBuild.ViewName) as UIBuild;
        UIIm imUI = UIMgr.Instance.OpenUI_(UIIm.ViewName) as UIIm;
        //播放循环的主界面背景音乐
        AudioSystemMgr.Instance.PlayMusicByName("Homemusic");
        imUI.transform.SetAsLastSibling();
        if (mParam != null)
        {
            mCurrentInitState = System.Convert.ToInt32(mParam);
            int curInstanceType = GameDataMgr.Instance.curInstanceType;
            //exit from normal instance
            if (curInstanceType == (int)InstanceType.Normal)
            {
                switch (mCurrentInitState)
                {
                    case (int)ExitInstanceType.Exit_Instance_Next:
                        {
                            InstanceMap uiInstance = uiBuild.OpenInstanceUI();
                            EnterInstanceParam curInstance = BattleController.Instance.GetCurrentInstance();
                            if (curInstance != null)
                            {
                                uiInstance.OpenNextInstance(curInstance.instanceData.instanceId);
                            }
                        }
                        break;
                    case (int)ExitInstanceType.Exit_Instance_Retry:
                        {
                            InstanceMap uiInstance = uiBuild.OpenInstanceUI();
                            EnterInstanceParam curInstance = BattleController.Instance.GetCurrentInstance();
                            if (curInstance != null)
                            {
                                uiInstance.ReOpenCurrentInstance(curInstance.instanceData.instanceId);
                            }
                        }
                        break;
                    case (int)ExitInstanceType.Exit_Instance_OK:
                        {
                            InstanceMap uiInstance = uiBuild.OpenInstanceUI();
                            EnterInstanceParam curInstance = BattleController.Instance.GetCurrentInstance();
                            if (curInstance != null)
                            {
                                InstanceMap.Instance.FocusOnChapterButton(curInstance.instanceData.instanceId);
                                GameEventMgr.Instance.FireEvent<string>(GameEventList.ShowInstanceList, curInstance.instanceData.instanceId);
                            }
                        }
                        break;
                    case (int)ExitInstanceType.Exit_Instance_Summon:
                        {
                            isSummon = true;
                        }
                        break;
                    case (int)ExitInstanceType.Exit_Instance_Pet:
                        {
                            EnterInstanceParam curInstance = BattleController.Instance.GetCurrentInstance();
                            if (curInstance != null)
                            {
                                FoundMgr.Instance.GoToMonster(0);
                            }
                        }
                        break;
                    case (int)ExitInstanceType.Exit_Instance_PVP:
                        {
                            PvpMain.Open();
                        }
                        break;
                }
            }
            else if (curInstanceType == (int)InstanceType.Guild)
            {
                GameDataMgr.Instance.SociatyDataMgrAttr.OpenSociatyTaskWithTeam(SociatyTaskContenType.MyTeam);
            }
        }
    }
	
	public override void OnExit()
	{
        UnBindListener();
        UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UIBuild.ViewName));
	}

    public void OnLogoutClick()
    {
        GameMain.Instance.ChangeModule<LoginModule>();
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
            Logger.LogWarning("player info sync begin!");
		}
		else if ( msgType == PB.code.ASSEMBLE_FINISH_S.GetHashCode() )
		{           
            UINetRequest.Close();

			//消息同步完成
			PB.HSAssembleFinish finishState = msg.GetProtocolBody<PB.HSAssembleFinish>();
            //GameDataMgr.Instance.PlayerDataAttr.InitMainUnitList();
			Logger.LogWarning("player info sync finished!");
            GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = finishState.allianceID;
            GameDataMgr.Instance.PlayerDataAttr.GonghuiCoinAttr = finishState.contribution;
            StatisticsDataMgr.Instance.BeginHeartBreak();
		}
	}
}


