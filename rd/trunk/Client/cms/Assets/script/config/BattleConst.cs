using UnityEngine;
using System.Collections;

public enum UnitCamp
{
    Player,
    Enemy,
}

public class BattleConst
{
    public const int offsiteSlot = -1;
    public const int slotIndexMin = 0;
    public const int slotIndexMax = 2;

    public const int maxFieldUnit = 3;

    //换宠冷却时间
    public const float switchPetCD = 10;

    public const int speedK = 1000;
    public const float speedFactorMin = 0.98f;
    public const float speedFactorMax = 1.02f;

    //能量积攒上限
    public const int enegyMax = 100;
    public const int maxSpeed = 10000;

    public const int processContinueRet = -1;

    //大招 怪物炫耀时长
    public const float dazhaoShowOffTime = 2;
	//大招慢镜头速度
	public	const	float	dazhaoAttackTimeScale = 1.0f;
	//大招慢镜头时长 
	public	const float	dazhaoAttackTimeLength = 0.0f;
	//法阵样式数量
	public	const	int	magicFazhencount = 5;

    //无条件关闭换宠UI
    public const int closeSwitchPetUI = -1;

    //战斗场景id
    public const int battleSceneGuid = -1;
    //怪物初始id
    public const int enemyStartID = -100;

    //换宠换出去时间
    public const float unitOutTime = 1.0f;
    public const float battleProcessTime = 3.0f;
    //换宠换进来时间
    public const float unitInTime = 0.5f;
    //头顶节点
    public const string headNode = "headnode";
    public const string lifeBarNode = "lifebarnode";
    public const float vitalChangeDispearTime = 1.0f;

    public const float unitRotSpeed = 10.0f;

    //怪物默认勤奋度
    public const int defaultLazy = 1;
    //public const string levelChangeEvent = "level_change";
    public const float distance = 3.0f;
    public const float moveTime = 0.8f;
    public const string unitExitandenter = "unitExitandenter";

    public const float floorHeight = 0.01f;
    public const float magicDazhaoDelay = 3.0f;
    public const float battleEndDelay = 2.0f;
    public const float entranceTime = 1.5f;
}
