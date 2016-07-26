using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum UnitState
{
    None,
    Dazhao,
    ToBeExit,
    ToBeEnter,
    Dead,
}

//////////////////////////////////////////////////////////////////////////
/// <summary>
/// 模拟战斗协议，只做测试用途
/// </summary>
/// 
[Serializable]
public class PbUnit
{
    //站位，0，1，2表示，-1表示在场下
    public int slot;
    public int guid;
    public UnitCamp camp;
    //静态数据id，可以通过此id获取prefab等
    public string id;
    //性格
	public int character;
	public int lazy;
    public int level;
    public int curExp;
    public int stage;//升星
    public List<PB.HSSkill> spellPbList;
}

//[Serializable]
public class GameUnit : IComparable
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
	public int bossType = 0;
	//public string Ai;
	public int character;//性格
	public int lazy;//勤奋度
	public bool isVisible = true;//是否可见
	public int friendship;
    public string closeUp;
    //掉落金币
    //public int goldNoteMin;
  //  public int goldNoteMax;
    //掉落经验
  //  public int expMin;
  //  public int expMax;
    //public int outputExp;//被吃掉产出
    public int currentExp;//当前经验值

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
    public int invincible = 0;//无敌
    public int stun = 0;//眩晕
    public int dazhao = 0;//AI释放大招状态
    public int dazhaoPrepareCount = 0;//准备大招需要的回合
    public int dazhaoDamageCount=0;//统计AI释放大招模式下受到的物理伤害次数
    public int energy;//能量值
    public float spellHealthRatio;
    public float spellStrengthRatio;
    public float spellIntelligenceRatio;
    public float spellSpeedRatio;
    public float spellDefenseRatio;
    public float spellEnduranceRatio;
    public float spellDefenseDamageRatio;
    public float spellphyReduceInjury;//物理减伤
    public float spellmgReduceInjury;//法术减伤
    public float spellPhyShield;//物理护盾
    public float spellMagicShield;//法术护盾
    public int tauntTargetID = BattleConst.battleSceneGuid;//sceneGuid means no taunt target

    //二级属性
    public int curLife;
    public int maxLife;
    public int magicAttack;
    public int phyAttack;

    public List<Buff> buffList;
    public Dictionary<string, Spell> spellList;
    //public List<Equipment> equipmentList;
    public List<string> weakPointList;
    //add: xiaolong 2015-9-9 15:41:15
    public EquipData[] equipList = new EquipData[6];

    //只在客户端计算使用的属性
    float lastActionOrder = 0.0f;
    float actionOrder = 0.0f;
    public float ActionOrder { get { return actionOrder; } }
    public string attackWpName = null;

    UnitState state = UnitState.None;
    public UnitState State { get { return state; } set { state = value; } }
    public bool backUp = false;

    //显示数据
    public Sprite headImg;
    public BattleObject battleUnit;

	//战斗单元统计数据 
	public	int attackCount = 0;
	public 	List<int> lazyList = new List<int>();
	public	List<int> dazhaoList = new List<int>();

    public static GameUnit FromPb(PbUnit unit, bool isPlayer)
    {
        var gameUnit = new GameUnit();
        gameUnit.pbUnit = unit;

        //初始化属性
        gameUnit.Init(isPlayer);

        return gameUnit;
    }

    //create a fake monster for view only
    public static GameUnit CreateFakeUnit(string unitID)
    {
        PbUnit pbUnit = new PbUnit();
        pbUnit.guid = BattleConst.enemyStartID;
        pbUnit.id = unitID;
        pbUnit.level = 1;
        pbUnit.camp = UnitCamp.Player;
        pbUnit.slot = 0;
        pbUnit.lazy = BattleConst.defaultLazy;

        var gameUnit = new GameUnit();
        gameUnit.pbUnit = pbUnit;
        UnitData unitRowData = StaticDataMgr.Instance.GetUnitRowData(unitID);
        gameUnit.assetID = unitRowData.assetID;
        gameUnit.name = unitRowData.NickNameAttr;

        return gameUnit;
    }

    void Init(bool isPlayer)
    {
		buffList = new List<Buff>();

		weakPointList = new List<string>();
		//wpHpList = new Dictionary<string, WeakPointRuntimeData>();

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
        //test only
        if (assetID.Contains("boss"))
        {
            isBoss = true;
        }

        isEvolutionable = unitRowData.isEvolutionable!=0;
        evolutionID = unitRowData.evolutionID;
        name = unitRowData.NickNameAttr;
		//Ai = unitRowData.AI;

        //TODO: 装备系统附加值
        criticalRatio = gdMgr.PlayerDataAttr.criticalRatio;
        antiCriticalRatio = 0.0f;
        hitRatio = gdMgr.PlayerDataAttr.hitRatio;
        additionDamageRatio = 0.0f;
        minusDamageRatio = 0.0f;
        additionHealRatio = 0.0f;
        defensePierce = 0.0f;
        spellphyReduceInjury = 0.0f;
        spellmgReduceInjury = 0.0f;
        spellPhyShield = 0;
        spellMagicShield = 0;

        //战斗状态值初始化
        invincible = 0;
        stun = 0;
        energy = 0;
        
        spellHealthRatio = 0.0f;
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
            //goldNoteMax = (int)(unitRowData.goldNoteMaxValueModifyRate * unitBaseRowData.goldNoteMax * instData.goldCoef);
            //goldNoteMin = (int)(unitRowData.goldNoteMinValueModifyRate * unitBaseRowData.goldNoteMin * instData.goldCoef);
          //  expMin = (int)(unitRowData.expMinValueModifyRate * unitBaseRowData.expMin * instData.expCoef);
           // expMax = (int)(unitRowData.expMaxValueModifyRate * unitBaseRowData.expMax * instData.expCoef);
            //弱点
            InitWeakPoint(unitRowData.weakpointList);
            //基础属性影响
            strength = (int)(strength * instData.instanceProtoData.attackCoef);
            intelligence = (int)(intelligence * instData.instanceProtoData.attackCoef);
            health = (int)(health * instData.instanceProtoData.lifeCoef);
        }
        else
        {
            headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(unitRowData.uiAsset) as Sprite;
        }

        //计算二级属性
        curLife = (int)SpellConst.healthToLife * health;
        maxLife = curLife;
        magicAttack = (int)(SpellConst.strengthToAttack * intelligence);
        phyAttack = (int)(SpellConst.intelligenceToAttack * strength);

        //初始化技能列表
        spellList = new Dictionary<string, Spell>();
        SpellProtoType spellPt = null;

        if (pbUnit.spellPbList != null)
        {
            int skillCount = pbUnit.spellPbList.Count;
            for (int skillIndex = 0; skillIndex < skillCount; ++skillIndex)
            {
                string spellID = pbUnit.spellPbList[skillIndex].skillId;
                spellPt = StaticDataMgr.Instance.GetSpellProtoData(spellID);
                if (spellPt != null)
                {
                    spellList.Add(spellID, new Spell(spellPt, pbUnit.spellPbList[skillIndex].level));
                }
            }
        }
        //TODO: remove, for fake enemy
        else
        {
            ArrayList spellArrayList = MiniJsonExtensions.arrayListFromJson(unitRowData.spellIDList);
            for (int i = 0; i < spellArrayList.Count; ++i)
            {
                string spellID = spellArrayList[i] as string;
                spellPt = StaticDataMgr.Instance.GetSpellProtoData(spellID);
                if (spellPt != null)
                {
                    if (
                        spellPt.category == (int)SpellType.Spell_Type_Defense ||
                        spellPt.category == (int)SpellType.Spell_Type_Passive ||
                        spellPt.category == (int)SpellType.Spell_Type_Beneficial ||
                        spellPt.category == (int)SpellType.Spell_Type_Negative ||
                        spellPt.category == (int)SpellType.Spell_Type_Lazy ||
                        spellPt.category == (int)SpellType.Spell_Type_PrepareDazhao ||
                        spellPt.category == (int)SpellType.Spell_Type_Hot
                        )
                    {
                        spellList.Add(spellID, new Spell(spellPt, 1));
                    }
                    else
                    {
                        spellList.Add(spellID, new Spell(spellPt, pbUnit.level));
                    }
                }
            }	
        }


		//性格，勤奋度,//怪物友好度
      
        lazy = pbUnit.lazy;
        closeUp = unitRowData.closeUp;
		if (isPlayer) 
		{
			friendship = 0;//player no use
			character = pbUnit.character;
		} 
		else 
		{
			friendship = unitRowData.friendship;
			character = unitRowData.character;
		}
	}

    public void LevelUp(int targetLvl)
    {
        pbUnit.level = targetLvl;
        //TODO: recalculate attribute
    }
	
	void InitWeakPoint(string strWeak)
	{
		if (strWeak == null || strWeak == "") 
		{
			return;
		}
		ArrayList weakArrayList = MiniJsonExtensions.arrayListFromJson (strWeak);

		weakPointList.Clear();
       
        WeakPointData wpData = null;
		for(int i = 0;weakArrayList !=null && i<weakArrayList.Count;++i)
		{
            wpData = StaticDataMgr.Instance.GetWeakPointData(weakArrayList[i] as string);
            if (wpData != null)
            {
                weakPointList.Add(wpData.id);

				if(!isBoss)
				{
					if(wpData.initialStatus == (int)WeakpointState.Hide)
					{
						isVisible = false;
					}
				}
            }
		}
	}

    public void OnDamageWeakPoint(string id, int damage, float damageTime)
    {
        WeakPointRuntimeData wpRuntimeData = null;
        if (battleUnit.wpGroup.allWpDic.TryGetValue(id, out wpRuntimeData))
        {
			if(wpRuntimeData.hp <=0)
			{
				return ;
			}

            wpRuntimeData.hp += damage;
            if (wpRuntimeData.hp <= 0)
            {
                wpRuntimeData.hp = 0;
                WeakPointDeadArgs deadArgs = new WeakPointDeadArgs();
                deadArgs.triggerTime = damageTime;
                deadArgs.targetID = pbUnit.guid;
                deadArgs.wpID = id;
                GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.WeakpoingDead, deadArgs);
            }
            else if (wpRuntimeData.hp > wpRuntimeData.maxHp)
            {
                wpRuntimeData.hp = wpRuntimeData.maxHp;
            }
        }
    }

    public void SetWpDead(EventArgs sArgs)
    {
        WeakPointDeadArgs wpDeadArgs = sArgs as WeakPointDeadArgs;
        Logger.LogFormat("weakpoint( ) dead!", wpDeadArgs.wpID);
       
		WeakPointRuntimeData wpRuntimeData = null;
		if (battleUnit.wpGroup.allWpDic.TryGetValue (wpDeadArgs.wpID, out wpRuntimeData))
		{
			wpRuntimeData.ChangeState(WeakpointState.Dead);
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
			if (item.Value.spellData.category == (int)SpellType.Spell_Type_PhyDaZhao)
                return item.Value;

			if (item.Value.spellData.category == (int)SpellType.Spell_Type_MagicDazhao)
				return item.Value;
        }

        return null;
    }

	public	Spell GetSpellWithType(SpellType spellType)
	{
		foreach (var item in spellList)
		{
			if(item.Value.spellData.category == (int) spellType)
				return item.Value;
		}
		return null;
	}

    /// <summary>
    /// 计算下次行动序列
    /// </summary>
    public void CalcNextActionOrder(float fixPreOrder = -1.0f)
    {
        float nextOrderCost = BattleConst.speedK / (speed * (1.0f + spellSpeedRatio) * UnityEngine.Random.Range(BattleConst.speedFactorMin, BattleConst.speedFactorMax));
        if (fixPreOrder > 0.0f)
        {
            lastActionOrder = fixPreOrder;
            actionOrder = fixPreOrder + nextOrderCost;
        }
        else
        {
            lastActionOrder = actionOrder;
            actionOrder += nextOrderCost;
        }
    }

    /// <summary>
    /// 重新计算本次行动序列（速度改变时）
    /// </summary>
    public void RecalcCurActionOrder(float fixPreOrder = -1.0f)
    {
        if (fixPreOrder > 0.0f)
        {
            lastActionOrder = actionOrder = fixPreOrder;
        }
        else
        {
            float nextOrderCost = BattleConst.speedK / (speed * (1.0f + spellSpeedRatio) * UnityEngine.Random.Range(BattleConst.speedFactorMin, BattleConst.speedFactorMax));
            actionOrder = lastActionOrder + nextOrderCost;
        }
    }

    //public void RemoveAllBuff()
    //{
    //    foreach (Buff buff in buffList)
    //    {
    //        buff.Finish();
    //    }
    //}
    public void ResetAcionOrder()
    {
        actionOrder = lastActionOrder = 0.0f;
    }

    public void OnStartNextProcess()
    {
        if (curLife <= 0 || state == UnitState.Dead)
        {
            return;
        }

        ResetAcionOrder();
        //切对局清buff
        int buffCout = buffList.Count;
        for (int index = 0; index < buffCout; ++index)
        {
            buffList[index].Finish(Time.time);
        }
        buffList.Clear();
        invincible = 0;
        stun = 0;
        dazhao = 0;
        dazhaoPrepareCount = 0;
        if (pbUnit.slot == BattleConst.offsiteSlot)
        {
            curLife += recovery;
            if (curLife > maxLife)
            {
                curLife = maxLife;
            }
            return;
        }

        //战后回血
        SpellVitalChangeArgs args = new SpellVitalChangeArgs();
        args.vitalType = (int)VitalType.Vital_Type_Default;
        //TODO: use battle time
        args.triggerTime = Time.time;
        args.casterID = BattleConst.battleSceneGuid;
        args.targetID = pbUnit.guid;
        args.isCritical = false;
        args.vitalChange = recovery;
        args.vitalCurrent = curLife + recovery;
        if (args.vitalCurrent >= maxLife)
        {
            args.vitalCurrent = maxLife;
        }
        args.vitalMax = maxLife;
        GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.SpellLifeChange, args);
        //battleUnit.TriggerEvent(BattleConst.levelChangeEvent, Time.time, null);
    }

    public void ResetAllState(bool isRevive)
    {
        backUp = false;
        invincible = 0;
        stun = 0;
        energy = 0;
        dazhao = 0;
        dazhaoPrepareCount = 0;
        spellHealthRatio = 0.0f;
        spellStrengthRatio = 0.0f;
        spellIntelligenceRatio = 0.0f;
        spellSpeedRatio = 0.0f;
        spellDefenseRatio = 0.0f;
        spellEnduranceRatio = 0.0f;
        spellDefenseDamageRatio = 0.0f;
        spellphyReduceInjury = 0.0f;
        spellmgReduceInjury = 0.0f;
        spellPhyShield = 0;
        spellMagicShield = 0;

        //二级属性
        curLife = maxLife;

        if (isRevive == false)
        {
            lastActionOrder = 0;
            actionOrder = 0;
        }
        attackWpName = null;

        state = UnitState.None;
	    attackCount = 0;
        buffList.Clear();
    }

    public void OnDead()
    {
        battleUnit.ClearEvent();
        buffList.Clear();
        invincible = 0;
        stun = 0;
        energy = 0;
        dazhao = 0;
        dazhaoPrepareCount = 0;
    }

    public bool IsPrepareDazhao()
    {
        int count = buffList.Count;
        for (int i = 0; i < count; ++i)
        {
            if (buffList[i].buffProto.category == (int)(BuffType.Buff_Type_Dazhao))
            {
                return true;
            }
        }

        return false;
    }

    public int CompareTo(object obj)
    {
        int result;
        try
        {
            GameUnit target = obj as GameUnit;

            if (this.pbUnit.level == target.pbUnit.level)
            {
                int selfGrade = StaticDataMgr.Instance.GetUnitRowData(this.pbUnit.id).grade;
                int targetGrade = StaticDataMgr.Instance.GetUnitRowData(target.pbUnit.id).grade;
                if (selfGrade == targetGrade)
                {
                    if (this.pbUnit.stage == target.pbUnit.stage)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = this.pbUnit.stage > target.pbUnit.stage ? -1 : 1;
                    }
                }
                else
                {
                    result = selfGrade > targetGrade ? -1 : 1;
                }
            }
            else
            {
                result = this.pbUnit.level > target.pbUnit.level ? -1 : 1;
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }

    public void CastPassiveSpell(float triggerTime)
    {
        var itor = spellList.GetEnumerator();
        while (itor.MoveNext())
        {
            Spell curSpell = itor.Current.Value;
            if (curSpell.spellData.category == (int)SpellType.Spell_Type_Passive && string.IsNullOrEmpty(curSpell.spellData.firstSpell) == true)
            {
                SpellService.Instance.SpellRequest(curSpell.spellData.id, this, this, triggerTime);
            }
        }
    }

    public string GetFirstSpell()
    {
        var itor = spellList.GetEnumerator();
        while (itor.MoveNext())
        {
            Spell curSpell = itor.Current.Value;
            if (curSpell.spellData.category == (int)SpellType.Spell_Type_Passive && string.IsNullOrEmpty(curSpell.spellData.firstSpell) == false)
            {
                return curSpell.spellData.firstSpell;
            }
        }

        return null;
    }

    public void OnHealthChange(float curTime)
    {
        //NOTE: health changes only through passive spell,so don't think life compensate
        float curMaxLife = maxLife;
        maxLife = (int)(SpellConst.healthToLife * health * (1.0f + spellHealthRatio));
        if (state != UnitState.Dead)
        {
            curLife = maxLife;
        }

        //fire vitalchange event to modify lifebar
        SpellVitalChangeArgs args = new SpellVitalChangeArgs();
        args.vitalType = (int)VitalType.Vital_Type_FixLife;
        args.triggerTime = curTime;
        args.casterID = pbUnit.guid;
        args.targetID = pbUnit.guid;
        args.isCritical = false;
        args.vitalChange = 0;
        args.vitalCurrent = curLife;
        args.vitalMax = maxLife;
        GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.SpellLifeChange, args);
    }

}
