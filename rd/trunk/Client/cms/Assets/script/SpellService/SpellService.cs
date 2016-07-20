using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpellService : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------
    static SpellService mInst = null;
    struct DeadData
    {
        public EventArgs deadEventArgs;
        public GameUnit deadUnit;
        public EffectDamage causeDeadEffect;
    }
    private Dictionary<int, DeadData> deadList = new Dictionary<int, DeadData>();
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

    Spell mCurActionSpell = null;
    //---------------------------------------------------------------------------------------------
    public void Start()
    {
    }
    //---------------------------------------------------------------------------------------------
    public void Init()
    {
    }
    //---------------------------------------------------------------------------------------------
    public void AddDeadData(SpellUnitDeadArgs args, GameUnit deadUnit, EffectDamage causeDeadEffect)
    {
        DeadData deadInfo;
        if (deadList.TryGetValue(args.deathID, out deadInfo))
        {
            SpellUnitDeadArgs curDeadEventArgs = deadInfo.deadEventArgs as SpellUnitDeadArgs;
            if (curDeadEventArgs.triggerTime < args.triggerTime)
            {
                curDeadEventArgs.triggerTime = args.triggerTime;
            }

            return;
        }

        deadInfo = new DeadData();
        deadInfo.deadEventArgs = args;
        deadInfo.deadUnit = deadUnit;
        deadInfo.causeDeadEffect = causeDeadEffect;
        deadList.Add(args.deathID, deadInfo);
    }
    //---------------------------------------------------------------------------------------------
    public bool IsInDeathList(int guid)
    {
        return deadList.ContainsKey(guid);
    }
    //---------------------------------------------------------------------------------------------
    public void Update()
    {
        //DeadData deadData;
        //List<Buff> buffList;
        //var itor = deadList.GetEnumerator();
        //while(itor.MoveNext())
        //{
        //    deadData = itor.Current.Value;
        //    if (deadData.deadUnit != null)
        //    {
        //        buffList = deadData.deadUnit.buffList;
        //        for (int i = 0; i < buffList.Count; ++i)
        //        {
        //            //TODO: use level time
        //            buffList[i].DeadResponse(Time.time, itor.Current.Value.causeDeadEffect);
        //        }
        //    }
        //}

        foreach (KeyValuePair<int, DeadData> deadInfo in deadList)
        {
            if (deadInfo.Value.deadUnit.curLife <= 0)
            {
                TriggerEvent(GameEventList.SpellUnitDead, deadInfo.Value.deadEventArgs);
            }
        }

        deadList.Clear();
    }
    //---------------------------------------------------------------------------------------------
    public void SpellRequest(string spellID, GameUnit caster, GameUnit target, float curTime, bool isFirstSpell = false)
    {
		mCurActionSpell = null;
        Spell curSpell = caster.GetSpell(spellID);
        if (caster.stun <= 0)
        {
            if (curSpell != null)
            {
                mCurActionSpell = curSpell;
                curSpell.Init(this);
                curSpell.casterID = caster.pbUnit.guid;
                curSpell.targetID = target.pbUnit.guid;
                curSpell.Apply(curTime, target.attackWpName, isFirstSpell);
            }
        }
        else 
        {
            //generate spell event
            if (curSpell != null)
            {
                if (caster.pbUnit.camp == UnitCamp.Enemy || curSpell.spellData.category != (int)SpellType.Spell_Type_MagicDazhao)
                {
                    SpellFireArgs args = new SpellFireArgs();
                    args.triggerTime = curTime;
                    args.casterID = caster.pbUnit.guid;
                    args.spellID = null;
                    args.aniTime = SpellConst.aniDelayTime;
                    args.firstSpell = false;
                    TriggerEvent(GameEventList.SpellFire, args);
                }
            }
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
		if (string.IsNullOrEmpty (id))
			return null;

        EffectPrototype effectPt = StaticDataMgr.Instance.GetEffectProtoData(id);
        if (effectPt == null)
            return null;

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
            case EffectType.Effect_Type_Switch:
                {
                    EffectSwitchPrototype pt = effectPt as EffectSwitchPrototype;
                    actualEffect = new EffectSwitch();
                    actualEffect.Init(pt, this);
                }
                break;
            case EffectType.Effect_Type_Dispel:
                {
                    EffectDispelProtoType pt = effectPt as EffectDispelProtoType;
                    actualEffect = new EffectDispel();
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
        if (buffPt == null)
            return null;

        Buff actualBuff = new Buff();
        actualBuff.Init(buffPt, this);
        return actualBuff;
    }
    //---------------------------------------------------------------------------------------------
    public List<BattleObject> GetUnitList(int camp)
    {
        var group = BattleController.Instance.BattleGroup;
        return camp == 0 ? group.PlayerFieldList : group.EnemyFieldList;
    }
    //---------------------------------------------------------------------------------------------
    public GameUnit GetUnit(int unitID)
    {
        BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(unitID);
        if (bo != null)
        {
            return bo.unit;
        }

        return null;
    }
    //---------------------------------------------------------------------------------------------
    public void TriggerEvent(string eventType, EventArgs args)
    {
        if (eventType == GameEventList.SpellFire)
        {
            SpellFireArgs curArgs = args as SpellFireArgs;
            Logger.LogFormat("[SpellService]{0} fire spell {1}", curArgs.casterID, curArgs.spellID);

            //trigger motion
            BattleObject caster = ObjectDataMgr.Instance.GetBattleObject(curArgs.casterID);
            if (caster != null && string.IsNullOrEmpty(curArgs.spellID) == false)
            {
                if (caster.camp == UnitCamp.Enemy || curArgs.category != (int)SpellType.Spell_Type_MagicDazhao)
                {
                    caster.TriggerEvent(curArgs.spellID, curArgs.triggerTime, null);
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
            
            ////trigger motion
            //BattleObject deather = ObjectDataMgr.Instance.GetBattleObject(curArgs.deathID);
            //if (deather != null)
            //{
            //    //deather.TriggerEvent("dead", curArgs.triggerTime);
            //}
        }
        else if (eventType == GameEventList.SpellBuff)
        {
            SpellBuffArgs curArgs = args as SpellBuffArgs;
            BattleObject target = ObjectDataMgr.Instance.GetBattleObject(curArgs.targetID);
            if (curArgs.isAdd)
            {
                Logger.LogFormat("[SpellService]{0} cast buff {1} to {2}", curArgs.casterID, curArgs.buffID, curArgs.targetID);
                //trigger motion
                if (target != null)
                {
                    target.TriggerEvent(curArgs.buffID, curArgs.triggerTime, null);
                }
            }
            else
            {
                Logger.LogFormat("[SpellService]buff {0} removed from {1}", curArgs.buffID, curArgs.targetID);
                //trigger motion
                if (target != null)
                {
                    target.TriggerEvent(curArgs.buffID + "Finish", curArgs.triggerTime, null);
                }
            }
        }
        else if (eventType == GameEventList.SpellEffect)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            Logger.LogFormat("[SpellService]effect {0} triggered caster {1} target {2}", curArgs.effectID, curArgs.casterID, curArgs.targetID);
            //trigger motion
            BattleObject target = ObjectDataMgr.Instance.GetBattleObject(curArgs.targetID);
            if (target != null)
            {
                target.TriggerEvent(curArgs.effectID, curArgs.triggerTime, curArgs.wpNode);
            }
        }
        //else if (eventType == GameEventList.SpellMiss)
        //{
        //    SpellEffectArgs curArgs = args as SpellEffectArgs;
        //    Logger.LogFormat("[SpellService]effect {0} missed caster {1} target {2}", curArgs.effectID, curArgs.casterID, curArgs.targetID);
        //}
        else if (eventType == GameEventList.SpellImmune)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            Logger.LogFormat("[SpellService]effect {0} immuned caster {1} target {2}", curArgs.effectID, curArgs.casterID, curArgs.targetID);
        }
        else if (eventType == GameEventList.SpellStun)
        {
            SpellBuffArgs curArgs = args as SpellBuffArgs;
            BattleObject target = ObjectDataMgr.Instance.GetBattleObject(curArgs.targetID);
            if (target != null)
            {
                //casterID refers buff is removed or added
                if (curArgs.casterID == 0)
                {
                    target.TriggerEvent("stunFinish", curArgs.triggerTime, null);
                }
                else
                {
                    target.TriggerEvent("stun", curArgs.triggerTime, null);
                }
            }
        }
        else if (eventType == GameEventList.SpellAbsrobed)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            BattleObject target = ObjectDataMgr.Instance.GetBattleObject(curArgs.targetID);
            {
                target.TriggerEvent("absorbed", curArgs.triggerTime, null);
            }
        }
        else if (eventType==GameEventList.NormalHit)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            BattleObject target = ObjectDataMgr.Instance.GetBattleObject(curArgs.targetID);
            {
                target.TriggerEvent("normalHit", curArgs.triggerTime, null);
            }
        }
        else if (eventType==GameEventList.BashHit)
        {
            SpellEffectArgs curArgs = args as SpellEffectArgs;
            BattleObject target = ObjectDataMgr.Instance.GetBattleObject(curArgs.targetID);
            {
                target.TriggerEvent("bashHit", curArgs.triggerTime, null);
            }
        }

        GameEventMgr.Instance.FireEvent<EventArgs>(eventType, args);
    }
    //---------------------------------------------------------------------------------------------
    public void SetSpellEndTime(float delayTime)
    {
        //NOTE: use curActionSpell instead of owned spell,for buff response 
        if (mCurActionSpell != null)
        {
            mCurActionSpell.SetSpellEndTime(delayTime);
        }
    }
    //---------------------------------------------------------------------------------------------
}
