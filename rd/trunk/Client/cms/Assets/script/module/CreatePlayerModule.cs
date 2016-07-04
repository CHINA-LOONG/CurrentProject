using UnityEngine;
using System.Collections;

public class CreatePlayerModule : ModuleBase 
{

	void Start()
	{
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener (GameEventList.CreatePlayerFinished, OnCreatePlayerFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener (GameEventList.CreatePlayerFinished, OnCreatePlayerFinished);
	}
	
	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI (UICreatePlayer.AssertName, UICreatePlayer.ViewName);
	}
	
	public override void OnEnter(ModuleBase prevModule, object param)
	{
		BindListener();
	}
		
	void OnCreatePlayerFinished()
	{
		UIMgr.Instance.CloseUI (UICreatePlayer.ViewName);
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
