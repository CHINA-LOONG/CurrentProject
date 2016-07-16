using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossYueguangsenlin18Meidusha : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}

    int yazhi_count = 0 ;
    int count = 0; //state chage count
    int attck_count = 0;

	public	 BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit meidushaUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> meidushaSpellDic = GetUnitSpellList (meidushaUnit);

		Spell useSpell = null;
		//jiuWeihuSpellDic.TryGetValue ("bossKarong_anyingzhua", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(meidushaUnit);

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
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.2)
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
            if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; })&&wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp04"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha6",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp04"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.2)
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
            if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; })&&wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp04"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha6",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp04"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.2)
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
            if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp03"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (yazhi_count<=0 && wpList.Exists(delegate(string wp) { return wp == "bossYueguangsenlin18Meidushawp04"; }))
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha5",out useSpell);
                yazhi_count=4;
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.2)
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
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.2)
            {
                meidushaSpellDic.TryGetValue("bossYueguangsenlin18Meidusha4",out useSpell);
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
        if (wp_count==0 && count ==0)
        {
            meidushaUnit.battleUnit.TriggerEvent("meidusha_state1to2",Time.time,null);           
            attck_count = GetAttackCount(meidushaUnit);
        }
        if ((GetAttackCount(meidushaUnit)-attck_count)<=5 && wp_count==0)
        {
            count++;
        }
        if (count ==6)
        {
            meidushaUnit.battleUnit.TriggerEvent("meidusha_state2to1",Time.time,null); 
            count = 0;
        }

        #endregion

		attackResult.useSpell = useSpell;
		return attackResult;
	}

	private List<GameUnit> GetCanAttackList(GameUnit meidushaUnit)
	{
		return BattleUnitAi.Instance.GetOppositeSideFiledList(meidushaUnit);
	}

	private GameUnit GetAttackRandomTarget(GameUnit meidushaUnit)
	{
		List<GameUnit> listTarge = GetCanAttackList (meidushaUnit);
		
		int index = Random.Range (0, listTarge.Count);
		
		return listTarge[index];
	}

	private int GetAttackCount(GameUnit meidushaUnit)
	{
		return meidushaUnit.attackCount;
	}

	private Dictionary<string,Spell> GetUnitSpellList(GameUnit battleUnit)
	{
		return battleUnit.spellList;
	}

	private List<Buff> GetUnitBuffList(GameUnit battleUnit)
	{
		return battleUnit.buffList;
	}

	private List<string> GetAliveWeakPointList(GameUnit gUnit)
	{
		List<string> wpList = new List<string> ();

		foreach (KeyValuePair<string,WeakPointRuntimeData> subWp in gUnit.battleUnit.wpGroup.allWpDic)
		{
			WeakPointRuntimeData wpData = subWp.Value;
			if(wpData.hp > 0)
			{
				wpList.Add(subWp.Key);
			}
		}
		return wpList;
	}

	private int GetUnitMaxHp(GameUnit battleUnit)
	{
		return battleUnit.maxLife;
	}

	private int GetUnitMinHp(GameUnit battleUnit)
	{
		return battleUnit.curLife;
	}

	private int GetUnitHp(GameUnit battleUnit)
	{
		return battleUnit.curLife;
	}
}
