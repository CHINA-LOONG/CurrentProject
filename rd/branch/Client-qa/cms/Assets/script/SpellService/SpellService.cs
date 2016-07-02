using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpellService : MonoBehaviour
{
    //test only
    List<GameUnit> playerList;
    List<GameUnit> enemyList;
    //---------------------------------------------------------------------------------------------
    static SpellService mInst = null;
    public static SpellService Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("SpellService");
                mInst = go.AddComponent<SpellService>();
            }
            return mInst;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Start()
    {
    }
    //---------------------------------------------------------------------------------------------
    public void Init()
    {
        //playerList = new List<GameUnit>();
        //enemyList = new List<GameUnit>();

        //PbUnit pu1 = new PbUnit();
        //pu1.id = "soul";
        //pu1.slot = 0;
        //pu1.guid = 0;
        //pu1.camp = UnitCamp.Player;
        //pu1.personality = 0;
        //pu1.level = 15;
        //pu1.curExp = 0;
        //pu1.starLevel = 0;
        //GameUnit unit1 = GameUnit.FromPb(pu1);
        //playerList.Add(unit1);

        //PbUnit pu2 = new PbUnit();
        //pu2.id = "soul";
        //pu2.slot = 1;
        //pu2.guid = 1;
        //pu2.camp = UnitCamp.Enemy;
        //pu2.personality = 1;
        //pu2.level = 15;
        //pu2.curExp = 0;
        //pu2.starLevel = 0;
        //GameUnit unit2 = GameUnit.FromPb(pu2);
        //enemyList.Add(unit2);

        //SpellRequest("s1", playerList[0], enemyList[0], 0);
    }
    //---------------------------------------------------------------------------------------------
    public void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            //buffList["b1"].Update(0);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SpellRequest(string spellID, GameUnit caster, GameUnit target, int curTime)
    {
        Spell curSpell = caster.GetSpell(spellID);
        curSpell.Init(this);
        if (curSpell != null)
        {
            curSpell.casterID = caster.pbUnit.guid;
            curSpell.targetID = target.pbUnit.guid;
            curSpell.Apply(curTime);
        }

        foreach (Buff bf in caster.buffList)
        {
            bf.Update(curTime);
        }
    }
    //---------------------------------------------------------------------------------------------
    //remove all buffs of the unit
    public void ResetUnit()
    {

    }
    //---------------------------------------------------------------------------------------------
    public Effect GetEffect(string id)
    {
        EffectPrototype effectPt = StaticDataMgr.Instance.GetEffectProtoData(id);
        EffectType effectType = (EffectType)(effectPt.effectType);
        Effect actualEffect = null;
        switch (effectType)
        {
            case EffectType.Effect_Type_Set:
                {
                    EffectSetPrototype setPt = effectPt as EffectSetPrototype;
                    actualEffect = new EffectSet();
                    actualEffect.Init(setPt, this);
                }
                break;
            case EffectType.Effect_Type_Search:
                {
                    EffectSearchPrototype pt = effectPt as EffectSearchPrototype;
                    actualEffect = new EffectSearch();
                    actualEffect.Init(pt, this);
                }
                break;
            case EffectType.Effect_Type_Persistent:
                {
                    EffectPersistentProtoType pt = effectPt as EffectPersistentProtoType;
                    actualEffect = new EffectPersistent();
                    actualEffect.Init(pt, this);
                }
                break;
            case EffectType.Effect_Type_Damage:
                {
                    EffectDamageProtoType pt = effectPt as EffectDamageProtoType;
                    actualEffect = new EffectDamage();
                    actualEffect.Init(pt, this);
                }
                break;
            case EffectType.Effect_Type_Buff:
                {
                    EffectApplyBuffPrototype pt = effectPt as EffectApplyBuffPrototype;
                    actualEffect = new EffectApplyBuff();
                    actualEffect.Init(pt, this);
                }
                break;
        }

        return actualEffect;
    }
    //---------------------------------------------------------------------------------------------
    public Buff GetBuff(string id)
    {
        BuffPrototype buffPt = StaticDataMgr.Instance.GetBuffProtoData(id);
        Buff actualBuff = new Buff();
        actualBuff.Init(buffPt, this);
        return actualBuff;
    }
    //---------------------------------------------------------------------------------------------
    public List<GameUnit> GetUnitList(int camp)
    {
        var group = BattleController.Instance.BattleGroup;
        return camp == 0 ? group.PlayerFieldList : group.EnemyFieldList;
    }
    //---------------------------------------------------------------------------------------------
    public GameUnit GetUnit(int unitID)
    {
        return BattleController.Instance.BattleGroup.GetUnitByGuid(unitID);
    }
    //---------------------------------------------------------------------------------------------
    public void TriggerEvent(string eventType, EventArgs args)
    {
        if (eventType == GameEventList.SpellFire)
        {
            SpellFireArgs curArgs = args as SpellFireArgs;
            Logger.LogFormat("[SpellService]{0} fire spell {1}", curArgs.casterID, curArgs.spellID);
        }
        else if (eventType == GameEventList.SpellLifeChange)
        {
            SpellVitalChangeArgs curArgs = args as SpellVitalChangeArgs;
            Logger.LogFormat(
                "[SpellService]{0} makes {1} damage/heal(critical {2}) to {3}, current life {4}",
                curArgs.casterID,
                curArgs.vitalChange,
                curArgs.isCritical,
                curArgs.targetID,
                curArgs.vitalCurrent
                );

        }
        else if (eventType == GameEventList.SpellEnergyChange)
        {
            SpellVitalChangeArgs curArgs = args as SpellVitalChangeArgs;
            Logger.LogFormat(
                "[SpellService]{0}'s energy changes {1}, current energy {2}",
                curArgs.casterID,
                curArgs.vitalChange,
                curArgs.vitalCurrent
                );
        }
        else if (eventType == GameEventList.SpellUnitDead)
        {
            SpellUnitDeadArgs curArgs = args as SpellUnitDeadArgs;
            Logger.LogFormat("[SpellService]{0} killed {1}", curArgs.casterID, curArgs.deathID);
        }
        else if (eventType == GameEventList.SpellBuff)
        {
            SpellBuffArgs curArgs = args as SpellBuffArgs;
            if (curArgs.isAdd)
            {
                Logger.LogFormat("[SpellService]{0} cast buff {1} to {2}", curArgs.casterID, curArgs.buffID, curArgs.targetID);
            }
            else
            {
                Logger.LogFormat("[SpellService]buff {0} removed from {1}", curArgs.buffID, curArgs.targetID);
            }
        }
        else if (eventType == GameEventList.SpellEffect)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            Logger.LogFormat("[SpellService]effect {0} triggered caster {1} target {2}", curArgs.effectID, curArgs.casterID, curArgs.targetID);
        }
        else if (eventType == GameEventList.SpellMiss)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            Logger.LogFormat("[SpellService]effect {0} missed caster {1} target {2}", curArgs.effectID, curArgs.casterID, curArgs.targetID);
        }
        else if (eventType == GameEventList.SpellImmune)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            Logger.LogFormat("[SpellService]effect {0} immuned caster {1} target {2}", curArgs.effectID, curArgs.casterID, curArgs.targetID);
        }
        else if (eventType == GameEventList.SpellStun)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            Logger.LogFormat("[SpellService]{0} is stun", curArgs.targetID);
        }

        GameEventMgr.Instance.FireEvent<EventArgs>(eventType, args);
    }
    //---------------------------------------------------------------------------------------------
}
