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
        ///BattleCamera.Instance.Init();
    }

    public override void OnInit(object param)
    {
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
}
