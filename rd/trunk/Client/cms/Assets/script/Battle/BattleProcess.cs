using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    GameUnit curTarget = null;

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.AddListener<int>(GameEventList.ChangeTarget, OnChangeTarget);
        GameEventMgr.Instance.AddListener(GameEventList.FireSpell, OnFireSpell);
        GameEventMgr.Instance.AddListener(GameEventList.LifeChange, OnLifeChange);
        GameEventMgr.Instance.AddListener(GameEventList.EnergyChange, OnEnergyChange);
        GameEventMgr.Instance.AddListener(GameEventList.UnitDead, OnUnitDead);
        GameEventMgr.Instance.AddListener(GameEventList.BuffAdd, OnBuffAdd);
        GameEventMgr.Instance.AddListener(GameEventList.BuffRemove, OnBuffRemove);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.ChangeTarget, OnChangeTarget);
        GameEventMgr.Instance.RemoveListener(GameEventList.FireSpell, OnFireSpell);
        GameEventMgr.Instance.RemoveListener(GameEventList.LifeChange, OnLifeChange);
        GameEventMgr.Instance.RemoveListener(GameEventList.EnergyChange, OnEnergyChange);
        GameEventMgr.Instance.RemoveListener(GameEventList.UnitDead, OnUnitDead);
        GameEventMgr.Instance.RemoveListener(GameEventList.BuffAdd, OnBuffAdd);
        GameEventMgr.Instance.RemoveListener(GameEventList.BuffRemove, OnBuffRemove);
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

        InitData();

        StartCoroutine(Process());
    }

    void InitData()
    {
        battleGroup.CalcActionOrder();
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
        OnActionOver();
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
        if (!IsProcessOver())
            StartCoroutine(StartAction());
        else
        {
            GetComponent<BattleController>().OnProcessOver();
        }
    }

    void OnUnitFight(GameUnit unit)
    {
        Logger.LogFormat("[Battle.Process]Unit {0} is moving...", unit.name);
        //TODO 执行战斗

        OnUnitFightOver(unit);
    }

    void OnUnitFightOver(GameUnit movedUnit)
    {
        battleGroup.ReCalcActionOrder(movedUnit.pbUnit.guid);
    }

    bool IsProcessOver()
    {
        return battleGroup.IsAnySideDead();
    }

    //////////////////////////////////////////////////////////////////////////
    //process action
    void SwitchPet(GameUnit exit, GameUnit enter)
    {
        int slot = exit.pbUnit.guid;

        //TODO 播放动画
        battleGroup.OnUnitExitField(exit, slot);
        battleGroup.OnUnitEnterField(enter, slot);
    }

    //////////////////////////////////////////////////////////////////////////
    //process event
    void OnSwitchPet(int exitId, int enterId)
    {
        Action action = new Action();
        action.type = ActionType.SwitchPet;
        action.caster = battleGroup.GetUnitByGuid(exitId);
        action.target = battleGroup.GetUnitByGuid(enterId);

        insertAction.Enqueue(action);
    }

    void OnChangeTarget(int id)
    {
        var unit = battleGroup.GetUnitByGuid(id);
        curTarget = unit;

        Logger.Log("[Battle.Process]Change Fire Targret To " + unit.name);
    }

    //spell event
    void OnFireSpell()
    {
        int movedUnitId = 0;
        var movedUnit = battleGroup.GetUnitByGuid(movedUnitId);

        Logger.Log("[Battle.Process]OnFireSpell");
    }

    void OnLifeChange()
    {
        Logger.Log("[Battle.Process]OnLifeChange");
    }

    void OnEnergyChange()
    {
        Logger.Log("[Battle.Process]OnEnergyChange");
    }

    void OnUnitDead()
    {
        int deadId = 0;
        var deadUnit = battleGroup.GetUnitByGuid(deadId);
        Logger.Log("[Battle.Process]OnUnitDead: " + deadUnit.name);

        int slot = deadUnit.pbUnit.slot;
        battleGroup.OnUnitExitField(deadUnit, slot);

        //查看是否还有需要上场的单位
        if (deadUnit.pbUnit.camp == UnitCamp.Enemy)
        {

        }
        else
        {
            //自己单位弹出换宠UI
        }
    }

    void OnBuffAdd()
    {

    }

    void OnBuffRemove()
    {

    }
}
