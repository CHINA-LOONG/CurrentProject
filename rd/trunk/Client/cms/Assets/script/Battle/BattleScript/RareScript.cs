using UnityEngine;
using System.Collections;

public static class RareScript
{
    public static BattleRetCode rareValiVic()
    {
        if (BattleController.Instance.BattleGroup.IsEnemyAllDead())
            return BattleRetCode.Success;

        if (BattleController.Instance.BattleGroup.IsPlayerAllDead())
            return BattleRetCode.Failed;

        return BattleRetCode.Normal;
    }


    public static int rareValiP1()
    {
        var rareUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (rareUnit != null)
        {
            if (rareUnit.curLife < rareUnit.maxLife * 0.8f)
                return 1;
        }

        return BattleConst.processContinueRet;
    }

    public static int rareValiP2()
    {
        var rareUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (rareUnit != null)
        {
            if (rareUnit.curLife < rareUnit.maxLife * 0.6f)
                return 1;
        }
        return BattleConst.processContinueRet;
    }

    public static int rareValiP3()
    {
        var rareUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (rareUnit != null)
        {
            if (rareUnit.curLife < rareUnit.maxLife * 0.4f)
                return 1;
        }
        return BattleConst.processContinueRet;
    }

    public static int rareValiP4()
    {
        var rareUnit = BattleController.Instance.BattleGroup.EnemyFieldList[1].unit;
        if (rareUnit != null)
        {
            if (rareUnit.curLife < rareUnit.maxLife * 0.2f)
                return 1;
        }
        return BattleConst.processContinueRet;
    }
}
