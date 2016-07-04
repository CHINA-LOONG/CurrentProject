using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGroup
{
    GameUnit[] enemyField = new GameUnit[BattleConst.maxFieldUnit];
    GameUnit[] playerField = new GameUnit[BattleConst.maxFieldUnit];
    List<GameUnit> enemyList = new List<GameUnit>();
    List<GameUnit> playerList = new List<GameUnit>();

    public List<GameUnit> EnemyFieldList
    {
        get { return new List<GameUnit>(enemyField); }
    }

    public List<GameUnit> PlayerFieldList
    {
        get { return new List<GameUnit>(playerField); }
    }

    public List<GameUnit> PlayerIdleList
    {
        get { return GetPlayerOffsiteUnits(); }
    }


    public void SetEnemyList(List<PbUnit> list)
    {
        enemyList.Clear();
        foreach (var item in list)
        {
            var unit = GameUnit.FromPb(item, false);
            unit.pbUnit.camp = UnitCamp.Enemy;
            //unit.Camp = UnitCamp.Enemy;
            if (item.slot >= BattleConst.slotIndexMin && item.slot <= BattleConst.slotIndexMax)
            {
                enemyField[item.slot] = unit;
                unit.OnEnterField();
            }
            enemyList.Add(unit);
        }
    }

    public void SetPlayerList(List<PbUnit> list)
    {
        playerList.Clear();
        foreach (var item in list)
        {
            var unit = GameUnit.FromPb(item, true);
            unit.pbUnit.camp = UnitCamp.Player;
            //unit.Camp = UnitCamp.Player;
            if (item.slot >= BattleConst.slotIndexMin && item.slot <= BattleConst.slotIndexMax)
            {
                playerField[item.slot] = unit;
                unit.OnEnterField();
            }
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
            if (unit != null)
            {
                if (unit.pbUnit.guid == movedUnitId)
                    unit.ReCalcSpeed();
                else
                    unit.CalcSpeed();
            }
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null)
            {
                if (unit.pbUnit.guid == movedUnitId)
                    unit.ReCalcSpeed();
                else
                    unit.CalcSpeed();
            }
        }
    }

    public GameUnit GetNextMoveUnit()
    {
        GameUnit fastestUnit = null;
        float fastestOrder = 10000;
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null && unit.curLife > 0 && unit.ActionOrder < fastestOrder)
            {
                fastestUnit = unit;
                fastestOrder = unit.ActionOrder;
            }
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null && unit.curLife > 0 && unit.ActionOrder < fastestOrder)
            {
                fastestUnit = unit;
                fastestOrder = unit.ActionOrder;
            }
        }

        return fastestUnit;
    }

    public bool IsAnySideDead()
    {
        return IsEnemyAllDead() || IsPlayerAllDead();
    }

    public bool IsEnemyAllDead()
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null && unit.curLife > 0)
                return false;
        }

        return true;
    }

    public bool IsPlayerAllDead()
    {
        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null && unit.curLife > 0)
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

    public GameUnit GetEnemyToField()
    {
        foreach (var item in enemyList)
        {
            if (item.pbUnit.slot == BattleConst.offsiteSlot && item.curLife > 0)
            {
                return item;
            }
        }
        return null;
    }

    public GameUnit GetPlayerToField()
    {
        var aviliableList = new List<GameUnit>();
        foreach (var item in playerList)
        {
            if (item.pbUnit.slot == BattleConst.offsiteSlot && item.curLife > 0)
            {
                aviliableList.Add(item);
            }
        }

        if (aviliableList.Count == 0)
            return null;
        else
            return aviliableList[Random.Range(0, aviliableList.Count)];
    }

    public List<GameUnit> GetPlayerOffsiteUnits()
    {
        List<GameUnit> result = new List<GameUnit>();
        foreach (var item in playerList)
        {
            if (item.pbUnit.slot == BattleConst.offsiteSlot)
            {
                result.Add(item);
            }
        }
        return result;
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

        //如果场上还有单位则替换
        if (field[slot] != null)
        {
            OnUnitExitField(field[slot], slot);
        }

        unit.pbUnit.slot = slot;
        field[slot] = unit;
        unit.OnEnterField();
    }

    public GameUnit RandomUnit(UnitCamp camp)
    {
        GameUnit unit = null;
        if (camp == UnitCamp.Enemy)
        {
            do
            {
                int index = Random.Range(0, 3);
                unit = enemyField[index];
            } while (unit == null);
        }
        else
        {
            do
            {
                int index = Random.Range(0, 3);
                unit = playerField[index];
            } while (unit == null);
        }

        return unit;
    }

    public void OnUnitExitField(GameUnit unit, int slot)
    {
        int fixedSlot = Mathf.Clamp(slot, BattleConst.slotIndexMin, BattleConst.slotIndexMax);
        if (fixedSlot != slot)
        {
            Logger.LogError("Slot[1,3] error:" + fixedSlot + " slot=" + slot);
            return;
        }

        GameUnit[] field;
        if (unit.pbUnit.camp == UnitCamp.Enemy)
            field = enemyField;
        else
            field = playerField;

        if (field[slot] != null)
        {
            unit.pbUnit.slot = BattleConst.offsiteSlot;
            field[slot] = null;
            unit.OnExitField();
        }
    }

    public void AllUnitsExitField()
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null)
            {
                OnUnitExitField(unit, unit.pbUnit.slot);
            }
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null)
            {
                OnUnitExitField(unit, unit.pbUnit.slot);
            }
        }
    }
}
