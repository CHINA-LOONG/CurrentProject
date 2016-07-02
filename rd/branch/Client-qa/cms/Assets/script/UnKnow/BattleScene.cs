using UnityEngine;
using System.Collections;

public class BattleScene : MonoBehaviour 
{
    public static BattleScene Instance;

    GameObject sceneGo;

    Vector3[] enemySlot = new Vector3[3];
    Vector3[] playerSlot = new Vector3[3];

	Vector3[] enemySlotLocalEuler = new Vector3[3];
	Vector3[] playerSlotLocalEuler = new Vector3[3];

	public void	InitWithBattleSceneName(string assertName, string battlePrefabName)
	{
        Instance = this;
		GameObject prefab = ResourceMgr.Instance.LoadAsset (assertName, battlePrefabName) ;

		GameObject go = Instantiate (prefab) as GameObject;

		go.transform.SetParent (transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;

        sceneGo = go;

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
			{
                enemySlot[subPositionProxy.slot] = subPositionProxy.transform.position;
				enemySlotLocalEuler[subPositionProxy.slot] = subPositionProxy.transform.localEulerAngles;
			}
            else
			{
                playerSlot[subPositionProxy.slot] = subPositionProxy.transform.position;
				playerSlotLocalEuler[subPositionProxy.slot] = subPositionProxy.transform.localEulerAngles;
			}
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

	public Vector3 GetSlotLocalEuler(UnitCamp camp,int slot)
	{
		if (slot < BattleConst.slotIndexMin || slot > BattleConst.slotIndexMax)
			return Vector3.zero;
		
		if (camp == UnitCamp.Enemy)
			return enemySlotLocalEuler[slot];
		else
			return playerSlotLocalEuler[slot];
	}

    public void OnExitBattle()
    {
        Destroy(sceneGo);
    }

    void OnDestroy()
    {
        OnExitBattle();
    }
}
