using UnityEngine;
using System.Collections;

public class PrefabLoadMgr 
{
	static string   UIPrefabPath	= "Assets/cmsAsset/prefabs/UI/";
	static string   BattleScenePrefabPath = "Assets/cmsAsset/prefabs/battleScene/";

	static public GameObject LoadUIPrefab(string name)
	{
		return LoadPrefabWithFullPath (UIPrefabPath + name + ".prefab");
	}

	static	public GameObject LoadBattleScenePrefab(string name)
	{
		return LoadPrefabWithFullPath (BattleScenePrefabPath + name + ".prefab");
	}

	static public GameObject LoadPrefabWithFullPath(string fullPath)
	{
		GameObject prefab = Resources.LoadAssetAtPath(fullPath,typeof(GameObject)) as GameObject;
		if (null == prefab) 
		{
			Debug.LogError("Error for load prefab " + fullPath);
		}
		return prefab;
	}
}
