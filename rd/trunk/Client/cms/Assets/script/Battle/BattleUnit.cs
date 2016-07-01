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
    //配置数据
    BattleUnitData config = null;
    //服务器发过来的协议中属性
    PbBattleUnit proto = null;

    //只读属性
    public int Guid { get { return proto.guid; } }
    public string Name { get { return proto.name; } }
    public int Level { get { return proto.level; } }
    public int BaseMaxHp { get { return proto.hp; } }
    public int BaseStrength { get { return proto.strength; } }
    public int BaseIntelligence { get { return proto.intelligence; } }
    public int BaseSpeed { get { return proto.speed; } }
    public int BaseResistance { get { return proto.resistence; } }
    public int BaseEndurance { get { return proto.endurance; } }
    public float BaseHitRate { get { return proto.hitRate; } }
    public float BaseCriticalRate { get { return proto.criticalRate; } }
    public float BaseCriticalDamageFactor { get { return proto.criticalDamageFactor; } }
    public float BaseRecovery { get { return proto.recovery; } }

    //可读写属性
    int hp;
    public int Hp { get { return hp; } set { hp = value; } }
    int maxHp;
    public int MaxHp { get { return maxHp; } set { hp = value; } }
    int strength;
    public int Strength { get { return strength; } set { strength = value; } }
    int intelligence;
    public int Intelligence { get { return intelligence; } set { intelligence = value; } }
    int speed;
    public int Speed { get { return speed; } set { speed = value; } }
    int resistance;
    public int Resistance { get { return resistance; } set { resistance = value; } }
    int endurance;
    public int Endurance { get { return endurance; } set { criticalRate = value; } }
    float hitRate;
    public float HitRate { get { return hitRate; } set { criticalRate = value; } }
    float criticalRate;
    public float CriticalRate { get { return criticalRate; } set { criticalRate = value; } }
    float criticalDamageFactor;
    public float CriticalDamageFactor { get { return criticalDamageFactor; } set { criticalDamageFactor = value; } }
    float recovery;
    public float Recovery { get { return recovery; } set { recovery = value; } }

    //只在客户端计算使用的属性
    float speedCount = 0;
    float actionOrder = 0;
    public float ActionOrder { get { return actionOrder; } }

    public static BattleUnit FromPb(PbBattleUnit unit)
    {
        var battleUnit = new BattleUnit();
        battleUnit.proto = unit;
        battleUnit.config = new BattleUnitData();

        //初始化属性
        battleUnit.Init();

        return battleUnit;
    }

    void Init()
    {
        hp = proto.hp;
        maxHp = proto.hp;
        strength = proto.strength;
        intelligence = proto.intelligence;
        speed = proto.speed;
        resistance = proto.resistence;
        endurance = proto.endurance;
        hitRate = proto.hitRate;
        criticalRate = proto.criticalRate;
        criticalDamageFactor = proto.criticalDamageFactor;
        recovery = proto.recovery;
    }

    /// <summary>
    /// 累计速度，计算order值
    /// </summary>
    public void CalcSpeed()
    {
        speedCount += proto.speed * Random.Range(BattleConst.speedFactorMin, BattleConst.speedFactorMax);
        actionOrder = BattleConst.speedK / speedCount;
        Logger.LogFormat("Unit {0}: speedCount: {1}, actionOrder: {2}", Name, speedCount, actionOrder);
    }

    /// <summary>
    /// 不累计速度，计算order值
    /// </summary>
    public void ReCalcSpeed()
    {
        speedCount = proto.speed * Random.Range(BattleConst.speedFactorMin, BattleConst.speedFactorMax);
        actionOrder = BattleConst.speedK / speedCount;
        Logger.LogFormat("Unit {0}: speedCount: {1}, actionOrder: {2}", Name, speedCount, actionOrder);
    }
}
