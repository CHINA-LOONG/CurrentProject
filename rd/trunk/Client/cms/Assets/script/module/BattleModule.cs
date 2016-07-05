using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleModule : ModuleBase
{
    BattleController controller;
    //BattleProcess process;
	WeakPointController  weakPointController;
    BattleUnitAi battleUnitAi;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<PbStartBattle>(GameEventList.StartBattle, controller.StartBattle);
        //GameEventMgr.Instance.AddListener(GameEventList.ShowBattleUI, OnShowBattleUI);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<PbStartBattle>(GameEventList.StartBattle, controller.StartBattle);
        //GameEventMgr.Instance.RemoveListener(GameEventList.ShowBattleUI, OnShowBattleUI);
    }

    void Start()
    {
        BattleCamera.Instance.Init();
    }

    public override void OnInit(object param)
    {
		string battlePrefabName = GameConfig.Instance.testBattlePrefab;
		string assetName = GameConfig.Instance.testBattleAssetName;

        controller = gameObject.AddComponent<BattleController>();
        //process = gameObject.AddComponent<BattleProcess>();
        controller.Init();
        //process.Init();

		weakPointController = gameObject.AddComponent<WeakPointController> ();
		weakPointController.Init ();

        battleUnitAi = gameObject.AddComponent<BattleUnitAi>();
		battleUnitAi.Init ();
    }

    public override void OnEnter(ModuleBase prevModule, object param)
    {
        BindListener();
        //var ui = UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
        //ui.GetComponent<UIBattle>().Init();
    }

    public override void OnExecute()
    {

    }

    public override void OnExit(ModuleBase nextModule)
    {
        UnBindListener();
        Destroy(controller);
        //Destroy(process);
        Destroy(weakPointController);
        Destroy(battleUnitAi);
        //UIMgr.Instance.CloseUI(UIBattle.ViewName);
    }

#region  Event
    //void OnShowBattleUI()
    //{
    //    var ui = UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
    //    ui.GetComponent<UIBattle>().Init();
    //}
#endregion
}
