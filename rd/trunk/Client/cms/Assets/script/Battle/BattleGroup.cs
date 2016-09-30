using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGroup
{
    //battle units in front
    List<BattleObject> enemyField = new List<BattleObject>(BattleConst.maxFieldUnit);
    List<BattleObject> playerField = new List<BattleObject>(BattleConst.maxFieldUnit);
    //total battle units(NOTE: some duplicate for front list, fucking the designer)
    List<BattleObject> enemyList = new List<BattleObject>();
    List<BattleObject> playerList = new List<BattleObject>();

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

    public List<BattleObject> GetAllUnitList(int camp)
    {
        if (camp == (int)UnitCamp.Player)
        {
            return playerList;
        }
        return enemyList;
    }

    public BattleGroup()
    {
        for (int i = 0; i < BattleConst.maxFieldUnit; ++i)
        {
            enemyField.Add(null);
            playerField.Add(null);
        }
    }

    public void SetEnemyList(List<PbUnit> list, bool isPvp = false)
    {
        DestroyEnemys();
        foreach (PbUnit pb in list)
        {
            GameUnit curUnit = GameUnit.FromPb(pb, false, isPvp);
            BattleObject bo = ObjectDataMgr.Instance.CreateBattleObject(curUnit, null, Vector3.zero, Quaternion.identity);
            if (pb.slot >= BattleConst.slotIndexMin && pb.slot <= BattleController.Instance.CurMaxSlotIndex)
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

    public void SetPlayerList(ref List<int> idList)
    {
        if (playerList.Count > 0)
        {
            Debug.LogError("player count not zero");
        }

        playerList.Clear();
        ObjectDataMgr objMgr = ObjectDataMgr.Instance;
        PlayerData mainPlayer = GameDataMgr.Instance.PlayerDataAttr;
        int count = idList.Count;
        for (int i = 0; i < count; ++i)
        {
            GameUnit curUnit = mainPlayer.GetPetWithKey(idList[i]);
            if (curUnit == null)
            {
                Logger.LogError("SetPlayerList failed, bcoz no such id");
                continue;
            }
            BattleObject bo = objMgr.CreateBattleObject(
                                curUnit,
                                null,
                                Vector3.zero,
                                Quaternion.identity
                                );

            curUnit.ResetAllState(false);
            if (i >= BattleConst.slotIndexMin && i <= BattleController.Instance.CurMaxSlotIndex)
            {
                curUnit.pbUnit.slot = i;
                curUnit.backUp = false;
                playerField[i] = bo;
                bo.OnEnterField();
            }
            else
            {
                curUnit.pbUnit.slot = BattleConst.offsiteSlot;
                curUnit.backUp = true;
                bo.OnExitField();
            }

            playerList.Add(bo);
        }
    }

    //summon from hell
    public void RevivePlayerList(float actionOrderTime)
    {
        int count = playerList.Count;
        int slot = 0;
        GameUnit curUnit = null;
        for (int i = 0; i < count; ++i)
        {
            curUnit = playerList[i].unit;
            slot = curUnit.pbUnit.slot;
            curUnit.ResetAllState(true);
            if (slot >= BattleConst.slotIndexMin && slot <= BattleController.Instance.CurMaxSlotIndex)
            {
                curUnit.backUp = false;
                playerField[slot] = playerList[i];
                curUnit.CalcNextActionOrder(actionOrderTime);
                playerList[i].OnEnterField(false);

                SpellReviveArgs reviveArgs = new SpellReviveArgs();
                reviveArgs.triggerTime = Time.time;//TODO: use battle level time;
                reviveArgs.targetID = playerList[i].guid;
                GameEventMgr.Instance.FireEvent<System.EventArgs>(GameEventList.spellUnitRevive, reviveArgs);
                //OnUnitEnterField(itor.Current, slot);
            }
            else
            {
                curUnit.pbUnit.slot = BattleConst.offsiteSlot;
                curUnit.backUp = true;
                curUnit.State = UnitState.None;
                playerList[i].OnExitField();
                //OnUnitExitField(itor.Current, slot);
            }
        }
    }

    public void RefreshPlayerPos()
    {
        int count = playerList.Count;
        for (int i = 0; i < count; ++i)
        {
            if (playerList[i].unit.pbUnit.slot != BattleConst.offsiteSlot &&
                playerList[i].unit.State != UnitState.Dead
                )
            {
                playerList[i].OnEnterField();
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

    public bool IsPlayerCasteDazhao()
    {
        for (int i = 0; i < playerField.Count; i++)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.isVisible)
            {
                if (bo.unit.dazhaoPrepareCount > 0)
                {
                    return true;
                }
            }
        }

        return false;
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
        
        for (int i = 0; i < playerField.Count; i++)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.isVisible)
            {
                //xxxxplayer need to insert action, so when 1 remained trigger dazhao
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

		//var shifaBo = MagicDazhaoController.Instance.GetCasterBattleObj ();
        for (int i = 0; i < playerField.Count; i++)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.isVisible && bo.unit.dazhao == 0)
            {
				//施法对象不参与行动序列
				//if(null != shifaBo && shifaBo.guid == bo.guid)
				//{
				//	continue;
				//}
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
        int playerCount = playerList.Count;
        BattleObject playerUnit = null;
        for (int i = 0; i < playerCount; ++i)
        {
            playerUnit = playerList[i];
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
        BattleObject curObj = null;
        int count = playerList.Count;
        for (int i = 0; i < count; ++i)
        {
            curObj = playerList[i];
            int slot = curObj.unit.pbUnit.slot;
            if (slot == BattleConst.offsiteSlot &&
                curObj.unit.curLife > 0 &&
                curObj.unit.State != UnitState.Dead &&
                curObj.unit.State != UnitState.ToBeEnter
               )
            {
                return curObj;
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
        int count = playerList.Count;
        for (int i = 0; i < count; ++i)
        {
            int slot = playerList[i].unit.pbUnit.slot;
            if (slot == BattleConst.offsiteSlot && playerList[i].unit.State != UnitState.ToBeEnter)
            {
                result.Add(playerList[i].unit);
            }
        }

        return result;
    }

    public void OnUnitEnterField(BattleObject bo, int slot)
    {
        int fixedSlot = Mathf.Clamp(slot, BattleConst.slotIndexMin, BattleController.Instance.CurMaxSlotIndex);
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

    public void OnUnitExitField(BattleObject bo, int slot)
    {
        int fixedSlot = Mathf.Clamp(slot, BattleConst.slotIndexMin, BattleController.Instance.CurMaxSlotIndex);
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

    public void DestroyEnemys()
    {
        foreach (BattleObject item in enemyList)
        {
            item.ClearEvent();
            ObjectDataMgr.Instance.RemoveBattleObject(item.guid);
        }
        enemyList.Clear();
    }

    public void DestroyPlayers()
    {
        int count = playerList.Count;
        for (int i = 0; i < count; ++i)
        {
            playerList[i].ClearEvent();
            ObjectDataMgr.Instance.RemoveBattleObject(playerList[i].guid);
        }
        playerList.Clear();
    }

    public void OnStartNewBattle(float triggerTime)
    {
        GameUnit curUnit = null;
        int count = playerList.Count;
        for (int i = 0; i < count; ++i)
        {
            curUnit = playerList[i].unit;
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
            if (bo != null && bo.unit != null && bo.unit.State != UnitState.Dead && bo.unit.isVisible)
            {
                InsertFirstSpellUnit(hasFirstSpellList, bo.unit);
            }
        }

        for (int i = 0; i < playerField.Count; ++i)
        {
            var bo = playerField[i];
            if (bo != null && bo.unit.State != UnitState.Dead && bo.unit.isVisible)
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
