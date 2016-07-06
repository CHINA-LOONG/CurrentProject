using UnityEngine;
using System.Collections;

public class GameConfig : MonoBehaviour {

	public	static GameConfig Instance = null; 
	// Use this for initialization
	void Start () 
	{
		Instance = this;
	}

	public float FindWeakPointFinishedNeedTime = 0.6f;
	public float MirrorRadius = 100f;
	//public Vector2 MirrorCenterOffset = new Vector2(-100, 120);
	public float FireFocusWpRadius = 50f;
	public float EnmeyUnitOffsetYForBloodUI  = 1.4f;

	public	float	MaxCureMagicLifeRate  = 0.75f;	
	
}
