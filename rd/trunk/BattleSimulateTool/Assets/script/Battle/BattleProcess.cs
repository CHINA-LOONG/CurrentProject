using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public enum BattleRetCode
{
    Normal,
    Success,
    Failed,
}

public class BattleProcess : MonoBehaviour
{
    public enum ActionType
    {
        None = 0,
        UnitFight,
        SwitchPet,
        UnitReplaceDead,//替换死亡怪物出场
        FirstSpell,//先置技能
        ReviveUnit,
        Dazhao
    }

	public enum DazhaoType
	{
		Phyics =0,
		Magic,
		Unkown
	}

    public class Action
    {
        public ActionType type;
        public BattleObject caster;
        public BattleObject target;
        public bool isInsert = false;

		//二级属性
		public DazhaoType dazhaoType = DazhaoType.Unkown;
    }

    public class ReplaceDeadUnitAction : Action
    {
        public int slot;
        public float triggerTime;
    }

    public class FirstSpellAction : Action
    {
        public string firstSpellID;
        public float triggerTime;
    }

    BattleGroup battleGroup;
    //ProcessData processData;
    BattleLevelData processData = null;

    List<Action> insertAction = new List<Action>();

    //MethodInfo battleVictorMethod;
    //MethodInfo processVictorMethod;

    //histroy
    //TODO: use multimap
    float lastUpdateTime;
    int round = 0;
    public int TotalRound
    {
        get { return round; }
    }
    public float lastActionOrder = 0.0f;
    //TODO: add battleobject event to here for record
    List<SpellFireArgs> spellEventList = new List<SpellFireArgs>();
    List<SpellVitalChangeArgs> lifeChangeEventList = new List<SpellVitalChangeArgs>();
    List<SpellVitalChangeArgs> energyEventList = new List<SpellVitalChangeArgs>();
    List<SpellUnitDeadArgs> deadEventList = new List<SpellUnitDeadArgs>();
    List<WeakPointDeadArgs> wpDeadEventList = new List<WeakPointDeadArgs>();
    List<SpellUnitDeadArgs> deathList = new List<SpellUnitDeadArgs>();
    List<SpellReviveArgs> reviveList = new List<SpellReviveArgs>();

    //如果没有集火目标，根据怪物各自AI进行战斗
    public	GameUnit fireFocusTarget = null;
    public	string fireAttackWpName = null;
    int replaceDeadUnitCount = 0;//total replace deadreplace
    bool hasInsertReplaceDeadUnitAction = false;//not insertaction deadreplace
    bool inDazhaoAction = false;
    public int mCurrentReviveCount = 0;
    //强制结果（GM）
    public int forceResult = -1;

    bool switchingPet = false;
    public bool SwitchingPet
    {
        get
        {
            return switchingPet;
        }

        set 
        {
            switchingPet = value;
        }
    }
    //换宠cd
    float lastSwitchTime = -BattleConst.switchPetCD;
    public float SwitchPetCD
    {
        get { return Mathf.Clamp(BattleConst.switchPetCD - (Time.time - lastSwitchTime), 0, BattleConst.switchPetCD); }
    }

