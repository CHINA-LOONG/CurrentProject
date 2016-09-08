

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossMinghe18Karong : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int jishu1 = 0 ;
	int jishu2 = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit meidushaUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> meidushaSpellDic = GetUnitSpellList (meidushaUnit);

		Spell useSpell = null;
		//jiuWeihuSpellDic.TryGetValue ("bossmeidusha_anyingzhua", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(meidushaUnit);
        if (attackResult.attackTarget == null)
            return null;

        List<string> wpList = null;
		wpList = GetAliveWeakPointList (meidushaUnit);

		if (NormalScript.GetWpLifeLeftRatio(meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp02")==0 && NormalScript.GetWpLifeLeftRatio(meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp03")==0)
		{
			
			meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha10", out useSpell);
			
			int i = 1;
			if (GetAttackCount(meidushaUnit) % 2 == 0) 
			{
				meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha11", out useSpell);
				attackResult.attackTarget = meidushaUnit;
			}
			
		} 
		else if (NormalScript.GetWpLifeLeftRatio(meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp02")==0)
		{
			
			meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha2", out useSpell);

			int i = 1;
			if (GetAttackCount(meidushaUnit) % 3 == 0) 
			{
				meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha3", out useSpell);
				attackResult.attackTarget = meidushaUnit;
			}

		} 
		else if (NormalScript.GetWpLifeLeftRatio(meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp03")==0)
		{
			
			meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha1", out useSpell);
			
			int i = 1;
			if (GetAttackCount(meidushaUnit) % 3 == 0) 
			{
				meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha4", out useSpell);
				attackResult.attackTarget = meidushaUnit;
			}

		}
		else 
		{
			meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha4", out useSpell);
			
			int i = 1;
			if (GetAttackCount(meidushaUnit) % 2== 0) 
			{
				meidushaSpellDic.TryGetValue ("bossYueguangsenlin18Meidusha3", out useSpell);
				attackResult.attackTarget = meidushaUnit;
			}

		}	

		float a = NormalScript.GetWpLifeLeftRatio (meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp04");
		if (NormalScript.GetWpLifeLeftRatio (meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp04") == 0 && jishu1 == 0) {

			meidushaSpellDic.TryGetValue ("bossmeidushazibao", out useSpell);
			jishu1++;
		}
		float b = NormalScript.GetWpLifeLeftRatio (meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp05");
		if (NormalScript.GetWpLifeLeftRatio (meidushaUnit.battleUnit, "bossYueguangsenlin18Meidushawp05") == 0 && jishu2 == 0) {

			meidushaSpellDic.TryGetValue ("bossmeidushazibao", out useSpell);
			jishu2++;
		}
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{   
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bossYueguangsenlin18Meidushawp02" && jishu==0)
        {
			target.TriggerEvent("meidusha_wp02_to4", Time.time, null);
			target.TriggerEvent("meidusha_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
        }

		if (args.wpID == "bossYueguangsenlin18Meidushawp03" && jishu==0)
		{
			target.TriggerEvent("meidusha_wp03_to4", Time.time, null);
			target.TriggerEvent("meidusha_state1to3", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
		}

		if (args.wpID == "bossYueguangsenlin18Meidushawp03" && args.wpID == "bossYueguangsenlin18Meidushawp02" && jishu==0)
		{
			target.TriggerEvent("meidusha_stateNto3", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
		}
	}
	//---------------------------------------------------------------------------------------------
}
