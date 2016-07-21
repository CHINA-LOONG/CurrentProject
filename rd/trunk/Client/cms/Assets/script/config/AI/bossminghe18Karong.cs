

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossMinghe18Karong : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public	override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit karongUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> karongSpellDic = GetUnitSpellList (karongUnit);

		Spell useSpell = null;
		//jiuWeihuSpellDic.TryGetValue ("bossKarong_anyingzhua", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(karongUnit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (karongUnit);

		if ( NormalScript.GetWpLifeLeftRatio (karongUnit.battleUnit, "bossMinghe18Karongwp02")> 0 && karongUnit.curLife > karongUnit.maxLife * 0.7 && jishu == 0) 
		{
			float randkey = UnityEngine.Random.Range (0.0f, 1.0f);	
			if (randkey >= 0.8) {
				karongSpellDic.TryGetValue ("bossMinghe18Karong1", out useSpell);
			} 
			else 
			{
				karongSpellDic.TryGetValue ("bossMinghe18Karong2", out useSpell);
			}

		} 
		else 
		{
			if(NormalScript.GetWpLifeLeft(karongUnit.battleUnit,"bossMinghe18Karongwp04")>0 && NormalScript.GetWpLifeLeft(karongUnit.battleUnit,"bossMinghe18Karongwp05")>0)
			{
				float randkey = UnityEngine.Random.Range (0.0f, 1.0f);	
				if (randkey >= 0.9) {
					karongSpellDic.TryGetValue ("bossMinghe18Karong4", out useSpell);
				} 
				else 
				{
					karongSpellDic.TryGetValue ("bossMinghe18Karong3", out useSpell);
				}
			}
			else if(NormalScript.GetWpLifeLeft(karongUnit.battleUnit,"bossMinghe18Karongwp04")>0 && NormalScript.GetWpLifeLeft(karongUnit.battleUnit,"bossMinghe18Karongwp05")==0)
			{
				float randkey = UnityEngine.Random.Range (0.0f, 1.0f);	
				if (randkey >= 0.9) {
					karongSpellDic.TryGetValue ("bossMinghe18Karong6", out useSpell);
				} 
				else 
				{
					karongSpellDic.TryGetValue ("bossMinghe18Karong3", out useSpell);
				}
			}
			else if(NormalScript.GetWpLifeLeft(karongUnit.battleUnit,"bossMinghe18Karongwp04")==0 && NormalScript.GetWpLifeLeft(karongUnit.battleUnit,"bossMinghe18Karongwp05")>0)
			{
				float randkey = UnityEngine.Random.Range (0.0f, 1.0f);	
				if (randkey >= 0.9) {
					karongSpellDic.TryGetValue ("bossMinghe18Karong4", out useSpell);
				} 
				else 
				{
					karongSpellDic.TryGetValue ("bossMinghe18Karong5", out useSpell);
				}
			}
			else
			{
				float randkey = UnityEngine.Random.Range (0.0f, 1.0f);	
				if (randkey >= 0.9) {
					karongSpellDic.TryGetValue ("bossMinghe18Karong6", out useSpell);
				} 
				else 
				{
					karongSpellDic.TryGetValue ("bossMinghe18Karong5", out useSpell);
				}
			}

		}			
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    public override void OnVitalChange(SpellVitalChangeArgs args)
    {
        BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
        if (target.unit.curLife <= target.unit.maxLife * 0.82 && jishu == 0)
        {
            target.TriggerEvent("karong_stage1to2_shuihuo", Time.time, null);
            jishu++;
        }
    }
    //---------------------------------------------------------------------------------------------
    public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
        if (args.wpID == "bossMinghe18Karongwp02" && jishu == 0)
        {
            target.TriggerEvent("karong_shuijingqiusiwang", Time.time, null);
            target.TriggerEvent("karong_stage1to2_shuisi", Time.time, null);
			jishu ++;
        }
		if (args.wpID == "bossMinghe18Karongwp05" && jishu ==1)
		{
			target.TriggerEvent("karong_rightarmsiwang",Time.time,null);
		}
		if (args.wpID == "bossMinghe18Karongwp04" && jishu ==1)
		{
			target.TriggerEvent("karong_leftarmsiwang",Time.time,null);
		}
    }
    //---------------------------------------------------------------------------------------------
}
