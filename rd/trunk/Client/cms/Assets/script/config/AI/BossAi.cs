using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossAi : MonoBehaviour
{
    public bool isUseXgAi = false;
    public virtual BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit battleUnit)
	{
		return null;
	}

    public virtual BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit battleUnit, BattleUnitAi.AiAttackResult xgAiResult)
    {
        return null;
    }

    //---------------------------------------------------------------------------------------------
    public virtual void OnVitalChange(SpellVitalChangeArgs args)
    {

    }
    //---------------------------------------------------------------------------------------------
    public virtual void OnWpDead(WeakPointDeadArgs args)
    {

    }

    //common method
    #region
    protected List<GameUnit> GetCanAttackList(GameUnit battleUnit)
    {
        return BattleUnitAi.Instance.GetOppositeSideFiledList(battleUnit);
    }

    protected GameUnit GetAttackRandomTarget(GameUnit battleUnit)
    {
        List<GameUnit> listTarge = GetCanAttackList(battleUnit);

        int index = Random.Range(0, listTarge.Count);

        return listTarge[index];
    }

    protected int GetAttackCount(GameUnit battleUnit)
    {
        return battleUnit.attackCount;
    }

    protected Dictionary<string, Spell> GetUnitSpellList(GameUnit battleUnit)
    {
        return battleUnit.spellList;
    }

    protected List<Buff> GetUnitBuffList(GameUnit battleUnit)
    {
        return battleUnit.buffList;
    }

    protected List<string> GetAliveWeakPointList(GameUnit battleUnit)
    {
        List<string> wpList = new List<string>();

        foreach (KeyValuePair<string, WeakPointRuntimeData> subWp in battleUnit.battleUnit.wpGroup.allWpDic)
        {
            WeakPointRuntimeData wpData = subWp.Value;
            if (wpData.hp > 0)
            {
                wpList.Add(subWp.Key);
            }
        }
        return wpList;
    }

    protected int GetUnitMaxHp(GameUnit battleUnit)
    {
        return battleUnit.maxLife;
    }

    protected int GetUnitMinHp(GameUnit battleUnit)
    {
        return battleUnit.curLife;
    }

    protected int GetUnitHp(GameUnit battleUnit)
    {
        return battleUnit.curLife;
    }
    #endregion
}
