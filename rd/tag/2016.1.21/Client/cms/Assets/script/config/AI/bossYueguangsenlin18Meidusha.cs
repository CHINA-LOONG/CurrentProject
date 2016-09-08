using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossYueguangsenlin18Meidusha : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}

    int yazhi_count = 0 ;
    int count = 0; //state chage count
    int attck_count = 0;

	public	 override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit meidushaUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> meidushaSpellDic = GetUnitSpellList (meidushaUnit);

		Spell useSpell = null;
		//jiuWeihuSpellDic.TryGetValue ("bossKarong_anyingzhua", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(meidushaUnit);
        if (attackResult.attackTarget == null)
            return null;

        List<string> wpList = null;
        wpList = GetAliveWeakPointList (meidushaUnit);
        int wp_count = wpList.Count -1;
        //spell_AI
#region
      if (wp_count == 4)
        {
            if (yazhi_count <= 0 )
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1)
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha8",out useSpell);
                yazhi_count--;
            }
            else
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha1",out useSpell);
                yazhi_count--;
            }
        }
        else if (wp_count == 3)
        {
            if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp02"; })&&wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp02"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha6",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha7",out useSpell);
                yazhi_count=4;
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1)
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha8",out useSpell);
                yazhi_count--;
            }
            else
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha2",out useSpell);
                yazhi_count--;
            }
        }
        else if (wp_count == 2)
        {
            if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp02"; })&&wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp02"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha6",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha7",out useSpell);
                yazhi_count=4;
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1)
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha8",out useSpell);
                yazhi_count--;
            }
            else
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha3",out useSpell);
                yazhi_count--;
            }
        }
        else if (wp_count == 1)
        {
            if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp02"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha6",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha7",out useSpell);
                yazhi_count=4;
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1)
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha8",out useSpell);
                yazhi_count--;
            }
            else
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha4",out useSpell);
                yazhi_count--;
            }
        }
        else
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1)
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha8",out useSpell);
                yazhi_count--;
            }
            else
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha9",out useSpell);
                yazhi_count--;
            }
        }
        #endregion
        //statechange
        #region
        if ((GetAttackCount(meidushaUnit)-attck_count)<=5 && wp_count==0)
        {
            count++;
        }
        #endregion
		attackResult.useSpell = useSpell;
		return attackResult;
	}
    //---------------------------------------------------------------------------------------------
    public override void OnWpDead(WeakPointDeadArgs args)
    {
        BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
        if (GetAliveWeakPointList(target.unit).Count-1 == 0 && count == 0)
        {
            target.TriggerEvent("meidusha_state1to2", Time.time, null);
            attck_count = GetAttackCount(target.unit);
        }
		if (count ==6)
		{
			target.TriggerEvent("meidusha_state2to1", Time.time, null);
			attck_count = GetAttackCount(target.unit);
		}

		if (args.wpID=="bossYueguangsenlin18Meidushawp02" && count ==0)
		{
			target.TriggerEvent("shefasi1",Time.time,null);
		}
		if (args.wpID=="bossYueguangsenlin18Meidushawp03" && count ==0)
		{
			target.TriggerEvent("shefasi2",Time.time,null);
		}
		if (args.wpID=="bossYueguangsenlin18Meidushawp04" && count ==0)
		{
			target.TriggerEvent("shefasi3",Time.time,null);
		}
		if (args.wpID=="bossYueguangsenlin18Meidushawp05" && count ==0)
		{
			target.TriggerEvent("shefasi4",Time.time,null);
		}
    }
	public override void OnVitalChange(SpellVitalChangeArgs args)
	{
		if (args.vitalCurrent==0 && args.vitalType == (int)VitalType.Vital_Type_Default)
		{
			BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
			target.TriggerEvent("meidushasi",Time.time,null);
		}
	}
}