    //当前行动
    Action curAction = null;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellFire, OnFireSpell);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellEnergyChange, OnEnergyChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellUnitDead, OnUnitDead);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.spellUnitRevive, OnUnitRevive);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.WeakpoingDead, OnWeakPointDead);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellFire, OnFireSpell);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellEnergyChange, OnEnergyChange);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellUnitDead, OnUnitDead);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.spellUnitRevive, OnUnitRevive);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.WeakpoingDead, OnWeakPointDead);
    }

    public void Init()
    {
        BindListener();
    }

    void Update()
    {
        //TODO: use battle start time as 0, not Time.time
        int eventCount = deadEventList.Count;
        float curTime = Time.time;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellUnitDeadArgs args = deadEventList[i];
            int deadId = args.deathID;
            var deadUnit = ObjectDataMgr.Instance.GetBattleObject(deadId);

            deathList.Add(args);
            int slot = deadUnit.unit.pbUnit.slot;
            deadUnit.unit.State = UnitState.Dead;

			//检测死亡的怪物是否 发动了大招，如果插入 则从插入事件中删除
			for (int j =0; j < insertAction.Count; ++j)
			{
				Action subAction = insertAction[j];
				if(subAction.type == ActionType.Dazhao && subAction.caster.guid == deadId)
				{
					insertAction.Remove(subAction);
					break;
				}
			}

            //查看是否还有需要上场的单位
            if (deadUnit.camp == UnitCamp.Enemy)
            {
                if (fireFocusTarget != null && fireFocusTarget.pbUnit.guid == deadUnit.guid)
                {
					HideFireFocus();
                }
                battleGroup.OnUnitExitField(deadUnit, slot);
                BattleObject bo = battleGroup.GetEnemyToField();
                if (bo != null)
                {
                    bo.unit.attackWpName = null;
                    fireAttackWpName = null;
                    InsertReplaceDeadAction(bo, slot, args.triggerTime);
                }
            }
            else
            {
                var switchAction = GetSwitchAction(deadId);
                if (switchAction != null)
                {
                    battleGroup.OnUnitExitField(deadUnit, slot);
                    var unit = switchAction.caster;
                    InsertReplaceDeadAction(unit, slot, args.triggerTime);
                    insertAction.Remove(switchAction);
                }
                else
                {
                    BattleObject unit = battleGroup.GetPlayerToField();
                    if (unit != null)
                    {
                        battleGroup.OnUnitExitField(deadUnit, slot);
                        deadUnit.unit.backUp = true;
                        InsertReplaceDeadAction(unit, slot, args.triggerTime);
                    }
                }
            }
        }
        deadEventList.Clear();

        //eventCount = reviveList.Count;
        //for (int i = 0; i < eventCount; ++i)
        //{
        //    SpellReviveArgs args = reviveList[i];
        //    if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
        //    {
        //        continue;
        //    }

        //    BattleObject reviveUnit = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
        //    if (null == reviveUnit)
        //    {
        //        Logger.LogError("Error:deadUnit is null,deadId = " + args.targetID);
        //    }
        //    reviveUnit.TriggerEvent("revive", args.triggerTime, null);
        //    reviveUnit.unit.CastPassiveSpell(args.triggerTime);
        //    BattleController.Instance.GetUIBattle().ShowUnitUI(reviveUnit, reviveUnit.unit.pbUnit.slot);
        //    SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
        //    energyArgs.vitalType = (int)VitalType.Vital_Type_Default;
        //    energyArgs.triggerTime = 0.0f;
        //    energyArgs.casterID = args.targetID;
        //    energyArgs.vitalChange = BattleConst.enegyMax;
        //    energyArgs.vitalCurrent = 0;
        //    energyArgs.vitalMax = 0;
        //    BattleController.Instance.GetUIBattle().ChangeEnergy(energyArgs);
        //}
        
        eventCount = lifeChangeEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellVitalChangeArgs args = lifeChangeEventList[i];
            int targetID = args.targetID;
            BattleObject targetOb = ObjectDataMgr.Instance.GetBattleObject(targetID);
            if (targetOb != null)
            {
                //BossAi bossAi = targetOb.GetComponent<BossAi>();
                BossAi bossAi = targetOb.GetBossAI();
                if (bossAi != null)
                {
                    bossAi.OnVitalChange(args);
                }
            }
        }
        lifeChangeEventList.Clear();

       eventCount = wpDeadEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            WeakPointDeadArgs args = wpDeadEventList[i];
            //if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
            //{
            //    continue;
            //}

            int targetID = args.targetID;
            BattleObject targetOb = ObjectDataMgr.Instance.GetBattleObject(targetID);
            if (targetOb != null)
            {
                targetOb.unit.SetWpDead(args);
                //TODO: remove to boss ai
                //targetOb.TriggerEvent(args.wpID + "_dead", args.triggerTime + BattleConst.vitalChangeDispearTime, null);

                if (fireFocusTarget == targetOb.unit &&
                    string.IsNullOrEmpty(fireAttackWpName) == false &&
                    fireAttackWpName.EndsWith(args.wpID)
                    )
                {
					HideFireFocus();
                }

                BossAi bossAi = targetOb.GetBossAI();
                if (bossAi != null)
                {
                    bossAi.OnWpDead(args);
                }
            }
        }
        wpDeadEventList.Clear();
        //lastUpdateTime = curTime;

        for (int i = deathList.Count - 1; i >= 0; --i)
        {
            SpellUnitDeadArgs args = deathList[i];
            if (lastUpdateTime - args.triggerTime > SpellConst.unitDeadTime)
            {
                BattleObject deadUnit = ObjectDataMgr.Instance.GetBattleObject(args.deathID);
                if (deadUnit != null)
                {
                    deadUnit.unit.OnDead();
                }

                deathList.RemoveAt(i);
            }
        }
    }

    void OnDestroy()
    {
        UnBindListener();
    }

	public void HideFireFocus()
	{
		fireFocusTarget = null;
		fireAttackWpName = null;
		GameEventMgr.Instance.FireEvent(GameEventList.HideFireFocus);
	}

    public void StartProcess(int index, BattleLevelData battleLevelData)
    {
        BattleToolMain.Instance.bureauNum = index;//对局
        LogResult.Instance.logData[LogResult.Instance.xhNumber].logIsWin[index] = 1;
        forceResult = -1;
        replaceDeadUnitCount = 0;
        hasInsertReplaceDeadUnitAction = false;
        lastUpdateTime = Time.time;
        processData = battleLevelData;
        
        battleGroup = BattleController.Instance.BattleGroup;

        round = 0;
        lastActionOrder = 0.0f;
        battleGroup.ResetActionOrder();
        battleGroup.OnStartNewBattle(Time.time);
        battleGroup.CastFirstSpell();
        Process(index);
    }

    public void Clear()
    {
        insertAction.Clear();
        SwitchingPet = false;
        spellEventList.Clear();
        lifeChangeEventList.Clear();
        energyEventList.Clear();
        deadEventList.Clear();
        wpDeadEventList.Clear();
        reviveList.Clear();
        deathList.Clear();
    }

    void Process(int battleLevelIndex)
    {
        if (battleLevelIndex == 0)
        {
            mCurrentReviveCount = 0;
        }
        BattleController.Instance.processStart = true;

        StartAction();
    }

    void StartAction()
    {
		Action action = GetNextAction();
		curAction = action;
        GameUnit actionCaster = null;
		if (action != null)
        {
            if (curAction.caster != null)
            {
                actionCaster = curAction.caster.unit;
            }
            else if (curAction.type != ActionType.ReviveUnit)
            {
                OnActionOver();
                return;
            }

			switch (action.type)
			{
			case ActionType.None:
				break;
			case ActionType.UnitFight:
                //NOTE: buff may change the speed of unit, so if actionOrder is less than last, keep as last
                lastActionOrder = (lastActionOrder > actionCaster.ActionOrder) ? lastActionOrder : actionCaster.ActionOrder;
                if (actionCaster.dazhao > 0)
                {
                    actionCaster.RecalcCurActionOrder(lastActionOrder);
                }
				RunUnitFightAction(action.caster);
				break;
			case ActionType.SwitchPet:
				//StartCoroutine(RunSwitchPetAction(action.caster, action.target));
				break;
            case ActionType.Dazhao:
				//if (action.dazhaoType == DazhaoType.Phyics)
                //{
                //    inDazhaoAction = true;
				//	PhyDazhaoController.Instance.RunActionWithDazhao(action.caster);
				//}
				//else if (action.dazhaoType == DazhaoType.Magic)
				//{
				//	MagicDazhaoController.Instance.RunActionWithDazhao(action.caster);
				//}				
				break;
            case ActionType.UnitReplaceDead:
                RunReplaceDeadAction(curAction as ReplaceDeadUnitAction);
                break;
            case ActionType.FirstSpell:
                RunFirstSpell(curAction);
                break;
            case ActionType.ReviveUnit:
                StartCoroutine(RunReviveAction(curAction));
                break;
			default:
				break;
			}
		}
    }

    Action GetNextAction()
    {
        //是否有插入动作，按队列顺序一个一个处理
        if (insertAction.Count > 0)
        {
            var act = insertAction[0];
            insertAction.RemoveAt(0);
            return act;
        }

        //下一个单位行动
        Action action = new Action();
        action.type = ActionType.UnitFight;
        action.caster = battleGroup.GetNextMoveUnit();

        return action;
    }

    void OnActionOver()
    {
        ++round;
        curAction = null;

        //判断进程是否结束
        //返回不包含在配置表key中的值时，表示当前进程没有结束
        
        BattleRetCode battleResult = BattleRetCode.Normal;
        if (processData.loseFunc != null)
        {
            battleResult = (BattleRetCode)processData.loseFunc.Invoke(null, null);
        }
        if (processData.winFunc != null)
        {
            battleResult = (BattleRetCode)processData.winFunc.Invoke(null, null);
        }
        if (battleResult == BattleRetCode.Normal)
        {
            battleResult = NormalScript.normalValiVic();
        }

        if (forceResult == 0)
        {
            battleResult = BattleRetCode.Failed;
        }
        else if (forceResult == 1)
        {
            battleResult = BattleRetCode.Success;
        }

        if (battleResult == BattleRetCode.Failed)
        {
            insertAction.Clear();
            LogResult.Instance.logData[LogResult.Instance.xhNumber].logIsWin[BattleToolMain.Instance.bureauNum] = 0;
            BattleController.Instance.OnBattleOver(false);
            return;
        }
        else if (battleResult == BattleRetCode.Success)
        {
            insertAction.Clear();
            if (BattleController.Instance.HasNextProcess() && forceResult == -1)
            {
                BattleController.Instance.StartNextProcess(BattleConst.battleProcessTime);
            }
            else
            {
                BattleController.Instance.OnBattleOver(true);
            }
            return;
        }

        //重新开始action
        StartAction();
    }

    public void ReviveSuccess(int reviveCount)
    {
        lastSwitchTime = -BattleConst.switchPetCD;
        mCurrentReviveCount = reviveCount;
        Action reviveAction = new Action();
        reviveAction.type = ActionType.ReviveUnit;
        InsertAction(reviveAction);
        StartAction();
    }

    #region Run Action
    //////////////////////////////////////////////////////////////////////////
    //run action
    void RunUnitFightAction(BattleObject bo)
    {
        Logger.LogFormat("[Battle.Process]Unit {0} is moving...", bo.unit.name);    
        //执行战斗
        var aiResult = BattleUnitAi.Instance.GetAiAttackResult(bo.unit);
        // Logger.LogFormat("Ai Attack style = {0} target = {1} ", aiResult.attackStyle, aiResult.attackTarget == null ? "no target--" : aiResult.attackTarget.name);
        LogResult.Instance.logData[LogResult.Instance.xhNumber].logRoundNumber[BattleToolMain.Instance.bureauNum]++;
        #region 暂时先不用小星的这个逻辑2015年11月6日15:21:43
        //for (int i = 0; i < BattleToolMain.Instance.operationData.gwID.Length; i++)
        //{
        //    if (BattleToolMain.Instance.operationData.duijuNum[i] == BattleToolMain.Instance.bureauNum
        //        && BattleToolMain.Instance.operationData.roundNum[i] == (round + 1))
        //    {
        //        for (int j = 0; j < BattleToolMain.Instance.mMainUnitList.Count; j++)
        //        {
        //            if (BattleToolMain.Instance.operationData.gwID[i] == BattleToolMain.Instance.mMainUnitList[j].unit.pbUnit.id
        //                && BattleToolMain.Instance.mMainUnitList[j].unit.energy == BattleConst.enegyMax)
        //            {
        //                Dictionary<string, Spell> spellDic = BattleToolMain.Instance.mMainUnitList[j].unit.spellList;
        //                Spell subSpel = null;
        //                foreach (KeyValuePair<string, Spell> subSpellDic in spellDic)
        //                {
        //                    subSpel = subSpellDic.Value;
        //                    BattleUnitAi.AiAttackStyle spellStyle = BattleUnitAi.Instance.GetAttackStyleWithSpellType(subSpel.spellData.category);
        //                    if (BattleUnitAi.AiAttackStyle.Dazhao == spellStyle)
        //                    {
        //                        aiResult.useSpell = subSpel;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
        if (bo.camp == UnitCamp.Player)
        {
            LogResult.Instance.logData[LogResult.Instance.xhNumber].playerData[BattleToolMain.Instance.bureauNum].monsterAttNumber++;
            if ( LogResult.Instance.isHitSuccessP)
            {
                LogResult.Instance.logData[LogResult.Instance.xhNumber].playerData[BattleToolMain.Instance.bureauNum].monsterHitNumber++;
                if (LogResult.Instance.isCriticalP)
                {
                    LogResult.Instance.logData[LogResult.Instance.xhNumber].playerData[BattleToolMain.Instance.bureauNum].monsterCritNumber++;
                }
            }
        }
        else
        {
            if (aiResult.attackTarget.pbUnit.camp == UnitCamp.Enemy)
            {
                for (int i = 0; i < battleGroup.PlayerFieldList.Count; i++)
                {
                    if (battleGroup.PlayerFieldList[i] != null)
                    {
                        aiResult.attackTarget = battleGroup.PlayerFieldList[i].unit;
                    }
                }
            }
            LogResult.Instance.logData[LogResult.Instance.xhNumber].enemyData[BattleToolMain.Instance.bureauNum].monsterAttNumber++;
            if (LogResult.Instance.isHitSuccessE)
            {
                LogResult.Instance.logData[LogResult.Instance.xhNumber].enemyData[BattleToolMain.Instance.bureauNum].monsterHitNumber++;
                if (LogResult.Instance.isCriticalE)
                {
                    LogResult.Instance.logData[LogResult.Instance.xhNumber].enemyData[BattleToolMain.Instance.bureauNum].monsterCritNumber++;
                }
            }
        }
        if (bo.camp == UnitCamp.Player && bo.unit.energy == BattleConst.enegyMax)
        {
            Dictionary<string, Spell> spellDic = bo.unit.spellList;
            Spell subSpel = null;
            foreach (KeyValuePair<string, Spell> subSpellDic in spellDic)
            {
                subSpel = subSpellDic.Value;
                BattleUnitAi.AiAttackStyle spellStyle = BattleUnitAi.Instance.GetAttackStyleWithSpellType(subSpel.spellData.category);
                if (BattleUnitAi.AiAttackStyle.Dazhao == spellStyle)
                {
                    aiResult.useSpell = subSpel;
                    if (aiResult.attackTarget.pbUnit.camp == UnitCamp.Player)
                    {
                        for (int i = 0; i < battleGroup.EnemyFieldList.Count; i++)
                        {
                            if (battleGroup.EnemyFieldList[i] != null)
                            {
                                aiResult.attackTarget = battleGroup.EnemyFieldList[i].unit;
                            }
                        }
                    }
                }
            }
        }
        if (bo.camp == UnitCamp.Enemy && bo.unit.energy == BattleConst.enegyMax)
        {
            LogResult.Instance.logData[LogResult.Instance.xhNumber].enemyData[BattleToolMain.Instance.bureauNum].monsterDazhaoNumber++;
        }
        if (bo.camp == UnitCamp.Player && BattleToolMain.Instance.bureauNum == 2)
        {
            if (battleGroup.EnemyFieldList[0] != null)
            {
                for (int i = 0; i < BattleToolMain.Instance.operationData.weaknessName.Length; i++)//minghe13/yueguangsenlin11_1
                {
                    if (battleGroup.EnemyFieldList[0].wpGroup.allWpDic[BattleToolMain.Instance.operationData.weaknessName[i]].wpState != WeakpointState.Dead)
                    {
                        if (BattleToolMain.Instance.roundNum == round)
                        {
                            aiResult.attackTarget = battleGroup.EnemyFieldList[0].unit;
                            //Debug.LogError("目标" + aiResult.attackTarget.name);
                            aiResult.attackTarget.attackWpName = BattleToolMain.Instance.operationData.weaknessName[i];
                            //Debug.LogError("弱点" + aiResult.attackTarget.attackWpName);
                        }
                    }
                    else
                    {
                        if ((i + 1) < BattleToolMain.Instance.operationData.weaknessName.Length)
                        {
                            BattleToolMain.Instance.roundNum = (round + BattleToolMain.Instance.operationData.IntervalRoundNum[i + 1]);
                        }
                    }
                }
            }
        }
        if (fireFocusTarget != null &&
            bo.camp == UnitCamp.Player)
        {
            if (aiResult.attackTarget.pbUnit.camp == UnitCamp.Enemy)
            {
                aiResult.attackTarget = fireFocusTarget;
                aiResult.attackTarget.attackWpName = fireAttackWpName;
                Logger.LogWarning("reset attack target is fireFocusTarget " + fireFocusTarget.name + fireFocusTarget.pbUnit.guid + " weakpointName " + aiResult.attackTarget.attackWpName);
            }
        }

        if (bo.unit.tauntTargetID != BattleConst.battleSceneGuid && aiResult.attackTarget.pbUnit.camp != bo.unit.pbUnit.camp)
        {
            BattleObject tauntTarget = ObjectDataMgr.Instance.GetBattleObject(bo.unit.tauntTargetID);
            if (tauntTarget != null && 
                tauntTarget.unit.pbUnit.slot >= BattleConst.slotIndexMin && 
                tauntTarget.unit.pbUnit.slot <= BattleConst.slotIndexMax &&
                tauntTarget.unit.State != UnitState.Dead
                )
            {
                aiResult.attackTarget = tauntTarget.unit;
            }
        }
        
        switch (aiResult.attackStyle)
        {
			case BattleUnitAi.AiAttackStyle.DazhaoPrepare:
			    //扣除能量
			    SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
                energyArgs.vitalType = (int)VitalType.Vital_Type_Default;
			    energyArgs.triggerTime = Time.time;
			    energyArgs.casterID = bo.guid;
			    energyArgs.vitalChange = BattleConst.enegyMax;
			    energyArgs.vitalCurrent = 0;
			    energyArgs.vitalMax = 0;
			    SpellService.Instance.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);
			    break;
            case BattleUnitAi.AiAttackStyle.Lazy:
                Logger.Log(bo.unit.name + "   lazy");
				break;
            case BattleUnitAi.AiAttackStyle.Defence:
                Logger.Log(bo.unit.name + "   defence");
                break;
            case BattleUnitAi.AiAttackStyle.Buff:
                Logger.Log(bo.unit.name + "   Beneficial");
                break;
            case BattleUnitAi.AiAttackStyle.MagicAttack:
            case BattleUnitAi.AiAttackStyle.PhysicsAttack:
            case BattleUnitAi.AiAttackStyle.Dazhao:
                break;
        }

        var curTarget = aiResult.attackTarget;
        if (null == curTarget)
        {
            Logger.LogError("Error for BattleUnitAI....");
        }
        bo.unit.attackCount++;
        if (aiResult.useSpell != null)
        {
            if (aiResult.useSpell.spellData.category == (int)SpellType.Spell_Type_PhyDaZhao || 
                aiResult.useSpell.spellData.category == (int)SpellType.Spell_Type_MagicDazhao)
            {
                if (bo.camp == UnitCamp.Enemy)
                    LogResult.Instance.logData[LogResult.Instance.xhNumber].enemyData[BattleToolMain.Instance.bureauNum].monsterDazhaoNumber++;
                else
                    LogResult.Instance.logData[LogResult.Instance.xhNumber].playerData[BattleToolMain.Instance.bureauNum].monsterDazhaoNumber++;
            }            
            SpellService.Instance.SpellRequest(aiResult.useSpell.spellData.id, bo.unit, aiResult.attackTarget, Time.time);
            bo.unit.OnRoundEnd(Time.time);
        }
        else
        {
            SpellService.Instance.SpellRequest("s1", bo.unit, aiResult.attackTarget, Time.time);
            bo.unit.OnRoundEnd(Time.time);
        }
    }

    void RunFirstSpell(Action action)
    {
        FirstSpellAction curAction = action as FirstSpellAction;

        if (curAction == null || curAction.caster == null || curAction.caster.unit.State == UnitState.Dead)
        {
            OnActionOver();
            return;
        }


        Dictionary<string, Spell> casterSpellList = curAction.caster.unit.spellList;
        Spell firstSpell;
        if (casterSpellList.TryGetValue(curAction.firstSpellID, out firstSpell) == true)
        {
            GameUnit target = BattleUnitAi.Instance.GetTargetThroughSpell(firstSpell, curAction.caster.unit);
            BattleUnitAi.Instance.CheckBossWeakPoint(target);
            SpellService.Instance.SpellRequest(curAction.firstSpellID, curAction.caster.unit, target, Time.time, true);
            curAction.caster.unit.OnRoundEnd(Time.time);

            SpellVitalChangeArgs args = new SpellVitalChangeArgs();
            args.vitalType = (int)VitalType.Vital_Type_FirstSpell;
            args.triggerTime = Time.time;
            args.casterID = curAction.caster.guid;
            args.targetID = args.casterID;
            args.isCritical = false;
            args.vitalChange = 0;
            args.vitalCurrent = 0;//TODO: need weak point life?
            args.vitalMax = 0;

            GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.SpellLifeChange, args);
            //BattleController.Instance.GetUIBattle().ChangeLife(args);
        }
    }

	public void RunMagicDazhao(Action action)
	{
	}

    //IEnumerator RunSwitchPetAction(BattleObject exit, BattleObject enter)
    //{
    //}

    void InsertReplaceDeadAction(BattleObject enter, int slot, float triggerTime)
    {
        ReplaceDeadUnitAction action = new ReplaceDeadUnitAction();
        action.type = ActionType.UnitReplaceDead;
        action.caster = enter;
        action.target = null;
        action.triggerTime = triggerTime;
        action.slot = slot;
        action.caster.unit.State = UnitState.ToBeEnter;
        action.isInsert = false;

        ++replaceDeadUnitCount;
        //if (replaceDeadUnitCount == 1 && inDazhaoAction == false)
        {
            action.isInsert = true;
            hasInsertReplaceDeadUnitAction = true;
            InsertAction(action);
        }
        //else 
        //{
        //    //if replaceDeadUnitCount is more than 1, means current action is replaceDeadUnitAction,so just trigget it
        //    StartCoroutine(RunReplaceDeadAction(action));
        //}
    }

    IEnumerator RunReviveAction(Action action)
    {
        yield return new WaitForSeconds(BattleConst.reviveTime);
        OnActionOver();
    }

    void RunReplaceDeadAction(ReplaceDeadUnitAction action)
    {
        if (action.isInsert == true)
        {
            hasInsertReplaceDeadUnitAction = false;
        }
        int slotID = action.slot;
        BattleObject enter = action.caster;
        
        if (enter.camp == UnitCamp.Enemy)
        {
            slotID = slotID + BattleConst.slotIndexMax + 1;
        }
        
        enter.unit.CalcNextActionOrder(lastActionOrder);
        battleGroup.OnUnitEnterField(enter, action.slot);

        --replaceDeadUnitCount;
        if (inDazhaoAction == false)
        {
            if (replaceDeadUnitCount == 0 ||    //the last action
                (replaceDeadUnitCount == 1 && hasInsertReplaceDeadUnitAction == true)
                )
            {
                OnActionOver();
            }
        }
    }

    #endregion

    #region Event
    //spell event
    void OnFireSpell(EventArgs sArgs)
    {
        SpellFireArgs args = sArgs as SpellFireArgs;
        int movedUnitId = args.casterID;
        BattleObject movedUnit = ObjectDataMgr.Instance.GetBattleObject(movedUnitId);
        //NOTE: if spellid is null, means cast spell failed
        if (string.IsNullOrEmpty(args.spellID) == false)
        {
            spellEventList.Add(args);
        }
        //cast passive spell no need to run next action
        if (args.category == (int)SpellType.Spell_Type_Passive && args.firstSpell == false)
        {
            return;
        }

        StartCoroutine(WaitAnim(movedUnit, args.firstSpell));
    }
    IEnumerator WaitAnim(BattleObject movedUnit, bool firstSpell)
    {
        if (curAction == null || curAction.type != ActionType.Dazhao)
        {
            yield return new WaitForEndOfFrame();
            OnUnitFightOver(movedUnit, firstSpell == false);
        }

    }
    void OnLifeChange(EventArgs sArgs)
    {
        //var args = sArgs as SpellVitalChangeArgs;
        //Logger.Log("[Battle.Process]OnLifeChange");

        SpellVitalChangeArgs vitalArgs = sArgs as SpellVitalChangeArgs;
        lifeChangeEventList.Add(vitalArgs);
    }

    void OnEnergyChange(EventArgs sArgs)
    {
        //var args = sArgs as SpellVitalChangeArgs;
        //Logger.Log("[Battle.Process]OnEnergyChange");

        SpellVitalChangeArgs vitalArgs = sArgs as SpellVitalChangeArgs;
        energyEventList.Add(vitalArgs);
    }

    void OnUnitDead(EventArgs sArgs)
    {
        SpellUnitDeadArgs args = sArgs as SpellUnitDeadArgs;
        deadEventList.Add(args);
    }

    void OnUnitRevive(EventArgs sArgs)
    {
        SpellReviveArgs args = sArgs as SpellReviveArgs;
        reviveList.Add(args);
    }

    void OnWeakPointDead(EventArgs args)
    {
        WeakPointDeadArgs wpDeadArgs = args as WeakPointDeadArgs;
        wpDeadEventList.Add(wpDeadArgs);
    }
    #endregion

    #region Utils
    void OnUnitFightOver(BattleObject movedUnit, bool calcActionOrder = true)
    {
        if (calcActionOrder == true)
        {
            battleGroup.CalcUnitNextAction(movedUnit.guid);
        }
        movedUnit.SetTargetRotate(Quaternion.identity, true);

        if (
            replaceDeadUnitCount == 0 ||
            (replaceDeadUnitCount == 1 && hasInsertReplaceDeadUnitAction == true)
            )
        {
            OnActionOver();
        }
    }

    Action GetSwitchAction(int toBeReplaedId)
    {
        Action action = null;
        foreach (Action item in insertAction)
        {
            if (item.type == ActionType.SwitchPet && item.target.guid == toBeReplaedId)
            {
                return item;
            }
        }
        return action;
    }

    public void InsertAction(Action act)
    {
        insertAction.Add(act);
    }

    public void InsertFirstSpellAction(BattleObject caster, string spellID)
    {
        FirstSpellAction action = new FirstSpellAction();
        action.type = ActionType.FirstSpell;
        action.caster = caster;
        action.target = caster;
        action.firstSpellID = spellID;
        //action.triggerTime = Time.time;//NOTE: not request triggerTime yet
        InsertAction(action);
    }

    #endregion
}
