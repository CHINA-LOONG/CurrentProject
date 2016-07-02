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
        return BattleConst.processContinueRet;
    }

    public static int rareValiP2()
    {
        return BattleConst.processContinueRet;
    }

    public static int rareValiP3()
    {
        return BattleConst.processContinueRet;
    }

    public static int rareValiP4()
    {
        return BattleConst.processContinueRet;
    }
}
