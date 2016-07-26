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

    public float ActionDelayTime
    {
        set {actionDelayTime = value;}
        get {return actionDelayTime;}
    }
    float actionDelayTime;

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
    float lastActionOrder = 0.0f;
    Dictionary<int, PB.HSRewardInfo> rewardInfoList = new Dictionary<int,PB.HSRewardInfo>();
    //TODO: add battleobject event to here for record
    List<SpellFireArgs> spellEventList = new List<SpellFireArgs>();
    List<SpellVitalChangeArgs> lifeChangeEventList = new List<SpellVitalChangeArgs>();
    List<SpellVitalChangeArgs> energyEventList = new List<SpellVitalChangeArgs>();
    List<SpellUnitDeadArgs> deadEventList = new List<SpellUnitDeadArgs>();
    List<SpellBuffArgs> buffEventList = new List<SpellBuffArgs>();
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
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
		GameEventMgr.Instance.AddListener<BattleObject>(GameEventList.DazhaoBtnClicked, OnUnitCastDazhao);
        GameEventMgr.Instance.AddListener<int, string>(GameEventList.ChangeTarget, OnChangeTarget);
        GameEventMgr.Instance.AddListener<int>(GameEventList.ShowHideMonster, OnShowHideMonster);

        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellFire, OnFireSpell);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellEnergyChange, OnEnergyChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellUnitDead, OnUnitDead);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.spellUnitRevive, OnUnitRevive);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellBuff, OnBuffChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.WeakpoingDead, OnWeakPointDead);

		GameEventMgr.Instance.AddListener<BattleObject> (GameEventList.DazhaoActionOver, OnDazhaoActionOver);
		GameEventMgr.Instance.AddListener (GameEventList.RemoveDazhaoAction, OnRemoveDazhaoActtion);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.RemoveListener<int, string>(GameEventList.ChangeTarget, OnChangeTarget);
		GameEventMgr.Instance.RemoveListener<BattleObject>(GameEventList.DazhaoBtnClicked, OnUnitCastDazhao);

        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellFire, OnFireSpell);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellEnergyChange, OnEnergyChange);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellUnitDead, OnUnitDead);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.spellUnitRevive, OnUnitRevive);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellBuff, OnBuffChange);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.WeakpoingDead, OnWeakPointDead);

		GameEventMgr.Instance.RemoveListener<BattleObject> (GameEventList.DazhaoActionOver, OnDazhaoActionOver);
		GameEventMgr.Instance.RemoveListener (GameEventList.RemoveDazhaoAction, OnRemoveDazhaoActtion);
    }

    public void Init()
    {
        BindListener();
    }

    void LateUpdate()
    {
        //if (curAction != null)
        //{
        //    switch (curAction.type)
        //    {
        //        case ActionType.None:
        //            break;
        //        case ActionType.UnitFight:
        //            break;
        //        case ActionType.SwitchPet:
        //            break;
        //        case ActionType.Dazhao:
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //TODO: use battle start time as 0, not Time.time
        int eventCount = deadEventList.Count;
        float curTime = Time.time;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellUnitDeadArgs args = deadEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
            {
                continue;
            }

            int deadId = args.deathID;
			GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, deadId);
            var deadUnit = ObjectDataMgr.Instance.GetBattleObject(deadId);

			if(null == deadUnit)
			{
				Logger.LogError("Error:deadUnit is null,deadId = " + deadId);
			}

            deathList.Add(args);
            //Logger.LogWarning("[Battle.Process]OnUnitDead: " + deadUnit.name);
            int slot = deadUnit.unit.pbUnit.slot;
            deadUnit.unit.State = UnitState.Dead;
            //deadUnit.ClearEvent();
            deadUnit.TriggerEvent("dead", args.triggerTime, null);

			BattleObject dazhaoCaster = MagicDazhaoController.Instance.GetCasterBattleObj();
			if(null!=dazhaoCaster)
			{
				if(dazhaoCaster.guid == deadId)
				{
					MagicDazhaoController.Instance.FinishDazhaoWithSelfDead();
				}
			}

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
			
			//大招模式下，检查被攻击方 所有怪是否都死亡
			if (curAction!=null && 
			    curAction.type == ActionType.Dazhao) 
			{
				bool actionOver = battleGroup.IsEnemyAllDead();
				if(actionOver)
				{
					if(curAction.dazhaoType == DazhaoType.Phyics)
					{
						PhyDazhaoController.Instance.FinishDazhaoWithAllEnemyDead();
					}
					else
					{
						MagicDazhaoController.Instance.FinishDazhaoWithAllEnemyDead();
					}
				}
			}


            //查看是否还有需要上场的单位
            if (deadUnit.camp == UnitCamp.Enemy)
            {
                //item drop
                PB.HSRewardInfo rewardInfo;
                if (rewardInfoList.TryGetValue(deadId, out rewardInfo) == true)
                {
                    //PB.RewardItem reward = null;
                    if (rewardInfoList.TryGetValue(deadId, out rewardInfo) == true)
                    {
                        int rewardID = 0;
                        foreach (var item in rewardInfo.RewardItems)
                        {
                            if (item.type == (int)PB.itemType.NONE_ITEM)
                                continue;
                            if (item.type <= (int)PB.itemType.MONSTER_ATTR)
                            {
                                rewardID = int.Parse(item.itemId);
                                if (rewardID > (int)PB.playerAttr.COIN)
                                    continue;
                            }
                            else if (item.type == (int)PB.itemType.ITEM || item.type == (int)PB.itemType.EQUIP)
                            {
                                rewardID = item.type;
                            }
                            ItemDropManager.Instance.Fall(rewardID, deadUnit.transform);

                        }
                        //ItemDropManager.Instance.Fall(1, deadUnit.transform);
                        //ItemDropManager.Instance.Fall(3, deadUnit.transform);
                    }
                }
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
                    //battleGroup.OnUnitEnterField(bo, slot);
                    InsertReplaceDeadAction(bo, slot, args.triggerTime);
                    //StartCoroutine(LoggerAnim(unit));
                }
            }
            else
            {
                var switchAction = GetSwitchAction(deadId);
                if (switchAction != null)
                {
                    battleGroup.OnUnitExitField(deadUnit, slot);
                    var unit = switchAction.caster;
                    //battleGroup.OnUnitEnterField(unit, slot);
                    InsertReplaceDeadAction(unit, slot, args.triggerTime);
                    //StartCoroutine(LoggerAnim(unit));
                    insertAction.Remove(switchAction);
                    Logger.LogWarning("Dead unit was to be replaced, get the replace unit to field.");
                }
                else
                {
                    BattleObject unit = battleGroup.GetPlayerToField();
                    if (unit != null)
                    {
						//if no unit,don't call exitfield() since the changepetview will show all exited pet
                        battleGroup.OnUnitExitField(deadUnit, slot);
                        deadUnit.unit.backUp = true;
                        //battleGroup.OnUnitEnterField(unit, slot);
                        InsertReplaceDeadAction(unit, slot, args.triggerTime);
                        //StartCoroutine(LoggerAnim(unit));
                    }
                    else
                    {
                        BattleController.Instance.GetUIBattle().HideUnitUI(deadUnit.guid);
                    }
                }
            }
        }

        eventCount = reviveList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellReviveArgs args = reviveList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
            {
                continue;
            }

            BattleObject reviveUnit = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
            if (null == reviveUnit)
            {
                Logger.LogError("Error:deadUnit is null,deadId = " + args.targetID);
            }
            reviveUnit.TriggerEvent("revive", args.triggerTime, null);
            reviveUnit.unit.CastPassiveSpell(args.triggerTime);
            BattleController.Instance.GetUIBattle().ShowUnitUI(reviveUnit, reviveUnit.unit.pbUnit.slot);
        }

            eventCount = spellEventList.Count;
        for (int i = 0; i < eventCount; ++i)
		{
			SpellFireArgs args = spellEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
			{
				continue;
			}

			//todo:大招结束处理
			var casterObject = ObjectDataMgr.Instance.GetBattleObject(args.casterID);
			if(null == casterObject)
			{
				continue;
			}
			var useSpell = casterObject.unit.GetSpell(args.spellID);

			if(null != useSpell &&
			   useSpell.spellData.category == (int) SpellType.Spell_Type_MagicDazhao)
			{
				MagicDazhaoController.Instance.DazhaoAttackFinished(args.casterID);
			}
			else if (null != useSpell && 
			         useSpell.spellData.category == (int) SpellType.Spell_Type_PhyDaZhao )
			{
				PhyDazhaoController.Instance.DazhaoAttackFinished(args.casterID);
			}
        }
        eventCount = lifeChangeEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellVitalChangeArgs args = lifeChangeEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
            {
                continue;
            }

            BattleController.Instance.GetUIBattle().ChangeLife(args);

            int targetID = args.targetID;
            BattleObject targetOb = ObjectDataMgr.Instance.GetBattleObject(targetID);
            if (targetOb != null)
            {
                BossAi bossAi = targetOb.GetComponent<BossAi>();
                if (bossAi != null)
                {
                    bossAi.OnVitalChange(args);
                }
            }
        }

        eventCount = energyEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellVitalChangeArgs args = energyEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
            {
                continue;
            }

            BattleController.Instance.GetUIBattle().ChangeEnergy(args);
        }
        eventCount = buffEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellBuffArgs args = buffEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
            {
                continue;
            }

            BattleController.Instance.GetUIBattle().ChangeBuffState(args);
        }
        eventCount = wpDeadEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            WeakPointDeadArgs args = wpDeadEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= curTime)
            {
                continue;
            }

            int targetID = args.targetID;
            BattleObject targetOb = ObjectDataMgr.Instance.GetBattleObject(targetID);
            if (targetOb != null)
            {
                targetOb.unit.SetWpDead(args);
                //TODO: remove to boss ai
                targetOb.TriggerEvent(args.wpID + "_dead", args.triggerTime + BattleConst.vitalChangeDispearTime, null);

                if (fireFocusTarget == targetOb.unit &&
                    string.IsNullOrEmpty(fireAttackWpName) == false &&
                    fireAttackWpName.EndsWith(args.wpID)
                    )
                {
					HideFireFocus();
                }

                BossAi bossAi = targetOb.GetComponent<BossAi>();
                if (bossAi != null)
                {
                    bossAi.OnWpDead(args);
                }
            }
        }
        lastUpdateTime = curTime;

        for (int i = deathList.Count - 1; i >= 0; --i)
        {
            SpellUnitDeadArgs args = deathList[i];
            if (lastUpdateTime - args.triggerTime > SpellConst.unitDeadTime)
            {
                BattleObject deadUnit = ObjectDataMgr.Instance.GetBattleObject(args.deathID);
                if (deadUnit != null)
                {
                    //do not remove enemy since dot need the dead enemy data, remove enemy when change level
                    //if (deadUnit.camp == UnitCamp.Player)
                    {
                        deadUnit.unit.OnDead();
                        deadUnit.gameObject.SetActive(false);
                    }
                    //else
                    //{
                    //    ObjectDataMgr.Instance.RemoveBattleObject(args.deathID);
                    //}
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

	void OnDazhaoActionOver(BattleObject casterObject)
	{
        inDazhaoAction = false;
		OnUnitFightOver (casterObject, false);
	}

    public void StartProcess(int index, BattleLevelData battleLevelData)
    {
        replaceDeadUnitCount = 0;
        hasInsertReplaceDeadUnitAction = false;
        lastUpdateTime = Time.time;
        //battleVictorMethod = victorMethod;
        processData = battleLevelData;

        Logger.Log("[Battle.Process]Start process");
        battleGroup = BattleController.Instance.BattleGroup;

        round = 0;
        lastActionOrder = 0.0f;
        battleGroup.ResetActionOrder();
        battleGroup.OnStartNewBattle(Time.time);//TODO: trigger time use leveltime
        battleGroup.CastFirstSpell();

        System.Action<float> endStartEvent = (delayTime) =>
        {
            StartCoroutine(Process(index));
        };
        if (!string.IsNullOrEmpty(processData.battleProtoData.endStartEvent) && BattleController.Instance.InstanceStar == 0)
        {
            UISpeech.Open(processData.battleProtoData.endStartEvent, endStartEvent);
        }
        else
        {
            endStartEvent(0.0f);
        }
    }

    public void ClearRewardItem()
    {
        rewardInfoList.Clear();
    }

    public void AddRewardItem(int id, PB.HSRewardInfo reward)
    {
        rewardInfoList.Add(id, reward);
    }

    public void Clear()
    {
        insertAction.Clear();
        SwitchingPet = false;
        spellEventList.Clear();
        lifeChangeEventList.Clear();
        energyEventList.Clear();
        deadEventList.Clear();
        buffEventList.Clear();
        wpDeadEventList.Clear();
        reviveList.Clear();
        ClearRewardItem();
        //enemy has removed in UnLoadScene()
        //for (int i = deathList.Count - 1; i >= 0; --i)
        //{
        //    SpellUnitDeadArgs args = deathList[i];
        //    BattleObject deadUnit = ObjectDataMgr.Instance.GetBattleObject(args.casterID)(args.deathID);
        //    if (deadUnit != null)
        //    {
        //        if (deadUnit.camp == UnitCamp.Player)
        //        {
        //            deadUnit.gameObject.SetActive(false);
        //        }
        //        else
        //        {
        //            ObjectDataMgr.Instance.RemoveBattleObject(args.deathID);
        //        }
        //    }

        //    deathList.RemoveAt(i);
        //}
        deathList.Clear();
    }

    IEnumerator Process(int battleLevelIndex)
    {
        BattleController.Instance.PlayEntranceAnim();
        yield return new WaitForSeconds(BattleConst.entranceTime);

        if (string.IsNullOrEmpty(processData.battleProtoData.startEvent))
        {

        }
        //if (IsClearBuff())
        //    yield return StartCoroutine(ClearBuff());

        if (battleLevelIndex == 0)
        {
            mCurrentReviveCount = 0;
            yield return StartCoroutine(PlayCountDownAnim());
        }
        else
        {
            //TODO fade in && fade out
        }

        BattleController.Instance.processStart = true;
        RefreshEnemyState();

        StartAction();
    }

    IEnumerator PlayCountDownAnim()
    {
        yield return new WaitForSeconds(1.0f);
        BattleController.Instance.GetUIBattle().ShowStartBattleUI();
        yield return new WaitForSeconds(2.0f);
        BattleController.Instance.GetUIBattle().DestroyStartBattleUI();
        yield return null;
    }

    private void RefreshEnemyState()
    {

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
				StartCoroutine(RunSwitchPetAction(action.caster, action.target));
				break;
            case ActionType.Dazhao:
				if (action.dazhaoType == DazhaoType.Phyics)
                {
                    inDazhaoAction = true;
					PhyDazhaoController.Instance.RunActionWithDazhao(action.caster);
				}
				else if (action.dazhaoType == DazhaoType.Magic)
				{
					MagicDazhaoController.Instance.RunActionWithDazhao(action.caster);
				}
				
				break;
            case ActionType.UnitReplaceDead:
                StartCoroutine(RunReplaceDeadAction(curAction as ReplaceDeadUnitAction));
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

        if (battleResult != BattleRetCode.Normal && insertAction.Count > 0)
        {
            for (int i = insertAction.Count - 1; i >= 0; --i)
            {
                if (insertAction[i].type == ActionType.SwitchPet ||
                    insertAction[i].type == ActionType.UnitReplaceDead
                    )
                {
                    battleResult = BattleRetCode.Normal;
                }
                else 
                {
                    insertAction.RemoveAt(i);
                }
            }
        }

        if (battleResult == BattleRetCode.Failed)
        {
            insertAction.Clear();
            if (mCurrentReviveCount < BattleConst.maxReviveCount)
            {
                StartCoroutine(ShowReviveUI());
            }
            else
            {
                BattleController.Instance.OnBattleOver(false);
            }
            //System.Action<float> failProcess = (delayTime) =>
            //{
            //    BattleController.Instance.OnBattleOver(false);
            //};

            //if (!string.IsNullOrEmpty(processData.battleProtoData.endEvent) && BattleController.Instance.InstanceStar == 0)
            //    UISpeech.Open(processData.battleProtoData.endEvent, failProcess);
            //else
            //failProcess(0.0f);
            return;
        }
        else if (battleResult == BattleRetCode.Success)
        {
            insertAction.Clear();
            if (BattleController.Instance.HasNextProcess())
            {
                System.Action<float> nextProcess = (delayTime) =>
                {
                    StartCoroutine(BattleController.Instance.StartNextProcess(delayTime));
                };
                if (!string.IsNullOrEmpty(processData.battleProtoData.endEvent) && BattleController.Instance.InstanceStar == 0)
                    UISpeech.Open(processData.battleProtoData.endEvent, nextProcess);
                else
                    nextProcess(BattleConst.battleProcessTime);
            }
            else 
            {
                System.Action<float> successProcess = (delayTime) =>
                {
                    BattleController.Instance.OnBattleOver(true);
                };
                if (!string.IsNullOrEmpty(processData.battleProtoData.endEvent) && BattleController.Instance.InstanceStar == 0)
                    UISpeech.Open(processData.battleProtoData.endEvent, successProcess);
                else
                    successProcess(0.0f);
            }
            return;
        }

        //重新开始action
        StartAction();
    }


    public void ReviveSuccess()
    {
        ++mCurrentReviveCount;
        Action reviveAction = new Action();
        reviveAction.type = ActionType.ReviveUnit;
        InsertAction(reviveAction);
        StartAction();
    }

    IEnumerator ShowReviveUI()
    {
        yield return new WaitForSeconds(SpellConst.unitDeadTime);
        UIBattle.Instance.ShowReviveUI();
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

        bool needRotate = false;
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
               // bo.unit.attackCount++;
              //  OnUnitFightOver(bo);
              //  return;
				break;
            case BattleUnitAi.AiAttackStyle.Defence:
                Logger.Log(bo.unit.name + "   defence");
                //unit.attackCount++;
                //OnUnitFightOver(unit);
                break;
            case BattleUnitAi.AiAttackStyle.Buff:
                needRotate = true;
                Logger.Log(bo.unit.name + "   Beneficial");
                //	unit.attackCount++;
                // OnUnitFightOver(unit);
                //return;
                break;
            case BattleUnitAi.AiAttackStyle.MagicAttack:
            case BattleUnitAi.AiAttackStyle.PhysicsAttack:
            case BattleUnitAi.AiAttackStyle.Dazhao:
                needRotate = (aiResult.useSpell != null && aiResult.useSpell.spellData.isAoe == 0);
                break;
        }

        var curTarget = aiResult.attackTarget;
        if (null == curTarget)
        {
            Logger.LogError("Error for BattleUnitAI....");
        }

        if (needRotate == true && curTarget != null && curTarget.battleUnit != bo)
        {
            Vector3 relativePos = curTarget.battleUnit.transform.position - bo.transform.position;
            bo.SetTargetRotate(Quaternion.LookRotation(relativePos), false);
        }
        bo.unit.attackCount++;
        if (aiResult.useSpell != null)
        {
            SpellService.Instance.SpellRequest(aiResult.useSpell.spellData.id, bo.unit, aiResult.attackTarget, Time.time);
        }
        else
        {
            SpellService.Instance.SpellRequest("s1", bo.unit, aiResult.attackTarget, Time.time);
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
		GameUnit attackTarget = null;
		if(fireFocusTarget != null)
		{
			attackTarget = fireFocusTarget;
			attackTarget.attackWpName = fireAttackWpName;
		}
		else
		{
			attackTarget = BattleUnitAi.Instance.GetMagicDazhaoAttackUnit(action.caster.unit);
		}
		if (null == attackTarget) 
		{
			Logger.LogError("no Target unit,ActionOver!");
			OnActionOver();
			return;
		}
		Spell dazhaoSpell = action.caster.unit.GetDazhao ();
		if (null != dazhaoSpell) 
		{
            if (dazhaoSpell.spellData.isAoe == 0)
            {
                Vector3 relativePos = attackTarget.battleUnit.transform.position - action.caster.transform.position;
                action.caster.SetTargetRotate(Quaternion.LookRotation(relativePos), false);
            }
			SpellService.Instance.SpellRequest (dazhaoSpell.spellData.id, action.caster.unit, attackTarget, Time.time);
		}
		else
		{
			Logger.LogError("Error: dazhaoAttack not have Dazhao Spell!");
		}
	}

    IEnumerator RunSwitchPetAction(BattleObject exit, BattleObject enter)
    {
        int slot = exit.unit.pbUnit.slot;

        if (enter.camp == UnitCamp.Player)
        {
            GameObject posRoot = BattleController.Instance.GetPositionRoot();
            if (posRoot == null)
            {
                Logger.LogError("root pos can not find");
            }

            //exit.TriggerEvent("unitExit", Time.time, null);
            string nodeName = posRoot.name + "/pos" + slot.ToString();
            BattleController.Instance.curBattleScene.TriggerEvent("unitExit", Time.time, nodeName);
            battleGroup.OnUnitExitField(exit, slot);
            exit.unit.backUp = true;
            yield return new WaitForSeconds(BattleConst.unitOutTime);

            BattleController.Instance.curBattleScene.TriggerEvent("unitEnter", Time.time, nodeName);
            yield return new WaitForSeconds(BattleConst.unitInTime);

            enter.unit.CalcNextActionOrder(lastActionOrder);
            battleGroup.OnUnitEnterField(enter, slot);
            switchingPet = false;

            //check if there is empty slot in field(only check once),since another pet may dead when switching
            int emptySlot = battleGroup.GetEmptyPlayerSlot();
            if (emptySlot <= BattleConst.slotIndexMax)
            {
                exit.unit.CalcNextActionOrder(lastActionOrder);
                battleGroup.OnUnitEnterField(exit, emptySlot);
            }
        }
        else
        {
            battleGroup.OnUnitExitField(exit, slot);
            enter.unit.CalcNextActionOrder(lastActionOrder);
            battleGroup.OnUnitEnterField(enter, slot);
        }

        OnActionOver();
        yield return null;
    }

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
        if (replaceDeadUnitCount == 1 && inDazhaoAction == false)
        {
            action.isInsert = true;
            hasInsertReplaceDeadUnitAction = true;
            InsertAction(action);
        }
        else 
        {
            //if replaceDeadUnitCount is more than 1, means current action is replaceDeadUnitAction,so just trigget it
            StartCoroutine(RunReplaceDeadAction(action));
        }
    }

    IEnumerator RunReviveAction(Action action)
    {
        yield return new WaitForSeconds(BattleConst.reviveTime);
        OnActionOver();
    }

    IEnumerator RunReplaceDeadAction(ReplaceDeadUnitAction action)
    {
        if (action.isInsert == true)
        {
            hasInsertReplaceDeadUnitAction = false;
        }
        int slotID = action.slot;
        //float triggerTime = action.triggerTime;
        BattleObject enter = action.caster;

        //++replaceDeadUnitCount;
        float delayTime = SpellConst.aniDelayTime * 2;
        yield return new WaitForSeconds(delayTime);
        if (enter.camp == UnitCamp.Enemy)
        {
            slotID = slotID + BattleConst.slotIndexMax + 1;
        }

        GameObject posRoot = BattleController.Instance.GetPositionRoot();
        if (posRoot == null)
        {
            Logger.LogError("root pos can not find");
        }

        string nodeName = posRoot.name + "/pos" + slotID.ToString();
        //BattleController.Instance.curBattleScene.TriggerEvent("unitEnter", triggerTime + delayTime, nodeName);
        BattleController.Instance.curBattleScene.TriggerEvent("unitEnter", Time.time, nodeName);
        yield return new WaitForSeconds(BattleConst.unitInTime);

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

        yield return null;
    }

    #endregion

    #region Event
    //process event, add action to insertAction List
    void OnSwitchPet(int exitId, int enterId)
    {
        BattleObject enterOb = ObjectDataMgr.Instance.GetBattleObject(enterId);
        if (enterOb == null || enterOb.unit.State == UnitState.ToBeEnter)
        {
            return;
        }

        Action action = new Action();
        action.type = ActionType.SwitchPet;
        action.caster = ObjectDataMgr.Instance.GetBattleObject(exitId);
        action.target = enterOb;

        action.target.unit.State = UnitState.ToBeEnter;
        action.caster.unit.State = UnitState.ToBeExit;

        GameObject posRoot = BattleController.Instance.GetPositionRoot();
        if (posRoot == null)
        {
            Logger.LogError("root pos can not find");
        }
        string nodeName = posRoot.name + "/pos" + action.caster.unit.pbUnit.slot.ToString();
        BattleController.Instance.curBattleScene.TriggerEvent("unitBeReplaced", Time.time, nodeName);
        //action.caster.TriggerEvent("unitBeReplaced", Time.time, null);

        InsertAction(action);
        lastSwitchTime = Time.time;
        switchingPet = true;
    }

	bool	isCastDazhao	= false;//debug661 同时释放物理大招和法术大招
    public void OnUnitCastDazhao(BattleObject bo)
    {
		if (isCastDazhao)
			return;
		isCastDazhao = true;

		if (IsHaveDazhaoAction())
		{
			isCastDazhao = false;
			Logger.Log("had a dazhaoAction,can't insert Another!");
			return;
		}
		//检测是否有换怪
		if (IsChangePeting (bo)) 
		{
			isCastDazhao = false;
			Logger.LogError("change Pet.....");
			return ;
		}
        //是否眩晕
        if (bo.unit.stun > 0)
        {
			isCastDazhao = false;
            Logger.Log("unit is stun");
            return;
        }


		Spell dazhaoSpell = bo.unit.GetDazhao ();
		if (null == dazhaoSpell)
		{
			isCastDazhao = false;
			Logger.LogError("Dazhao configError: no dazhao");
			return;
		}
		
        Action action = new Action();
        action.type = ActionType.Dazhao;
        action.caster = bo;
        bo.unit.energy = 0;
		//能量扣除
		{
			SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs ();
			energyArgs.vitalType = (int)VitalType.Vital_Type_Default;
			energyArgs.triggerTime = Time.time;
			energyArgs.casterID = bo.guid;
			energyArgs.vitalChange = BattleConst.enegyMax;
			energyArgs.vitalCurrent = 0;
			energyArgs.vitalMax = 0;
			SpellService.Instance.TriggerEvent (GameEventList.SpellEnergyChange, energyArgs);
		}
        action.caster.unit.State = UnitState.Dazhao;
		
		if (dazhaoSpell.spellData.category == (int)SpellType.Spell_Type_PhyDaZhao) 
		{
			action.dazhaoType = DazhaoType.Phyics;
			InsertAction(action);
			PhyDazhaoController.Instance.PrepareDazhao (bo);
		}
		else if (dazhaoSpell.spellData.category == (int)SpellType.Spell_Type_MagicDazhao) 
		{
			action.dazhaoType = DazhaoType.Magic;
            MagicDazhaoController.Instance.PrepareShifa(action);
		}
		isCastDazhao = false;
    }

    public void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
        if (battleGo.unit.curLife < 1 || battleGo.unit.State == UnitState.Dead) 
		{
			return;
		}
        //if (battleGo.camp == UnitCamp.Enemy)
        {
            if (curAction != null)
            {
                switch (curAction.type)
                {
                    case ActionType.Dazhao:
					PhyDazhaoController.Instance.HitBattleObjectWithDazhao(battleGo,weakpointName);
                        break;
                    default:
                        OnChangeTarget(battleGo.guid, weakpointName);
                        break;
                }
            }
            else
            {
                //OnChangeTarget(battleGo.id, weakpointName);
            }

        }
    }

    public void OnChangeTarget(int id, string weakpointName)
    {
        BattleObject unit = ObjectDataMgr.Instance.GetBattleObject(id);
		string lastSelWp = null;
		if (fireFocusTarget != null)
		{
			lastSelWp = fireFocusTarget.attackWpName;
		}

		if (fireFocusTarget == unit.unit  && lastSelWp == weakpointName) 
		{
			//取消集火
			HideFireFocus();
		}
		else
		{
			fireFocusTarget = unit.unit;
			fireFocusTarget.attackWpName = weakpointName;
			fireAttackWpName = weakpointName;
			GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.ShowFireFocus, unit);
			Logger.Log("[Battle.Process]Change Fire Targret To " + unit.name + "  weakpoint name : " + weakpointName);
		} 
    }

    public void OnShowHideMonster(int id)
    {
        BattleController.Instance.GetUIBattle().SetBattleUnitVisible(id, true);

        BattleObject showUnit = ObjectDataMgr.Instance.GetBattleObject(id);
        if (showUnit != null)
        {
            showUnit.unit.CalcNextActionOrder(lastActionOrder);
        }
    }

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

        StartCoroutine(WaitAnim(movedUnit, args.aniTime + SpellConst.aniDelayTime + actionDelayTime, args.firstSpell));
        actionDelayTime = 0.0f;
    }

    IEnumerator WaitAnim(BattleObject movedUnit, float waitLen, bool firstSpell)
    {
        if (curAction == null ||  curAction.type != ActionType.Dazhao)
        {
            yield return new WaitForSeconds(waitLen);

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

	void OnRemoveDazhaoActtion()
	{
		for (int i =0; i < insertAction.Count; ++i)
		{
			Action subAction = insertAction[i];
			if(subAction.type == ActionType.Dazhao)
			{
				insertAction.Remove(subAction);
				break;
			}
		}

		Logger.LogError ("大招被打断，删除插入事件。。。。!");
	}

    //IEnumerator LoggerAnim(GameUnit movedUnit)
    //{
    //    float totalTime = 0.2f;
    //    float speed = 1f;
    //    float startTime = Time.time;
    //    while (Time.time - startTime < totalTime)
    //    {
    //        movedUnit.gameObject.transform.position += Vector3.up * speed * Time.deltaTime;
    //        yield return null;
    //    }
    //    startTime = Time.time;
    //    while (Time.time - startTime < totalTime)
    //    {
    //        movedUnit.gameObject.transform.position -= Vector3.up * speed * Time.deltaTime;
    //        yield return null;
    //    }

    //    //最后放置到指定位置上，避免时间偏差引起的位置问题
    //    movedUnit.gameObject.transform.position = BattleScene.Instance.GetSlotPosition(movedUnit.pbUnit.camp, movedUnit.pbUnit.slot);
    //}

    void OnBuffChange(EventArgs sArgs)
    {
        SpellBuffArgs args = sArgs as SpellBuffArgs;
        buffEventList.Add(args);
    }

    void OnWeakPointDead(EventArgs args)
    {
        WeakPointDeadArgs wpDeadArgs = args as WeakPointDeadArgs;
        wpDeadEventList.Add(wpDeadArgs);
    }
    #endregion

    #region Utils
    void OnUnitPrepareDazhaoOver(BattleObject moveUnit)
    {
        OnUnitFightOver(moveUnit);
    }
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

    Action GetReplaceDeadUnitAction()
    {
        Action action = null;
        foreach (Action item in insertAction)
        {
            if (item.type == ActionType.UnitReplaceDead)
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

	bool IsHaveDazhaoAction()
	{
		if (null == curAction)
			return false;
		if (curAction.type == ActionType.Dazhao)
			return true;

		//Action action = null;
		foreach (Action item in insertAction)
		{
			if (item.type == ActionType.Dazhao )
			{
				return true;
			}
		}
		return false;
	}

	bool IsChangePeting(BattleObject bo)
	{
		foreach (Action item in insertAction)
		{
			if (item.type == ActionType.SwitchPet )
			{
				if(item.caster.guid == bo.guid)
					return true;
			}
		}
		return false;
	}

    #endregion
}
