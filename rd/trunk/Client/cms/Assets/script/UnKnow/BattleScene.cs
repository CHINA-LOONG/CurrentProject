using UnityEngine;
using System.Collections;

public class BattleScene : MonoBehaviour 
{
    public static BattleScene Instance;

    Vector3[] enemySlot = new Vector3[3];
    Vector3[] playerSlot = new Vector3[3];

	public void	InitWithBattleSceneName(string assertName, string battlePrefabName)
	{
        Instance = this;
		GameObject prefab = ResourceMgr.Instance.LoadAsset (assertName, battlePrefabName) ;

		GameObject go = Instantiate (prefab) as GameObject;

		go.transform.SetParent (transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;

		SetBattlePosition ();
		SetBattleEffectPosition ();
	}

	private	void	SetBattlePosition()
	{
		BattlePositionProxy [] positionProxy = GetComponentsInChildren<BattlePositionProxy> ();
		if (null == positionProxy)
		{
			return;
		}

		BattlePositionProxy subPositionProxy;
		for (int i = 0; i < positionProxy.Length; ++ i)
		{
			subPositionProxy = positionProxy[i];
			subPositionProxy.gameObject.SetActive(false);
            if (subPositionProxy.camp == UnitCamp.Enemy)
                enemySlot[subPositionProxy.slot] = subPositionProxy.transform.position;
            else
                playerSlot[subPositionProxy.slot] = subPositionProxy.transform.position;
		}
	}

	private void SetBattleEffectPosition()
	{
		BattleEffectProxy [] effectProxy = GetComponentsInChildren<BattleEffectProxy> ();
		if (null == effectProxy)
		{
			return;
		}
		
		BattleEffectProxy subEffectProxy;
		for (int i = 0; i < effectProxy.Length; ++ i)
		{
			subEffectProxy = effectProxy[i];
			subEffectProxy.gameObject.SetActive(false);
		}
	}

    public Vector3 GetSlotPosition(UnitCamp camp, int slot)
    {
        if (slot < BattleConst.slotIndexMin || slot > BattleConst.slotIndexMax)
            return Vector3.zero;

        if (camp == UnitCamp.Enemy)
            return enemySlot[slot];
        else
            return playerSlot[slot];
    }
}
