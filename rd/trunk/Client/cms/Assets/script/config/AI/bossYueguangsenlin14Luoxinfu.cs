using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossYueguangsenlin14Luoxinfu : BossAi
{

	// Use this for initialization
	void Start () 
	{
	
	}


    public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit luoxinfuUnit, BattleUnitAi.AiAttackResult xgAiResult)
	{

		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();
		attackResult = xgAiResult;
		//spell todo
		Dictionary<string,Spell> luoxinfuSpellDic = GetUnitSpellList (luoxinfuUnit);

        Spell useSpell = null;
        //jiuWeihuSpellDic.TryGetValue ("bossKarong_anyingzhua", out useSpell);
        //attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;		
        //attackResult.attackTarget = GetAttackRandomTarget(luoxinfuUnit);

        List<string> wpList = null;
        wpList = GetAliveWeakPointList (luoxinfuUnit);
        int wp_count = wpList.Count -1;

		if (attackResult.useSpell.spellData.textId == "attackStabTriSlight")
		{
	        if (wp_count >2 && wp_count <=4)
	        {
	            luoxinfuSpellDic.TryGetValue("bossYueguangsenlin14Luoxinfu1", out useSpell);
	            attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;
	            attackResult.useSpell = useSpell;
	        }
	        else if (wp_count > 0 && wp_count <= 2)
	        {
	            luoxinfuSpellDic.TryGetValue("bossYueguangsenlin14Luoxinfu2", out useSpell);
	            attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;
	            attackResult.useSpell = useSpell;
	        }
	        else if (wp_count == 0)
	        {
	            luoxinfuSpellDic.TryGetValue("bossYueguangsenlin14Luoxinfu3", out useSpell);
	            attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;
	            attackResult.useSpell = useSpell;
	        }
		}
		return attackResult;
	}

}
