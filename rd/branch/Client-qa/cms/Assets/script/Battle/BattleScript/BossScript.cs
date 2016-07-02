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
        return BattleConst.processContinueRet;
    }

    public static int bossValiP2()
    {
        return BattleConst.processContinueRet;
    }

    public static int bossValiP3()
    {
        return BattleConst.processContinueRet;
    }

    public static int bossValiP4()
    {
        return BattleConst.processContinueRet;
    }
}
