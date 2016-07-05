using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGroup
{
    BattleObject[] enemyField = new BattleObject[BattleConst.maxFieldUnit];
    BattleObject[] playerField = new BattleObject[BattleConst.maxFieldUnit];
    List<BattleObject> enemyList = new List<BattleObject>();

    public List<BattleObject> EnemyFieldList
    {
        get { return new List<BattleObject>(enemyField); }
    }

    public List<BattleObject> PlayerFieldList
    {
        get { return new List<BattleObject>(playerField); }
    }

    public List<GameUnit> PlayerIdleList
    {
        get { return GetPlayerOffsiteUnits(); }
    }


    public void SetEnemyList(List<PbUnit> list)
    {
        enemyList.Clear();
        foreach (PbUnit pb in list)
        {
            GameUnit curUnit = GameUnit.FromPb(pb, false);
            BattleObject bo = ObjectDataMgr.Instance.CreateBattleObject(curUnit, null, Vector3.zero, Quaternion.identity);
            if (pb.slot >= BattleConst.slotIndexMin && pb.slot <= BattleConst.slotIndexMax)
            {
                enemyField[pb.slot] = bo;
                bo.OnEnterField();
            }
            else 
            {
                bo.OnExitField();
            }
            enemyList.Add(bo);
        }
    }

    public void SetPlayerList()
    {
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        var itor = playerUnitList.GetEnumerator();
        int slot = 0;
        while (itor.MoveNext())
        {
            itor.Current.unit.ResetAllState();
            if (slot >= BattleConst.slotIndexMin && slot <= BattleConst.slotIndexMax)
            {
                itor.Current.unit.pbUnit.slot = slot;
                playerField[slot] = itor.Current;
                itor.Current.OnEnterField();
            }
            else
            {
                itor.Current.unit.pbUnit.slot = BattleConst.offsiteSlot;
                itor.Current.OnExitField();
            }

            ++slot;
        }
    }

    //public List<GameUnit> GetAllUnits()
    //{
    //    List<GameUnit> all = new List<GameUnit>(enemyList);
    //    all.AddRange(playerList);
    //    return all;
    //}

    public void CalcActionOrder()
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i].unit;
            if (unit != null)
                unit.CalcSpeed();
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i].unit;
            if (unit != null)
                unit.CalcSpeed();
        }
    }

    public void ReCalcActionOrder(int movedUnitId)
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var bo = enemyField[i];
            if (bo != null)
            {
                if (bo.guid == movedUnitId)
                    bo.unit.ReCalcSpeed();
                else
                    bo.unit.CalcSpeed();
            }
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var bo = playerField[i];
            if (bo != null)
            {
                if (bo.guid == movedUnitId)
                    bo.unit.ReCalcSpeed();
                else
                    bo.unit.CalcSpeed();
            }
        }
    }

    public BattleObject GetNextMoveUnit()
    {
        BattleObject fastestUnit = null;
        float fastestOrder = 10000;
        for (int i = 0; i < enemyField.Length; i++)
        {
            var bo = enemyField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.ActionOrder < fastestOrder && bo.unit.isVisible)
            {
                fastestUnit = bo;
                fastestOrder = bo.unit.ActionOrder;
            }
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.ActionOrder < fastestOrder && bo.unit.isVisible)
            {
                fastestUnit = bo;
                fastestOrder = bo.unit.ActionOrder;
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
            var bo = enemyField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.isVisible)
			{
                return false;
			}
        }

        return true;
    }

    public bool IsPlayerAllDead()
    {
        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null && unit.unit.curLife > 0)
                return false;
        }

        return true;
    }

    public BattleObject GetEnemyToField()
    {
        foreach (BattleObject item in enemyList)
        {
            if (item.unit.pbUnit.slot == BattleConst.offsiteSlot && item.unit.curLife > 0)
            {
                return item;
            }
        }
        return null;
    }

    public BattleObject GetPlayerToField()
    {
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        BattleObject curObj = null;
        var itor = playerUnitList.GetEnumerator();
        while (itor.MoveNext())
        {
            curObj = itor.Current;
            int slot = curObj.unit.pbUnit.slot;
            if (slot == BattleConst.offsiteSlot &&
                curObj.unit.curLife > 0 &&
                curObj.unit.State != UnitState.ToBeEnter
               )
            {
                return itor.Current;
            }
        }

        return null;
    }

    public int GetEmptyPlayerSlot()
    {
        for (int i = 0; i < playerField.Length; i++)
        {
            if (playerField[i] == null || playerField[i].unit.State == UnitState.Dead)
                return i;
        }

        return BattleConst.maxFieldUnit;
    }

    public List<GameUnit> GetPlayerOffsiteUnits()
    {
        List<GameUnit> result = new List<GameUnit>();
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        var itor = playerUnitList.GetEnumerator();
        while (itor.MoveNext())
        {
            int slot = itor.Current.unit.pbUnit.slot;
            if (slot == BattleConst.offsiteSlot)
            {
                result.Add(itor.Current.unit);
            }
        }
        return result;
    }

    public void OnUnitEnterField(BattleObject bo, int slot)
    {
        int fixedSlot = Mathf.Clamp(slot, BattleConst.slotIndexMin, BattleConst.slotIndexMax);
        if (fixedSlot != slot)
        {
            Logger.LogError("Slot[1,3] error:" + slot);
            return;
        }

        BattleObject[] field;
        field = (bo.camp == UnitCamp.Enemy) ? enemyField : playerField;

        //如果场上还有单位则替换
        if (field[slot] != null)
        {
            OnUnitExitField(field[slot], slot);
        }

        bo.unit.pbUnit.slot = slot;
        field[slot] = bo;
        bo.OnEnterField();
        BattleController.Instance.GetUIBattle().ShowUnitUI(bo, slot);
    }

    public GameUnit RandomUnit(UnitCamp camp)
    {
        BattleObject bo = null;
        if (camp == UnitCamp.Enemy)
        {
            do
            {
                int index = Random.Range(0, 3);
                bo = enemyField[index];
            } while (bo == null);
        }
        else
        {
            do
            {
                int index = Random.Range(0, 3);
                bo = playerField[index];
            } while (bo == null);
        }

        return bo.unit;
    }

    public void OnUnitExitField(BattleObject bo, int slot)
    {
        int fixedSlot = Mathf.Clamp(slot, BattleConst.slotIndexMin, BattleConst.slotIndexMax);
        if (fixedSlot != slot)
        {
            Logger.LogError("Slot[1,3] error:" + fixedSlot + " slot=" + slot);
            return;
        }

        BattleController.Instance.GetUIBattle().HideUnitUI(bo.guid);
        BattleObject[] field;
        if (bo.camp == UnitCamp.Enemy)
            field = enemyField;
        else
            field = playerField;

        if (field[slot] != null)
        {
            bo.unit.pbUnit.slot = BattleConst.offsiteSlot;
            field[slot] = null;
            if (bo.unit.State != UnitState.Dead)
                bo.OnExitField();
        }
    }

    public void AllUnitsExitField()
    {
        for (int i = 0; i < enemyField.Length; i++)
        {
            var unit = enemyField[i];
            if (unit != null)
            {
                OnUnitExitField(unit, unit.unit.pbUnit.slot);
            }
        }

        for (int i = 0; i < playerField.Length; i++)
        {
            var unit = playerField[i];
            if (unit != null)
            {
                OnUnitExitField(unit, unit.unit.pbUnit.slot);
            }
        }
    }

    public void DestroyEnemys()
    {
        foreach (BattleObject item in enemyList)
        {
            ObjectDataMgr.Instance.RemoveBattleObject(item.guid);
        }
    }
}
