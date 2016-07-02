using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGroupUI : MonoBehaviour
{
    public BattleUnitUI[] unitUI;

    // Use this for initialization
    void Init(List<GameUnit> units)
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
    }
}
