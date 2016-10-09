using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BattleGroupUI : MonoBehaviour
{
    //public BattleUnitUI[] playerUnitUI;
    //public EnemyUnitUI[] enemyUnitUI;
    public string bundleName;
    public string playerUIName;
    public string enemyUIName;
    public EnemyUnitUI bossUnitUI;

    private Transform playerGroupTrans;
    private Transform enemyGroupTrans;
    Dictionary<int, BattleUnitUI> playerUIList = new Dictionary<int,BattleUnitUI>();
    Dictionary<int, EnemyUnitUI> enemyUIList = new Dictionary<int,EnemyUnitUI>();

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        GameObject playerGroup = Util.FindChildByName(gameObject, "PlayerGroup");
        GameObject enemyGroup = Util.FindChildByName(gameObject, "EnemyGroup");

        playerGroupTrans = playerGroup.transform;
        enemyGroupTrans = enemyGroup.transform;
    }
    //---------------------------------------------------------------------------------------------
    public void Init(List<BattleObject> playerUnits, List<BattleObject> enemyUnits)
    {
        playerUIList.Clear();
        enemyUIList.Clear();

        //init player unit ui
        int count = playerUnits.Count;
        for (int i = 0; i < count; ++i)
        {
            if (playerUnits[i] != null)
            {
                BattleUnitUI bu = AddPlayerUI(playerUnits[i].guid);
                bu.Show(playerUnits[i]);
            }
        }

        //init enemy unit ui
        bossUnitUI.Hide();
        count = enemyUnits.Count;
        for (int i = 0; i < count; ++i)
        {
            if (enemyUnits[i] != null)
            {
                EnemyUnitUI eu = AddEnemyUI(enemyUnits[i].guid, enemyUnits[i].unit.isBoss);
                eu.Show(enemyUnits[i]);
            }
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

        //Logger.LogWarningFormat("can not find unit {0} in BattleGroupUI:ChangeBuffState", args.targetID);
    }
    //---------------------------------------------------------------------------------------------
    public void ShowUnit(BattleObject unit, int slot)
    {
        //NOTE; if have more than one boss, there will be display error
        if (unit.unit.isBoss)
        {
            bossUnitUI.Show(unit);
        }
        else if(unit.camp == UnitCamp.Enemy)
        {
            EnemyUnitUI enemyUI = GetEnemyUI(unit.guid);
            if (enemyUI == null)
                enemyUI = AddEnemyUI(unit.guid, false);

            enemyUI.Show(unit);
        }
        else 
        {
            BattleUnitUI playerUI = GetPlayerUI(unit.guid);
            if (playerUI == null)
                playerUI = AddPlayerUI(unit.guid);

            playerUI.Show(unit);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void HideUnit(int id)
    {
        BattleUnitUI playerUI = GetPlayerUI(id);
        if (playerUI != null)
        {
            playerUIList.Remove(id);
            playerUI.Destroy();
            return;
        }

        EnemyUnitUI enemyUI = GetEnemyUI(id);
        if (enemyUI != null)
        {
            if (enemyUI.Unit.unit.isBoss == false)
            {
                enemyUIList.Remove(id);
                enemyUI.Destroy();
            }
            else 
            {
                enemyUI.Hide();
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeLife(SpellVitalChangeArgs lifeChange)
    {
        if (lifeChange.vitalType != (int)VitalType.Vital_Type_Default &&
            lifeChange.vitalType != (int)VitalType.Vital_Type_FixLife &&
            lifeChange.vitalType != (int)VitalType.Vital_Type_Shield
            )
            return;

        BattleUnitUI playerUI = GetPlayerUI(lifeChange.targetID);
        if (playerUI != null)
        {
            if (lifeChange.vitalType!=(int)VitalType.Vital_Type_Shield)
            {
                playerUI.lifeBar.SetTargetLife(lifeChange.vitalCurrent, lifeChange.vitalMax);
                return;
            }
            else
            {
                playerUI.lifeBar.RefreshShieldUI();
            }
        }

        EnemyUnitUI enemyUI = GetEnemyUI(lifeChange.targetID);
        if (enemyUI != null)
        {
            if (lifeChange.vitalType != (int)VitalType.Vital_Type_Shield)
            {
                enemyUI.lifeBar.SetTargetLife(lifeChange.vitalCurrent, lifeChange.vitalMax);
                enemyUI.StartShake();//血条震动
                return;
            }
            else
            {
                enemyUI.lifeBar.RefreshShieldUI();
            }
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

        if (BattleController.Instance.PvpParam != null)
        {
            EnemyUnitUI enemyUI = GetEnemyUI(energyChange.casterID);
            if (enemyUI != null)
            {
                enemyUI.SetEnergy(energyChange.vitalCurrent);
                return;
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetRoundChange(int unitID, float fraction)
    {
        EnemyUnitUI enemyUI = GetEnemyUI(unitID);
        if (enemyUI != null)
        {
            enemyUI.SetEnergyBar(fraction);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetBattleUnitVisible(int id, bool visible)
    {
        //NOTE: only enemy need this check(照妖镜隐藏怪物)
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
    EnemyUnitUI GetEnemyUI(int id)
    {
        EnemyUnitUI enemyUI = null;
        if (enemyUIList.TryGetValue(id, out enemyUI))
        {
            return enemyUI;
        }

        if (bossUnitUI.Unit && bossUnitUI.Unit.guid == id)
            return bossUnitUI;

        return null;
    }
    //---------------------------------------------------------------------------------------------
    EnemyUnitUI AddEnemyUI(int id, bool isBoss)
    {
        EnemyUnitUI curEnemyUI = null;
        if (isBoss == false)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset(enemyUIName);
            curEnemyUI = go.GetComponent<EnemyUnitUI>();
            enemyUIList.Add(id, curEnemyUI);
            curEnemyUI.transform.SetParent(enemyGroupTrans);
            curEnemyUI.transform.localScale = Vector3.one;
            curEnemyUI.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else 
        {
            curEnemyUI = bossUnitUI;
        }
        return curEnemyUI;
    }
    //---------------------------------------------------------------------------------------------
    BattleUnitUI GetPlayerUI(int id)
    {
        BattleUnitUI playerUI = null;
        if (playerUIList.TryGetValue(id, out playerUI))
        {
            return playerUI;
        }

        return null;
    }
    //---------------------------------------------------------------------------------------------
    BattleUnitUI AddPlayerUI(int id)
    {
        GameObject playerUI = ResourceMgr.Instance.LoadAsset(playerUIName);
        //GameObject go = Instantiate(playerUISrc) as GameObject;
        BattleUnitUI curPlayerUI = null;
        curPlayerUI = playerUI.GetComponent<BattleUnitUI>();
        playerUIList.Add(id, curPlayerUI);
        curPlayerUI.transform.SetParent(playerGroupTrans);
        curPlayerUI.transform.localScale = Vector3.one;
        curPlayerUI.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        return curPlayerUI;
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
