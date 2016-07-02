using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginModule : ModuleBase 
{
	void Start()
	{
		
	}
	void BindListener()
	{
		GameEventMgr.Instance.AddListener (GameEventList.LoginClick, OnLoginClick);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener (GameEventList.LoginClick, OnLoginClick);		
	}

	void	OnLoginClick()
	{
		GameMain.Instance.ChangeModule<BuildModule>();
		UIMgr.Instance.CloseUI (UILogin.ViewName);
	}

	void OnNetLogin(object param)
	{
		GameMain.Instance.ChangeModule<BuildModule>();
	}
	
	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI (UILogin.AssertName, UILogin.ViewName);
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
