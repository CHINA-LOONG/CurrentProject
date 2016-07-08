using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class NormalScript
{
    //默认对局胜利失败
    public static BattleRetCode normalValiVic()
    {
        if (BattleController.Instance.BattleGroup.IsEnemyAllDead())
            return BattleRetCode.Success;

        if (BattleController.Instance.BattleGroup.IsPlayerAllDead())
            return BattleRetCode.Failed;

        return BattleRetCode.Normal;
    }

    //---------------------------------------------------------------------------------------------
    //切进程条件组
    public static int GetTotalRound()
    {
        return BattleController.Instance.Process.TotalRound;
    }
    //---------------------------------------------------------------------------------------------
    public static int GetBossRound()
    {
        int round = 0;
        List<BattleObject> enemyList = BattleController.Instance.BattleGroup.EnemyFieldList;
        int count = enemyList.Count;
        for (int i = 0; i < count; ++i)
        {
            if (enemyList[i] != null && enemyList[i].unit.isBoss)
            {
                round = enemyList[i].unit.attackCount;
                break;
            }
        }

        return round;
    }
    //---------------------------------------------------------------------------------------------
    public static float GetWpLifeLeftRatio(BattleObject bo, string wpName)
    {
        float lifeRatio = 0.0f;
        Dictionary<string, WeakPointRuntimeData> wpRutimeDataList = bo.wpGroup.allWpDic;
        if (wpRutimeDataList != null)
        {
            //if wpName is empty, refs total weak point
            if (string.IsNullOrEmpty(wpName))
            {
                int curLife = 0;
                int maxLife = 0;
                var itor = wpRutimeDataList.GetEnumerator();
                while (itor.MoveNext())
                {
                    curLife += itor.Current.Value.hp;
                    maxLife += itor.Current.Value.maxHp;
                }

                lifeRatio = curLife / maxLife;
            }
            else 
            {
                WeakPointRuntimeData wpRuntionData;
                if (wpRutimeDataList.TryGetValue(wpName, out wpRuntionData))
                {
                    lifeRatio = wpRuntionData.hp / wpRuntionData.maxHp;
                }
            }
        }

        return lifeRatio;
    }
    //---------------------------------------------------------------------------------------------
    public static int GetWpLifeLeft(BattleObject bo, string wpName)
    {
        int curLife = 0;
        Dictionary<string, WeakPointRuntimeData> wpRutimeDataList = bo.wpGroup.allWpDic;
        if (wpRutimeDataList != null)
        {
            //if wpName is empty, refs total weak point
            if (string.IsNullOrEmpty(wpName))
            {
                var itor = wpRutimeDataList.GetEnumerator();
                while (itor.MoveNext())
                {
                    curLife += itor.Current.Value.hp;
                }
            }
            else
            {
                WeakPointRuntimeData wpRuntionData;
                if (wpRutimeDataList.TryGetValue(wpName, out wpRuntionData))
                {
                    curLife = wpRuntionData.hp;
                }
            }
        }

        return curLife;
    }
    //---------------------------------------------------------------------------------------------
    public static bool IsWpDead(BattleObject bo, string wpName)
    {
        bool allDead = true;
        Dictionary<string, WeakPointRuntimeData> wpRutimeDataList = bo.wpGroup.allWpDic;
        if (wpRutimeDataList != null)
        {
            //if wpName is empty, refs total weak point
            if (string.IsNullOrEmpty(wpName))
            {
                var itor = wpRutimeDataList.GetEnumerator();
                while (itor.MoveNext())
                {
                    if (itor.Current.Value.hp > 0)
                    {
                        allDead = false;
                        break;
                    }
                }
            }
            else
            {
                WeakPointRuntimeData wpRuntionData;
                if (wpRutimeDataList.TryGetValue(wpName, out wpRuntionData))
                {
                    allDead = wpRuntionData.hp <= 0;
                }
            }
        }

        return allDead;
    }
    //---------------------------------------------------------------------------------------------
}
