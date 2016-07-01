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
        public BattleUnit caster;
        public BattleUnit target;
    }

    BattleGroup battleGroup;
    PbBattleProcess process;

    Queue<Action> insertAction = new Queue<Action>();

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<int, int>(GameEventList.SwitchPet, OnSwitchPet);
    }

    public void Init()
    {
        BindListener();
    }

    void OnDestory()
    {
        UnBindListener();
    }

    public void StartProcess(PbBattleProcess process, BattleGroup group)
    {
        this.process = process;
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
        return process.processAnim != 0;
    }

    IEnumerator PlayProcessAnim()
    {
        yield break;
    }

    private bool HasPreAnim()
    {
        return process.preAnim != 0;
    }

    IEnumerator PlayPreAnim()
    {
        yield break;
    }

    private bool IsClearBuff()
    {
        return process.needClearBuff;
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

    void OnUnitFight(BattleUnit unit)
    {
        Logger.LogFormat("[Battle.Process]Unit {0} is moving...", unit.Name);
        OnUnitFightOver(unit);
    }

    void OnUnitFightOver(BattleUnit movedUnit)
    {
        battleGroup.ReCalcActionOrder(movedUnit.Guid);
    }

    bool IsProcessOver()
    {
        return battleGroup.IsAnySideDead();
    }

    //////////////////////////////////////////////////////////////////////////
    //process action
    void SwitchPet(BattleUnit exit, BattleUnit enter)
    {
        int slot = exit.Slot;

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

    //spell event

}
