using UnityEngine;
using System;
using System.Collections.Generic;

public enum UnitState
{
    None,
    Dazhao,
    ToBeReplaced,
    Dead,
}

//////////////////////////////////////////////////////////////////////////
/// <summary>
/// 模拟战斗协议，只做测试用途
/// </summary>
public class PbUnit
{
    //站位，0，1，2表示，-1表示在场下
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

[Serializable]
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
	public List<string> weakPointList;
	public List<string> findWeakPointlist = null;
	public Dictionary<string,GameObject> weakPointMeshDic;
	public Dictionary<string,GameObject> weakPointEffectDic;

    //只在客户端计算使用的属性
    float speedCount = 0;
    float actionOrder = 0;
    public float ActionOrder { get { return actionOrder; } }

    UnitState state = UnitState.None;
    public UnitState State { get { return state; } set { state = value; } }

	//战斗单元统计数据 
	public	int attackCount = 0;
	public 	List<int> lazyList = new List<int>();
	public	List<int> dazhaoList = new List<int>();

    //////////////////////////////////////////////////////////////////////////
    //显示部分    
    GameObject unitObject;
    public GameObject gameObject
    {
        get { return unitObject; }
    }

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
        UnitData unitRowData = StaticDataMgr.Instance.GetUnitRowData(pbUnit.id);
        UnitBaseData unitBaseRowData = StaticDataMgr.Instance.GetUnitBaseRowData(pbUnit.level);
        health = (int)(unitRowData.healthModifyRate * unitBaseRowData.health + gdMgr.PlayerDataAttr.equipHealth);
        Logger.LogFormat("总体力：{0}  基础体力：{1}   体力修正：{2}  装备附加体力：{3}", health, unitBaseRowData.health, unitRowData.healthModifyRate, gdMgr.PlayerDataAttr.equipHealth);
        strength = (int)(unitRowData.strengthModifyRate * unitBaseRowData.strength + gdMgr.PlayerDataAttr.equipStrength);
        Logger.LogFormat("总力量：{0}  基础力量：{1}   力量修正：{2}  装备附加力量：{3}", strength, unitBaseRowData.strength, unitRowData.strengthModifyRate, gdMgr.PlayerDataAttr.equipStrength);
        intelligence = (int)(unitRowData.intelligenceModifyRate * unitBaseRowData.intelligence + gdMgr.PlayerDataAttr.equipIntelligence);
        Logger.LogFormat("总智力：{0}  基础智力：{1}   智力修正：{2}  装备附加智力：{3}", intelligence, unitBaseRowData.intelligence, unitRowData.intelligenceModifyRate, gdMgr.PlayerDataAttr.equipIntelligence);
        speed = (int)(unitRowData.speedModifyRate * unitBaseRowData.speed + gdMgr.PlayerDataAttr.equipSpeed);
        Logger.LogFormat("总速度：{0}  基础速度：{1}   速度修正：{2}  装备附加速度：{3}", speed, unitBaseRowData.speed, unitRowData.speedModifyRate, gdMgr.PlayerDataAttr.equipSpeed);
        defense = (int)(unitRowData.defenseModifyRate * unitBaseRowData.defense + gdMgr.PlayerDataAttr.equipDefense);
        Logger.LogFormat("总防御：{0}  基础防御：{1}   防御修正：{2}  装备附加防御：{3}", defense, unitBaseRowData.defense, unitRowData.defenseModifyRate, gdMgr.PlayerDataAttr.equipDefense);
        endurance = (int)(unitRowData.enduranceModifyRate * unitBaseRowData.endurance + gdMgr.PlayerDataAttr.equipEndurance);
        Logger.LogFormat("总耐力：{0}  基础耐力：{1}   耐力修正：{2}  装备附加耐力：{3}", endurance, unitBaseRowData.endurance, unitRowData.enduranceModifyRate, gdMgr.PlayerDataAttr.equipEndurance);
        recovery = (int)(unitRowData.recoveryRate * unitBaseRowData.recovery);
        Logger.LogFormat("总战后回血：{0} 基础战后回血：{1}  战后回血比例修正：{2}", recovery, unitBaseRowData.recovery, unitRowData.recoveryRate);
        property = unitRowData.property;
        Logger.LogFormat("怪物属性：{0}  （1=金，2=木，3=水，4=火，5=土）", property);
        assetID = unitRowData.assetID;
        isEvolutionable = unitRowData.isEvolutionable!=0;
        evolutionID = unitRowData.evolutionID;
        name = unitRowData.nickName;
        //TODO: 玩家宠物不需要
        goldNoteMax = (int)(unitRowData.goldNoteMaxValueModifyRate * unitBaseRowData.goldNoteMax);
        goldNoteMin = (int)(unitRowData.goldNoteMinValueModifyRate * unitBaseRowData.goldNoteMin);
        expMin = (int)(unitRowData.expMinValueModifyRate * unitBaseRowData.expMin);
        expMax = (int)(unitRowData.expMaxValueModifyRate * unitBaseRowData.expMax);
		InitWeakPoint (unitRowData.weakpointList);
        //TODO: 装备系统附加值
        criticalRatio = gdMgr.PlayerDataAttr.criticalRatio;
        antiCriticalRatio = 0.0f;
        hitRatio = gdMgr.PlayerDataAttr.hitRatio;
        additionDamageRatio = 0.0f;
        minusDamageRatio = 0.0f;
        additionHealRatio = 0.0f;
        defensePierce = 0.0f;
        Logger.LogFormat("暴击率：{0}   暴击抗性：{1}    附加命中率：{2} 伤害加深：{3}    伤害减免：{4}    治疗加成：{5}    防御穿透：{6}", criticalRatio, antiCriticalRatio, hitRatio, additionDamageRatio, minusDamageRatio, additionHealRatio, defensePierce);
        Logger.LogFormat("========================|{0}|==============================", name);
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

