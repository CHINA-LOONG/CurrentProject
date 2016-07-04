using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class BuffPrototype
{
    public string id;
    public string periodEffectID;
    public int category;
    //public int isClear;//切进程是否删除
    public int duration;

    //状态改变
    public int stun;//眩晕
    public int invincible;//无敌

    //属性改变
    public float strengthRatio;
    public float intelligenceRatio;
    public float defenseRatio;
    public float speedRatio;

    //显示相关
    public string icon;
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
    private bool isFinish;
    public bool IsFinish
    {
        get { return isFinish; }
    }
    //---------------------------------------------------------------------------------------------
    public void Init(BuffPrototype buffPt, SpellService owner)
    {
        spellService = owner;
        buffProto = new BuffPrototype();
        buffProto.id = buffPt.id;
        buffProto.icon = buffPt.icon;
        buffProto.category = buffPt.category;
        buffProto.periodEffectID = buffPt.periodEffectID;
        buffProto.duration = buffPt.duration;
        buffProto.stun = buffPt.stun;
        buffProto.invincible = buffPt.invincible;
        buffProto.strengthRatio = buffPt.strengthRatio;
        buffProto.intelligenceRatio = buffPt.intelligenceRatio;
        buffProto.defenseRatio = buffPt.defenseRatio;
        buffProto.speedRatio = buffPt.speedRatio;
        isFinish = false;
    }
    //---------------------------------------------------------------------------------------------
    public void SetOwnedSpell(Spell spell)
    {
        ownedSpell = spell;
    }
    //---------------------------------------------------------------------------------------------
    public void Apply(float curTime)
    {
        periodCount = 0;
        isFinish = false;
        GameUnit target = spellService.GetUnit(targetID);
        AddBuff(target.buffList);
    }
    //---------------------------------------------------------------------------------------------
    public void Update(float curTime)
    {
        if (isFinish)
        {
            return;
        }

        List<Buff> buffList = spellService.GetUnit(targetID).buffList;

        //hot and dot
        ++periodCount;
        if (buffProto.periodEffectID.Length > 0)
        {
            Logger.Log("[SpellService]take periodic effect");
            Effect eft = spellService.GetEffect(buffProto.periodEffectID);
            if (eft != null)
            {
                eft.SetOwnedSpell(ownedSpell);
                eft.SetOwnedBuff(this);
                eft.Apply(curTime);
            }
        }

        if (periodCount >= buffProto.duration)
        {
            Finish();
            //buffList.Remove(this);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Reset()
    {
        periodCount = 0;
        isFinish = false;
    }
    //---------------------------------------------------------------------------------------------
    void AddBuff(List<Buff> buffList)
    {
        if (buffProto.category == (int)(BuffType.Buff_Type_Dot))
        {
            Buff firstDot = null;
            int dotCount = 0;
            foreach (Buff buff in buffList)
            {
                if (buff.buffProto.category == (int)(BuffType.Buff_Type_Dot))
                {
                    if (++dotCount == 1)
                    {
                        firstDot = buff;
                    }

                    //同源同id，刷新
                    if (buff.casterID == casterID && buff.buffProto.id == buffProto.id)
                    {
                        buff.Reset();
                        return;
                    }

                    if (dotCount >= 3)
                    {
                        firstDot.Finish();
                        firstDot.ModifyUnit(true);
                        buffList.Remove(firstDot);
                        break;
                    }
                }
            }
        }
        else
        {
            foreach (Buff buff in buffList)
            {
                //同名刷新，不区分来源
                if (buff.buffProto.id == buffProto.id)
                {
                    buff.Reset();
                    return;
                }
            }
        }

        buffList.Add(this);
        ModifyUnit(false);
        SpellBuffArgs args = new SpellBuffArgs();
        args.triggerTime = 0;
        args.casterID = casterID;
        args.targetID = targetID;
        args.isAdd = true;
        args.buffID = buffProto.id;
        spellService.TriggerEvent(GameEventList.SpellBuff, args);
    }
    //---------------------------------------------------------------------------------------------
    void Finish()
    {
        ModifyUnit(true);
        isFinish = true;

        SpellBuffArgs args = new SpellBuffArgs();
        args.triggerTime = 0;
        args.casterID = casterID;
        args.targetID = targetID;
        args.isAdd = false;
        args.buffID = buffProto.id;
        spellService.TriggerEvent(GameEventList.SpellBuff, args);
    }
    //---------------------------------------------------------------------------------------------
    void ModifyUnit(bool isRemove)
    {
        GameUnit target = spellService.GetUnit(targetID);

        //状态改变
        if (buffProto.stun > 0)
        {
            ++target.stun;
        }
        else
        {
            --target.stun;
        }

        if (buffProto.invincible > 0)
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
