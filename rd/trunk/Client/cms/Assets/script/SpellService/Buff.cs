using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class BuffPrototype
{
    public string id;
    public string category;
    public string periodEffectID;
    public int duration;

    //状态改变
    public bool stun;//眩晕
    public bool invincible;//无敌

    //属性改变
    public float strengthRatio;
    public float intelligenceRatio;
    public float defenseRatio;
    public float speedRatio;
}

public class BuffPrototypes : ScriptableObject
{
    public List<BuffPrototype> data = new List<BuffPrototype>();
}

public class Buff
{
    public SpellService spellService;
    public BuffPrototype buffProto;
    public Spell ownedSpell;
    public int casterID;
    public int targetID;
    private int periodCount;
    //---------------------------------------------------------------------------------------------
    public void Init(BuffPrototype buffPt, SpellService owner)
    {
        spellService = owner;
        buffProto = new BuffPrototype();
        buffProto.id = buffPt.id;
        buffProto.category = buffPt.category;
        buffProto.periodEffectID = buffPt.periodEffectID;
        buffProto.duration = buffPt.duration;
        buffProto.stun = buffPt.stun;
        buffProto.invincible = buffPt.invincible;
        buffProto.strengthRatio = buffPt.strengthRatio;
        buffProto.intelligenceRatio = buffPt.intelligenceRatio;
        buffProto.defenseRatio = buffPt.defenseRatio;
        buffProto.speedRatio = buffPt.speedRatio;
    }
    //---------------------------------------------------------------------------------------------
    public void SetOwnedSpell(Spell spell)
    {
        ownedSpell = spell;
    }
    //---------------------------------------------------------------------------------------------
    public void Apply(int curTime)
    {
        periodCount = 0;
        GameUnit target = spellService.GetUnit(targetID);
        AddBuff(target.buffList);
    }
    //---------------------------------------------------------------------------------------------
    public void Update(int curTime)
    {
        List<Buff> buffList = spellService.GetUnit(targetID).buffList;
        if (periodCount >= buffProto.duration)
        {
            Finish();
            buffList.Remove(this);
            return;
        }

        //hot and dot
        if (buffProto.periodEffectID.Length > 0)
        {
            Logger.Log("[SpellService]take periodic effect");
            ++periodCount;
            Effect eft = spellService.GetEffect(buffProto.periodEffectID);
            if (eft != null)
            {
                eft.SetOwnedSpell(ownedSpell);
                eft.SetOwnedBuff(this);
                eft.Apply(curTime);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Reset()
    {
        periodCount = 0;
    }
    //---------------------------------------------------------------------------------------------
    void AddBuff(List<Buff> buffList)
    {
        SpellBuffArgs args = new SpellBuffArgs();
        args.triggerTime = 0;
        args.casterID = casterID;
        args.targetID = targetID;
        args.isAdd = true;
        args.buffID = buffProto.id;
        spellService.TriggerEvent(GameEventList.SpellBuff, args);

        foreach (Buff buff in buffList)
        {
            if (buffProto.id == buff.buffProto.id)
            {
                buff.Reset();
                return;
            }
        }

        DealReplace(buffList);
        ModifyUnit(false);
    }
    //---------------------------------------------------------------------------------------------
    void Finish()
    {
        SpellBuffArgs args = new SpellBuffArgs();
        args.triggerTime = 0;
        args.casterID = casterID;
        args.targetID = targetID;
        args.isAdd = false;
        args.buffID = buffProto.id;
        spellService.TriggerEvent(GameEventList.SpellBuff, args);
        
        ModifyUnit(true);
    }
    //---------------------------------------------------------------------------------------------
    void DealReplace(List<Buff> buffList)
    {

    }
    //---------------------------------------------------------------------------------------------
    void ModifyUnit(bool isRemove)
    {
        GameUnit target = spellService.GetUnit(targetID);

        //状态改变
        if (buffProto.stun == true)
        {
            ++target.stun;
        }
        else
        {
            --target.stun;
        }

        if (buffProto.invincible == true)
        {
            ++target.invincible;
        }
        else
        {
            --target.invincible;
        }

        //属性改变
        if (isRemove)
        {
            target.spellStrengthRatio -= buffProto.strengthRatio;
            target.spellIntelligenceRatio -= buffProto.intelligenceRatio;
            target.spellSpeedRatio -= buffProto.speedRatio;
            target.spellDefenseRatio -= buffProto.defenseRatio;
        }
        else
        {
            target.spellStrengthRatio += buffProto.strengthRatio;
            target.spellIntelligenceRatio += buffProto.intelligenceRatio;
            target.spellSpeedRatio += buffProto.speedRatio;
            target.spellDefenseRatio += buffProto.defenseRatio;
        }
    }
    //---------------------------------------------------------------------------------------------
}
