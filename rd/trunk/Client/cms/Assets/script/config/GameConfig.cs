using UnityEngine;
using System.Collections;

public class GameConfig : MonoBehaviour {

	public	static GameConfig Instance = null; 
	// Use this for initialization
	void Start () 
	{
		Instance = this;
	}

	public	string	GoldExchangeId = "1";

	public float FindWeakPointFinishedNeedTime = 1.5f;
	public float MirrorRadius = 60f;
	//public Vector2 MirrorCenterOffset = new Vector2(-100, 120);
	public float FireFocusWpRadius = 50f;
	public float EnmeyUnitOffsetYForBloodUI  = 1.4f;

	public	float	MaxCureMagicLifeRate  = 0.75f;

    //照妖镜能量相关
    public float MirrorInitallyEnegy = 50;
    public float MirrorMaxEnegy = 100;
    public float UseMirrorMinEnegy = 20;
    public float RecoveryMirrorEnegyUnit = 2;//每200毫秒恢复能量数值
    public float ConsumMirrorEnegyUnit = 4;//

    //battle
    public  DG.Tweening.Ease InPhyDazhaoEaseAni = DG.Tweening.Ease.Linear;
    public  DG.Tweening.Ease OutPhyDazhaoEaseAni = DG.Tweening.Ease.Linear;


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

    public static int MaxPlayerLevel = 50;

	//背包中购买钥匙最大数
	public	int	maxBuyItemCount	= 50;
	//背包 最大开宝箱数量 
	public	int	maxOpenBoxCount = 10;

    //活力值恢复
    public int RestoreHuoLiNeedSeconds = 600;

    public int FirstBackupOpenNeedLevel = 5;
    public int SecondBackupOpenNeedLevel = 10;

    public string saodangQuanId = "10003";
    public int saodangOpenLevelForOneTimes = 15;//扫荡1次开启等级
    public int saodangOpenLevelForTenTimes = 20;//扫荡10次开启等级

    //公会
    public int createSociatyCostZuanshi = 500;//创建公会花费
}
