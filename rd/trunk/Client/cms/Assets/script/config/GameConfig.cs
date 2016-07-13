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

	//副本入口
	public float	BossEnemyIconScale = 1.2f;

    //心跳间隔
    public static int HeartBreakInterval = 5;
    //时间同步误差
    public static int TimSynInterval = 5;
    //技能点数最大值
    public static int MaxSkillPoint = 10;
    //技能恢复时间
    public static int SkillPointTime = 360;

    public static int MaxMonsterStage = 15;

    public static int MaxMonsterCount = 180;

    public static int MaxMonsterLevel = 99;
}
