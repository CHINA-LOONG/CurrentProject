using UnityEngine;
using System.Collections;

public enum UnitCamp
{
    Player,
    Enemy,
}

public class BattleConst
{
    public const int slotIndexMin = 1;
    public const int slotIndexMax = 1;
    public const int maxFieldUnit = 3;
    public const int speedK = 1000;
    public const float speedFactorMin = 0.98f;
    public const float speedFactorMax = 1.02f;
}
