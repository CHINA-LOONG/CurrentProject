using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//for single unit buff view
public class UIBuffView : MonoBehaviour
{
    public BuffIcon[] dotBuffList;
    public BuffIcon otherBuff;
    
    private BattleObject targetUnit;
    private int curOtherBuffIndex;
    private float otherBuffInterval;
    private List<string> otherBuffList;

	public void Init ()
    {
        this.targetUnit = null;
        otherBuffInterval = 0.0f;
        curOtherBuffIndex = 0;
        otherBuffList = new List<string>();
	}

    public void SetTargetUnit(BattleObject targetUnit)
    {
        if (this.targetUnit != targetUnit)
        {
            this.targetUnit = targetUnit;

            ClearBuff();
        }
    }
	
	void Update ()
    {
        UpdateBuff();
	}

    public void OnBuffChanged(EventArgs args)
    {
        SpellBuffArgs buffArgs = args as SpellBuffArgs;
        if (targetUnit != null && buffArgs.targetID != targetUnit.guid)
        {
            return;
        }
        BuffPrototype curBuff = StaticDataMgr.Instance.GetBuffProtoData(buffArgs.buffID);

        //dot类buff
        if (curBuff.category == (int)BuffType.Buff_Type_Dot)
        {
            RefreshDotBuff();
        }
        //非dot类buff 刷新buff表
        else 
        {
            if (targetUnit == null)
                return;

            int buffCount = targetUnit.unit.buffList.Count;
            otherBuffList.Clear();
            BuffPrototype buffPb = null;
            for (int i = 0; i < buffCount; ++i)
            {
                buffPb = targetUnit.unit.buffList[i].buffProto;
                if (buffPb.category != (int)(BuffType.Buff_Type_Dot) && buffPb.category != (int)(BuffType.Buff_Type_Defend))
                {
                    otherBuffList.Add(buffPb.icon);
                }
            }
        }
    }

    private void UpdateBuff()
    {
        if (targetUnit == null)
            return;

        int buffCount = otherBuffList.Count;
        if (buffCount <= 0)
            return;

        otherBuffInterval += Time.deltaTime;
        if (otherBuffInterval > SpellConst.buffShowInterval)
        {
            otherBuffInterval = 0.0f;
            ++curOtherBuffIndex;
            curOtherBuffIndex %= buffCount;
            otherBuff.ShowBuff(otherBuffList[curOtherBuffIndex]);
        }
    }
    private void ClearBuff()
    {
        RefreshDotBuff();
        otherBuff.RemoveBuff();
        otherBuffList.Clear();
    }

    private void RefreshDotBuff()
    {
        for (int i = 0; i < dotBuffList.Length; ++i)
        {
            dotBuffList[i].RemoveBuff();
        }

        if (targetUnit == null)
            return;

        int buffCount = targetUnit.unit.buffList.Count;
        int dotIndex = 0;
        Buff curBuff = null;
        for (int i = 0; i < buffCount; ++i)
        {
            curBuff = targetUnit.unit.buffList[i];
            if (curBuff.IsFinish == false && curBuff.buffProto.category == (int)(BuffType.Buff_Type_Dot))
            {
                dotBuffList[dotIndex].ShowBuff(curBuff.buffProto.icon);
                ++dotIndex;
            }
        }
    }
}
