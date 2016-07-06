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

		//二级属性
		public DazhaoType dazhaoType = DazhaoType.Unkown;
    }

    public float ActionDelayTime
    {
        set {actionDelayTime = value;}
        get {return actionDelayTime;}
    }
    float actionDelayTime;

    BattleGroup battleGroup;
    ProcessData processData;

    List<Action> insertAction = new List<Action>();

    MethodInfo battleVictorMethod;
    //MethodInfo processVictorMethod;

    //histroy
    //TODO: use multimap
    float lastUpdateTime;
    List<SpellFireArgs> spellEventList = new List<SpellFireArgs>();
    List<SpellVitalChangeArgs> lifeChangeEventList = new List<SpellVitalChangeArgs>();
    List<SpellVitalChangeArgs> energyEventList = new List<SpellVitalChangeArgs>();
    List<SpellUnitDeadArgs> deadEventList = new List<SpellUnitDeadArgs>();
    List<SpellBuffArgs> buffEventList = new List<SpellBuffArgs>();
    List<SpellUnitDeadArgs> deathList = new List<SpellUnitDeadArgs>();

    //如果没有集火目标，根据怪物各自AI进行战斗
    GameUnit fireFocusTarget = null;
    string fireAttackWpName = null;

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
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellBuff, OnBuffChange);

		GameEventMgr.Instance.AddListener<BattleObject> (GameEventList.DazhaoActionOver, OnDazhaoActionOver);
		GameEventMgr.Instance.AddListener (GameEventList.RemoveDazhaoAction, OnRemoveDazhaoActtion);
		GameEventMgr.Instance.AddListener<GameUnit,string> (GameEventList.WeakpoingDead, OnWeakpointDead);
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
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellBuff, OnBuffChange);

		GameEventMgr.Instance.RemoveListener<BattleObject> (GameEventList.DazhaoActionOver, OnDazhaoActionOver);
		GameEventMgr.Instance.RemoveListener (GameEventList.RemoveDazhaoAction, OnRemoveDazhaoActtion);
		GameEventMgr.Instance.RemoveListener<GameUnit,string> (GameEventList.WeakpoingDead, OnWeakpointDead);
    }

    public void Init()
    {
        BindListener();
    }

    void Update()
    {
        if (curAction != null)
        {
            switch (curAction.type)
            {
                case ActionType.None:
                    break;
                case ActionType.UnitFight:
                    break;
                case ActionType.SwitchPet:
                    break;
                case ActionType.Dazhao:
                    break;
                default:
                    break;
            }
        }

        //TODO: use battle start time as 0, not Time.time
        int eventCount = deadEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellUnitDeadArgs args = deadEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= Time.time)
            {
                continue;
            }

            int deadId = args.deathID;
			GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, deadId);
            var deadUnit = ObjectDataMgr.Instance.GetBattleObject(deadId);
            deathList.Add(args);
            //Logger.LogWarning("[Battle.Process]OnUnitDead: " + deadUnit.name);
            int slot = deadUnit.unit.pbUnit.slot;
            deadUnit.unit.State = UnitState.Dead;
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
                if (fireFocusTarget != null && fireFocusTarget.pbUnit.guid == deadUnit.guid)
                {
                    fireFocusTarget = null;
                    fireAttackWpName = null;
                    GameEventMgr.Instance.FireEvent(GameEventList.HideFireFocus);
                }

                battleGroup.OnUnitExitField(deadUnit, slot);

                BattleObject bo = battleGroup.GetEnemyToField();
                if (bo != null)
                {
                    bo.unit.attackWpName = null;
                    fireAttackWpName = null;
                    battleGroup.OnUnitEnterField(bo, slot);
                    //StartCoroutine(DebugAnim(unit));
                }
            }
            else
            {
                var switchAction = GetSwitchAction(deadId);
                if (switchAction != null)
                {
                    battleGroup.OnUnitExitField(deadUnit, slot);
                    var unit = switchAction.caster;
                    battleGroup.OnUnitEnterField(unit, slot);
                    //StartCoroutine(DebugAnim(unit));
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
                        battleGroup.OnUnitEnterField(unit, slot);
                        //StartCoroutine(DebugAnim(unit));
                    }
                    else
                    {
                        BattleController.Instance.GetUIBattle().HideUnitUI(deadUnit.guid);
                    }
                }
            }

        }

        eventCount = spellEventList.Count;
        for (int i = 0; i < eventCount; ++i)
		{
			SpellFireArgs args = spellEventList[i];
			if (args.triggerTime < lastUpdateTime || args.triggerTime >= Time.time)
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

			if(useSpell.spellData.category == (int) SpellType.Spell_Type_MagicDazhao)
			{
				MagicDazhaoController.Instance.DazhaoAttackFinished(args.casterID);
			}
			 else if (useSpell.spellData.category == (int) SpellType.Spell_Type_PhyDaZhao )
			{
				PhyDazhaoController.Instance.DazhaoAttackFinished(args.casterID);
			}


        }
        eventCount = lifeChangeEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellVitalChangeArgs args = lifeChangeEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= Time.time)
            {
                continue;
            }

            BattleController.Instance.GetUIBattle().ChangeLife(args);
        }
        eventCount = energyEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellVitalChangeArgs args = energyEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= Time.time)
            {
                continue;
            }

            BattleController.Instance.GetUIBattle().ChangeEnergy(args);
        }
        eventCount = buffEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {
            SpellBuffArgs args = buffEventList[i];
            if (args.triggerTime < lastUpdateTime || args.triggerTime >= Time.time)
            {
                continue;
            }

            BattleController.Instance.GetUIBattle().ChangeBuffState(args);
        }
        lastUpdateTime = Time.time;

        for (int i = deathList.Count - 1; i >= 0; --i)
        {
            SpellUnitDeadArgs args = deathList[i];
            if (lastUpdateTime - args.triggerTime > SpellConst.aniDelayTime * 2)
            {
                BattleObject deadUnit = ObjectDataMgr.Instance.GetBattleObject(args.deathID);
                if (deadUnit != null)
                {
                    if (deadUnit.camp == UnitCamp.Player)
                    {
                        deadUnit.unit.OnDead();
                        deadUnit.gameObject.SetActive(false);
                    }
                    else
                    {
                        ObjectDataMgr.Instance.RemoveBattleObject(args.deathID);
                    }
                }

                deathList.RemoveAt(i);
            }
        }
    }

    void OnDestroy()
    {
        UnBindListener();
    }

	void OnDazhaoActionOver(BattleObject casterObject)
	{
		OnUnitFightOver (casterObject);
	}

    public void StartProcess(ProcessData process, MethodInfo victorMethod)
    {
        lastUpdateTime = Time.time;
        processData = process;
        battleVictorMethod = victorMethod;

        Logger.Log("[Battle.Process]Start process");
        battleGroup = BattleController.Instance.BattleGroup;

        StartCoroutine(Process());
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

    IEnumerator Process()
    {
        if (HasProcessAnim())
            yield return StartCoroutine(PlayProcessAnim());

        if (HasPreAnim())
            yield return StartCoroutine(PlayPreAnim());

        if (IsClearBuff())
            yield return StartCoroutine(ClearBuff());

        //NOTE: normal battle index=0, boss's first inde=1
        if (processData.index <= 1)
            yield return StartCoroutine(PlayCountDownAnim());

        RefreshEnemyState();

        StartAction();
    }

    private bool HasProcessAnim()
    {
        return processData != null && !string.IsNullOrEmpty(processData.processAnim);
    }

    IEnumerator PlayProcessAnim()
    {
        yield break;
    }

    private bool HasPreAnim()
    {
        return processData != null && !string.IsNullOrEmpty(processData.preAnim);
    }

    IEnumerator PlayPreAnim()
    {
        yield break;
    }

    private bool IsClearBuff()
    {
        return processData != null && processData.needClearBuff;
    }

    IEnumerator ClearBuff()
    {
        yield break;
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
		if (action != null)
		{
			switch (action.type)
			{
			case ActionType.None:
				break;
			case ActionType.UnitFight:
				RunUnitFightAction(action.caster);
				break;
			case ActionType.SwitchPet:
				StartCoroutine(RunSwitchPetAction(action.caster, action.target));
				break;
			case ActionType.Dazhao:
				if(action.dazhaoType == DazhaoType.Phyics)
				{
					PhyDazhaoController.Instance.RunActionWithDazhao(action.caster);
				}
				else if (action.dazhaoType == DazhaoType.Magic)
				{
					MagicDazhaoController.Instance.RunActionWithDazhao(action.caster);
				}
				
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
        curAction = null;

        //判断战斗是否胜利
        var battleRet = (BattleRetCode)battleVictorMethod.Invoke(null, null);
        if (battleRet == BattleRetCode.Success)
        {
            BattleController.Instance.OnBattleOver(true);
            return;
        }
        else if (battleRet == BattleRetCode.Failed)
        {
            BattleController.Instance.OnBattleOver(false);
            return;
        }

        //判断进程是否结束
        //返回不包含在配置表key中的值时，表示当前进程没有结束
        var processRet = (int)processData.method.Invoke(null, null);
        if (processData.rets.ContainsKey(processRet))
        {
            var gotoVal = processData.rets[processRet];
            BattleController.Instance.OnProcessSwitch(gotoVal);
            return;
        }

        //重新开始action
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
                bo.unit.attackCount++;
                OnUnitFightOver(bo);
                return;
            case BattleUnitAi.AiAttackStyle.Defence:
                Logger.Log(bo.unit.name + "   defence");
                //unit.attackCount++;
                //OnUnitFightOver(unit);
                break;
            case BattleUnitAi.AiAttackStyle.Beneficial:
                Logger.Log(bo.unit.name + "   Beneficial");
                //	unit.attackCount++;
                // OnUnitFightOver(unit);
                //return;
                break;
            case BattleUnitAi.AiAttackStyle.MagicAttack:
                break;
            case BattleUnitAi.AiAttackStyle.PhysicsAttack:
                break;
        }

        var curTarget = aiResult.attackTarget;
        if (null == curTarget)
        {
            Debug.LogError("Error for BattleUnitAI....");
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
		Spell dazhaoSpell = action.caster.unit.GetDazhao ();
		if (null != dazhaoSpell) 
		{
			SpellService.Instance.SpellRequest (dazhaoSpell.spellData.id, action.caster.unit, attackTarget, Time.time);
		}
		else
		{
			Debug.LogError("Error: dazhaoAttack not have Dazhao Spell!");
		}
	}

    IEnumerator RunSwitchPetAction(BattleObject exit, BattleObject enter)
    {
        int slot = exit.unit.pbUnit.slot;

        if (enter.camp == UnitCamp.Player)
        {
            //exit.TriggerEvent("unitExit", Time.time, null);
            string nodeName = "pos" + slot.ToString();
            BattleController.Instance.curBattleScene.TriggerEvent("unitExit", Time.time, nodeName);
            battleGroup.OnUnitExitField(exit, slot);
            yield return new WaitForSeconds(BattleConst.unitOutTime);

            BattleController.Instance.curBattleScene.TriggerEvent("unitEnter", Time.time, nodeName);
            yield return new WaitForSeconds(BattleConst.unitInTime);
            battleGroup.OnUnitEnterField(enter, slot);
            switchingPet = false;

            //check if there is empty slot in field(only check once),since another pet may dead when switching
            int emptySlot = battleGroup.GetEmptyPlayerSlot();
            if (emptySlot <= BattleConst.slotIndexMax)
            {
                battleGroup.OnUnitEnterField(exit, emptySlot);
            }
        }
        else
        {
            battleGroup.OnUnitExitField(exit, slot);
            battleGroup.OnUnitEnterField(enter, slot);
        }

        OnActionOver();
        yield return null;
    }

    #endregion

    #region Event
    //process event, add action to insertAction List
    void OnSwitchPet(int exitId, int enterId)
    {
        Action action = new Action();
        action.type = ActionType.SwitchPet;
        action.caster = ObjectDataMgr.Instance.GetBattleObject(exitId);
        action.target = ObjectDataMgr.Instance.GetBattleObject(enterId);

        action.target.unit.State = UnitState.ToBeEnter;
        action.caster.unit.State = UnitState.ToBeExit;
        string nodeName = "pos" + action.caster.unit.pbUnit.slot.ToString();
        BattleController.Instance.curBattleScene.TriggerEvent("unitBeReplaced", Time.time, nodeName);
        //action.caster.TriggerEvent("unitBeReplaced", Time.time, null);

        InsertAction(action);
        lastSwitchTime = Time.time;
        switchingPet = true;
    }

    public void OnUnitCastDazhao(BattleObject bo)
    {
		if (IsHaveDazhaoAction())
		{
			Logger.Log("had a dazhaoAction,can't insert Another!");
			return;
		}
		if (IsChangePeting (bo)) 
		{
			Logger.LogError("change Pet.....");
			return ;
		}
		//检测是否有换怪


		Spell dazhaoSpell = bo.unit.GetDazhao ();
		if (null == dazhaoSpell)
		{
			Logger.LogError("Dazhao configError: no dazhao");
			return;
		}
		
        Action action = new Action();
        action.type = ActionType.Dazhao;
        action.caster = bo;
       // bo.unit.energy = 0;
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
    }

    public void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
		if (battleGo.unit.curLife < 1) 
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
			fireFocusTarget = null;
			GameEventMgr.Instance.FireEvent(GameEventList.HideFireFocus);
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
    }

    //spell event
    void OnFireSpell(EventArgs sArgs)
    {
        SpellFireArgs args = sArgs as SpellFireArgs;
        int movedUnitId = args.casterID;
        BattleObject movedUnit = ObjectDataMgr.Instance.GetBattleObject(movedUnitId);
        spellEventList.Add(args);

        StartCoroutine(WaitAnim(movedUnit, args.aniTime + SpellConst.aniDelayTime + actionDelayTime));
        actionDelayTime = 0.0f;
    }

    IEnumerator WaitAnim(BattleObject movedUnit, float waitLen)
    {
        if (curAction == null ||  curAction.type != ActionType.Dazhao)
        {
            yield return new WaitForSeconds(waitLen);

            OnUnitFightOver(movedUnit);
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

	/// <summary>
	/// 弱点死亡，只处理集火
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="wp">Wp.</param>
	void OnWeakpointDead(GameUnit unit,string wp)
	{
        unit.battleUnit.TriggerEvent(wp + "_dead", Time.time, null);

		if (fireFocusTarget == null || string.IsNullOrEmpty (fireAttackWpName))
			return;

		if (fireFocusTarget == unit && fireAttackWpName.EndsWith (wp))
		{
			fireFocusTarget = null;
			fireAttackWpName = null;
			GameEventMgr.Instance.FireEvent(GameEventList.HideFireFocus);
		}
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

		Debug.LogError ("大招被打断，删除插入事件。。。。!");
	}

    //IEnumerator DebugAnim(GameUnit movedUnit)
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
    #endregion

    #region Utils
    void OnUnitFightOver(BattleObject movedUnit)
    {
        battleGroup.ReCalcActionOrder(movedUnit.guid);
        OnActionOver();
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

	bool IsHaveDazhaoAction()
	{
		if (null == curAction)
			return false;
		if (curAction.type == ActionType.Dazhao)
			return true;

		Action action = null;
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
