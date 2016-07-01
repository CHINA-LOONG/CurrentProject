using UnityEngine;
using System;
using System.Collections.Generic;

//////////////////////////////////////////////////////////////////////////
/// <summary>
/// 模拟战斗协议，只做测试用途
/// </summary>
public class PbUnit
{
    //站位，1，2，3表示，0表示在场下
    public int slot;
    public int guid;
    public UnitCamp camp;
    //静态数据id，可以通过此id获取prefab等
    public string id;
    //性格
    public int personality;
    public int level;
    public int curExp;
    public int starLevel;//升星
    //public List<string> spellIDList;
}

public class GameUnit
{
    public PbUnit pbUnit;

    //static data
    public string name;
    public string assetID;//prefab name
    //所有的百分比加成的基础值都是：属性基础值+装备值+升星
    //一阶属性
    public int health;//体力
    public int strength;
    public int intelligence;
    public int speed;
    public int defense;
    public int endurance;
    public int property;//五行属性
    public int recovery;//战后回血
    //掉落金币
    public int goldNoteMin;
    public int goldNoteMax;
    //掉落经验
    public int expMin;
    public int expMax;
    //public int outputExp;//被吃掉产出
    public float criticalRatio;//暴击率
    public float antiCriticalRatio;//抗暴击
    public float criticalDamageRatio;//暴击伤害系数
    public float hitRatio;//命中率
    public float additionDamageRatio;//伤害加成
    public float minusDamageRatio;//伤害减免
    public float additionHealRatio;//治疗加成
    public float defensePierce;//穿透
    //进化
    public bool isEvolutionable;
    public string evolutionID;

    //状态数据
    //技能加成系数
    public int invincible;//无敌
    public int stun;//眩晕
    public int energy;//能量值
    public float spellStrengthRatio;
    public float spellIntelligenceRatio;
    public float spellSpeedRatio;
    public float spellDefenseRatio;
    public float spellEnduranceRatio;

    //二级属性
    public int curLife;
    public int maxLife;
    public int magicAttack;
    public int phyAttack;

    public List<Buff> buffList;
    public Dictionary<string, Spell> spellList;
    //public List<Equipment> equipmentList;
    //只在客户端计算使用的属性
    float speedCount = 0;
    float actionOrder = 0;
    public float ActionOrder { get { return actionOrder; } }

    public static GameUnit FromPb(PbUnit unit)
    {
        var gameUnit = new GameUnit();
        gameUnit.pbUnit = unit;

        //初始化属性
        gameUnit.Init();

        return gameUnit;
    }

    void Init()
    {
        GameDataMgr gdMgr = GameDataMgr.Instance;
        UnitData.RowData unitRowData = StaticDataMgr.Instance.GetUnitRowData(pbUnit.id);
        UnitBaseData.RowData unitBaseRowData = StaticDataMgr.Instance.GetUnitBaseRowData(pbUnit.level);
        health = (int)(unitRowData.healthModifyRate * unitBaseRowData.health + gdMgr.PlayerDataAttr.equipHealth);
        strength = (int)(unitRowData.strengthModifyRate * unitBaseRowData.strength + gdMgr.PlayerDataAttr.equipStrength);
        intelligence = (int)(unitRowData.intelligenceModifyRate * unitBaseRowData.intelligence + gdMgr.PlayerDataAttr.equipIntelligence);
        speed = (int)(unitRowData.speedModifyRate * unitBaseRowData.speed + gdMgr.PlayerDataAttr.equipSpeed);
        defense = (int)(unitRowData.defenseModifyRate * unitBaseRowData.defense + gdMgr.PlayerDataAttr.equipDefense);
        endurance = (int)(unitRowData.enduranceModifyRate * unitBaseRowData.endurance + gdMgr.PlayerDataAttr.equipEndurance);
        recovery = (int)(unitRowData.recoveryRate * unitBaseRowData.recovery);
        property = unitRowData.property;
        assetID = unitRowData.assetID;
        isEvolutionable = unitRowData.isEvolutionable;
        evolutionID = unitRowData.evolutionID;
        name = unitRowData.nickName;
        //TODO: 玩家宠物不需要
        goldNoteMax = (int)(unitRowData.goldNoteMaxValueModifyRate * unitBaseRowData.goldNoteMax);
        goldNoteMin = (int)(unitRowData.goldNoteMinValueModifyRate * unitBaseRowData.goldNoteMin);
        expMin = (int)(unitRowData.expMinValueModifyRate * unitBaseRowData.expMin);
        expMax = (int)(unitRowData.expMaxValueModifyRate * unitBaseRowData.expMax);
        //TODO: 装备系统附加值
        criticalRatio = gdMgr.PlayerDataAttr.criticalRatio;
        antiCriticalRatio = 0.0f;
        hitRatio = gdMgr.PlayerDataAttr.hitRatio;
        additionDamageRatio = 0.0f;
        minusDamageRatio = 0.0f;
        additionHealRatio = 0.0f;
        defensePierce = 0.0f;

        //战斗状态值初始化
        invincible = 0;
        stun = 0;
        energy = 0;
        spellStrengthRatio = 0.0f;
        spellIntelligenceRatio = 0.0f;
        spellSpeedRatio = 0.0f;
        spellDefenseRatio = 0.0f;
        spellEnduranceRatio = 0.0f;

        //计算二级属性
        curLife = (int)SpellConst.healthToLife * health;
        maxLife = curLife;
        magicAttack = (int)(SpellConst.strengthToAttack * strength);
        phyAttack = (int)(SpellConst.intelligenceToAttack * intelligence);

        //初始化技能列表
        spellList = new Dictionary<string, Spell>();
        string[] spellIDList = unitRowData.spellIDList.Split(';');
        SpellProtoType spellPt = null;
        for (int i = 0; i < spellIDList.Length; ++i)
        {
            spellPt = StaticDataMgr.Instance.GetSpellProtoData(spellIDList[i]);
            if (spellPt != null)
            {
                spellList.Add(spellIDList[i], new Spell(spellPt));
            }
        }

        buffList = new List<Buff>();
    }

    public Spell GetSpell(string spellID)
    {
        Spell spell;
        if (spellList.TryGetValue(spellID, out spell))
        {
            return spell;
        }

        return null;
    }

    /// <summary>
    /// 累计速度，计算order值
    /// </summary>
    public void CalcSpeed()
    {
        speedCount += speed * UnityEngine.Random.Range(BattleConst.speedFactorMin, BattleConst.speedFactorMax);
        actionOrder = BattleConst.speedK / speedCount;
        Logger.LogFormat("Unit {0}: speedCount: {1}, actionOrder: {2}", name, speedCount, actionOrder);
    }

    /// <summary>
    /// 不累计速度，计算order值
    /// </summary>
    public void ReCalcSpeed()
    {
        speedCount = speed * UnityEngine.Random.Range(BattleConst.speedFactorMin, BattleConst.speedFactorMax);
        actionOrder = BattleConst.speedK / speedCount;
        Logger.LogFormat("Unit {0}: speedCount: {1}, actionOrder: {2}", name, speedCount, actionOrder);
    }

    public void OnEnterField()
    {

    }

    public void OnExitField()
    {

    }
}
