using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    int battleId;
    BattleData.RowData battleData;
    BattleProcess process;
    BattleGroup battleGroup;
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
        if (Input.GetMouseButtonUp(0))
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
        battleData = StaticDataMgr.Instance.BattleData.getRowDataFromLevel(battleId);

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
        Logger.Log("[Battle]Show Battle UI...");
    }

    void StartProcess()
    {
        var curProcess = GetNextProcess();
        if (curProcess != -1)
            process.StartProcess(curProcess, battleGroup);
        else
            OnAllProcessOver();        
    }

    int GetNextProcess()
    {
        curProcessIndex++;

        if (curProcessIndex == 0 &&
            battleData.processList.Count == 0)
        {
            //小怪进程
            return 0;
        }

        if (curProcessIndex >= battleData.processList.Count)
            return -1;
        else
            return battleData.processList[curProcessIndex];
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

}
