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

	public float FindWeakPointFinishedNeedTime = 0.8f;
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
    //大冒险最大刷新次数
    public static int MaxAdventurePoint = 10;
    //大冒险恢复时间
    public static int AdventurePointTime = 3600;

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

    public int FirstBackupOpenNeedLevel = 6;
    public int SecondBackupOpenNeedLevel = 10;

    public string saodangQuanId = "10003";
    public int saodangOpenLevelForOneTimes = 8;//扫荡1次开启等级
    public int saodangOpenLevelForTenTimes = 20;//扫荡10次开启等级

    //公会
    public int createSociatyCostCoin = 500;//创建公会花费
    public int maxContributionToday = 3000;//一天公会贡献值上限
    public int contributionRewordLevel1 = 500;//公会贡献值领取
    public int contributionRewordLevel2 = 1500;
    public int contributionRewordLevel3 = 3000;
    public int sociatyTaskMaxCount = 3;
    public int sociatyTeamMaxMember = 3;

    //公会基地
    public int JidiPosition0Contribution = 0;
    public int JidiPosition1Contribution = 1000;
    public int JidiPosition2Contribution = 2000;

    //功能开启等级限制
    public int OpenLevelForPvp = 25;//公会开发等级
    public int OpenLevelForGonghui = 20;//公会开发等级
    public int OpenLevelForTower = 10;//通天塔开放等级
    //抽蛋消耗
    public int jinBiSum = 100;
    public int zuanShiSum = 10;
    public int zuanShiFree = 60;
    public int jinBiFree = 180;

    //Pvp
    public int pvpFightTimesMax = 10;
    public int pvpRestorTimeNeedSecond = 3600;
}
