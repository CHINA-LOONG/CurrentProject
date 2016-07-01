using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGroup
{
    GameUnit[] enemyField = new GameUnit[BattleConst.maxFieldUnit];
    GameUnit[] playerField = new GameUnit[BattleConst.maxFieldUnit];
    List<GameUnit> enemyList = new List<GameUnit>();
    List<GameUnit> playerList = new List<GameUnit>();

    public void SetEnemyList(List<PbUnit> list)
    {
        enemyList.Clear();
        foreach (var item in list)
        {
            var unit = GameUnit.FromPb(item);
            unit.pbUnit.camp = UnitCamp.Enemy;
            //unit.Camp = UnitCamp.Enemy;
            if (item.slot > 0 && item.slot <= BattleConst.maxFieldUnit)
                enemyField[item.slot - 1] = unit;
            enemyList.Add(unit);
        }
    }

    public void SetPlayerList(List<PbUnit> list)
    {
        playerList.Clear();
        foreach (var item in list)
        {
            var unit = GameUnit.FromPb(item);
            unit.pbUnit.camp = UnitCamp.Player;
            //unit.Camp = UnitCamp.Player;
            if (item.slot > 0 && item.slot <= BattleConst.maxFieldUnit)
                playerField[item.slot - 1] = unit;
            playerList.Add(unit);
        }
    }

    public List<GameUnit> GetAllUnits()
    {
        List<GameUnit> all = new List<GameUnit>(enemyList);
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
            if (unit.pbUnit.guid == movedUnitId)
                unit.ReCalcSpeed();
            else
                unit.CalcSpeed();
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit.pbUnit.guid == movedUnitId)
                unit.ReCalcSpeed();
            else
                unit.CalcSpeed();
        }
    }

    public GameUnit GetNextMoveUnit()
    {
        GameUnit fastestUnit = null;
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

    public GameUnit GetUnitByGuid(int id)
    {
        foreach (var item in enemyList)
        {
            if (item.pbUnit.guid == id)
                return item;
        }

        foreach (var item in playerList)
        {
            if (item.pbUnit.guid == id)
                return item;
        }

        Logger.LogWarning("Battle Unit Not Found: " + id);
        return null;
    }

    public void OnUnitEnterField(GameUnit unit, int slot)
    {
        int fixedSlot = Mathf.Clamp(slot, BattleConst.slotIndexMin, BattleConst.slotIndexMax);
        if (fixedSlot != slot)
        {
            Logger.LogError("Slot[1,3] error:" + slot);
            return;
        }

        GameUnit[] field;
        field = (unit.pbUnit.camp == UnitCamp.Enemy) ? enemyField : playerField;
        //if (unit.Camp == UnitCamp.Enemy)
        //    field = enemyField;
        //else
        //    field = playerField;

        if (field[slot] != null)
        {
            OnUnitExitField(field[slot], slot);
        }

        unit.pbUnit.slot = slot;
        field[slot] = unit;
        unit.OnEnterField();
    }

    public void OnUnitExitField(GameUnit unit, int slot)
    {
        int fixedSlot = Mathf.Clamp(slot, BattleConst.slotIndexMin, BattleConst.slotIndexMax);
        if (fixedSlot != slot)
        {
            Logger.LogError("Slot[1,3] error:" + slot);
            return;
        }

        GameUnit[] field;
        if (unit.pbUnit.camp == UnitCamp.Enemy)
            field = enemyField;
        else
            field = playerField;

        if (field[slot] != null)
        {
            unit.pbUnit.slot = 0;
            field[slot] = null;
            unit.OnExitField();
        }
    }
}
