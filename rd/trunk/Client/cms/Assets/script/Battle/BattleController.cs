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
    int curProcessIndex = 0;
    int maxProcessIndex = 0;
    int battleStartID = BattleConst.enemyStartID;
    BattleType battleType;
    InstanceData instanceData;
    PB.HSInstanceEnterRet curInstance = null;
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

    BattleGroup battleGroup;
    public BattleGroup BattleGroup
    {
        get { return battleGroup; }
    }
    //战斗胜利method
    MethodInfo victorMethod = null;

    static BattleController instance;
    public static BattleController Instance
    {
        get { return instance; }
    }

    public BattleObject curBattleScene
    {
        set;
        get;
    }
    private UIBattle uiBattle;
    public bool processStart;

	private	Dictionary<string,Transform> cameraNodeDic = new Dictionary<string, Transform>();

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Init()
    {
        instance = this;
        BindListener();
        process = gameObject.AddComponent<BattleProcess>();
        process.Init();
        //battleGroup = new BattleGroup();
    }
    //---------------------------------------------------------------------------------------------
	void OnDestroy()
	{
        Destroy(process);
		UnBindListener ();
	}
    //---------------------------------------------------------------------------------------------
	void BindListener()
	{
		GameEventMgr.Instance.AddListener<Vector3>(GameEventList.MirrorClicked ,OnMirrorClilced );
	}
    //---------------------------------------------------------------------------------------------
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<Vector3> (GameEventList.MirrorClicked, OnMirrorClilced);
	}
    //---------------------------------------------------------------------------------------------
	void  OnMirrorClilced(Vector3 inputPos)
	{
		RaycastBattleObject (inputPos);
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
                if (!LastEvenType.Instance.IsDrag())
                {
                    RaycastBattleObject(inputPos);
                }
            }
            else 
            {
                GameObject curGo = EventSystem.current.currentSelectedGameObject;
                if (curGo != null && curGo.GetComponent<PetSwitchItem>() == null)
                {
                    GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
                }
            }
		}
    }
    //---------------------------------------------------------------------------------------------
	void RaycastBattleObject(Vector3 inputPos)
	{
		RaycastHit hit;
        //Ray ray = BattleCamera.Instance.GetComponentInChildren<Camera>().ScreenPointToRay(inputPos);
        Ray ray = Camera.main.ScreenPointToRay(inputPos);
		
		if (Physics.Raycast(ray, out hit, 1000))
		{
			var battleGo = hit.collider.gameObject.GetComponent<BattleObject>();
			if (battleGo)
			{
				if(battleGo.unit.isVisible)
				{
					OnHitBattleObject(battleGo, null);
				}
				return;
			}
            if (hit.collider.gameObject.tag == "DropItem")
            {
                Transform parent = hit.collider.gameObject.transform.parent;
                DropItems dropItems = parent.GetComponent<DropItems>();
                if (dropItems != null)
                {                    
                    dropItems.OnHit();
                }
            }

			var wpTarget = hit.collider.gameObject.GetComponent<WeakpointTarget>();
			if(wpTarget)
			{
				BattleObject bo = wpTarget.battleObject;
				WeakPointRuntimeData wpRuntimeData = null;
				if(bo.wpGroup.allWpDic.TryGetValue(wpTarget.wpId,out wpRuntimeData))
				{
					if( wpRuntimeData.IsCanFireFocus())
					{
						OnHitBattleObject(bo,wpTarget.wpId);
						return;
					}
				}

			}
		}
           
		GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
	}
    //---------------------------------------------------------------------------------------------
    void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
        if (battleGo.camp == UnitCamp.Enemy)
        {
            //集火或者大招
			process.OnHitBattleObject(battleGo, weakpointName);
            GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
            Logger.LogWarning("hit enemy gameobject....");
        }
        else if (battleGo.camp == UnitCamp.Player && process.SwitchingPet == false)
        {
            //换宠
            ShowSwitchPetUIArgs args = new ShowSwitchPetUIArgs();
            args.targetId = battleGo.guid;
            GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.ShowSwitchPetUI, args);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattle(EnterInstanceParam enterParam)
    {
        curProcessIndex = 0;
        processStart = false;
        battleStartID = BattleConst.enemyStartID;
        curInstance = enterParam.instanceData;
        //battleType = (BattleType)proto.battleType;
        instanceData = StaticDataMgr.Instance.GetInstanceData(enterParam.instanceData.instanceId);
        maxProcessIndex = enterParam.instanceData.battle.Count;
        instanceStar = InstanceMapService.Instance.GetRuntimeInstance(enterParam.instanceData.instanceId).star;
        if (!InitVictorMethod())
            return;
        AudioSystemMgr.Instance.PlayMusic(instanceData.instanceProtoData.backgroundmusic);
        //加载场景
        LoadBattleScene(instanceData.instanceProtoData.sceneID);
        //设置battlegroup 并且创建模型
        //battleGroup.SetEnemyList(proto.enemyList);
        GameDataMgr.Instance.PlayerDataAttr.SetMainUnits(enterParam.playerTeam);
        GameDataMgr.Instance.PlayerDataAttr.InitMainUnitList();
        battleGroup.SetPlayerList();

        var ui = UIMgr.Instance.OpenUI(UIBattle.ViewName);
        uiBattle = ui.GetComponent<UIBattle>();
        uiBattle.Init();

        StartProcess(curProcessIndex);
    }
    //---------------------------------------------------------------------------------------------
    public UIBattle GetUIBattle()
    {
        return uiBattle;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetSceneRoot()
    {
        return (curBattleScene != null) ? curBattleScene.gameObject : null;
    }
    //---------------------------------------------------------------------------------------------
    void LoadBattleScene(string sceneName)
    {
        battleGroup = new BattleGroup();
        int index = sceneName.LastIndexOf('/');
        string assetbundle = sceneName.Substring(0, index);
        string assetname = sceneName.Substring(index + 1, sceneName.Length - index - 1);
        curBattleScene = ObjectDataMgr.Instance.CreateSceneObject(BattleConst.battleSceneGuid, assetbundle, assetname);
        SetCameraDefault();
    }
    //---------------------------------------------------------------------------------------------
    public void SetCameraDefault()
    {
        Transform defaultTrans = GetDefaultCameraNode();
        BattleCamera.Instance.transform.localPosition = defaultTrans.position;
        BattleCamera.Instance.transform.localRotation = defaultTrans.rotation;
        BattleCamera.Instance.transform.localScale = Vector3.Scale(transform.localScale, defaultTrans.localScale);
    }
    //---------------------------------------------------------------------------------------------
    public Transform GetDefaultCameraNode()
    {
        string cameraSlotName = "cameraNormal";
        if (battleType == BattleType.Boss)
        {
            cameraSlotName = "cameraBoss";
        }
		Transform defaultTrans = null;
		if (cameraNodeDic.TryGetValue (cameraSlotName, out defaultTrans))
		{
			return defaultTrans;
		}

        GameObject posParent = GetPositionRoot();
        GameObject cameraNode = Util.FindChildByName(posParent, cameraSlotName);
		cameraNodeDic.Add (cameraSlotName, cameraNode.transform);
		return cameraNode.transform;
    }
	//---------------------------------------------------------------------------------------------
	public	Transform GetPhyDazhaoCameraNode()
    {
		string name = "cameraPhyDazhao";
		Transform dazhaoTrans = null;
		if (cameraNodeDic.TryGetValue (name, out dazhaoTrans))
		{
			return dazhaoTrans;
		}

        GameObject posParent = GetPositionRoot();
		GameObject cameraNode = Util.FindChildByName(posParent,name);
		cameraNodeDic.Add (name, cameraNode.transform);
		return cameraNode.transform;
	}
    //---------------------------------------------------------------------------------------------
    void UnLoadBattleScene()
    {
        battleGroup.DestroyEnemys();
        curBattleScene.ClearEvent();
        ObjectDataMgr.Instance.RemoveBattleObject(BattleConst.battleSceneGuid);
        //Destroy(curBattleScene);
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        for (int i = 0; i < playerUnitList.Count; ++i)
        {
            playerUnitList[i].ClearEvent();
            ObjectDataMgr.Instance.RemoveBattleObject(playerUnitList[i].guid);
            //playerUnitList[i].gameObject.SetActive(false);
        }
        battleGroup = null;
        curBattleScene = null;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetSlotNode(UnitCamp camp, int slotID, bool isBoss)
    {
        if (curBattleScene == null)
        {
            Logger.LogError("battle scene is null");
            return GameMain.Instance.gameObject;
        }
        GameObject posParent = GetPositionRoot();

        string nodeName = "pos";
        if (isBoss)
        {
            nodeName = "bosspos";
        }
        else 
        {
            if (camp == UnitCamp.Enemy)
            {
                slotID = slotID + BattleConst.slotIndexMax + 1;
            }

            nodeName = nodeName + slotID.ToString();
        }

        GameObject slotNode = Util.FindChildByName(posParent, nodeName);
        if (slotNode != null)
        {
            string testName = slotNode.name;
            return slotNode;
        }

        return GameMain.Instance.gameObject;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetPositionRoot()
    {
        GameObject posParent = Util.FindChildByName(curBattleScene.gameObject, "battlePosition" + curProcessIndex.ToString());
        if (posParent == null)
        {
            Logger.LogError("can not find pos slot parent!");
            posParent = curBattleScene.gameObject;
        }
        return posParent;
    }
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
    void StartProcess(int index)
    {
        if (index < maxProcessIndex)
        {
            uiBattle.SetBattleLevelProcess(index+1, maxProcessIndex);
            process.ClearRewardItem();
            PB.HSBattle curBattle = curInstance.battle[index];
            curBattleLevel = StaticDataMgr.Instance.GetBattleLevelData(curBattle.battleCfgId);
            if (curBattleLevel.battleProtoData.id.Contains("boss"))
            {
                battleType = BattleType.Boss;
            }
            else
            {
                battleType = BattleType.Normal;
            }

            List<PbUnit> pbList = new List<PbUnit>();

            int monsterCount = curBattle.monsterCfgId.Count;
            int dropCount = curBattle.monsterDrop.Count;
            if (monsterCount != dropCount)
            {
                Logger.LogError("monster count not equal to drop count!");
            }

            for (int i = 0; i < curBattle.monsterCfgId.Count; ++i)
            {
                PbUnit pbUnit = new PbUnit();
                //enemy use minus uid
                pbUnit.guid = --battleStartID;
                //if (StaticDataMgr.Instance.GetUnitRowData(instanceData.bossID) != null)
                //    pbUnit.id = instanceData.bossID;
                //else
                pbUnit.id = curBattle.monsterCfgId[i]; //instanceData.rareID;
                pbUnit.level = instanceData.instanceProtoData.level;
                pbUnit.camp = UnitCamp.Enemy;
                pbUnit.slot = i;
                pbUnit.character = BattleConst.defaultCharacter;
                pbUnit.lazy = BattleConst.defaultLazy;

                //TODO:
                pbUnit.testBossType = 1;//九尾狐
                //pbUnit.testBossType = 2;//混沌
                pbList.Add(pbUnit);

                if (i < dropCount)
                {
                    process.AddRewardItem(pbUnit.guid, curBattle.monsterDrop[i]);
                }
            }
            battleGroup.SetEnemyList(pbList);
            //if (index == 0)
            //{
            //    AudioSystemMgr.Instance.PlayMusic(BattleController.Instance.InstanceData.instanceProtoData.backgroundmusic);
            //}
            //TODO:动画1
            System.Action<float> preStartEvent = (delayTime) =>
            {
                process.StartProcess(index, curBattleLevel);
            };
            //Debug.Log("当前副本ID：" + curBattleLevel.battleProtoData.id);
            //demo_1_level2
            if (!string.IsNullOrEmpty(curBattleLevel.battleProtoData.preStartEvent) && InstanceStar == 0)
            {
                UISpeech.Open(curBattleLevel.battleProtoData.preStartEvent, preStartEvent);
            }
            else
            {
                preStartEvent(0.0f);
            }
        }
        else 
        {
            curBattleLevel = null;
            OnBattleOver(true);
        }
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator StartNextProcess(float delayTime)
    {
        if (delayTime > 0.0f)
        {
            yield return new WaitForSeconds(delayTime);
        }

        //对局切换过度
        battleGroup.DestroyEnemys();
        int playerCount = battleGroup.PlayerFieldList.Count;
        for (int index = 0; index < playerCount; ++index)
        {
            BattleObject bo = battleGroup.PlayerFieldList[index];
            if (bo != null)
            {
                bo.unit.OnStartNextProcess();
            }
        }

		MagicDazhaoController.Instance.ClearAll ();
		PhyDazhaoController.Instance.ClearAll ();
		GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
		process.HideFireFocus ();

        uiBattle.ShowUI(false);
        float waitTime = BattleConst.unitOutTime * 0.5f * GameSpeedService.Instance.GetBattleSpeed();
        Appearance(false, waitTime);
        IsOcclusion(true);
        //Fade.FadeOut(waitTime);
        ItemDropManager.Instance.ClearDropItem();
        GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
        yield return new WaitForSeconds(waitTime);

        ++curProcessIndex;
        //Fade.FadeIn(waitTime);
        cameraNodeDic.Clear();
        Appearance(true, waitTime);
        IsOcclusion(false);
        yield return new WaitForSeconds(waitTime);
        battleGroup.RefreshPlayerPos();
        uiBattle.ShowUI(true);
        uiBattle.gameObject.BroadcastMessage("OnAnimationFinish");

        StartProcess(curProcessIndex);
        SetCameraDefault();
        //TODO: fade in and fade out && end event &&recover life etc
    }
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
    public void Appearance(bool goOut, float moveTime)//move out/move in
    {
        BattleObject battleObj = null;

        Vector3 vec;//方向
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//位置
        Vector3 moveTag = new Vector3(0.0f, 0.0f, 0.0f);//移动目标
        for (int i = 0; i < battleGroup.PlayerFieldList.Count; i++)
        {
            battleObj = battleGroup.PlayerFieldList[i];
            if (battleObj == null)
            {
                continue;
            }

            Transform playerNode = GetSlotNode(UnitCamp.Player, battleObj.unit.pbUnit.slot, false).transform;
            vec = new Vector3(playerNode.forward.x, playerNode.forward.y, playerNode.forward.z);
            Vector3.Normalize(vec);
            if (goOut)//进场
            {
                newPos = BattleConst.distance * vec * -1.0f + playerNode.transform.position;
                moveTag = playerNode.position;
            }
            else//离场
            {
                newPos = vec + playerNode.position;
                moveTag = BattleConst.distance * vec + playerNode.transform.position;
            }
            battleGroup.PlayerFieldList[i].transform.localPosition = newPos;
            battleGroup.PlayerFieldList[i].transform.DOMove(moveTag, moveTime);
            battleObj.TriggerEvent(BattleConst.unitExitandenter, Time.time, null);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnBattleOver(bool isSuccess)
    {
        ItemDropManager.Instance.ClearDropItem();
        processStart = false;
		Logger.LogWarning("Battle " + (isSuccess ? "Success" : "Failed"));
        StartCoroutine(ProcessBattleOver(isSuccess));
		MagicDazhaoController.Instance.ClearAll ();
		PhyDazhaoController.Instance.ClearAll ();
		process.HideFireFocus ();
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator ProcessBattleOver(bool isSuccess)
    {
        //yield return new WaitForSeconds(5.0f);
        //胜利失败动画
        yield return StartCoroutine(PlayBalanceAnim(isSuccess));
        //后置剧情动画
        yield return StartCoroutine(PlayPostStoryAnim());
        //结束副本音乐
        AudioSystemMgr.Instance.StopMusic();
        //结算面板UI
        ShowBalanceUI();

        //怪物全部退场
        //battleGroup.AllUnitsExitField();//do this in UnLoadBattleScen

        //回到副本层
        UnLoadBattleScene();
        process.Clear();
        GameMain.Instance.ChangeModule<BuildModule>();
        UIMgr.Instance.CloseUI(UIBattle.ViewName);

		UIMgr.Instance.CloseUI (UIFazhen.ViewName);
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator PlayBalanceAnim(bool isSuccess)
    {
        uiBattle.ShowEndBattleUI(isSuccess);
        string eventName = isSuccess ? "win" : "failed";
        float curTime = Time.time;
        for (int i = 0; i < battleGroup.EnemyFieldList.Count; ++i)
        {
            BattleObject bo = battleGroup.PlayerFieldList[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
            {
                battleGroup.PlayerFieldList[i].TriggerEvent(eventName, curTime, null);
            }
        }
        yield return new WaitForSeconds(3.0f);
        uiBattle.DestroyEndBattleUI();
        yield return null;
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator PlayPostStoryAnim()
    {
        Logger.Log("[Battle]Playing Post Story Anim...");
        yield return null;
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator ShowBalanceUI()
    {
        Logger.Log("[Battle]Showing Balance UI...");
        yield return null;
    }
    //---------------------------------------------------------------------------------------------
}
