using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginModule : ModuleBase 
{
	void BindListener()
	{
		
	}
	
	void UnBindListener()
	{
		
	}
	
	void Start()
	{
		
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(100, 100, 200, 50), "Login!"))
		{
			Dictionary<string,string> dict = new Dictionary<string, string>();
			dict.Add("pid",SystemInfo.deviceUniqueIdentifier);
			
			//NetMgr.Instance.SendNetMsg(NetMsgList.Net_Login, OnNetLogin, dict);
			GameMain.Instance.ChangeModule<TrainModule>();
		}
	}
	
	void OnNetLogin(object param)
	{
		
		GameMain.Instance.ChangeModule<TrainModule>();
		
		
	}
	
	public override void OnInit(object param)
	{
		
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
