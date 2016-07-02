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
    enum ActionType
    {
        None,
        UnitFight,
        SwitchPet,
        Dazhao,
    }

    class Action
    {
        public ActionType type;
        public GameUnit caster;
        public GameUnit target;
    }

    BattleGroup battleGroup;
    ProcessData processData;

    List<Action> insertAction = new List<Action>();

    MethodInfo battleVictorMethod;
    MethodInfo processVictorMethod;

    //如果没有集火目标，根据怪物各自AI进行战斗
    GameUnit fireFocusTarget = null;

    //换宠cd
    float lastSwitchTime = -BattleConst.switchPetCD;
    public float SwitchPetCD
    {
        get { return Mathf.Clamp(BattleConst.switchPetCD - (Time.time - lastSwitchTime), 0, 10); }
    }

    //当前行动
    Action curAction = null;

    //大招
    float dazhaoStartTime = 0;
    float dazhaoCount = 0;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.HitDazhaoBtn, OnUnitCastDazhao);
        GameEventMgr.Instance.AddListener<int, string>(GameEventList.ChangeTarget, OnChangeTarget);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellFire, OnFireSpell);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellEnergyChange, OnEnergyChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellUnitDead, OnUnitDead);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellBuff, OnBuffChange);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.RemoveListener<int, string>(GameEventList.ChangeTarget, OnChangeTarget);
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.HitDazhaoBtn, OnUnitCastDazhao);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellFire, OnFireSpell);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellEnergyChange, OnEnergyChange);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellUnitDead, OnUnitDead);
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellBuff, OnBuffChange);
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
                    var spell = curAction.caster.GetDazhao();
                    if (Time.time - dazhaoStartTime > (spell != null ? spell.spellData.channelTime : BattleConst.dazhaoDefaultTime))
                    {
                        Logger.LogWarning("[Battle.Procee]Dazhao time over!!!");
                        OnActionOver();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void OnDestroy()
    {
        UnBindListener();
    }

    public void StartProcess(ProcessData process, MethodInfo victorMethod)
    {
        processData = process;
        battleVictorMethod = victorMethod;

        Logger.Log("[Battle.Process]Start process");
        battleGroup = BattleController.Instance.BattleGroup;

        StartCoroutine(Process());
    }

    IEnumerator Process()
    {
        if (HasProcessAnim())
            yield return StartCoroutine(PlayProcessAnim());

        if (HasPreAnim())
            yield return StartCoroutine(PlayPreAnim());

        if (IsClearBuff())
            yield return StartCoroutine(ClearBuff());

        if (processData.index == 0)
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
        Logger.Log("[Battle.Process]3...");
        yield return new WaitForSeconds(1);
        Logger.Log("[Battle.Process]2...");
        yield return new WaitForSeconds(1);
        Logger.Log("[Battle.Process]1...");
        yield return new WaitForSeconds(1);
        Logger.Log("[Battle.Process]GO!!!");
    }

    private void RefreshEnemyState()
    {

    }

    void StartAction()
    {
        if (IsProcessOver())
        {
            GetComponent<BattleController>().OnProcessSwitch(1);
            return;
        }

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
                    RunSwitchPetAction(action.caster, action.target);
                    break;
                case ActionType.Dazhao:
                    RunDazhaoAction(action);
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
    void RunUnitFightAction(GameUnit unit)
    {
        Logger.LogFormat("[Battle.Process]Unit {0} is moving...", unit.name);

        //执行战斗
        var aiResult = BattleUnitAi.Instance.GetAiAttackResult(unit);
       // Logger.LogFormat("Ai Attack style = {0} target = {1} ", aiResult.attackStyle, aiResult.attackTarget == null ? "no target--" : aiResult.attackTarget.name);

        if (fireFocusTarget != null &&
            unit.pbUnit.camp == UnitCamp.Player)
        {
            aiResult.attackTarget = fireFocusTarget;
            Logger.LogWarning("reset attack target is fireFocusTarget " + fireFocusTarget.name + fireFocusTarget.pbUnit.guid + " weakpointName " + aiResult.attackTarget.attackWpName);
        }

        switch (aiResult.attackStyle)
        {
            case BattleUnitAi.AiAttackStyle.Dazhao:
                break;
            case BattleUnitAi.AiAttackStyle.Lazy:
                Logger.Log(unit.name + "   lazy");
			unit.attackCount++;
                OnUnitFightOver(unit);
                return;
            case BattleUnitAi.AiAttackStyle.Defence:
                Logger.Log(unit.name + "   defence");
			unit.attackCount++;
                OnUnitFightOver(unit);
                return;
			case BattleUnitAi.AiAttackStyle.Beneficial:
				Logger.Log(unit.name + "   Beneficial");
			unit.attackCount++;
                OnUnitFightOver(unit);
                return;
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
		if (aiResult.useSpell != null)
		{
			SpellService.Instance.SpellRequest(aiResult.useSpell.spellData.id, unit, aiResult.attackTarget, Time.time);
		} else
		{
			SpellService.Instance.SpellRequest("s1", unit, aiResult.attackTarget, Time.time);
		}
		unit.attackCount ++;
    }

    void RunSwitchPetAction(GameUnit exit, GameUnit enter)
    {
        int slot = exit.pbUnit.slot;

        //TODO 播放动画
        battleGroup.OnUnitExitField(exit, slot);
        battleGroup.OnUnitEnterField(enter, slot);

        StartCoroutine(DebugAnim(enter.gameObject));

        OnActionOver();
    }

    void RunDazhaoAction(Action act)
    {
        Logger.Log("[Battle.Process]Enter Dazhao Mode!!!");

        //倒计时
        dazhaoStartTime = Time.time;
        dazhaoCount = 0;

        //之后在OnHitBattleObject中处理
    }
    #endregion

    #region Event
    //process event, add action to insertAction List
    void OnSwitchPet(int exitId, int enterId)
    {
        Action action = new Action();
        action.type = ActionType.SwitchPet;
        action.caster = battleGroup.GetUnitByGuid(exitId);
        action.target = battleGroup.GetUnitByGuid(enterId);

        action.target.State = UnitState.ToBeReplaced;

        InsertAction(action);
        lastSwitchTime = Time.time;
    }

    public void OnUnitCastDazhao(GameUnit unit)
    {
        Action action = new Action();
        action.type = ActionType.Dazhao;
        action.caster = unit;
        unit.energy = 0;

        action.caster.State = UnitState.Dazhao;
        InsertAction(action);
    }

    public void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
        if (battleGo.camp == UnitCamp.Enemy)
        {
            if (curAction != null)
            {
                switch (curAction.type)
                {
                    case ActionType.Dazhao:
                        Logger.Log("Dazhao hit something");
                        battleGo.unit.attackWpName = weakpointName;
                        var caster = curAction.caster;
                        var spell = caster.GetDazhao();
                        if (spell == null)
                        {
                            Logger.LogErrorFormat("[SERIOUS]Unit {0}'s dazhao error! No dazhao is configured! Exit dazhao mode!!!", caster.pbUnit.id);
                            OnActionOver();
                            break;
                        }
                        SpellService.Instance.SpellRequest(spell.spellData.id, curAction.caster, battleGo.unit, Time.time);
                        dazhaoCount++;
                        if (dazhaoCount >= spell.spellData.actionCount)
                        {
                            OnActionOver();
                        }
                        break;
                    default:
                        OnChangeTarget(battleGo.id, weakpointName);
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
        var unit = battleGroup.GetUnitByGuid(id);
        fireFocusTarget = unit;
        fireFocusTarget.attackWpName = weakpointName;

        Logger.Log("[Battle.Process]Change Fire Targret To " + unit.name + "  weakpoint name : " + weakpointName);
    }

    //spell event
    void OnFireSpell(EventArgs sArgs)
    {
        var args = sArgs as SpellFireArgs;
        int movedUnitId = args.casterID;
        var movedUnit = battleGroup.GetUnitByGuid(movedUnitId);

        StartCoroutine(SimulateAnim(movedUnit));
    }

    IEnumerator SimulateAnim(GameUnit movedUnit)
    {
        if (curAction == null || (curAction != null && curAction.type != ActionType.Dazhao))
        {
            float totalTime = 1f;
            float speed = 0.5f;
            float startTime = Time.time;
            while (Time.time - startTime < totalTime)
            {
                //movedUnit.gameObject.transform.position += Vector3.up * speed * Time.deltaTime;
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < totalTime)
            {
               // movedUnit.gameObject.transform.position -= Vector3.up * speed * Time.deltaTime;
                yield return null;
            }

            OnUnitFightOver(movedUnit);
        }
    }

    void OnLifeChange(EventArgs sArgs)
    {
        //var args = sArgs as SpellVitalChangeArgs;
        //Logger.Log("[Battle.Process]OnLifeChange");
    }

    void OnEnergyChange(EventArgs sArgs)
    {
        //var args = sArgs as SpellVitalChangeArgs;
        Logger.Log("[Battle.Process]OnEnergyChange");
    }

    void OnUnitDead(EventArgs sArgs)
    {
        var args = sArgs as SpellUnitDeadArgs;
        int deadId = args.deathID;
        var deadUnit = battleGroup.GetUnitByGuid(deadId);
        //Logger.LogWarning("[Battle.Process]OnUnitDead: " + deadUnit.name);

        int slot = deadUnit.pbUnit.slot;
        deadUnit.State = UnitState.Dead;

        //查看是否还有需要上场的单位
        if (deadUnit.pbUnit.camp == UnitCamp.Enemy)
        {
            //敌方单位死亡直接退场
            battleGroup.OnUnitExitField(deadUnit, slot);

            var unit = battleGroup.GetEnemyToField();
            if (unit != null)
            {
                unit.attackWpName = null;
                battleGroup.OnUnitEnterField(unit, slot);
                StartCoroutine(DebugAnim(unit.gameObject));
            }
            fireFocusTarget = null;
        }
        else
        {
            var switchAction = GetSwitchAction(deadId);
            if (switchAction != null)
            {
                battleGroup.OnUnitExitField(deadUnit, slot);
                var unit = switchAction.caster;
                battleGroup.OnUnitEnterField(unit, slot);
                StartCoroutine(DebugAnim(unit.gameObject));
                insertAction.Remove(switchAction);
                Logger.LogWarning("Dead unit was to be replaced, get the replace unit to field.");
            }
            else
            {
                var unit = battleGroup.GetPlayerToField();
                if (unit != null)
                {
                    battleGroup.OnUnitExitField(deadUnit, slot);
                    battleGroup.OnUnitEnterField(unit, slot);
                    StartCoroutine(DebugAnim(unit.gameObject));
                }
            }
        }
    }

	IEnumerator DebugAnim(GameObject movedUnit)
    {
		float totalTime = 0.2f;
		float speed = 1f;
		float startTime = Time.time;
        while (Time.time - startTime < totalTime)
        {
			movedUnit.gameObject.transform.position += Vector3.up * speed * Time.deltaTime;
            yield return null;
		}
		startTime = Time.time;
		while (Time.time - startTime < totalTime)
		{
			movedUnit.gameObject.transform.position -= Vector3.up * speed * Time.deltaTime;
			yield return null;
		}
	}
	
	void OnBuffChange(EventArgs sArgs)
    {
        //var args = sArgs as SpellBuffArgs;

    }
    #endregion

    #region Utils
    void OnUnitFightOver(GameUnit movedUnit)
    {
        battleGroup.ReCalcActionOrder(movedUnit.pbUnit.guid);
        OnActionOver();
    }

    bool IsProcessOver()
    {
        return battleGroup.IsAnySideDead();
    }

    Action GetSwitchAction(int toBeReplaedId)
    {
        Action action = null;
        foreach (var item in insertAction)
        {
            if (item.type == ActionType.SwitchPet && item.target.pbUnit.guid == toBeReplaedId)
            {
                return item;
            }
        }
        return action;
    }

    void InsertAction(Action act)
    {
        insertAction.Add(act);
    }

    #endregion
}
