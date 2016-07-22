using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleModule : ModuleBase
{
    BattleController controller;
    //BattleProcess process;
	WeakPointController  weakPointController;
    BattleUnitAi battleUnitAi;
	PhyDazhaoController phyDazhaoController;
	MagicDazhaoController magicDazhaoController;
    bool startBattle = false;

    void BindListener()
    {
        //GameEventMgr.Instance.AddListener<PbStartBattle>(GameEventList.StartBattle, controller.StartBattle);
        //GameEventMgr.Instance.AddListener(GameEventList.ShowBattleUI, OnShowBattleUI);
    }

    void UnBindListener()
    {
        //GameEventMgr.Instance.RemoveListener<PbStartBattle>(GameEventList.StartBattle, controller.StartBattle);
        //GameEventMgr.Instance.RemoveListener(GameEventList.ShowBattleUI, OnShowBattleUI);
    }

    void Start()
    {
        BattleCamera.Instance.Init();
    }

    void Update()
    {
        if (startBattle == true)
        {
            if (ResourceMgr.Instance.GetAssetRequestCount() == 0)
            {
                startBattle = false;
                StartCoroutine(FinishLoad());
            }
        }
    }

    IEnumerator FinishLoad()
    {
        //wait for ui
        yield return new WaitForFixedUpdate();
        controller.StartBattle();
    }

    public override void OnInit(object param)
    {
        controller = gameObject.AddComponent<BattleController>();
        //process = gameObject.AddComponent<BattleProcess>();
        controller.Init();
        //process.Init();

		weakPointController = gameObject.AddComponent<WeakPointController> ();
		weakPointController.Init ();

        battleUnitAi = gameObject.AddComponent<BattleUnitAi>();
		battleUnitAi.Init ();

		phyDazhaoController = gameObject.AddComponent<PhyDazhaoController> ();

		magicDazhaoController = gameObject.AddComponent<MagicDazhaoController> ();
		
    }

    public override void OnEnter(object param)
    {
        BindListener();
        EnterInstanceParam enterParam = param as EnterInstanceParam;
        if (enterParam != null)
        {
            //StartCoroutine(LoadResource());
            controller.StartBattlePrepare(enterParam);
            UILoading loading = UIMgr.Instance.GetUI(UILoading.ViewName) as UILoading;
            if (loading)
            {
                loading.UpdateTotalAssetCount();
            }
        }
        //var ui = UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
        //ui.GetComponent<UIBattle>().Init();
        startBattle = true;
    }

    public override void OnExecute()
    {

    }

    public override IEnumerator LoadResource()
    {
        //ResourceMgr.Instance.loa
        yield return null;
    }

    public override void OnExit()
    {
        UnBindListener();
        Destroy(controller);
        //Destroy(process);
        Destroy(weakPointController);
        Destroy(battleUnitAi);
        phyDazhaoController.DestroyController();
        magicDazhaoController.DestroyController();
		Destroy (phyDazhaoController);
		Destroy (magicDazhaoController);
        //UIMgr.Instance.CloseUI(UIBattle.ViewName);
        //destroy battle camera manual,since camera may attach to gamemain throw ani
        Destroy(BattleCamera.Instance.gameObject);
        startBattle = false;
    }

#region  Event
    //void OnShowBattleUI()
    //{
    //    var ui = UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
    //    ui.GetComponent<UIBattle>().Init();
    //}
#endregion
}
