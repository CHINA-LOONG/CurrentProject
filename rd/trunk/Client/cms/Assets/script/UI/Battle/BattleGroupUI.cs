using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BattleGroupUI : MonoBehaviour
{
    public BattleUnitUI[] unitUI;

    // Use this for initialization
    public void Init(List<GameUnit> units)
    {
        for (int i = 0; i < unitUI.Length; i++)
        {
            if (i < unitUI.Length)
            {
                unitUI[i].Show(units[i]);
            }
            else
            {
                unitUI[i].Hide();
            }
        }

        BindListener();
    }

    void OnDestroy()
    {
        UnBindListener();
    }

    void BindListener()
    {
    }

    void UnBindListener()
    {
    }

    void Update()
    {
        var units = BattleController.Instance.BattleGroup.PlayerFieldList;
        for (int i = 0; i < unitUI.Length; i++)
        {
            if (i < units.Count)
            {
                unitUI[i].Show(units[i]);
            }
            else
            {
                unitUI[i].Hide();
            }
        }
    }

    BattleUnitUI GetUnitById(int id)
    {
        for (int i = 0; i < unitUI.Length; i++)
        {
            if (unitUI[i].Unit.pbUnit.guid == id)
                return unitUI[i];
        }

        return null;
    }

#region Event
#endregion
}
