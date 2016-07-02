using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleModule : ModuleBase
{
    BattleController controller;
    BattleProcess process;
	WeakPointController  weakPointController;

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
		string assetName = GameConfig.Instance.testBattleAssetName;
		BattleScene battleScene = gameObject.AddComponent<BattleScene> ();
		battleScene.InitWithBattleSceneName (assetName, battlePrefabName);

        controller = gameObject.AddComponent<BattleController>();
        process = gameObject.AddComponent<BattleProcess>();
        controller.Init(process);
        process.Init();

		weakPointController = gameObject.AddComponent<WeakPointController> ();
		weakPointController.Init ();
    }

    public override void OnEnter(ModuleBase prevModule, object param)
    {
        BindListener();

        UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
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
		Destroy (weakPointController);
    }

	void LateUpdate()
	{
        //raycast全部在battlecontroller的Update中处理

		/*if (Input.GetMouseButtonUp (0) && false) 
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
		}*/
	}
}
