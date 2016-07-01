using UnityEngine;
using System.Collections;

public class BattleProcess : MonoBehaviour
{
    BattleGroup battleGroup;
    PbBattleProcess process;

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

        yield return StartCoroutine(StartFight());
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

    IEnumerator StartFight()
    {
        var unit = battleGroup.GetNextMoveUnit();

        yield return new WaitForSeconds(1);
        OnUnitAction(unit);
    }

    void OnUnitAction(BattleUnit unit)
    {
        Logger.LogFormat("[Battle.Process]Unit {0} is moving...", unit.Name);
        OnUnitActionOver(unit);
    }

    void OnUnitActionOver(BattleUnit movedUnit)
    {
        battleGroup.ReCalcActionOrder(movedUnit.Guid);

        if(!IsProcessOver())
            StartCoroutine(StartFight());
        else
        {
            GetComponent<BattleController>().OnProcessOver();
        }
    }

    bool IsProcessOver()
    {
        return battleGroup.IsAnySideDead();
    }

    //////////////////////////////////////////////////////////////////////////
    //event

}
