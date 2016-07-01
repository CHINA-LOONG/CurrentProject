using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BuildModule : ModuleBase 
{	
	void BindListener()
	{
        GameEventMgr.Instance.AddListener(GameEventList.BattleBtnClick, OnBattleBtnClick);
	}
	
	void UnBindListener()
	{
        GameEventMgr.Instance.RemoveListener(GameEventList.BattleBtnClick, OnBattleBtnClick);
	}
	
	void Start()
	{
		
	
	}
	
	void RequestHeroData()
	{
		//Dictionary<string,string> dict = new Dictionary<string, string> ();
		//dict.Add("heroid", GameDataMgr.Instance.PlayerDataAttr.Heroid);
		//NetMgr.Instance.SendNetMsg(NetMsgList.Net_GetPlayerData, OnGetHeroData,dict);
		
		//	NetMgr.Instance.SendNetMsg(NetMsgList.Net_GetHeroData, OnGetHeroData);
	}
	
	void OnGetHeroData(object param)
	{
		
	}
	

	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI (UIBuild.ViewName);
		string battlePrefabName = GameConfig.Instance.testBattlePrefab;

	}
	
	public override void OnEnter(ModuleBase prevModule, object param)
	{
		BindListener();
		RequestHeroData ();
	}
	
	public override void OnExecute()
	{
		
	}
	
	public override void OnExit(ModuleBase nextModule)
	{
		UnBindListener();
	}

    private void OnBattleBtnClick()
    {
        GameMain.Instance.ChangeModule<BattleModule>();
        UIMgr.Instance.CloseUI(UIBuild.ViewName);
    }
}