		findWeakPointlist = new List<string> ();
		weakPointMeshDic = new Dictionary<string, GameObject> ();
		weakPointEffectDic = new Dictionary<string, GameObject> ();
		
    }

	void	InitWeakPoint(string strWeak)
	{
		if (strWeak == null || strWeak == "") 
		{
			return;
		}
		string[] weakArray = strWeak.Split (';');
		if (null == weakPointList) 
		{
			weakPointList = new List<string>();
		}

		weakPointList.Clear();
		for(int i = 0;i<weakArray.Length;++i)
		{
			weakPointList.Add(weakArray[i]);
		}
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
	/// 战斗对象AI
	/// </summary>
	/// <returns>The ai attack resul.</returns>
	public	BattleUnitAi.AiAttackResult  GetAiAttackResul()
	{
		return BattleUnitAi.Instance.GetAiAttackResult (this);
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
        var go = ResourceMgr.Instance.LoadAsset("monster", assetID);
        unitObject = GameObject.Instantiate(go);
        var com = unitObject.AddComponent<BattleObject>();
        com.camp = pbUnit.camp;
        com.id = pbUnit.guid;
        com.unit = this;

        //get slot position
        unitObject.transform.position = BattleScene.Instance.GetSlotPosition(pbUnit.camp, pbUnit.slot);

		//
		if (com.camp == UnitCamp.Enemy)
		{
			GameEventMgr.Instance.FireEvent<GameUnit> (GameEventList.LoadBattleObjectFinished, this);
		}

        ReCalcSpeed();

        Logger.LogFormat("Unit {0} guid:{1} has entered field", name, pbUnit.guid);
    }

    public void OnExitField()
    {
        GameObject.Destroy(unitObject);
        unitObject = null;

        Logger.LogFormat("Unit {0} guid:{1} has exited field", name, pbUnit.guid);

    }
}
