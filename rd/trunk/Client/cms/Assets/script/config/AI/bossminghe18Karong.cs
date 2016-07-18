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


		if (NormalScript.GetWpLifeLeftRatio (karongUnit.battleUnit, "bossMinghe18Karongwp02") > 0 && karongUnit.curLife > karongUnit.maxLife * 0.7) 
		{									
			float randkey = UnityEngine.Random.Range (0.0f, 1.0f);	
			if (randkey >= 0.5) {
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
				if (randkey >= 0.5) {
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
				if (randkey >= 0.5) {
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
				if (randkey >= 0.5) {
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
				if (randkey >= 0.5) {
					karongSpellDic.TryGetValue ("bossMinghe18Karong6", out useSpell);
				} 
				else 
				{
					karongSpellDic.TryGetValue ("bossMinghe18Karong5", out useSpell);
				}
			}

		}

		if ((NormalScript.GetWpLifeLeftRatio(karongUnit.battleUnit,"bossMinghe18Karongwp02") == 0 && jishu == 0)|| (karongUnit.curLife <= karongUnit.maxLife*0.7 && jishu == 0))									
		{	
				
			if(NormalScript.GetWpLifeLeft(karongUnit.battleUnit,"bossMinghe18Karongwp02")>0 )
			{
				karongUnit.battleUnit.TriggerEvent("karong_stage1to2_shuihuo", Time.time, null);
				jishu ++;
			}
			else
			{
				karongUnit.battleUnit.TriggerEvent("karong_stage1to2_shuisi", Time.time, null);
				jishu ++;
			}
											
		}									
		attackResult.useSpell = useSpell;

		return attackResult;
	}	
}
