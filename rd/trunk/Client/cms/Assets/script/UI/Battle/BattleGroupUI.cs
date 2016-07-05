using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BattleGroupUI : MonoBehaviour
{
    public BattleUnitUI[] playerUnitUI;
    public EnemyUnitUI[] enemyUnitUI;
    public EnemyUnitUI bossUnitUI;

    //---------------------------------------------------------------------------------------------
    public void Init(List<BattleObject> playerUnits, List<BattleObject> enemyUnits)
    {
        //init player unit ui
        int count = playerUnits.Count;
        count = (count < playerUnitUI.Length) ? count : playerUnitUI.Length;
        int i = 0;
        for (; i < count; ++i)
        {
            playerUnitUI[i].Show(playerUnits[i]);
        }
        for (; i < playerUnitUI.Length; ++i)
        {
            playerUnitUI[i].Hide();
        }

        //init enemy unit ui
        bossUnitUI.Hide();
        count = enemyUnits.Count;
        count = (count < enemyUnitUI.Length) ? count : enemyUnitUI.Length;
        i = 0;
        for (; i < count; ++i)
        {
            if (enemyUnits[i] != null && enemyUnits[i].unit.isBoss)
            {
                bossUnitUI.Show(enemyUnits[i]);
                enemyUnitUI[i].Hide();
            }
            else
            {
                enemyUnitUI[i].Show(enemyUnits[i]);
            }
        }
        for (; i < playerUnitUI.Length; ++i)
        {
            enemyUnitUI[i].Hide();
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeBuffState(SpellBuffArgs args)
    {
        //TODO: fix BattleUnitUI and EnemyUnitUI
        BattleUnitUI playerUI = GetPlayerUI(args.targetID);
        if (playerUI != null)
        {
            playerUI.ChangeBuffState(args);
            return;
        }

        EnemyUnitUI enemyUI = GetEnemyUI(args.targetID);
        if (enemyUI != null)
        {
            enemyUI.ChangeBuffState(args);
            return;
        }

        Logger.LogWarningFormat("can not find unit {0} in BattleGroupUI:ChangeBuffState", args.targetID);
    }
    //---------------------------------------------------------------------------------------------
    public void ShowUnit(BattleObject unit, int slot)
    {
        //NOTE; if have more than one boss, there will be display error
        if (unit.unit.isBoss)
        {
            bossUnitUI.Show(unit);
        }
        else 
        {
            if (slot < enemyUnitUI.Length)
            {
                enemyUnitUI[slot].Show(unit);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void HideUnit(int id)
    {
        BattleUnitUI playerUI = GetPlayerUI(id);
        if (playerUI != null)
        {
            playerUI.Hide();
            return;
        }

        EnemyUnitUI enemyUI = GetEnemyUI(id);
        if (enemyUI != null)
        {
            enemyUI.Hide();
            return;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeLife(SpellVitalChangeArgs lifeChange)
    {
        BattleUnitUI playerUI = GetPlayerUI(lifeChange.targetID);
        if (playerUI != null)
        {
            playerUI.lifeBar.SetTargetLife(lifeChange.vitalCurrent, lifeChange.vitalMax);
            return;
        }

        EnemyUnitUI enemyUI = GetEnemyUI(lifeChange.targetID);
        if (enemyUI != null)
        {
            enemyUI.lifeBar.SetTargetLife(lifeChange.vitalCurrent, lifeChange.vitalMax);
            return;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeEnergy(SpellVitalChangeArgs energyChange)
    {
        BattleUnitUI playerUI = GetPlayerUI(energyChange.casterID);
        if (playerUI != null)
        {
            playerUI.SetEnergy(energyChange.vitalCurrent);
            return;
        }

        //EnemyUnitUI enemyUI = GetEnemyUI(lifeChange.targetID);
        //if (enemyUI != null)
        //{
        //    enemyUI.lifeBar.SetTargetLife(lifeChange.vitalCurrent, lifeChange.vitalMax);
        //    return;
        //}
    }
    //---------------------------------------------------------------------------------------------
    public void SetBattleUnitVisible(int id, bool visible)
    {
        //NOTE: only enemy need this check
        EnemyUnitUI enemy = GetEnemyUI(id);
        if (enemy != null)
        {
            Logger.LogFormat("change monster {0}'s visible to {1}", id, visible);
            enemy.gameObject.SetActive(visible);
            if (visible)
                enemy.Show(enemy.Unit);

            return;
        }

        Logger.LogWarningFormat("can not find hide monst id={0}", id);
    }
    //---------------------------------------------------------------------------------------------
    BattleUnitUI GetPlayerUI(int id)
    {
        for (int i = 0; i < playerUnitUI.Length; i++)
        {
            if (playerUnitUI[i].Unit && playerUnitUI[i].Unit.guid == id)
                return playerUnitUI[i];
        }

        return null;
    }
    //---------------------------------------------------------------------------------------------
    EnemyUnitUI GetEnemyUI(int id)
    {
        for (int i = 0; i < enemyUnitUI.Length; i++)
        {
            if (enemyUnitUI[i].Unit && enemyUnitUI[i].Unit.guid == id)
                return enemyUnitUI[i];
        }

        if (bossUnitUI.Unit && bossUnitUI.Unit.guid == id)
            return bossUnitUI;

        return null;
    }
    //---------------------------------------------------------------------------------------------
    //void Update()
    //{
    //    var units = BattleController.Instance.BattleGroup.PlayerFieldList;
    //    for (int i = 0; i < playerUnitUI.Length; i++)
    //    {
    //        if (i < units.Count)
    //        {
    //            playerUnitUI[i].Show(units[i]);
    //        }
    //        else
    //        {
    //            playerUnitUI[i].Hide();
    //        }
    //    }
    //}
    //---------------------------------------------------------------------------------------------
    //BattleUnitUI GetUnitById(int id)
    //{
    //    for (int i = 0; i < unitUI.Length; i++)
    //    {
    //        if (unitUI[i].Unit.guid == id)
    //            return unitUI[i];
    //    }

    //    return null;
    //}
    //---------------------------------------------------------------------------------------------

#region Event
#endregion
}
