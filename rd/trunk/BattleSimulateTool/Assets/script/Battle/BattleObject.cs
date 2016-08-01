using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BattleObjectType
{
    Scene,
    Unit,
}

public class BattleObject
{
    public BattleObjectType type = BattleObjectType.Unit ;
    public UnitCamp camp;
    public int guid;
    public GameUnit unit;
	public WeakPointGroup wpGroup = null;
    //new added
    public BossAi mBossAi = null;
    //---------------------------------------------------------------------------------------------
    public void SetBossAI(BossAi ai)
    {
        mBossAi = ai;
    }
    //---------------------------------------------------------------------------------------------
    public BossAi GetBossAI()
    {
        return mBossAi;
    } 
    //---------------------------------------------------------------------------------------------
    public void TriggerEvent(string eventID, float triggerTime, string rootNode)
    {
    }
    //---------------------------------------------------------------------------------------------
    public void OnEnterField(bool recalcSpeed = true)
    {
        unit.State = UnitState.None;
        unit.backUp = false;

        if (recalcSpeed == true)
        {
            unit.RecalcCurActionOrder();
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetTargetRotate(Quaternion rot, bool reset)
    {
    }
    //---------------------------------------------------------------------------------------------
    public void OnExitField()
    {
        if (unit.State != UnitState.Dead)
        {
            unit.State = UnitState.None;
        }
    }
    //---------------------------------------------------------------------------------------------
    void LateUpdate()
    {
    }
    //---------------------------------------------------------------------------------------------
}
