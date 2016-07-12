using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGroup
{
    //BattleObject[] enemyField = new BattleObject[BattleConst.maxFieldUnit];
    //BattleObject[] playerField = new BattleObject[BattleConst.maxFieldUnit];
    //enemy in front
    List<BattleObject> enemyField = new List<BattleObject>(BattleConst.maxFieldUnit);
    List<BattleObject> playerField = new List<BattleObject>(BattleConst.maxFieldUnit);
    //total enemy
    List<BattleObject> enemyList = new List<BattleObject>();

    public List<BattleObject> EnemyFieldList
    {
        get { return enemyField; }
    }

    public List<BattleObject> PlayerFieldList
    {
        get { return playerField; }
    }

    public List<GameUnit> PlayerIdleList
    {
        get { return GetPlayerOffsiteUnits(); }
    }

    public BattleGroup()
    {
        for (int i = 0; i < BattleConst.maxFieldUnit; ++i)
        {
            enemyField.Add(null);
            playerField.Add(null);
        }
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
                //enemyField[pb.slot] = bo;
                //bo.OnEnterField();
                OnUnitEnterField(bo, pb.slot);
            }
            else 
            {
                bo.unit.pbUnit.slot = BattleConst.offsiteSlot;
                bo.OnExitField();
                //OnUnitExitField(bo, pb.slot);
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
                itor.Current.unit.backUp = false;
                playerField[slot] = itor.Current;
                itor.Current.OnEnterField();
                //OnUnitEnterField(itor.Current, slot);
            }
            else
            {
                itor.Current.unit.pbUnit.slot = BattleConst.offsiteSlot;
                itor.Current.unit.backUp = true;
                itor.Current.OnExitField();
                //OnUnitExitField(itor.Current, slot);
            }

            ++slot;
        }
    }

    public void RefreshPlayerPos()
    {
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        var itor = playerUnitList.GetEnumerator();
        while (itor.MoveNext())
        {
            if (itor.Current.unit.pbUnit.slot != BattleConst.offsiteSlot && itor.Current.unit.State != UnitState.Dead)
            {
                itor.Current.OnEnterField();
            }
        }
    }

    //public List<GameUnit> GetAllUnits()
    //{
    //    List<GameUnit> all = new List<GameUnit>(enemyList);
    //    all.AddRange(playerList);
    //    return all;
    //}

    public void ResetActionOrder()
    {
        for (int i = 0; i < enemyField.Count; i++)
        {
            var bo = enemyField[i];
            if (bo != null && bo.unit != null && bo.unit.State != UnitState.Dead)
            {
                bo.unit.ResetAcionOrder();
                bo.unit.CalcNextActionOrder();
            }
        }

        for (int i = 0; i < playerField.Count; i++)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.State != UnitState.Dead)
            {
                bo.unit.ResetAcionOrder();
                bo.unit.CalcNextActionOrder();
            }
        }
    }

    public void CalcUnitNextAction(int movedUnitId)
    {
        for (int i = 0; i < enemyField.Count; i++)
        {
            var bo = enemyField[i];
            if (bo != null)
            {
                if (bo.guid == movedUnitId)
                {
                    bo.unit.CalcNextActionOrder();
                    return;
                }
            }
        }

        for (int i = 0; i < playerField.Count; i++)
        {
            var bo = playerField[i];
            if (bo != null)
            {
                if (bo.guid == movedUnitId)
                {
                    bo.unit.CalcNextActionOrder();
                    return;
                }
            }
        }
    }

    public BattleObject GetNextMoveUnit()
    {
        //check if enemy is preparing dazhao, only enemy need do this
        for (int i = 0; i < enemyField.Count; i++)
        {
            var bo = enemyField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.dazhao > 0 && bo.unit.State != UnitState.Dead)
            {
                if (--bo.unit.dazhaoPrepareCount == 0)
                {
                    return bo;
                }
            }
        }

        BattleObject fastestUnit = null;
        float fastestOrder = BattleConst.maxSpeed;
        for (int i = 0; i < enemyField.Count; i++)
        {
            var bo = enemyField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.isVisible && bo.unit.dazhao == 0 && bo.unit.State != UnitState.Dead)
            {
                if (bo.unit.ActionOrder < fastestOrder)
                {
                    fastestUnit = bo;
                    fastestOrder = bo.unit.ActionOrder;
                }
                else if (fastestUnit != null && bo.unit.ActionOrder == fastestOrder && bo.unit.speed > fastestUnit.unit.speed)
                {
                    fastestUnit = bo;
                    fastestOrder = bo.unit.ActionOrder;
                }
            }
        }

		var shifaBo = MagicDazhaoController.Instance.GetCasterBattleObj ();
        for (int i = 0; i < playerField.Count; i++)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.isVisible)
            {
				//施法对象不参与行动序列
				if(null != shifaBo && shifaBo.guid == bo.guid)
				{
					continue;
				}
                if (bo.unit.ActionOrder < fastestOrder)
                {
                    fastestUnit = bo;
                    fastestOrder = bo.unit.ActionOrder;

                }
                else if (fastestUnit != null && bo.unit.ActionOrder == fastestOrder && bo.unit.speed > fastestUnit.unit.speed && bo.unit.State != UnitState.Dead)
                {
                    fastestUnit = bo;
                    fastestOrder = bo.unit.ActionOrder;
                }
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
        int enemyCount = enemyList.Count;
        for (int i = 0; i < enemyCount; i++)
        {
            var bo = enemyList[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.isVisible && bo.unit.State != UnitState.Dead)
			{
                return false;
			}
        }

        return true;
    }

    public bool IsPlayerAllDead()
    {
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        int playerCount = playerUnitList.Count;
        BattleObject playerUnit = null;
        for (int i = 0; i < playerCount; ++i)
        {
            playerUnit = playerUnitList[i];
            if (playerUnit != null && playerUnit.unit.curLife > 0 && playerUnit.unit.State != UnitState.Dead)
            {
                return false;
            }
        }

        return true;
    }

    public BattleObject GetEnemyToField()
    {
        foreach (BattleObject item in enemyList)
        {
            if (item.unit.pbUnit.slot == BattleConst.offsiteSlot &&
                item.unit.curLife > 0 &&
                item.unit.State != UnitState.Dead &&
                item.unit.State != UnitState.ToBeEnter
                )
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
                curObj.unit.State != UnitState.Dead &&
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
        for (int i = 0; i < playerField.Count; i++)
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

        List<BattleObject> field;
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
        List<BattleObject> field;
        if (bo.camp == UnitCamp.Enemy)
            field = enemyField;
        else
            field = playerField;

        if (field[slot] != null)
        {
            bo.unit.pbUnit.slot = BattleConst.offsiteSlot;
            field[slot] = null;
        }
        if (bo.unit.State != UnitState.Dead)
        {
            bo.OnExitField();
        }
    }

    public void AllUnitsExitField()
    {
        for (int i = 0; i < enemyField.Count; i++)
        {
            var unit = enemyField[i];
            if (unit != null)
            {
                OnUnitExitField(unit, unit.unit.pbUnit.slot);
            }
        }

        for (int i = 0; i < playerField.Count; i++)
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
            item.ClearEvent();
            ObjectDataMgr.Instance.RemoveBattleObject(item.guid);
        }
    }

    public void OnStartNewBattle(float triggerTime)
    {
        GameUnit curUnit = null;
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        int count = playerUnitList.Count;
        for (int i = 0; i < count; ++i)
        {
            curUnit = playerUnitList[i].unit;
            curUnit.CastPassiveSpell(triggerTime);
        }
        
        count = enemyList.Count;
        for (int i = 0; i < count; ++i)
        {
            curUnit = enemyList[i].unit;
            curUnit.CastPassiveSpell(triggerTime);
        }
    }

    public void CastFirstSpell()
    {
        List<GameUnit> hasFirstSpellList = new List<GameUnit>();

        for (int i = 0; i < enemyField.Count; ++i)
        {
            var bo = enemyField[i];
            if (bo != null && bo.unit != null && bo.unit.State != UnitState.Dead)
            {
                InsertFirstSpellUnit(hasFirstSpellList, bo.unit);
            }
        }

        for (int i = 0; i < playerField.Count; ++i)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.State != UnitState.Dead)
            {
                InsertFirstSpellUnit(hasFirstSpellList, bo.unit);
            }
        }

        int count = hasFirstSpellList.Count;
        string firstSpellID;
        GameUnit curUnit;
        for (int i = 0; i < count; ++i)
        {
            curUnit = hasFirstSpellList[i];
            firstSpellID = curUnit.GetFirstSpell();
            if (firstSpellID != null)
            {
                BattleController.Instance.Process.InsertFirstSpellAction(curUnit.battleUnit, firstSpellID);
            }
        }
        hasFirstSpellList.Clear();
        hasFirstSpellList = null;
    }

    void InsertFirstSpellUnit(List<GameUnit> firstSpellList, GameUnit unit)
    {
        int count = firstSpellList.Count;
        if (count == 0)
        {
            firstSpellList.Add(unit);
        }
        else 
        {
            for (int i = 0; i < count; ++i)
            {
                GameUnit curUnit = firstSpellList[i];
                if (curUnit.speed < unit.speed)
                {
                    firstSpellList.Insert(i, unit);
                    return;
                }
            }
            firstSpellList.Add(unit);
        }
    }
}
