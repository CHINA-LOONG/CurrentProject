

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie18Ladong5 : BossAi {

    private byte mState = 0; //0:normal 1:stun
    private BattleObject mCurBo;
	// Use this for initialization

    void Start()
    {
        mCurBo = gameObject.GetComponent<BattleObject>();
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<System.EventArgs>(GameEventList.SpellStun, OnStun);
    } 

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<System.EventArgs>(GameEventList.SpellStun, OnStun);
    }

    private void OnStun(System.EventArgs args)
    {
        if (mState == 0)
        {
            SpellBuffArgs buffArgs = args as SpellBuffArgs;
            if (buffArgs != null && buffArgs.isAdd == true)
            {
                mState = 1;
                mCurBo.TriggerEvent("Ladong5_state1to2", Time.time, null);
                BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(0.5f);
            }
        }
    }

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Ladong5Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Ladong5SpellDic = GetUnitSpellList (Ladong5Unit);

		Spell useSpell = null;
		Ladong5SpellDic.TryGetValue ("bossdajie18Ladong51", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Ladong5Unit);
		if (NormalScript.GetWpLifeLeft(Ladong5Unit.battleUnit, "bossdajie18Ladong5wp02") == 0 )
		{
			Ladong5SpellDic.TryGetValue ("bossdajie18Ladong53", out useSpell);
		}
		else if (GetAttackCount(Ladong5Unit) % 7 == 0 && GetAttackCount(Ladong5Unit) != 0) 
		{
			Ladong5SpellDic.TryGetValue ("bossdajie18Ladong54", out useSpell);
		}
		else if (GetAttackCount(Ladong5Unit) % 3 == 0 && GetAttackCount(Ladong5Unit) != 0) 
		{
			Ladong5SpellDic.TryGetValue ("bossdajie18Ladong52", out useSpell);
		}

        int curStunbuffCount = 0;
        Buff curStunBuff;
		for(int n = Ladong5Unit.buffList.Count -1 ;n > 0;n--)
		{
            curStunBuff = Ladong5Unit.buffList[n];
            if (
                curStunBuff.IsFinish == false &&
                curStunBuff.buffProto.category == (int)BuffType.Buff_Type_Stun &&
                curStunBuff.PeriodCount > (curStunBuff.buffProto.duration - 1))
			{
				curStunbuffCount++;
                break;
			}
		}

        if (curStunbuffCount == 0 && mState == 1)
        {
            mState = 0;
            mCurBo.aniControl.SetBool("shoukong", false);
            Ladong5Unit.battleUnit.TriggerEvent("Ladong5_state2to1", Time.time, null);
            BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(0.5f);
        }

		attackResult.useSpell = useSpell;
		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Ladong5wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Ladong5_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
