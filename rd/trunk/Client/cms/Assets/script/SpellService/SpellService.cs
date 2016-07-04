using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpellService : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------
    static SpellService mInst = null;
    private Dictionary<int, EventArgs> deadList;
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
        deadList = new Dictionary<int, EventArgs>();
    }
    //---------------------------------------------------------------------------------------------
    public void AddDeadData(SpellUnitDeadArgs args)
    {
        if (deadList.ContainsKey(args.deathID))
            return;

        deadList.Add(args.deathID, args);
    }
    //---------------------------------------------------------------------------------------------
    public void Update()
    {
        foreach (KeyValuePair<int, EventArgs> args in deadList)
        {
            TriggerEvent(GameEventList.SpellUnitDead, args.Value);
        }

        deadList.Clear();
    }
    //---------------------------------------------------------------------------------------------
    public void SpellRequest(string spellID, GameUnit caster, GameUnit target, float curTime)
    {
        Spell curSpell = caster.GetSpell(spellID);
        if (curSpell != null)
        {
            curSpell.Init(this);
            curSpell.casterID = caster.pbUnit.guid;
            curSpell.targetID = target.pbUnit.guid;
            curSpell.Apply(curTime);
        }

        //buff list可能会在update里被修改，只会被增加，删除buff下面单独处理，避免遍历出错
        for (int i = 0; i < caster.buffList.Count; ++i)
        {
            caster.buffList[i].Update(curTime);
        }

        for (int i = caster.buffList.Count - 1; i >= 0; --i)
        {
            if (caster.buffList[i].IsFinish)
            {
                caster.buffList.RemoveAt(i);
            }
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

            //trigger motion
            GameUnit caster = GetUnit(curArgs.casterID);
            if (caster != null)
            {
                BattleObject casterBo = caster.gameObject.GetComponent<BattleObject>();
                if (casterBo)
                {
                    casterBo.TriggerEvent(curArgs.spellID, curArgs.triggerTime);
                }
            }
        }
        else if (eventType == GameEventList.SpellLifeChange)
        {
            SpellVitalChangeArgs curArgs = args as SpellVitalChangeArgs;
            Logger.LogFormat(
                "[SpellService]{0} makes {1} damage/heal(critical {2}) to {3}, current life {4} wpID={5}",
                curArgs.casterID,
                curArgs.vitalChange,
                curArgs.isCritical,
                curArgs.targetID,
                curArgs.vitalCurrent,
                curArgs.wpID
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
            
            //trigger motion
            GameUnit deather = GetUnit(curArgs.deathID);
            if (deather != null)
            {
                BattleObject bo = deather.gameObject.GetComponent<BattleObject>();
                if (bo)
                {
                    bo.TriggerEvent("dead", curArgs.triggerTime);
                }
            }
        }
        else if (eventType == GameEventList.SpellBuff)
        {
            SpellBuffArgs curArgs = args as SpellBuffArgs;
            GameUnit target = GetUnit(curArgs.targetID);
            BattleObject bo = null;
            if (target != null)
            {
                bo = target.gameObject.GetComponent<BattleObject>();
            }
            if (curArgs.isAdd)
            {
                Logger.LogFormat("[SpellService]{0} cast buff {1} to {2}", curArgs.casterID, curArgs.buffID, curArgs.targetID);
                //trigger motion
                if (bo != null)
                {
                    bo.TriggerEvent(curArgs.buffID, curArgs.triggerTime);
                }
            }
            else
            {
                Logger.LogFormat("[SpellService]buff {0} removed from {1}", curArgs.buffID, curArgs.targetID);
                //trigger motion
                if (bo != null)
                {
                    bo.TriggerEvent(curArgs.buffID + "Finish", curArgs.triggerTime);
                }
            }
        }
        else if (eventType == GameEventList.SpellEffect)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            Logger.LogFormat("[SpellService]effect {0} triggered caster {1} target {2}", curArgs.effectID, curArgs.casterID, curArgs.targetID);
            //trigger motion
            GameUnit target = GetUnit(curArgs.targetID);
            BattleObject bo = null;
            if (target != null)
            {
                bo = target.gameObject.GetComponent<BattleObject>();
            }
            if (bo != null)
            {
                bo.TriggerEvent(curArgs.effectID, curArgs.triggerTime);
            }
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
            //trigger motion
            GameUnit target = GetUnit(curArgs.targetID);
            BattleObject bo = null;
            if (target != null)
            {
                bo = target.gameObject.GetComponent<BattleObject>();
            }
            if (bo != null)
            {
                bo.TriggerEvent("stun", curArgs.triggerTime);
            }
        }

        GameEventMgr.Instance.FireEvent<EventArgs>(eventType, args);
    }
    //---------------------------------------------------------------------------------------------
}
