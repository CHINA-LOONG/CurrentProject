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

}
