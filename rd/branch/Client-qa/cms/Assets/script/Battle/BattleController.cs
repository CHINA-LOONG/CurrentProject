using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    BattleProcess mProcess;
    BattleGroup battleGroup;

    //战斗进程
    List<PbBattleProcess> battleProcessList = new List<PbBattleProcess>();
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
        battleProcessList = proto.processList;
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

    void CreateUnit(BattleUnit unit)
    {
        Logger.LogFormat("[Battle]Create unit {0}", unit.Name);
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
        if (curProcess != null)
            mProcess.StartProcess(curProcess, battleGroup);
        else
            OnAllProcessOver();        
    }

    PbBattleProcess GetNextProcess()
    {
        curProcessIndex++;

        if (curProcessIndex >= battleProcessList.Count)
            return null;
        else
            return battleProcessList[curProcessIndex];
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
