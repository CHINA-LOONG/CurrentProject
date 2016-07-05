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
        public BattleObject caster;
        public BattleObject target;
    }

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

    //换宠cd
    float lastSwitchTime = -BattleConst.switchPetCD;
    public float SwitchPetCD
    {
        get { return Mathf.Clamp(BattleConst.switchPetCD - (Time.time - lastSwitchTime), 0, BattleConst.switchPetCD); }
    }

    //当前行动
    Action curAction = null;

    //大招
    float dazhaoStartTime = 0;
    int dazhaoCount = 0;
    public float DazhaoLeftTime
    {
        get 
        {
            var spell = curAction.caster.unit.GetDazhao();
            if (spell != null)
            {
                float passTime = Time.time - dazhaoStartTime;
                return Mathf.Clamp(spell.spellData.channelTime - passTime, 0, spell.spellData.channelTime);
            }
            return 0;
        }
    }

    public int DazhaoLeftCount
    {
        get
        {
            var spell = curAction.caster.unit.GetDazhao();
            if (spell != null)
            {
                float passTime = Time.time - dazhaoStartTime;
                return Mathf.Clamp(spell.spellData.actionCount - dazhaoCount, 0, spell.spellData.actionCount);
            }
            return 0;
        }
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.AddListener<BattleObject>(GameEventList.HitDazhaoBtn, OnUnitCastDazhao);
        GameEventMgr.Instance.AddListener<int, string>(GameEventList.ChangeTarget, OnChangeTarget);
        GameEventMgr.Instance.AddListener<int>(GameEventList.ShowHideMonster, OnShowHideMonster);

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
        GameEventMgr.Instance.RemoveListener<BattleObject>(GameEventList.HitDazhaoBtn, OnUnitCastDazhao);
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
                    var spell = curAction.caster.unit.GetDazhao();
                    if (Time.time - dazhaoStartTime > (spell != null ? spell.spellData.channelTime : BattleConst.dazhaoDefaultTime))
                    {
                        GameEventMgr.Instance.FireEvent(GameEventList.HideDazhaoTip);
                        OnActionOver();
                    }
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
            var deadUnit = battleGroup.GetUnitByGuid(deadId);
            deathList.Add(args);
            //Logger.LogWarning("[Battle.Process]OnUnitDead: " + deadUnit.name);

            GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, deadUnit.guid);

            int slot = deadUnit.unit.pbUnit.slot;
            deadUnit.unit.State = UnitState.Dead;
            deadUnit.TriggerEvent("dead", args.triggerTime);

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
                        battleGroup.OnUnitExitField(deadUnit, slot);
                        battleGroup.OnUnitEnterField(unit, slot);
                        //StartCoroutine(DebugAnim(unit));
                    }
                }
            }

        }

        eventCount = spellEventList.Count;
        for (int i = 0; i < eventCount; ++i)
        {

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
                BattleObject deadUnit = battleGroup.GetUnitByGuid(args.deathID);
                if (deadUnit != null)
                {
                    if (deadUnit.camp == UnitCamp.Player)
                    {
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

    public void StartProcess(ProcessData process, MethodInfo victorMethod)
    {
        lastUpdateTime = Time.time;
        processData = process;
        battleVictorMethod = victorMethod;

        Logger.Log("[Battle.Process]Start process");
        battleGroup = BattleController.Instance.BattleGroup;

        StartCoroutine(Process());
    }
    public void ClearEvent()
    {
        spellEventList.Clear();
        lifeChangeEventList.Clear();
        energyEventList.Clear();
        deadEventList.Clear();
        buffEventList.Clear();
        //enemy has removed in UnLoadScene()
        //for (int i = deathList.Count - 1; i >= 0; --i)
        //{
        //    SpellUnitDeadArgs args = deathList[i];
        //    BattleObject deadUnit = battleGroup.GetUnitByGuid(args.deathID);
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
			case BattleUnitAi.AiAttackStyle.Dazhao:
			//扣除能量
			SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
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

    void RunSwitchPetAction(BattleObject exit, BattleObject enter)
    {
        int slot = exit.unit.pbUnit.slot;

        //TODO 播放动画
        battleGroup.OnUnitExitField(exit, slot);
        battleGroup.OnUnitEnterField(enter, slot);

        //StartCoroutine(DebugAnim(enter));

        OnActionOver();
    }

    void RunDazhaoAction(Action act)
    {
        Logger.Log("[Battle.Process]Enter Dazhao Mode!!!");

        //倒计时
        dazhaoStartTime = Time.time;
        dazhaoCount = 0;

        GameEventMgr.Instance.FireEvent(GameEventList.ShowDazhaoTip);

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

        action.target.unit.State = UnitState.ToBeReplaced;

        InsertAction(action);
        lastSwitchTime = Time.time;
    }

    public void OnUnitCastDazhao(BattleObject bo)
    {
        Logger.Log("OnUnitCastDazhao");
        Action action = new Action();
        action.type = ActionType.Dazhao;
        action.caster = bo;

        bo.unit.energy = 0;

        action.caster.unit.State = UnitState.Dazhao;
        InsertAction(action);
    }

    public void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
        //if (battleGo.camp == UnitCamp.Enemy)
        {
            if (curAction != null)
            {
                switch (curAction.type)
                {
                    case ActionType.Dazhao:
                        Logger.Log("Dazhao hit something");
                        battleGo.unit.attackWpName = weakpointName;
                        var caster = curAction.caster;
                        var spell = caster.unit.GetDazhao();
                        if (spell == null)
                        {
                            Logger.LogErrorFormat("[SERIOUS]Unit {0}'s dazhao error! No dazhao is configured! Exit dazhao mode!!!", caster.guid);
                            OnActionOver();
                            break;
                        }
                        SpellService.Instance.SpellRequest(spell.spellData.id, curAction.caster.unit, battleGo.unit, Time.time);
                        dazhaoCount++;
                        if (dazhaoCount >= spell.spellData.actionCount)
                        {
                            GameEventMgr.Instance.FireEvent(GameEventList.HideDazhaoTip);
                            OnActionOver();
                        }
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
        BattleObject unit = battleGroup.GetUnitByGuid(id);
        fireFocusTarget = unit.unit;
        fireFocusTarget.attackWpName = weakpointName;
        fireAttackWpName = weakpointName;
        GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.ShowFireFocus, unit);
        Logger.Log("[Battle.Process]Change Fire Targret To " + unit.name + "  weakpoint name : " + weakpointName);
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
        BattleObject movedUnit = battleGroup.GetUnitByGuid(movedUnitId);
        spellEventList.Add(args);

        StartCoroutine(WaitAnim(movedUnit, args.aniTime + SpellConst.aniDelayTime));
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

    void InsertAction(Action act)
    {
        insertAction.Add(act);
    }

    #endregion
}
