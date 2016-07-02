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
    }

    class Action
    {
        public ActionType type;
        public GameUnit caster;
        public GameUnit target;
    }

    BattleGroup battleGroup;
    ProcessData.RowData processData;

    Queue<Action> insertAction = new Queue<Action>();

    //如果没有集火目标，根据怪物各自AI进行战斗
    GameUnit target = null;

    //换宠cd
    float lastSwitchTime = -BattleConst.switchPetCD;

    //for debug
    int actionIndex = 0;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
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

    void OnDestory()
    {
        UnBindListener();
    }

    public void StartProcess(int processId, BattleGroup group)
    {
        if (processId != 0)
            processData = StaticDataMgr.Instance.ProcessData.getRowDataFromLevel(processId);
        else
            //默认进程数据
            processData = new ProcessData.RowData();

        Logger.Log("[Battle.Process]Start process");
        battleGroup = group;

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

        yield return StartCoroutine(PlayCountDownAnim());

        RefreshEnemyState();

        StartCoroutine(StartAction());
    }

    private bool HasProcessAnim()
    {
        return processData != null && !string.IsNullOrEmpty(processData.processAnimation);
    }

    IEnumerator PlayProcessAnim()
    {
        yield break;
    }

    private bool HasPreAnim()
    {
        return processData != null && !string.IsNullOrEmpty(processData.preAnimation);
    }

    IEnumerator PlayPreAnim()
    {
        yield break;
    }

    private bool IsClearBuff()
    {
        return processData != null && processData.isClearBuff;
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
        Logger.Log("[Battle.Process]Fight!!!");
    }

    private void RefreshEnemyState()
    {

    }

    IEnumerator StartAction()
    {
        if (IsProcessOver())
        {
            GetComponent<BattleController>().OnProcessSuccess();
            yield break;
        }

        Action action = GetNextAction();
        if (action != null)
        {
            switch (action.type)
            {
                case ActionType.None:
                    break;
                case ActionType.UnitFight:
                    OnUnitFight(action.caster);
                    break;
                case ActionType.SwitchPet:
                    SwitchPet(action.caster, action.target);
                    break;
                default:
                    break;
            }
        }

        yield return null;
    }

    Action GetNextAction()
    {
        //是否有插入动作，按队列顺序一个一个处理
        if (insertAction.Count > 0)
            return insertAction.Dequeue();

        //下一个单位行动
        Action action = new Action();
        action.type = ActionType.UnitFight;
        action.caster = battleGroup.GetNextMoveUnit();

        return action;
    }

    void OnActionOver()
    {
        StartCoroutine(StartAction());
    }

    void OnUnitFight(GameUnit unit)
    {
        Logger.LogFormat("[Battle.Process]Unit {0} is moving...", unit.name);

        //TODO 执行战斗
        var curTarget = SimulateAI(unit);
        Logger.Log(unit.name + " " + curTarget.name);
        SpellService.Instance.SpellRequest("s1", unit, curTarget, 0);
    }

    GameUnit SimulateAI(GameUnit unit)
    {
        GameUnit rTarget = null;
        if (unit.pbUnit.camp == UnitCamp.Player)
        {
            if (target == null)
            {
                //随机一个目标
                rTarget = battleGroup.RandomUnit(UnitCamp.Enemy);
            }
            else
            {
                rTarget = target;
            }
        }
        else
        {
            //随机一个目标
            rTarget = battleGroup.RandomUnit(UnitCamp.Player);
        }

        return rTarget;
    }

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

    //////////////////////////////////////////////////////////////////////////
    //process action
    void SwitchPet(GameUnit exit, GameUnit enter)
    {
        int slot = exit.pbUnit.slot;

        //TODO 播放动画
        battleGroup.OnUnitExitField(exit, slot);
        battleGroup.OnUnitEnterField(enter, slot);

        StartCoroutine(DebugAnim(enter.gameObject));

        OnActionOver();
    }

    #region Event
    //process event
    void OnSwitchPet(int exitId, int enterId)
    {
        Action action = new Action();
        action.type = ActionType.SwitchPet;
        action.caster = battleGroup.GetUnitByGuid(exitId);
        action.target = battleGroup.GetUnitByGuid(enterId);

        insertAction.Enqueue(action);
        lastSwitchTime = Time.time;
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

        Logger.Log("[Battle.Process]OnFireSpell");
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
        var args = sArgs as SpellVitalChangeArgs;
        Logger.Log("[Battle.Process]OnLifeChange");
    }

    void OnEnergyChange(EventArgs sArgs)
    {
        var args = sArgs as SpellVitalChangeArgs;
        Logger.Log("[Battle.Process]OnEnergyChange");
    }

    void OnUnitDead(EventArgs sArgs)
    {
        var args = sArgs as SpellUnitDeadArgs;
        int deadId = args.deathID;
        var deadUnit = battleGroup.GetUnitByGuid(deadId);
        Logger.LogWarning("[Battle.Process]OnUnitDead: " + deadUnit.name);

        int slot = deadUnit.pbUnit.slot;
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
            var unit = battleGroup.GetPlayerToField();
            if (unit != null)
            {
                battleGroup.OnUnitEnterField(unit, slot);
                StartCoroutine(DebugAnim(unit.gameObject));
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
        var args = sArgs as SpellBuffArgs;

    }
    #endregion
}
