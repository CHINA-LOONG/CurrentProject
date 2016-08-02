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
    struct BuffStateData
    {
        public BuffStateData(string icon, int remainRound)
        {
            this.buffIcon = icon;
            this.buffRemainRound = remainRound;
        }
        
        public string buffIcon;
        public int buffRemainRound;
    }
    private List<BuffStateData> otherBuffList;


	public void Init ()
    {
        this.targetUnit = null;
        otherBuffInterval = 0.0f;
        curOtherBuffIndex = 0;
        otherBuffList = new List<BuffStateData>();
	}

    public void SetTargetUnit(BattleObject targetUnit)
    {
        if (this.targetUnit != targetUnit)
        {
            this.targetUnit = targetUnit;

            ClearBuff();
            //TODO: merge
            RefreshDotBuff();
            RefreshOtherBuff();
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
        if (buffArgs.buffID == "internal_all")
        {
            RefreshDotBuff();
            RefreshOtherBuff();
        }
        else
        {
            BuffPrototype curBuff = StaticDataMgr.Instance.GetBuffProtoData(buffArgs.buffID);

            //dot类buff
            if (curBuff.category == (int)BuffType.Buff_Type_Dot)
            {
                RefreshDotBuff();
            }
            //非dot类buff 刷新buff表
            else
            {
                RefreshOtherBuff();
            }
        }
    }

    private void UpdateBuff()
    {
        if (targetUnit == null)
        {
            if (otherBuffList.Count > 0)
            {
                otherBuff.RemoveBuff();
                otherBuffList.Clear();
            }
            return;
        }

        int buffCount = otherBuffList.Count;
        if (buffCount <= 0)
        {
            if (otherBuff.IconName != null)
                otherBuff.RemoveBuff();

            return;
        }

        otherBuffInterval += Time.deltaTime;
        if (otherBuffInterval > SpellConst.buffShowInterval)
        {
            otherBuffInterval = 0.0f;
            ++curOtherBuffIndex;
            curOtherBuffIndex %= buffCount;
            otherBuff.ShowBuff(otherBuffList[curOtherBuffIndex].buffIcon, otherBuffList[curOtherBuffIndex].buffRemainRound);
        }
    }
    private void ClearBuff()
    {
        //RefreshDotBuff();

        for (int i = 0; i < dotBuffList.Length; ++i)
        {
            dotBuffList[i].RemoveBuff();
        } 
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
            if (curBuff.IsFinish == false && curBuff.casterID != BattleConst.battleSceneGuid && curBuff.buffProto.category == (int)(BuffType.Buff_Type_Dot))
            {
                dotBuffList[dotIndex].ShowBuff(curBuff.buffProto.icon, curBuff.BuffRemainRound);
                ++dotIndex;
            }
        }
    }

    private void RefreshOtherBuff()
    {
        if (targetUnit == null)
            return;

        int buffCount = targetUnit.unit.buffList.Count;
        otherBuffList.Clear();
        Buff curBuff = null;
        for (int i = 0; i < buffCount; ++i)
        {
            curBuff = targetUnit.unit.buffList[i];

            if (targetUnit.unit.buffList[i].IsFinish == false && 
                curBuff.casterID != BattleConst.battleSceneGuid &&
                    (
                    curBuff.buffProto.category == (int)(BuffType.Buff_Type_Normal) ||
                    curBuff.buffProto.category == (int)(BuffType.Buff_Type_Hot) ||
                    curBuff.buffProto.category == (int)(BuffType.Buff_Type_Debuff) ||
                    curBuff.buffProto.category == (int)(BuffType.Buff_Type_Benefit)
                    )
                )
            {
                otherBuffList.Add(new BuffStateData(curBuff.buffProto.icon, curBuff.BuffRemainRound));
            }
        }
    }
}
