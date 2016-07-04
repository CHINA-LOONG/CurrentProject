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
        List<BattleObject> listEnmey = BattleController.Instance.BattleGroup.EnemyFieldList;

        bool isHaveBoss = false;
        BattleObject subUnit = null;
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
                if (subUnit.unit.isBoss)
                {
                    isHaveBoss = true;
					subEnmeyUI.gameObject.SetActive(false);
                    subEnmeyUI = enemyUIArray[BattleConst.maxFieldUnit];
                }
				if(subUnit.unit.isVisible)
				{
					subEnmeyUI.gameObject.SetActive(true);
					subEnmeyUI.UpdateShow(subUnit);
				}
				else
				{
					subEnmeyUI.gameObject.SetActive(false);
				}
                
            }
        }

        if (!isHaveBoss)
        {
            enemyUIArray[BattleConst.maxFieldUnit].gameObject.SetActive(false);
        }
    }

}
