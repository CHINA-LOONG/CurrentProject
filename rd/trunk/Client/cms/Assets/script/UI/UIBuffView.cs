using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//for single unit buff view
public class UIBuffView : UIBase
{
    public BuffIcon[] dotBuffList;
    public BuffIcon otherBuff;
    
    private BattleObject targetUnit;
    private int curOtherBuffIndex;
    private float otherBuffInterval;
    private List<string> otherBuffList;

	public void Init ()
    {
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellBuff, OnBuffChanged);
        this.targetUnit = null;
        otherBuffInterval = 0.0f;
        curOtherBuffIndex = 0;
        otherBuffList = new List<string>();
        ////TODO: 3
        //dotBuffList = new BuffIcon[3]{};
        //dotBuffList[0] = 
        //otherBuff = new BuffIcon();
	}

    public void SetTargetUnit(BattleObject targetUnit)
    {
        if (this.targetUnit != targetUnit)
        {
            this.targetUnit = targetUnit;

            RefreshBuff();
        }
    }
	
	void Update () 
    {
        UpdateBuff();
	}

    void OnDestory()
    {
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellBuff, OnBuffChanged);
    }

    private void OnBuffChanged(EventArgs args)
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
            if (buffArgs.isAdd)
            {
                for (int i = 0; i < dotBuffList.Length; ++i)
                {
                    //find first empty dot slot
                    if (dotBuffList[i].IsActive() == false)
                    {
                        dotBuffList[i].ShowBuff(curBuff.icon);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < dotBuffList.Length; ++i)
                {
                    if (dotBuffList[i].IsActive() == true && dotBuffList[i].IconName == curBuff.icon)
                    {
                        dotBuffList[i].RemoveBuff();
                        break;
                    }
                }
            }
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
        //TODO: 2.0 is buff change time
        if (otherBuffInterval > 2.0f)
        {
            otherBuffInterval = 0.0f;
            ++curOtherBuffIndex;
            curOtherBuffIndex %= buffCount;
            otherBuff.ShowBuff(otherBuffList[curOtherBuffIndex]);
        }
    }
    private void RefreshBuff()
    {
        for (int i = 0; i < dotBuffList.Length; ++i)
        {
            dotBuffList[i].RemoveBuff();
        }
        otherBuff.RemoveBuff();
        otherBuffList.Clear();

        if (targetUnit == null)
            return;

        int buffCount = targetUnit.unit.buffList.Count;
        int dotIndex = 0;
        BuffPrototype buffPb = null;
        for (int i = 0; i < buffCount; ++i)
        {
            buffPb = targetUnit.unit.buffList[i].buffProto;
            if (buffPb.category == (int)(BuffType.Buff_Type_Dot))
            {
                dotBuffList[dotIndex].ShowBuff(buffPb.icon);
                ++dotIndex;
            }
        }
    }
}
