using UnityEngine;
using System.Collections;

public class GameConfig : MonoBehaviour {

	public	static GameConfig Instance = null; 
	// Use this for initialization
	void Start () 
	{
		Instance = this;
	}
	public string testBattlePrefab;
	public string testBattleAssetName;

	public float FindWeakPointFinishedNeedTime = 2.0f;
	public float MirrorRadius = 5f;
}
