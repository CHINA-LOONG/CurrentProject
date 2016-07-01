using UnityEngine;
using System.Collections;

public class GameApp : MonoBehaviour 
{
	void Start () 
	{
		Init();
		InitMgr();
	}

	void Init()
	{
		Application.targetFrameRate = 60;
		//Physics.gravity = GamePlayConfig.Instance.GameGravity;
		//Debuger.EnableLog = true;
	}

	void InitMgr()
	{
		//StaticDataMgr.Instance.Init ();
		GameDataMgr.Instance.Init();
		//NetMgr.Instance.Init();
		GameEventMgr.Instance.Init();
		//UIMgr.Instance.Init();
		GameMain.Instance.Init();
	}
}
