using UnityEngine;
using System.Collections;

public class BattleScene : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{

	}

	public void	InitWithBattleSceneName(string battlePrefabName)
	{
		GameObject go = Instantiate (PrefabLoadMgr.LoadBattleScenePrefab (battlePrefabName))as GameObject;
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
}
