using UnityEngine;
using System.Collections;

public static class BossScript
{
    public static BattleRetCode bossValiVic()
    {
        if (BattleController.Instance.BattleGroup.IsEnemyAllDead())
            return BattleRetCode.Success;

        if (BattleController.Instance.BattleGroup.IsPlayerAllDead())
            return BattleRetCode.Failed;

        return BattleRetCode.Normal;
    }

    public static int bossValiP1()
    {
        var bossUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (bossUnit != null)
        {
            if (bossUnit.curLife < bossUnit.maxLife * 0.8f)
                return 1;
        }

        return BattleConst.processContinueRet;
    }

    public static int bossValiP2()
    {
        var bossUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (bossUnit != null)
        {
            if (bossUnit.curLife < bossUnit.maxLife * 0.6f)
                return 1;
        }
        return BattleConst.processContinueRet;
    }

    public static int bossValiP3()
    {
        var bossUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (bossUnit != null)
        {
            if (bossUnit.curLife < bossUnit.maxLife * 0.4f)
                return 1;
        }
        return BattleConst.processContinueRet;
    }

    public static int bossValiP4()
    {
        var bossUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (bossUnit != null)
        {
            if (bossUnit.curLife < bossUnit.maxLife * 0.0f)
                return 1;
        }
        return BattleConst.processContinueRet;
    }
}
