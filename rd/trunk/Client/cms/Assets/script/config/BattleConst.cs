using UnityEngine;
using System.Collections;

public enum UnitCamp
{
    Player,
    Enemy,
}

public enum AttrType
{
    Strength,
    Health,
    Intelligence,
    Defense,
    Speed
}

public class BattleConst
{
    public const int offsiteSlot = -1;
    public const int slotIndexMin = 0;
    public const int slotIndexMax = 2;

    public const int maxFieldUnit = 3;

    //换宠冷却时间
    public const float switchPetCD = 15;

    public const int speedK = 1000;
    public const float speedFactorMin = 0.98f;
    public const float speedFactorMax = 1.02f;

    //能量积攒上限
    public const int enegyMax = 100;
    public const int maxSpeed = 10000;

    public const int processContinueRet = -1;

    //大招 怪物炫耀时长
    public const float dazhaoShowOffTime = 2.5f;
    //大招慢镜头速度
    public const float dazhaoAttackTimeScale = 1.0f;
    //大招慢镜头时长 
    public const float dazhaoAttackTimeLength = 0.0f;
    //法阵样式数量
    public const int magicFazhencount = 5;

    //无条件关闭换宠UI
    public const int closeSwitchPetUI = -1;

    //战斗场景id
    public const int battleSceneGuid = -1;
    //怪物初始id
    public const int enemyStartID = -100;

    //换宠换出去时间
    public const float unitOutTime = 1.0f;
    public const float battleProcessTime = 2.0f;
    //换宠换进来时间
    public const float unitInTime = 0.5f;
    //头顶节点
    public const string headNode = "headnode";
    public const string lifeBarNode = "lifebarnode";
    public const float vitalChangeDispearTime = 1.0f;

    public const float unitRotSpeed = 10.0f;

    //怪物默认勤奋度
    public const int defaultLazy = 1;
    public const string levelChangeEvent = "level_change";
    public const float distance = 3.0f;
    public const float moveTime = 0.8f;
    public const string unitExitandenter = "unitExitandenter";

    public const float floorHeight = 0.01f;
    public const float magicDazhaoDelay = 3.0f;
    public const float battleEndDelay = 2.0f;
    public const float entranceTime = 2.0f;
    public const float reviveTime = 0.5f;

    //无效的宠物id -1；
    public const int invalidMonsterID = -1;
    //无效的装备id 
    public const long invalidEquipID = -1;
    //无效的宝石id
    public const string invalidGemID = "0";
    //最大最小宝石等级
    public const int maxGemLevel = 6;       //当前版本等级
    public const int minGemLevel = 1;
    //最大最小装备等级 9 0
    public const int maxEquipLevel = 9;
    public const int minEquipLevel = 0;
    //最大最小装备品阶 6 1
    public const int maxEquipStage = 6;
    public const int minEquipStage = 1;
    //装备类型个数
    public const int equipTypeCount = 4;
    //最大宝石个数 6-4,5-3,4-2,3-1,2-0,1-0
    public const int maxGemCount = 4;
    //最小宝石出现等级
    public const int minGemStage = 3;
    //宠物升级自身作为材料ID
    public const string stageSelfId = "self";

    public const byte maxReviveCount = 3;

    //金币图标资源名
    public const string icon_jinbi = "icon_jinbi";
    //结算动画相关
    public const float scoreTitleStayTime = 1.5f;
    public const float scoreTitleUpTime = 1.0f;
    public const float scoreStarInterval = 0.4f;
    //最大消息数
    public const int maxMsg = 100;
    public const float hintImageLength = 50f;

    public const float intervalTime = 0.1f;//各种弹数字间隔
    public const float battleLevelTime = 2.0f;//显示对局信息时间
    //万能碎片ID
    public const string commonFragmentID = "20001";

    //战力相关
    public const float bpSpellBasic = 100.0f;
    public const float bpDazhaoLvl = 1.0f;
    public const float bpPhyLvl = 1.0f;
    public const float bpMagicLvl = 1.0f;
    public const float bpDotLvl = 1.0f;

    public const float lifeBarDistance = 0.2f;

    public static Quaternion rotYPIDegree = Quaternion.identity;//new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
}
