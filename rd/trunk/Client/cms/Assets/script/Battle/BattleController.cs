using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleController : MonoBehaviour
{
    string battleId;
    BattleData battleData;
    BattleProcess process;
    BattleGroup battleGroup;
	bool	isMouseOnUI = false;
    public BattleGroup BattleGroup
    {
        get { return battleGroup; }
    }

    //初值为-1
    int curProcessIndex = -1;

    static BattleController instance;
    public static BattleController Instance
    {
        get { return instance; }
    }


    // Use this for initialization
    public void Init(BattleProcess process)
    {
        instance = this;
        this.process = process;
        battleGroup = new BattleGroup();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = BattleCamera.Instance.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		if (Input.GetMouseButtonDown (0)) 
		{
			isMouseOnUI = EventSystem.current.IsPointerOverGameObject ();
		}
		
		if (Input.GetMouseButtonUp(0) && !isMouseOnUI )
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                var battleGo = hit.collider.gameObject.GetComponent<BattleObject>();
                if (battleGo)
                {
                    OnHitBattleObject(battleGo);
                } 
                else
                {
                    Logger.LogWarning("Hit something but not a battle object!");
                }
            }
            else
            {
                GameEventMgr.Instance.FireEvent(GameEventList.HideSwitchPetUI);
            }
        }        
    }

    void OnHitBattleObject(BattleObject battleGo)
    {
        if (battleGo.camp == UnitCamp.Enemy)
        {
            //设置集火目标
            Logger.Log("Set hit target to " + battleGo.id);
            process.OnChangeTarget(battleGo.id);
        }
        else if (battleGo.camp == UnitCamp.Player)
        {
            if (process.CanSwitchPet())
            {
                //换宠
                ShowSwitchPetUIArgs args = new ShowSwitchPetUIArgs();
                args.targetId = battleGo.id;
                args.idleUnits = battleGroup.GetPlayerOffsiteUnits();
                GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.ShowSwitchPetUI, args);
            }            
        }
    }

    public void StartBattle(PbStartBattle proto)
    {
        battleId = proto.battleId;
        battleData = StaticDataMgr.Instance.GetBattleDataFromLevel(battleId);

        //设置battlegroup 并且创建模型
        battleGroup.SetEnemyList(proto.enemyList);
        battleGroup.SetPlayerList(proto.playerList);

        PlayPreStoryAnim();

        ShowUI();

        StartProcess();
    }

    void PlayPreStoryAnim()
    {
        Logger.Log("[Battle]Play Story Animation before any process...");
    }

    void ShowUI()
    {
        GameEventMgr.Instance.FireEvent(GameEventList.ShowBattleUI);
    }

    void StartProcess()
    {
        var curProcess = GetNextProcess();
        if (curProcess != null)
            process.StartProcess(curProcess, battleGroup);
        else
            OnAllProcessOver();        
    }

    ProcessData GetNextProcess()
    {
        curProcessIndex++;

        if (curProcessIndex == 0 &&
            battleData.processList.Count == 0)
        {
            //小怪进程
            return new ProcessData();
        }

        if (curProcessIndex >= battleData.processList.Count)
            return null;
        else
            return null;
    }

    public void OnProcessSuccess()
    {
        StartProcess();
    }

    public void OnProcessFailed()
    {
        OnAllProcessOver();
    }

    public void OnAllProcessOver()
    {
        //胜利失败动画
        PlayBalanceAnim();
        //后置剧情动画
        PlayPostStoryAnim();
        //结算面板UI
        ShowBalanceUI();
    }

    private void PlayBalanceAnim()
    {
        Logger.Log("[Battle]Playing Balance Anim...");
    }

    private void PlayPostStoryAnim()
    {
        Logger.Log("[Battle]Playing Post Story Anim...");
    }

    private void ShowBalanceUI()
    {
        Logger.Log("[Battle]Showing Balance UI...");
    }

    public void OnUnitCastDazhao(GameUnit unit)
    {

    }
}
