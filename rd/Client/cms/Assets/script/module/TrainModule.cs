using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TrainModule : ModuleBase 
{	
	bool mDataInit = true;
	GameObject mRoom;
	void BindListener()
	{
		
	}
	
	void UnBindListener()
	{
		
	}
	
	void Start()
	{
		
	
	}
	
	void RequestHeroData()
	{
		Dictionary<string,string> dict = new Dictionary<string, string> ();
		//dict.Add("heroid", GameDataMgr.Instance.PlayerDataAttr.Heroid);
		//NetMgr.Instance.SendNetMsg(NetMsgList.Net_GetPlayerData, OnGetHeroData,dict);
		
		//	NetMgr.Instance.SendNetMsg(NetMsgList.Net_GetHeroData, OnGetHeroData);
	}
	
	void OnGetHeroData(object param)
	{
		
	}

	void OnGUI()
	{
		if (mDataInit)
		{
			GUI.Label(new Rect(10, 10, 300, 50), "Name: " +  "tranModule");
			
		}
	}
	
	public override void OnInit(object param)
	{
		
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
	

}


