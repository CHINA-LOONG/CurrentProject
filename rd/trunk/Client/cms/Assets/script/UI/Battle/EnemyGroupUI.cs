using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGroupUI : MonoBehaviour
{
    public enum EnemyGroupStyle
    {
        Normal,
        Boss
    }
    public EnemyGroupStyle enemyGroupStyle = EnemyGroupStyle.Normal;

    private EnemyUnitUI[] enemyUIArray = new EnemyUnitUI[BattleConst.maxFieldUnit + 1];

    void Start()
    {
        EnemyUnitUI[] allEnmeyUI = GetComponentsInChildren<EnemyUnitUI>();

        EnemyUnitUI subUnitUI = null;
        for (int i = 0; i < allEnmeyUI.Length; ++i)
        {
            subUnitUI = allEnmeyUI[i];
            if (subUnitUI.slot < BattleConst.maxFieldUnit)
            {
                enemyUIArray[subUnitUI.slot] = subUnitUI;
            }
            else
            {
                enemyUIArray[BattleConst.maxFieldUnit] = subUnitUI;
            }
        }

    }

    void Update()
    {
        UpdateEnmeyUICo();
    }

    void UpdateEnmeyUICo()
    {
        List<GameUnit> listEnmey = BattleController.Instance.BattleGroup.EnemyFieldList;

        bool isHaveBoss = false;
        GameUnit subUnit = null;
        EnemyUnitUI subEnmeyUI = null;

        for (int i = 0; i < listEnmey.Count; ++i)
        {
            subUnit = listEnmey[i];
            subEnmeyUI = enemyUIArray[i];

            if (null == subUnit)
            {
                subEnmeyUI.gameObject.SetActive(false);
            }
            else
            {
                if (subUnit.isBoss)
                {
                    isHaveBoss = true;
                    subEnmeyUI = enemyUIArray[BattleConst.maxFieldUnit];
                }
                subEnmeyUI.gameObject.SetActive(false);
                subEnmeyUI.UpdateShow(subUnit);
            }
        }

        if (!isHaveBoss)
        {
            enemyUIArray[BattleConst.maxFieldUnit].gameObject.SetActive(false);
        }
    }

}
