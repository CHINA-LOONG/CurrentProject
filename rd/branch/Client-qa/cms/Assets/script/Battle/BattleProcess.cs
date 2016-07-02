using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    //如果没有集火目标，根据怪物各自AI进行战斗
    GameUnit target = null;

    //换宠cd
    float lastSwitchTime = -BattleConst.switchPetCD;

    //当前行动
    Action curAction = null;

    //大招
    float dazhaoStartTime = 0;
    float dazhaoCount = 0;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.HitDazhaoBtn, OnUnitCastDazhao);
        GameEventMgr.Instance.AddListener<int>(GameEventList.ChangeTarget, OnChangeTarget);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellFire, OnFireSpell);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellEnergyChange, OnEnergyChange);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellUnitDead, OnUnitDead);
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellBuff, OnBuffChange);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.ChangeTarget, OnChangeTarget);
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
                    if (Time.time - dazhaoStartTime > 5)
                    {
                        OnActionOver();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void OnDestory()
    {
        UnBindListener();
    }

    public void StartProcess(ProcessData process)
    {
        processData = new ProcessData();

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
            GetComponent<BattleController>().OnProcessSuccess();
            return;
        }

        Action action = GetNextAction();
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
            var a = insertAction[0];
            insertAction.RemoveAt(0);
            return a;
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
        Logger.LogFormat("Ai Attack style = {0} target = {1} ", aiResult.attackStyle, aiResult.attackTarget == null ? "no target--" : aiResult.attackTarget.name);

        switch (aiResult.attackStyle)
        {
            case BattleUnitAi.AiAttackStyle.Dazhao:
                break;
            case BattleUnitAi.AiAttackStyle.Lazy:
                Logger.Log(unit.name + "   lazy");
                OnUnitFightOver(unit);
                return;
            case BattleUnitAi.AiAttackStyle.Defence:
                Logger.Log(unit.name + "   defence");
                OnUnitFightOver(unit);
                return;
            case BattleUnitAi.AiAttackStyle.Gain:
                Logger.Log(unit.name + "   Gain");
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
        SpellService.Instance.SpellRequest("s1", unit, aiResult.attackTarget, 0);
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
        curAction = act;

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

    public void OnHitBattleObject(BattleObject battleGo)
    {
        if (curAction != null)
        {
            if (battleGo.camp == UnitCamp.Enemy)
            {
                switch (curAction.type)
                {
                    case ActionType.Dazhao:
                        Logger.Log("Dazhao hit something");
                        dazhaoCount++;
                        if (dazhaoCount >= 5)
                        {
                            OnActionOver();
                        }
                        break;
                    default:
                        OnChangeTarget(battleGo.id);
                        break;
                }
            }
        }
    }

    public void OnChangeTarget(int id)
    {
        var unit = battleGroup.GetUnitByGuid(id);
        target = unit;

        Logger.Log("[Battle.Process]Change Fire Targret To " + unit.name);
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
        float totalTime = 0.5f;
        float speed = 1;
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
        OnUnitFightOver(movedUnit);
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
        battleGroup.OnUnitExitField(deadUnit, slot);

        //查看是否还有需要上场的单位
        if (deadUnit.pbUnit.camp == UnitCamp.Enemy)
        {
            var unit = battleGroup.GetEnemyToField();
            if (unit != null)
            {
                battleGroup.OnUnitEnterField(unit, slot);
                StartCoroutine(DebugAnim(unit.gameObject));
            }
        }
        else
        {
            var switchAction = GetSwitchAction(deadId);
            if (switchAction != null)
            {
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
                    battleGroup.OnUnitEnterField(unit, slot);
                    StartCoroutine(DebugAnim(unit.gameObject));
                }
            }
        }

        //检查是否结束战斗
        if (battleGroup.IsPlayerAllDead())
        {
            GetComponent<BattleController>().OnProcessFailed();
        }
    }

    IEnumerator DebugAnim(GameObject go)
    {
        float totalTime = 1;
        float startTime = Time.time;
        while (Time.time - startTime < totalTime)
        {
            go.transform.Rotate(Vector3.up, 90 * Time.deltaTime);
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

    public bool CanSwitchPet()
    {
        return Time.time - lastSwitchTime >= BattleConst.switchPetCD;
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
