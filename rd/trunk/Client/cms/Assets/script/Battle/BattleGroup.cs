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
            var unit = BattleUnit.FromPb(item);
            if (item.slot > 0 && item.slot <= BattleConst.maxFieldUnit)
                enemyField[item.slot - 1] = unit;
            enemyList.Add(unit);
        }
    }

    public void SetPlayerList(List<PbBattleUnit> list)
    {
        playerList.Clear();
        foreach (var item in list)
        {
            var unit = BattleUnit.FromPb(item);
            if (item.slot > 0 && item.slot <= BattleConst.maxFieldUnit)
                playerField[item.slot - 1] = unit;
            playerList.Add(unit);
        }
    }

    public List<BattleUnit> GetAllUnits()
    {
        List<BattleUnit> all = new List<BattleUnit>(enemyList);
        all.AddRange(playerList);
        return all;
    }

    public void CalcActionOrder()
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null)
                unit.CalcSpeed();
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null)
                unit.CalcSpeed();
        }
    }

    public void ReCalcActionOrder(int movedUnitId)
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit.Guid == movedUnitId)
                unit.ReCalcSpeed();
            else
                unit.CalcSpeed();
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit.Guid == movedUnitId)
                unit.ReCalcSpeed();
            else
                unit.CalcSpeed();
        }
    }

    public BattleUnit GetNextMoveUnit()
    {
        BattleUnit fastestUnit = null;
        float fastestOrder = 10000;
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null && unit.ActionOrder < fastestOrder)
            {
                fastestUnit = unit;
                fastestOrder = unit.ActionOrder;
            }
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null && unit.ActionOrder < fastestOrder)
            {
                fastestUnit = unit;
                fastestOrder = unit.ActionOrder;
            }
        }

        return fastestUnit;
    }

    public bool IsAnySideDead()
    {
        return IsEnemyAllDead() && IsPlayerAllDead();
    }

    public bool IsEnemyAllDead()
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null)
                return false;
        }

        return true;
    }

    public bool IsPlayerAllDead()
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null)
                return false;
        }

        return true;
    }
}
