using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleModule : ModuleBase 
{


	void BindListener()
	{
		//GameEventMgr.Instance.AddListener(GameEventList.RestartGame, OnRestartGame);
		//GameEventMgr.Instance.AddListener<float>(GameEventList.LifeDataChanged, OnLifeDataChanged)
	}

	void UnBindListener()
	{
		//GameEventMgr.Instance.RemoveListener(GameEventList.RestartGame, OnRestartGame);
		//GameEventMgr.Instance.RemoveListener<float>(GameEventList.LifeDataChanged, OnLifeDataChanged);
	}

	void Start()
	{
		///BattleCamera.Instance.Init();
		BindListener();
	}

	

	public override void OnInit(object param)
	{
		
	}
	
	public override void OnEnter(ModuleBase prevModule, object param)
	{

	}
	
	public override void OnExecute()
	{
		
	}
	
	public override void OnExit(ModuleBase nextModule)
	{
		UnBindListener();
	}	
}
