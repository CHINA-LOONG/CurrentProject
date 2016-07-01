using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGroup
{
    BattleUnit[] enemyField = new BattleUnit[BattleConst.maxFieldUnit];
    BattleUnit[] playerField = new BattleUnit[BattleConst.maxFieldUnit];
    List<BattleUnit> enemyList = new List<BattleUnit>();
    List<BattleUnit> playerList = new List<BattleUnit>();

    public void SetEnemyList(List<PbBattleUnit> list)
    {
        enemyList.Clear();
        foreach (var item in list)
        {
            enemyList.Add(BattleUnit.FromPb(item));
        }
    }

    public void SetPlayerList(List<PbBattleUnit> list)
    {
        playerList.Clear();
        foreach (var item in list)
        {
            playerList.Add(BattleUnit.FromPb(item));
        }
    }

    public List<BattleUnit> GetAllUnits()
    {
        List<BattleUnit> all = new List<BattleUnit>(enemyList);
        all.AddRange(playerList);
        return all;
    }
}
