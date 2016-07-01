using UnityEngine;
using System.Collections;

/// <summary>
/// 静态数据结构
/// </summary>
public class BattleUnitData
{
    public int id;
    public string name;
    public string prefab;
    //性格
    public int personality;

    //--基础属性，不加buff
    public int level;
    public int hp;
    public int strength;
    public int intelligence;
    public int speed;
    public int resistence;
    public int endurance;
    public float hitRate;
    public float criticalRate;
    public float criticalDamageFactor;
    public float recovery;

    public string spellList;
}

/// <summary>
/// 战斗数据除了显示部分（model等）应该全部从server获取
/// 即使是配置的静态数据，也应该从server获取
/// </summary>
public class BattleUnit
{
    public int guid;
    //静态表中的id，如果是玩家的monster下面两项可能为空
    public int id;
    public string name;
    //配置数据
    BattleUnitData data = null;
    PbBattleUnit proto = null;

    //--基础属性
    public int level;
	public int baseStrength;
	public int baseLife;
	public int baseIntelligence;
	public int baseSpeed;
	public int baseResistance;
	public int baseEndurance;
	public int baseHitRate;
	public int baseCriticalRate;
	public int baseCriticalDamageFactor;
    public int baseRecovery;

    //--战斗中的属性，可能受buff等影响
    public int hp;
    public int maxHp;
    public int strength;
    public int intelligence;
    public int speed;
    public int resistance;
    public int endurance;
    public float hitRate;
    public float criticalRate;
    public float criticalDamageFactor;
    public float recovery;

    public int phyAtk;
    public int magAtk;
    public int defend;

    public static BattleUnit FromPb(PbBattleUnit unit)
    {
        var battleUnit = new BattleUnit();
        battleUnit.name = unit.name;
        battleUnit.proto = unit;
        battleUnit.data = new BattleUnitData();

        return battleUnit;
    }
}
