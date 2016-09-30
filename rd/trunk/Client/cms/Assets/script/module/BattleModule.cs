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

    //void Update()
    //{
    //    if (startBattle == true)
    //    {
    //        if (ResourceMgr.Instance.GetAssetRequestCount() == 0)
    //        {
    //            startBattle = false;
    //            StartCoroutine(FinishLoad());
    //        }
    //    }
    //}

    public override void OnInit(object param)
    {
        controller = gameObject.AddComponent<BattleController>();
        //process = gameObject.AddComponent<BattleProcess>();
        controller.Init();

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
        PvpFightParam pvpParam = null;
        if (enterParam == null)
        {
            pvpParam = param as PvpFightParam;
        }
        //pve
        if (enterParam != null)
        {
            UILoading loading = UIMgr.Instance.OpenUI_(UILoading.ViewName) as UILoading;
            if (loading != null)
            {
                UIMgr.Instance.FixBrokenWord();
                controller.StartBattlePrepare(enterParam);
                loading.SetLoadingCallback(LoadResourceFinish);
                loading.UpdateTotalAssetCount();
            }
        }
        else if (pvpParam != null)
        {
            UILoading loading = UIMgr.Instance.OpenUI_(UILoading.ViewName) as UILoading;
            if (loading != null)
            {
                UIMgr.Instance.FixBrokenWord();
                controller.StartBattlePvpPrepare(pvpParam);
                loading.SetLoadingCallback(LoadResourceFinish);
                loading.UpdateTotalAssetCount();
            }
        }
        UIIm.Instance.UpdateIMPos(true);
    }

    public override void OnExecute()
    {

    }

    public override IEnumerator LoadResourceFinish()
    {
        //wait for ui
        yield return new WaitForFixedUpdate();
        controller.StartBattle();
        UIMgr.Instance.CloseUI_(UILoading.ViewName);
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
        UIIm.Instance.UpdateIMPos(false);
    }

#region  Event
    //void OnShowBattleUI()
    //{
    //    var ui = UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
    //    ui.GetComponent<UIBattle>().Init();
    //}
#endregion
}
