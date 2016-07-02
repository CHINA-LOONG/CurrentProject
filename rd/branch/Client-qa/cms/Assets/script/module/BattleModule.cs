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
        GameEventMgr.Instance.AddListener(GameEventList.ShowBattleUI, OnShowBattleUI);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<PbStartBattle>(GameEventList.StartBattle, controller.StartBattle);
        GameEventMgr.Instance.RemoveListener(GameEventList.ShowBattleUI, OnShowBattleUI);
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

		BattleUnitAi battleUnitAi = gameObject.AddComponent<BattleUnitAi> ();
		battleUnitAi.Init ();
    }

    public override void OnEnter(ModuleBase prevModule, object param)
    {
        BindListener();

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

#region  Event
    void OnShowBattleUI()
    {
        var ui = UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
        ui.GetComponent<UIBattle>().Init();
    }
#endregion
}
