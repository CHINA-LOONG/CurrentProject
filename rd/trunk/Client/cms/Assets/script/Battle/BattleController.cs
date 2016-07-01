using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    int battleId;
    BattleData.RowData battleData;
    BattleProcess mProcess;
    BattleGroup battleGroup;

    //初值为-1
    int curProcessIndex = -1;

    // Use this for initialization
    public void Init(BattleProcess process)
    {
        mProcess = process;
        battleGroup = new BattleGroup();
    }

    public void StartBattle(PbStartBattle proto)
    {
        battleId = proto.battleId;
        battleData = StaticDataMgr.Instance.BattleData.getRowDataFromLevel(battleId);

        battleGroup.SetEnemyList(proto.enemyList);
        battleGroup.SetPlayerList(proto.playerList);

        CreateAllUnits();

        PlayPreStoryAnim();

        ShowUI();

        StartProcess();
    }

    void CreateAllUnits()
    {
        var all = battleGroup.GetAllUnits();
        foreach (var item in all)
        {
            CreateUnit(item);
        }
    }

    void CreateUnit(GameUnit unit)
    {
        Logger.LogFormat("[Battle]Create unit {0}", unit.name);
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
            mProcess.StartProcess(curProcess, battleGroup);
        else
            OnAllProcessOver();        
    }

    int GetNextProcess()
    {
        curProcessIndex++;

        if (curProcessIndex == 0 &&
            (battleData.processList==null || battleData.processList.Count == 0))
        {
            return 0;
        }

        if (curProcessIndex >= battleData.processList.Count)
            return -1;
        else
            return battleData.processList[curProcessIndex];
    }

    public void OnProcessOver()
    {
        StartProcess();
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
