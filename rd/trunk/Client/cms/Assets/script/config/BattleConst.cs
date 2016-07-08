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
    public const float dazhaoShowOffTime = 3;
	//大招慢镜头速度
	public	const	float	dazhaoAttackTimeScale = 0.4f;
	//大招慢镜头时长 
	public	const float	dazhaoAttackTimeLength = 2.0f;
	//法阵样式数量
	public	const	int	magicFazhencount = 5;

    //无条件关闭换宠UI
    public const int closeSwitchPetUI = -1;

    //战斗场景id
    public const int battleSceneGuid = -1;
    //怪物初始id
    public const int enemyStartID = -100;

    //换宠换出去时间
    public const float unitOutTime = 2.5f;
    //换宠换进来时间
    public const float unitInTime = 0.5f;
    //头顶节点
    public const string headNode = "headnode";
    public const string lifeBarNode = "lifebarnode";
    public const float vitalChangeDispearTime = 1.0f;

    public const float unitRotSpeed = 10.0f;

    //怪物默认性格
    public const int defaultCharacter = 3;
    //怪物默认勤奋度
    public const int defaultLazy = 3;
    public const string levelChangeEvent = "level_change";
}
