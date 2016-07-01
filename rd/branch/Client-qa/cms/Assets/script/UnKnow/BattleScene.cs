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
	}
}
