using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using DG.Tweening;

public enum BattleType
{
    Normal,
    Boss,
    Rare,
}

public class BattleController : MonoBehaviour
{
    public static float floorHeight = 0.0f;
    int curProcessIndex = 0;
    int maxProcessIndex = 0;
    int battleStartID = BattleConst.enemyStartID;
    BattleType battleType;
    InstanceData instanceData;
    BattleLevelData curBattleLevel = null;
    int instanceStar = 0;
    GameObject occlusionY;
    GameObject occlusion1;
    GameObject occlusion2;
    Animator occlusionAnimator;

    public int InstanceStar
    {
        get { return instanceStar; }
    }

    public InstanceData InstanceData
    {
        get { return instanceData; }
    }
    BattleProcess process;
    public BattleProcess Process
    {
        get { return process; }
    }
    bool isMouseOnUI = false;

    //BattleGroup battleGroup;
    //public BattleGroup BattleGroup
    //{
    //    get { return battleGroup; }
    //}
    //战斗胜利method
    MethodInfo victorMethod = null;

    static BattleController instance;
    public static BattleController Instance
    {
        get { return instance; }
    }
    //private UIBattle uiBattle;
    public bool processStart;
   // private UIScore mUIScore;

	private	Dictionary<string,Transform> cameraNodeDic = new Dictionary<string, Transform>();
   // private EnterInstanceParam curInstanceParam;
    private bool battleSuccess;
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Init()
    {
        instance = this;
        process = gameObject.AddComponent<BattleProcess>();
       // process.Init();
        //battleGroup = new BattleGroup();
    }
    //---------------------------------------------------------------------------------------------
    void Update()
    {
		Vector3 inputPos = Input.mousePosition;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }  
        else if (Input.GetMouseButtonDown(0))
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject();
        }

		if (Input.GetMouseButtonUp (0))
		{
            if (!isMouseOnUI)
            {
            //    if (!LastEvenType.Instance.IsDrag())
            //    {
            //        RaycastBattleObject(inputPos);
            //    }
            //}
            //else 
            //{
            //    GameObject curGo = EventSystem.current.currentSelectedGameObject;
            //    if (curGo != null && curGo.GetComponent<PetSwitchItem>() == null)
            //    {
            //        GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
            //    }
            }
		}
    }
    //---------------------------------------------------------------------------------------------
    //void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    //{
    //    if (battleGo.camp == UnitCamp.Enemy)
    //    {
    //        //集火或者大招
    //        process.OnHitBattleObject(battleGo, weakpointName);
    //        GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
    //        Logger.LogWarning("hit enemy gameobject....");
    //    }
    //    else if (battleGo.camp == UnitCamp.Player && process.SwitchingPet == false)
    //    {
    //        //换宠
    //        ShowSwitchPetUIArgs args = new ShowSwitchPetUIArgs();
    //        args.targetId = battleGo.guid;
    //        GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.ShowSwitchPetUI, args);
    //    }
    //}
    //---------------------------------------------------------------------------------------------
    //public void StartBattlePrepare(EnterInstanceParam enterParam)
    //{
    //    curInstanceParam = enterParam;
    //    //add player to load
    //    PbUnit pb = null;
    //    Dictionary<int, PbUnit> unitPbList = GameDataMgr.Instance.PlayerDataAttr.unitPbList;
    //    int count = enterParam.playerTeam.Count;
    //    for (int i = 0; i < count; ++i)
    //    {
    //        if (unitPbList.TryGetValue(enterParam.playerTeam[i], out pb))
    //        {
    //            AddUnitDataRequestInternal(pb.id);
    //        }
    //    }
    //    //add enemy to load
    //    curInstance = enterParam.instanceData;
    //    instanceData = StaticDataMgr.Instance.GetInstanceData(enterParam.instanceData.instanceId);
    //    count = curInstance.battle.Count;
    //    for (int index = 0; index < count; ++index)
    //    {
    //        PB.HSBattle curBattle = curInstance.battle[index];
    //        int monsterCount = curBattle.monsterCfgId.Count;
    //        for (int i = 0; i < curBattle.monsterCfgId.Count; ++i)
    //        {
    //            AddUnitDataRequestInternal(curBattle.monsterCfgId[i]);
    //        }
    //    }
    //}
    //---------------------------------------------------------------------------------------------
    //public EnterInstanceParam GetCurrentInstance()
    //{
    //    return curInstanceParam;
    //}
    //---------------------------------------------------------------------------------------------
    public void StartBattle()
    {
        curProcessIndex = 0;
        processStart = false;
        battleStartID = BattleConst.enemyStartID;
        //battleType = (BattleType)proto.battleType;
        //instanceData = StaticDataMgr.Instance.GetInstanceData(curInstanceParam.instanceData.instanceId);
        //maxProcessIndex = curInstanceParam.instanceData.battle.Count;
        //instanceStar = InstanceMapService.Instance.GetRuntimeInstance(curInstanceParam.instanceData.instanceId).star;
        //if (!InitVictorMethod())
        //    return;
        //AudioSystemMgr.Instance.PlayMusic(instanceData.instanceProtoData.backgroundmusic);
        ////设置battlegroup 并且创建模型
        ////battleGroup.SetEnemyList(proto.enemyList);
        //GameDataMgr.Instance.PlayerDataAttr.SetMainUnits(curInstanceParam.playerTeam);
    }
    //---------------------------------------------------------------------------------------------
    //public UIBattle GetUIBattle()
    //{
    //    return uiBattle;
    //}
    //---------------------------------------------------------------------------------------------
    //public GameObject GetSceneRoot()
    //{
    //    return (curBattleScene != null) ? curBattleScene.gameObject : null;
    //}
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 反射获取战斗/进程胜利的方法
    /// </summary>
    bool InitVictorMethod()
    {
        //获取战斗胜利的func
        //switch (battleType)
        //{ 
        //    case BattleType.Normal:
        //        {
        //            if (instanceData.normalValiVicMethod != null)
        //            {
        //                victorMethod = instanceData.normalValiVicMethod;
        //                break;
        //            }

        //            string funcName = instanceData.normalValiVic;
        //            var cls = typeof(NormalScript);
        //            victorMethod = cls.GetMethod(funcName);
        //            if (victorMethod == null)
        //            {
        //                Logger.LogErrorFormat("Instance {0}'s normalValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                return false;
        //            }
        //            instanceData.normalValiVicMethod = victorMethod;
        //            break;
        //        }
        //    case BattleType.Boss:
        //        {
        //            if (instanceData.bossValiVicMethod != null)
        //            {
        //                victorMethod = instanceData.bossValiVicMethod;
        //                break;
        //            }

        //            string funcName = instanceData.bossValiVic;
        //            var cls = typeof(BossScript);
        //            victorMethod = cls.GetMethod(funcName);
        //            if (victorMethod == null)
        //            {
        //                Logger.LogErrorFormat("Instance {0}'s bossValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                return false;
        //            }
        //            instanceData.bossValiVicMethod = victorMethod;

        //            foreach (var item in instanceData.bossProcess)
        //            {
        //                if (item.method != null)
        //                    break;

        //                funcName = item.func;
        //                var method = cls.GetMethod(funcName);
        //                if (method == null)
        //                {
        //                    Logger.LogErrorFormat("Instance {0}'s BossProcessValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                    return false;
        //                }
        //                item.method = method;
        //            }
        //            break;
        //        }
        //    case BattleType.Rare:
        //        {
        //            if (instanceData.rareValiVicMethod != null)
        //            {
        //                victorMethod = instanceData.rareValiVicMethod;
        //                break;
        //            }

        //            string funcName = instanceData.rareValiVic;
        //            var cls = typeof(RareScript);
        //            victorMethod = cls.GetMethod(funcName);
        //            if (victorMethod == null)
        //            {
        //                Logger.LogErrorFormat("Instance {0}'s rareValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                return false;
        //            }
        //            instanceData.rareValiVicMethod = victorMethod;

        //            foreach (var item in instanceData.rareProcess)
        //            {
        //                if (item.method != null)
        //                    break;

        //                funcName = item.func;
        //                var method = cls.GetMethod(funcName);
        //                if (method == null)
        //                {
        //                    Logger.LogErrorFormat("Instance {0}'s RareProcessValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                    return false;
        //                }
        //                item.method = method;
        //            }
        //            break;
        //        }
        //    default:
        //        Logger.LogError("Battle type error" + battleType);
        //        return false;
        //}

        return true;
    }
    //---------------------------------------------------------------------------------------------
    //void ShowUI()
    //{
    //    GameEventMgr.Instance.FireEvent(GameEventList.ShowBattleUI);
    //}
    //---------------------------------------------------------------------------------------------
    //void StartProcess(int index)
    //{
    //    if (index < maxProcessIndex)
    //    {
    //        //uiBattle.SetBattleLevelProcess(index+1, maxProcessIndex);
    //        //process.ClearRewardItem();
    //        //PB.HSBattle curBattle = curInstance.battle[index];
    //        curBattleLevel = StaticDataMgr.Instance.GetBattleLevelData(curBattle.battleCfgId);
    //        if (curBattleLevel.battleProtoData.id.Contains("boss"))
    //        {
    //            battleType = BattleType.Boss;
    //        }
    //        else
    //        {
    //            battleType = BattleType.Normal;
    //        }

    //        List<PbUnit> pbList = new List<PbUnit>();

    //        int monsterCount = curBattle.monsterCfgId.Count;
    //        int dropCount = curBattle.monsterDrop.Count;
    //        if (monsterCount != dropCount)
    //        {
    //            Logger.LogError("monster count not equal to drop count!");
    //        }

    //        for (int i = 0; i < curBattle.monsterCfgId.Count; ++i)
    //        {
    //            PbUnit pbUnit = new PbUnit();
    //            //enemy use minus uid
    //            pbUnit.guid = --battleStartID;
    //            //if (StaticDataMgr.Instance.GetUnitRowData(instanceData.bossID) != null)
    //            //    pbUnit.id = instanceData.bossID;
    //            //else
    //            pbUnit.id = curBattle.monsterCfgId[i]; //instanceData.rareID;
    //            pbUnit.level = instanceData.instanceProtoData.level;
    //            pbUnit.camp = UnitCamp.Enemy;
    //            pbUnit.slot = i;
    //            pbUnit.lazy = BattleConst.defaultLazy;

    //            pbList.Add(pbUnit);

    //            if (i < dropCount)
    //            {
    //                process.AddRewardItem(pbUnit.guid, curBattle.monsterDrop[i]);
    //            }
    //        }
    //        battleGroup.SetEnemyList(pbList);
    //        //if (index == 0)
    //        //{
    //        //    AudioSystemMgr.Instance.PlayMusic(BattleController.Instance.InstanceData.instanceProtoData.backgroundmusic);
    //        //}
    //        //TODO:动画1
    //        System.Action<float> preStartEvent = (delayTime) =>
    //        {
    //            process.StartProcess(index, curBattleLevel);
    //        };
    //        //Debug.Log("当前副本ID：" + curBattleLevel.battleProtoData.id);
    //        //demo_1_level2
    //        if (!string.IsNullOrEmpty(curBattleLevel.battleProtoData.preStartEvent) && InstanceStar == 0)
    //        {
    //            UISpeech.Open(curBattleLevel.battleProtoData.preStartEvent, preStartEvent);
    //        }
    //        else
    //        {
    //            preStartEvent(0.0f);
    //        }
    //    }
    //    else 
    //    {
    //        curBattleLevel = null;
    //        OnBattleOver(true);
    //    }
    //}
    //---------------------------------------------------------------------------------------------
    public bool HasNextProcess()
    {
        return curProcessIndex + 1 < maxProcessIndex;
    }
    //---------------------------------------------------------------------------------------------
    public void IsOcclusion(bool goOut)
    {
         if (occlusionY == null)
         {
             occlusionY = GameObject.Find("UIRoot/OcclusionY");
             if (occlusionY==null)
             {
                 return;
             }
             occlusion1 = occlusionY.transform.FindChild("y1").gameObject;
             occlusion2 = occlusionY.transform.FindChild("y2").gameObject;
             occlusion1.SetActive(true);
             occlusion2.SetActive(true);
             occlusionAnimator = occlusionY.GetComponent<Animator>();
         }
         occlusionAnimator.SetBool("out", !goOut);
         occlusionAnimator.SetBool("go", goOut);
    }
    //---------------------------------------------------------------------------------------------
    public void OnBattleOver(bool isSuccess)
    {
        battleSuccess = isSuccess;
        //ItemDropManager.Instance.ClearDropItem();
        //processStart = false;
        ////Logger.LogWarning("Battle " + (isSuccess ? "Success" : "Failed"));
        //AudioSystemMgr.Instance.StopMusic();
        //string selfEvent = isSuccess ? "win" : "failed";
        //string enemyEvent = isSuccess ? "failed" : "win";
        //float curTime = Time.time;
        //for (int i = 0; i < battleGroup.PlayerFieldList.Count; ++i)
        //{
        //    BattleObject bo = battleGroup.PlayerFieldList[i];
        //    if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
        //    {
        //        battleGroup.PlayerFieldList[i].TriggerEvent(selfEvent, curTime, null);
        //    }
        //}
        //for (int i = 0; i < battleGroup.EnemyFieldList.Count; ++i)
        //{
        //    BattleObject bo = battleGroup.EnemyFieldList[i];
        //    if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
        //    {
        //        battleGroup.EnemyFieldList[i].TriggerEvent(enemyEvent, curTime, null);
        //    }
        //}
        //MagicDazhaoController.Instance.ClearAll();
        //PhyDazhaoController.Instance.ClearAll();
        //process.HideFireFocus();

        //PB.HSInstanceSettle instanceParam = new PB.HSInstanceSettle();
        //instanceParam.victory = isSuccess;
        //GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_SETTLE_C.GetHashCode(), instanceParam, false);
    } 
    //---------------------------------------------------------------------------------------------
    //void OnInstanceSettleResult(ProtocolMessage msg)
    //{
    //    UINetRequest.Close();
    //    if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
    //    {
    //        Logger.LogError("instance settle result error");
    //        UnLoadBattleScene(0);
    //    }
    //    else
    //    {
    //        GameSpeedService.Instance.SetBattleSpeed(1.0f);
    //        //PB.HSInstanceSettleRet scoreInfo = msg.GetProtocolBody<PB.HSInstanceSettleRet>();
    //        if (mUIScore == null)
    //        {
    //            mUIScore = UIMgr.Instance.OpenUI_(UIScore.ViewName) as UIScore;
    //        }
    //        mUIScore.ShowScoreUI(battleSuccess);
    //        uiBattle.HideBattleUI();
    //    }
    //}
    //---------------------------------------------------------------------------------------------
    //void OnScoreReward(ProtocolMessage msg)
    //{
    //    PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
    //    if (reward != null && reward.hsCode == PB.code.INSTANCE_SETTLE_C.GetHashCode())
    //    {
    //        if (mUIScore == null)
    //        {
    //            mUIScore = UIMgr.Instance.OpenUI_(UIScore.ViewName) as UIScore;
    //        }
    //        mUIScore.SetScoreInfo(reward);
    //    }
    //}
    //---------------------------------------------------------------------------------------------
}
