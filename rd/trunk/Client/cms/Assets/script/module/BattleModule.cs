using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleModule : ModuleBase
{
    BattleController controller;
    BattleProcess process;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<PbStartBattle>(GameEventList.StartBattle, controller.StartBattle);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<PbStartBattle>(GameEventList.StartBattle, controller.StartBattle);
    }

    void Start()
    {
        BattleCamera.Instance.Init();
    }

    public override void OnInit(object param)
    {
		string battlePrefabName = GameConfig.Instance.testBattlePrefab;
		BattleScene battleScene = gameObject.AddComponent<BattleScene> ();
		battleScene.InitWithBattleSceneName (battlePrefabName);

        controller = gameObject.AddComponent<BattleController>();
        process = gameObject.AddComponent<BattleProcess>();
        controller.Init(process);
        process.Init();
    }

    public override void OnEnter(ModuleBase prevModule, object param)
    {
        BindListener();

        UIMgr.Instance.OpenUI(UIBattle.ViewName);
        Logger.Log("Enter Battle");
        BattleTest.Test();
    }

    public override void OnExecute()
    {

    }

    public override void OnExit(ModuleBase nextModule)
    {
        UnBindListener();
        Destroy(controller);
        Destroy(process);
    }

	void LateUpdate()
	{
		if (Input.GetMouseButtonUp (0)) 
		{
			Ray r = BattleCamera.Instance.CameraAttr.ScreenPointToRay(Input.mousePosition);
			RaycastHit rh;
			if (Physics.Raycast(r, out rh))
			{
				//test 
				MirrorTarget target = rh.collider.GetComponent<MirrorTarget>();
				if (target != null)
				{
					GameEventMgr.Instance.FireEvent<int>(GameEventList.ShowSwitchPetUI,0);
				}
				else
				{
					GameEventMgr.Instance.FireEvent(GameEventList.HideSwitchPetUI);
				}
			}
			else
			{
				GameEventMgr.Instance.FireEvent(GameEventList.HideSwitchPetUI);
			}
		}
	}
}
