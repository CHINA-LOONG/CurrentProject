using UnityEngine;
using System.Collections;

public enum UnitCamp
{
    Player,
    Enemy,
}

public class BattleConst
{
    public const int offsiteSlot = -1;
    public const int slotIndexMin = 0;
    public const int slotIndexMax = 2;

    public const int maxFieldUnit = 3;

    //换宠冷却时间
    public const float switchPetCD = 10;

    public const int speedK = 1000;
    public const float speedFactorMin = 0.98f;
    public const float speedFactorMax = 1.02f;

    //能量积攒上限
    public const int enegyMax = 100;

    public const int processContinueRet = -1;

    //大招默认施法时长
    public const float dazhaoDefaultTime = 5;

    //无条件关闭换宠UI
    public const int closeSwitchPetUI = -1;

    //战斗场景id
    public const int battleSceneGuid = -1;

    //换宠换出去时间
    public const float unitOutTime = 1.5f;
    //换宠换进来时间
    public const float unitInTime = 1.5f;
}
