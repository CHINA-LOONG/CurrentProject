using UnityEngine;
using System;
using System.Collections;
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
	public bool isBoss = false;
	public string Ai;
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
    public Dictionary<string, WeakPointRuntimeData> wpHpList;
	public Dictionary<string,GameObject> weakPointDumpDic;
	public List<string> findWeakPointlist = null;
	public Dictionary<string,GameObject> weakPointMeshDic;
	public Dictionary<string,GameObject> weakPointEffectDic;

    //只在客户端计算使用的属性
    float speedCount = 0;
    float actionOrder = 0;
    public float ActionOrder { get { return actionOrder; } }
    public string attackWpName = null;

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

    public static GameUnit FromPb(PbUnit unit, bool isPlayer)
    {
        var gameUnit = new GameUnit();
        gameUnit.pbUnit = unit;

        //初始化属性
        gameUnit.Init(isPlayer);

        return gameUnit;
    }

    void Init(bool isPlayer)
    {
		buffList = new List<Buff>();
		
		findWeakPointlist = new List<string> ();
		weakPointMeshDic = new Dictionary<string, GameObject> ();
		weakPointEffectDic = new Dictionary<string, GameObject> ();
		weakPointDumpDic = new Dictionary<string, GameObject> ();
		weakPointList = new List<string>();
		wpHpList = new Dictionary<string, WeakPointRuntimeData>();

        GameDataMgr gdMgr = GameDataMgr.Instance;
        UnitData unitRowData = StaticDataMgr.Instance.GetUnitRowData(pbUnit.id);
        UnitBaseData unitBaseRowData = StaticDataMgr.Instance.GetUnitBaseRowData(pbUnit.level);
        health = (int)(unitRowData.healthModifyRate * unitBaseRowData.health + gdMgr.PlayerDataAttr.equipHealth);
        strength = (int)(unitRowData.strengthModifyRate * unitBaseRowData.strength + gdMgr.PlayerDataAttr.equipStrength);
        intelligence = (int)(unitRowData.intelligenceModifyRate * unitBaseRowData.intelligence + gdMgr.PlayerDataAttr.equipIntelligence);
        speed = (int)(unitRowData.speedModifyRate * unitBaseRowData.speed + gdMgr.PlayerDataAttr.equipSpeed);
        defense = (int)(unitRowData.defenseModifyRate * unitBaseRowData.defense + gdMgr.PlayerDataAttr.equipDefense);
        endurance = (int)(unitRowData.enduranceModifyRate * unitBaseRowData.endurance + gdMgr.PlayerDataAttr.equipEndurance);
        recovery = (int)(unitRowData.recoveryRate * unitBaseRowData.recovery);
        property = unitRowData.property;
        assetID = unitRowData.assetID;
        isEvolutionable = unitRowData.isEvolutionable!=0;
        evolutionID = unitRowData.evolutionID;
        name = unitRowData.nickName;
		Ai = unitRowData.AI;

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

        //不是玩家宠物有副本加成 
        if (isPlayer == false)
        {
            InstanceData instData = BattleController.Instance.InstanceData;
            //掉落
            goldNoteMax = (int)(unitRowData.goldNoteMaxValueModifyRate * unitBaseRowData.goldNoteMax * instData.goldCoef);
            goldNoteMin = (int)(unitRowData.goldNoteMinValueModifyRate * unitBaseRowData.goldNoteMin * instData.goldCoef);
            expMin = (int)(unitRowData.expMinValueModifyRate * unitBaseRowData.expMin * instData.expCoef);
            expMax = (int)(unitRowData.expMaxValueModifyRate * unitBaseRowData.expMax * instData.expCoef);
            //弱点
            InitWeakPoint(unitRowData.weakpointList);
            //基础属性影响
            strength = (int)(strength * instData.attackCoef);
            intelligence = (int)(intelligence * instData.attackCoef);
            health = (int)(health * instData.lifeCoef);
        }

        //计算二级属性
        curLife = (int)SpellConst.healthToLife * health;
        maxLife = curLife;
        magicAttack = (int)(SpellConst.strengthToAttack * strength);
        phyAttack = (int)(SpellConst.intelligenceToAttack * intelligence);

        //初始化技能列表
        spellList = new Dictionary<string, Spell>();
        //string[] spellIDList = unitRowData.spellIDList.Split(';');
		ArrayList spellArrayList = MiniJsonExtensions.arrayListFromJson (unitRowData.spellIDList);

        SpellProtoType spellPt = null;
		for (int i = 0; i < spellArrayList.Count; ++i)
        {
			string spellID = spellArrayList[i]as string ;
			spellPt = StaticDataMgr.Instance.GetSpellProtoData(spellID);
            if (spellPt != null)
            {
				spellList.Add(spellID, new Spell(spellPt));
            }
        }	
	}
	
	void InitWeakPoint(string strWeak)
	{
		if (strWeak == null || strWeak == "") 
		{
			return;
		}
		ArrayList weakArrayList = MiniJsonExtensions.arrayListFromJson (strWeak);

		weakPointList.Clear();
        wpHpList.Clear();
        WeakPointData wpData = null;
		for(int i = 0;weakArrayList !=null && i<weakArrayList.Count;++i)
		{
            wpData = StaticDataMgr.Instance.GetWeakPointData(weakArrayList[i] as string);
            if (wpData != null)
            {
                weakPointList.Add(wpData.id);
                WeakPointRuntimeData wpRuntimeData = new WeakPointRuntimeData();
                wpRuntimeData.id = wpData.id;
                wpRuntimeData.maxHp = wpRuntimeData.hp = wpData.health;
                wpHpList.Add(wpData.id, wpRuntimeData);
            }
		}
	}

    public void OnDamageWeakPoint(string id, int damage)
    {
        WeakPointRuntimeData wpRuntimeData = null;
        if (wpHpList.TryGetValue(id, out wpRuntimeData))
        {
            wpRuntimeData.hp += damage;
            if (wpRuntimeData.hp < 0)
            {
                wpRuntimeData.hp = 0;
				GameObject meshObj = null;
				if(weakPointMeshDic.TryGetValue(id,out meshObj))
				{
					meshObj.SetActive(false);
				}
                //Logger.LogError("TODO: weak point is dead (lws)");
            }
            else if (wpRuntimeData.hp > wpRuntimeData.maxHp)
            {
                wpRuntimeData.hp = wpRuntimeData.maxHp;
            }
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

    public Spell GetDazhao()
    {
        foreach (var item in spellList)
        {
            if (item.Value.spellData.category == (int)SpellType.Spell_Type_DaZhao)
                return item.Value;
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
        //Logger.LogFormat("Unit {0}: speedCount: {1}, actionOrder: {2}", name, speedCount, actionOrder);
    }

    /// <summary>
    /// 不累计速度，计算order值
    /// </summary>
    public void ReCalcSpeed()
    {
        speedCount = speed * UnityEngine.Random.Range(BattleConst.speedFactorMin, BattleConst.speedFactorMax);
        actionOrder = BattleConst.speedK / speedCount;
        //Logger.LogFormat("Unit {0}: speedCount: {1}, actionOrder: {2}", name, speedCount, actionOrder);
    }

    public void OnEnterField()
    {
        var go = ResourceMgr.Instance.LoadAsset("monster", assetID);
        unitObject = GameObject.Instantiate(go);
        var com = unitObject.AddComponent<BattleObject>();
        com.camp = pbUnit.camp;
        com.id = pbUnit.guid;
        com.unit = this;
        com.aniControl = unitObject.AddComponent<AnimControl>();

        //get slot position
        unitObject.transform.position = BattleScene.Instance.GetSlotPosition(pbUnit.camp, pbUnit.slot);
		unitObject.transform.localEulerAngles = BattleScene.Instance.GetSlotLocalEuler(pbUnit.camp, pbUnit.slot);

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
