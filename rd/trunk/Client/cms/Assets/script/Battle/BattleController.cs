using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using DG.Tweening;

public enum ExitInstanceType
{
    Exit_Instance_OK,
    Exit_Instance_Next,
    Exit_Instance_Retry,
    Exit_Instance_Summon,
    Exit_Instance_Pet,
    Exit_Instance_PVP,
    Num_Exit_Instance_Type
}

public enum BattleType
{
    Normal,
    Boss,
    Rare,
}

public class BattleController : MonoBehaviour
{
    public static float floorHeight = 0.0f;
    public string instanceSpell;
    int curProcessIndex = 0;
    int maxProcessIndex = 0;
    int battleStartID = BattleConst.enemyStartID;
    public  BattleType battleType;
    InstanceData instanceData;
    BattleLevelData curBattleLevel = null;
    int instanceStar = 0;
    GameObject occlusionY;
    GameObject occlusion1;
    GameObject occlusion2;
    Animator occlusionAnimator;
    public double beginChangeEnegyTime = 0;
    private float mirrorEnegy = 0;
    public  float MirrorEnegyAttr
    {
        get
        {
            return mirrorEnegy;
        }
        set
        {
            mirrorEnegy = value;
            if(mirrorEnegy < 0)
            {
                mirrorEnegy = 0;
            }
            if(mirrorEnegy > GameConfig.Instance.MirrorMaxEnegy)
            {
                mirrorEnegy = GameConfig.Instance.MirrorMaxEnegy;
            }
            if (uiBattle != null && uiBattle.m_MirrorDray != null)
            {
                uiBattle.m_MirrorDray.UpdateMirrorEnegy();
            }
        }
    }
    public MirrorState mirrorState = MirrorState.CannotUse;

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

    bool mRevived;
    public bool IsRevived
    {
        set { mRevived = value; }
        get { return mRevived; }
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
    private UIScore mUIScore;
    public int mHuoliBeforeScore;
    private UILevelInfo mUILevelInfo;

	private	Dictionary<string,Transform> cameraNodeDic = new Dictionary<string, Transform>();
    private EnterInstanceParam curInstanceParam;
    public PvpFightParam PvpParam
    {
        get { return mCurPvpParam; }
    }
    private PvpFightParam mCurPvpParam;

    public GuideLevelParam GuideLevelParam
    {
        get { return mCurGuideLevelParam; }
    }
    private GuideLevelParam mCurGuideLevelParam;

    public int PvpHornorPointGet
    {
        get { return mPvpHornorPointGet; }
        set { mPvpHornorPointGet = value; }
    }
    private int mPvpHornorPointGet;

    private byte mBattleResult;
    private byte mCurMaxSlotIndex;
    public byte CurMaxSlotIndex
    {
        get { return mCurMaxSlotIndex; }
    }

    public bool isUseWpFindWpInBattle = false;
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
        GameEventMgr.Instance.AddListener<Vector3>(GameEventList.MirrorClicked, OnMirrorClilced);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_C.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_S.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnScoreReward);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_SETTLE_C.GetHashCode().ToString(), OnPvpSettleResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_SETTLE_S.GetHashCode().ToString(), OnPvpSettleResult);
    }
    //---------------------------------------------------------------------------------------------
	void UnBindListener()
	{
        GameEventMgr.Instance.RemoveListener<Vector3>(GameEventList.MirrorClicked, OnMirrorClilced);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_C.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_S.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnScoreReward);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_SETTLE_C.GetHashCode().ToString(), OnPvpSettleResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_SETTLE_S.GetHashCode().ToString(), OnPvpSettleResult);
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
        int changeStep = 0;
        //mirror enegy
        if(battleType == BattleType.Boss)
        {
            if (mirrorState == MirrorState.Recover)
            {
                changeStep = 1;
            }
            else if (mirrorState == MirrorState.Consum)
            {
                changeStep = -1;
            }
        }

        if (changeStep != 0 && process.PauseEnable == false)
        {
            //double curTime = GameTimeMgr.Instance.TimeStampAsMilliseconds();
            if (changeStep == 1)
            {
                MirrorEnegyAttr += (GameConfig.Instance.RecoveryMirrorEnegyUnit * Time.unscaledDeltaTime);
            }
            else
            {
                MirrorEnegyAttr -= (GameConfig.Instance.ConsumMirrorEnegyUnit * Time.unscaledDeltaTime);
            }
            //if (curTime - beginChangeEnegyTime > 200)
            //{
            //    int times = (int)((curTime - beginChangeEnegyTime) * 0.005);
            //    if(changeStep == 1)
            //    {
            //        MirrorEnegyAttr += times * GameConfig.Instance.RecoveryMirrorEnegyUnit;
            //    }
            //    else
            //    {
            //        MirrorEnegyAttr -= times * GameConfig.Instance.ConsumMirrorEnegyUnit;
            //    }
            //    beginChangeEnegyTime += times * 200;
            //}
            if(changeStep == -1 && MirrorEnegyAttr < 0.001)
            {
                GameEventMgr.Instance.FireEvent<bool,bool>(GameEventList.SetMirrorModeState, false,true);
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("battle_zhaoyaojing_002"), (int)PB.ImType.PROMPT);
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
            // Logger.LogWarning("hit enemy gameobject....");
        }
        else if (
            battleGo.camp == UnitCamp.Player &&
            mCurPvpParam == null &&
            process.SwitchingPet == false &&
            uiBattle.gameObject.activeSelf == true &&
            battleGo.unit.backUp == false &&
            process.InDazhao == false
            )
        {
            //换宠
            ShowSwitchPetUIArgs args = new ShowSwitchPetUIArgs();
            args.targetId = battleGo.guid;
            GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.ShowSwitchPetUI, args);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattlePrepare(EnterInstanceParam enterParam)
    {
        curInstanceParam = enterParam;
        mCurPvpParam = null;
        mCurGuideLevelParam = null;
        //add player to load
        PbUnit pb = null;
        Dictionary<int, PbUnit> unitPbList = GameDataMgr.Instance.PlayerDataAttr.unitPbList;
        int count = enterParam.playerTeam.Count;
        for (int i = 0; i < count; ++i)
        {
            if (unitPbList.TryGetValue(enterParam.playerTeam[i], out pb))
            {
                AddUnitDataRequestInternal(pb.id);
            }
        }
        //add enemy to load
        instanceData = StaticDataMgr.Instance.GetInstanceData(enterParam.instanceData.instanceId);
        count = enterParam.instanceData.battle.Count;
        for (int index = 0; index < count; ++index)
        {
            PB.HSBattle curBattle = enterParam.instanceData.battle[index];
            int monsterCount = curBattle.monsterCfgId.Count;
            for (int i = 0; i < curBattle.monsterCfgId.Count; ++i)
            {
                AddUnitDataRequestInternal(curBattle.monsterCfgId[i]);
            }
        }

        mHuoliBeforeScore = GameDataMgr.Instance.PlayerDataAttr.HuoliAttr;
        StartBattlePrepareCommon();
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattlePvpPrepare(PvpFightParam pvpParam)
    {
        mCurPvpParam = pvpParam;
        mPvpHornorPointGet = 0;
        curInstanceParam = null;
        mCurGuideLevelParam = null;
        //add player to load
        PbUnit pb = null;
        Dictionary<int, PbUnit> unitPbList = GameDataMgr.Instance.PlayerDataAttr.unitPbList;
        int count = pvpParam.playerTeam.Count;
        for (int i = 0; i < count; ++i)
        {
            if (unitPbList.TryGetValue(pvpParam.playerTeam[i], out pb))
            {
                AddUnitDataRequestInternal(pb.id);
            }
        }

        //add enemy to load
        List<PB.HSMonster> defendList = pvpParam.targetData.defenceData.monsterInfo;
        count = defendList.Count;
        for (int i = 0; i < count; ++i)
        {
            AddUnitDataRequestInternal(defendList[i].cfgId);
        }

        instanceData = StaticDataMgr.Instance.GetInstanceData("pvp_scene");

        StartBattlePrepareCommon();
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattleGuidePrepare(GuideLevelParam guideParam)
    {
        mCurGuideLevelParam = guideParam;
        mCurPvpParam = null;
        curInstanceParam = null;
        //add player to load
        int count = mCurGuideLevelParam.selfIdList.Count;
        for (int i = 0; i < count; ++i)
        {
            AddUnitDataRequestInternal(mCurGuideLevelParam.selfIdList[i]);
        }
        //add enemy to load
        count = mCurGuideLevelParam.enemyIdList.Count;
        for (int i = 0; i < count; ++i)
        {
            AddUnitDataRequestInternal(mCurGuideLevelParam.enemyIdList[i]);
        }
        StartBattlePrepareCommon();
    }
    //---------------------------------------------------------------------------------------------
    private void StartBattlePrepareCommon()
    {
        ActorEventService.Instance.AddResourceGroup("common");
        ActorEventService.Instance.AddResourceGroup("commonAttack");
        ActorEventService.Instance.AddResourceGroup("commonBuff");
        ActorEventService.Instance.AddResourceGroup("cure");
        ActorEventService.Instance.AddResourceGroup("commonDeBuff");
        ActorEventService.Instance.AddResourceGroup("commonPassive");

        if (mCurGuideLevelParam == null)
        {
            //add scoreui
            UIScore.AddResourceRequest();
            //add hint message
            ResourceMgr.Instance.AddAssetRequest(new AssetRequest("hintMessage"));
        }
    }
    //---------------------------------------------------------------------------------------------
    public EnterInstanceParam GetCurrentInstance()
    {
        return curInstanceParam;
    }
    //---------------------------------------------------------------------------------------------
    private void AddUnitDataRequestInternal(string unitID)
    {
        ResourceMgr resMgr = ResourceMgr.Instance;
        UnitData unitRowData = StaticDataMgr.Instance.GetUnitRowData(unitID);
        if (unitRowData != null)
        {
            resMgr.AddAssetRequest(new AssetRequest(unitRowData.assetID));
        }

        //TODO:add spell request
        ActorEventService.Instance.AddResourceGroup(unitID);
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattle()
    {
        mCurMaxSlotIndex = BattleConst.slotIndexMax;
        if (mCurPvpParam != null)
        {
            mCurMaxSlotIndex = BattleConst.slotIndexMaxPVP;
        }
        isUseWpFindWpInBattle = false;
        mRevived = false;
        //ResourceMgr.Instance.UnloadCachedBundles(true);
        curProcessIndex = 0;
        processStart = false;
        battleStartID = BattleConst.enemyStartID;
        if (curInstanceParam != null)
        {
            //battleType = (BattleType)proto.battleType;
            instanceData = StaticDataMgr.Instance.GetInstanceData(curInstanceParam.instanceData.instanceId);
            maxProcessIndex = curInstanceParam.instanceData.battle.Count;
            InstanceEntryRuntimeData instanceRuntimeData = InstanceMapService.Instance.GetRuntimeInstance(curInstanceParam.instanceData.instanceId);
            if (instanceRuntimeData != null)
            {
                instanceStar = instanceRuntimeData.star;
            }
            //GameDataMgr.Instance.PlayerDataAttr.SetMainUnits(curInstanceParam.playerTeam);
        }
        else if (mCurPvpParam != null)
        {
            instanceData = StaticDataMgr.Instance.GetInstanceData("pvp_scene");
            maxProcessIndex = 1;
            instanceStar = 0;
        }
        else if (mCurGuideLevelParam != null)
        {
            instanceData = StaticDataMgr.Instance.GetInstanceData(mCurGuideLevelParam.instanceId);
            maxProcessIndex = 1;
            instanceStar = 0;
        }

        AudioSystemMgr.Instance.PlayMusicByID(instanceData.instanceProtoData.backgroundmusic);

        //加载场景
        LoadBattleScene();
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
    void LoadBattleScene()
    {
        battleGroup = new BattleGroup();
        GameObject sceneRoot = GameObject.Find("Root");
        if (sceneRoot == null)
        {
            Logger.LogError("can not find pos slot parent!");
            sceneRoot = gameObject;
        }
        curBattleScene = ObjectDataMgr.Instance.AddSceneObject(BattleConst.battleSceneGuid, sceneRoot);
        
        //init scene spell if there has
        InstanceEntry entryData = StaticDataMgr.Instance.GetInstanceEntry(instanceData.instanceProtoData.id);
        if (entryData != null && string.IsNullOrEmpty(entryData.instanceSpell) == false)
        {
            curBattleScene.unit.spellList = new Dictionary<string, Spell>();
            instanceSpell = entryData.instanceSpell;
            SpellProtoType spellPt = StaticDataMgr.Instance.GetSpellProtoData(instanceSpell);
            if (spellPt != null)
            {
                if (
                    spellPt.category == (int)SpellType.Spell_Type_Defense ||
                    spellPt.category == (int)SpellType.Spell_Type_Passive ||
                    spellPt.category == (int)SpellType.Spell_Type_Beneficial ||
                    spellPt.category == (int)SpellType.Spell_Type_Negative ||
                    spellPt.category == (int)SpellType.Spell_Type_Lazy ||
                    spellPt.category == (int)SpellType.Spell_Type_PrepareDazhao ||
                    spellPt.category == (int)SpellType.Spell_Type_Hot
                    )
                {
                    curBattleScene.unit.spellList.Add(instanceSpell, new Spell(spellPt, 1));
                }
                else
                {
                    curBattleScene.unit.spellList.Add(instanceSpell, new Spell(spellPt, instanceData.instanceProtoData.level));
                }
            }
        }
        
        if (mCurPvpParam != null)
        {
            battleGroup.SetPlayerList(ref mCurPvpParam.playerTeam);
            //curBattleScene.TriggerEvent("pvp_init_pos", Time.time, null);
            SetCameraDefault("pvp_init_pos");
            //SetCameraDefault();
        }
        else if (curInstanceParam != null)
        {
            SetCameraDefault(null);
            battleGroup.SetPlayerList(ref curInstanceParam.playerTeam);
        }
        else if (mCurGuideLevelParam != null)
        {
            SetCameraDefault(null);
            battleGroup.SetGuidePlayerList(ref mCurGuideLevelParam.selfIdList);
        }

        uiBattle = UIMgr.Instance.OpenUI_(UIBattle.ViewName) as UIBattle;
        uiBattle.Initialize();
        uiBattle.ShowUI(false);
        mUILevelInfo = UIMgr.Instance.OpenUI_(UILevelInfo.ViewName) as UILevelInfo;
        if (entryData != null)
        {
            mUILevelInfo.SetInstanceName(entryData.NameAttr);
        }
        else if (mCurPvpParam != null)
        {
            mUILevelInfo.SetInstanceName(StaticDataMgr.Instance.GetTextByID("pvp_level_name"));
        }
        else if (mCurGuideLevelParam != null)
        {
            mUILevelInfo.SetInstanceName(StaticDataMgr.Instance.GetTextByID("guide_level_name"));
        }
        //UIIm.Instance.transform.SetAsLastSibling();
        StartProcess(curProcessIndex);
    }
    //---------------------------------------------------------------------------------------------
    public void SetCameraDefault(string nodeName)
    {
        BattleCamera bCamera = BattleCamera.Instance;
        Transform defaultTrans = GetDefaultCameraNode(nodeName);
        bCamera.transform.localPosition = defaultTrans.position;
        bCamera.transform.localRotation = defaultTrans.rotation;
        bCamera.transform.localScale = Vector3.Scale(transform.localScale, defaultTrans.localScale);
    }
    //---------------------------------------------------------------------------------------------
    public Transform GetDefaultCameraNode(string node = null)
    {
        string cameraSlotName;
        if (string.IsNullOrEmpty(node) == false)
        {
            cameraSlotName = node;
        }
        else
        {
            cameraSlotName = "cameraNormal";
            if (battleType == BattleType.Boss)
            {
                cameraSlotName = "cameraBoss";
            }
        }

		Transform defaultTrans = null;
		if (cameraNodeDic.TryGetValue (cameraSlotName, out defaultTrans))
		{
			return defaultTrans;
		}

        GameObject posParent = GetPositionRoot();
        GameObject cameraNode = Util.FindChildByName(posParent, cameraSlotName);
        if (cameraNode == null)
        {
            cameraNode = Util.FindChildByName(posParent, "cameraNormal");
        }
        else
        {
            cameraNodeDic.Add(cameraSlotName, cameraNode.transform);
        }
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
    public void UnLoadBattleScene(ExitInstanceType state)
    {
        UILoading uiloading = UIMgr.Instance.GetUI(UILoading.ViewName) as UILoading;
        if (uiloading == null)
        {
            uiloading = UIMgr.Instance.OpenUI_(UILoading.ViewName) as UILoading;
        }
        if (uiloading.gameObject.activeSelf == false)
        {
            uiloading.SetLoading(LoadingType.loadingDefault);
        }
        StartCoroutine(UnLoadBattleSceneInternal(state));
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator UnLoadBattleSceneInternal(ExitInstanceType state)
    {
        yield return new WaitForEndOfFrame();
        //state 0 confirm back to instance choose
        //state 1 next back to instance info
        //state 2 retry back to team choose
        battleGroup.DestroyEnemys();
        battleGroup.DestroyPlayers();
        curBattleScene.ClearEvent();
        ObjectDataMgr.Instance.RemoveBattleObject(BattleConst.battleSceneGuid);
        battleGroup = null;
        curBattleScene = null;

        process.Clear();
        if (mCurGuideLevelParam == null)
        {
            GameMain.Instance.ChangeModule<BuildModule>((int)state);
        }
        else
        {
            GameMain.Instance.ChangeModule<CreatePlayerModule>();
        }
        UIMgr.Instance.DestroyUI(mUIScore);
        UIMgr.Instance.DestroyUI(mUILevelInfo);
        UIMgr.Instance.DestroyUI(uiBattle);
        //ResourceMgr.Instance.LoadLevelAsyn("mainstage", false, null);
        //curInstanceParam = null;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetAoeNode(UnitCamp curCamp, UnitCamp targetCamp)
    {
        if (curBattleScene == null)
        {
            Logger.LogError("battle scene is null");
            return GameMain.Instance.gameObject;
        }
        GameObject posParent = GetPositionRoot();

        string aoeNodeName = null;
        if (curCamp == UnitCamp.Player)
        {
            if (targetCamp == UnitCamp.Player)
            {
                aoeNodeName = "aoe_player";
            }
            else
            {
                aoeNodeName = "aoe_enemy";
            }
        }
        else
        {
            if (targetCamp == UnitCamp.Player)
            {
                aoeNodeName = "aoe_enemy";
            }
            else
            {
                aoeNodeName = "aoe_player";
            }
        }

        GameObject aoeNode = Util.FindChildByName(posParent, aoeNodeName);
        if (aoeNode != null)
        {
            return aoeNode;
        }

        return GameMain.Instance.gameObject;
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
        if (mCurPvpParam == null)
        {
            if (isBoss)
            {
                nodeName = "bosspos";
            }
            else
            {
                if (camp == UnitCamp.Enemy)
                {
                    slotID = slotID + mCurMaxSlotIndex + 1;
                }

                nodeName = nodeName + slotID.ToString();
            }
        }
        else
        {
            nodeName = GetSlotNodePvp(camp, slotID);
        }

        GameObject slotNode = Util.FindChildByName(posParent, nodeName);
        if (slotNode != null)
        {
            return slotNode;
        }

        return GameMain.Instance.gameObject;
    }
    //---------------------------------------------------------------------------------------------
    private string GetSlotNodePvp(UnitCamp camp, int slotID)
    {
        int count = 0;
        if (camp == UnitCamp.Player)
        {
            count = mCurPvpParam.playerTeam.Count;
        }
        else
        {
            count = mCurPvpParam.targetData.defenceData.monsterInfo.Count;
            slotID = slotID + mCurMaxSlotIndex + 1;
        }
        
        if (count == 2 || count == 4)
        {
            slotID = slotID + 1;
        }

        string nodeName = string.Format("pos{0}", slotID);
        return nodeName;
    }
    //---------------------------------------------------------------------------------------------
    //TODO: only need get once every battle level
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
    //void ShowUI()
    //{
    //    GameEventMgr.Instance.FireEvent(GameEventList.ShowBattleUI);
    //}
    public void ShowLevelInfo(bool isVisible)
    {
        if (isVisible == true)
        {
            uiBattle.ShowUI(false);
            if (mCurGuideLevelParam == null)
            {
                UIIm.Instance.SetLevelVisible(false);
            }
            mUILevelInfo.SetBattleLevelProcess(curProcessIndex + 1, maxProcessIndex);
        }
        else
        {
            uiBattle.ShowUI(true);
            if (mCurGuideLevelParam == null)
            {
                UIIm.Instance.SetLevelVisible(true);
            }
            mUILevelInfo.SetVisible(false);
        }
    }
    //---------------------------------------------------------------------------------------------
    void StartProcess(int index)
    {
        if (index < maxProcessIndex)
        {
            GameObject slotNode = BattleController.Instance.GetSlotNode(UnitCamp.Player, 0, false);
            floorHeight = slotNode.transform.position.y;
            process.ClearRewardItem();

            //pvp
            if (mCurPvpParam != null)
            {
                curBattleLevel = StaticDataMgr.Instance.GetBattleLevelData(instanceData.battleLevelList[0]);
                List<PbUnit> pbList = new List<PbUnit>();
                List<PB.HSMonster> pvpEnemyList = mCurPvpParam.targetData.defenceData.monsterInfo;
                int count = pvpEnemyList.Count;
                for (int i = 0; i < count; ++i)
                {
                    PbUnit curPbUnit = Util.CreatePbUnitFromHsMonster(pvpEnemyList[i], UnitCamp.Enemy);
                    curPbUnit.slot = i;
                    pbList.Add(curPbUnit);
                }
                battleGroup.SetEnemyList(pbList, true);
            }
            //pve
            else if (curInstanceParam != null)
            {
                PB.HSBattle curBattle = curInstanceParam.instanceData.battle[index];
                curBattleLevel = StaticDataMgr.Instance.GetBattleLevelData(curBattle.battleCfgId);
                if (curBattleLevel.battleProtoData.id.Contains("Boss"))
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
                    pbUnit.lazy = BattleConst.defaultLazy;

                    pbList.Add(pbUnit);

                    if (i < dropCount)
                    {
                        process.AddRewardItem(pbUnit.guid, curBattle.monsterDrop[i]);
                    }
                }
                battleGroup.SetEnemyList(pbList);
            }
            //guide level
            else if (mCurGuideLevelParam != null)
            {
                curBattleLevel = StaticDataMgr.Instance.GetBattleLevelData(instanceData.battleLevelList[0]);
                ObjectDataMgr objMgr = ObjectDataMgr.Instance;
                List<PbUnit> pbList = new List<PbUnit>();
                int count = mCurGuideLevelParam.enemyIdList.Count;
                int enemyStartID = -100;
                for (int i = 0; i < count; ++i)
                {
                    PbUnit pbUnit = new PbUnit();
                    pbUnit.guid = --enemyStartID;
                    pbUnit.id = mCurGuideLevelParam.enemyIdList[i];
                    pbUnit.level = 40;
                    pbUnit.camp = UnitCamp.Enemy;
                    pbUnit.slot = i;
                    pbUnit.lazy = BattleConst.defaultLazy;
                    pbList.Add(pbUnit);
                }
                battleGroup.SetEnemyList(pbList);
            }

            System.Action<float> preStartEvent = (delayTime) =>
            {
                process.StartProcess(index, curBattleLevel);
            };
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
            OnBattleOver(0);
        }
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator StartNextProcess(float delayTime)
    {
        if (delayTime > 0.0f)
        {
            yield return new WaitForSeconds(delayTime);
        }

        List<BattleObject> playerUnitList = battleGroup.GetAllUnitList((int)UnitCamp.Player);
        int count = playerUnitList.Count;
        for (int i = 0; i < count; ++i)
        {
            playerUnitList[i].unit.OnStartNextProcess();
        }

        MagicDazhaoController.Instance.ClearAll ();
		PhyDazhaoController.Instance.ClearAll ();
        //no use now, only the last process has mirror
		//GameEventMgr.Instance.FireEvent<bool,bool> (GameEventList.SetMirrorModeState, false,false);
		process.HideFireFocus ();

        //uiBattle.gameObject.BroadcastMessage("OnAnimationFinish");
        uiBattle.ShowUI(false);
        UIIm.Instance.SetLevelVisible(false);
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
        IsOcclusion(false);
        yield return new WaitForSeconds(waitTime);        
        battleGroup.RefreshPlayerPos();
        //uiBattle.ShowUI(true);
        //uiBattle.gameObject.BroadcastMessage("OnAnimationFinish");
        StartProcess(curProcessIndex);
        SetCameraDefault(null);
        Appearance(true, waitTime);
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
                battleGroup.PlayerFieldList[i].transform.localPosition = newPos;
            }
            else//离场
            {
                newPos = vec + playerNode.position;
                moveTag = BattleConst.distance * vec + playerNode.transform.position;
            }
            battleGroup.PlayerFieldList[i].transform.DOMove(moveTag, BattleConst.moveTime);
            //tw.SetEase(Ease.Linear);
            battleObj.TriggerEvent(BattleConst.unitExitandenter, Time.time, null);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void PlayEntranceAnim()
    {
        float curTime = Time.time;
        for (int i = 0; i < battleGroup.EnemyFieldList.Count; ++i)
        {
            BattleObject bo = battleGroup.EnemyFieldList[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
            {
                //TODO: use level time
                battleGroup.EnemyFieldList[i].TriggerEvent("chuchang", curTime, null);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnBattleOver(byte battleResult)
    {
        mirrorState = MirrorState.CannotUse;
        mBattleResult = battleResult;
        ItemDropManager.Instance.ClearDropItem();
        processStart = false;
        //Logger.LogWarning("Battle " + (isSuccess ? "Success" : "Failed"));
        AudioSystemMgr.Instance.StopMusic();
        string selfEvent = null;
        string enemyEvent = null;
        if (mBattleResult == 0)
        {
            selfEvent = "win";
            enemyEvent = "failed";
        }
        else if (mBattleResult == 2)
        {
            selfEvent = "failed";
            enemyEvent = "win";
        }

        float curTime = Time.time;
        if (string.IsNullOrEmpty(selfEvent) == false)
        {
            for (int i = 0; i < battleGroup.PlayerFieldList.Count; ++i)
            {
                BattleObject bo = battleGroup.PlayerFieldList[i];
                if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
                {
                    battleGroup.PlayerFieldList[i].TriggerEvent(selfEvent, curTime, null);
                }
            }
            for (int i = 0; i < battleGroup.EnemyFieldList.Count; ++i)
            {
                BattleObject bo = battleGroup.EnemyFieldList[i];
                if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
                {
                    battleGroup.EnemyFieldList[i].TriggerEvent(enemyEvent, curTime, null);
                }
            }
        }
        MagicDazhaoController.Instance.ClearAll();
        PhyDazhaoController.Instance.ClearAll();
        process.HideFireFocus();

        if (mCurPvpParam != null)
        {
            PB.HSPVPSettle pvpInstanceParam = new PB.HSPVPSettle();
            pvpInstanceParam.result = mBattleResult;

            GameApp.Instance.netManager.SendMessage(PB.code.PVP_SETTLE_C.GetHashCode(), pvpInstanceParam, false);
        }
        else if (curInstanceParam != null)
        {
            PB.HSInstanceSettle instanceParam = new PB.HSInstanceSettle();
            instanceParam.deadMonsterCount = 0;
            if (mRevived == false)
            {
                List<BattleObject> boList = battleGroup.GetAllUnitList((int)UnitCamp.Player);
                for (int i = 0; i < boList.Count; ++i)
                {
                    BattleObject bo = boList[i];
                    if (bo != null)
                    {
                        if (bo.unit.curLife <= 0 || bo.unit.State == UnitState.Dead)
                        {
                            instanceParam.deadMonsterCount++;
                        }
                    }
                }
            }
            else
            {
                //3 means 1 star
                instanceParam.deadMonsterCount = 3;
            }

            instanceParam.passBattleCount = (mBattleResult == 0) ? maxProcessIndex : curProcessIndex;
            GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_SETTLE_C.GetHashCode(), instanceParam, false);
        }
        else if (mCurGuideLevelParam != null)
        {
            if (mBattleResult == 0)
            {
                UnLoadBattleScene(ExitInstanceType.Num_Exit_Instance_Type);
            }
            else if (mBattleResult == 2)
            {
                MsgBox.PromptMsg.Open(
                    MsgBox.MsgBoxType.Conform,
                    string.Format(StaticDataMgr.Instance.GetTextByID("guid_level_failed")),
                    OnGuideLevelFailed,
                    true,
                    false
                    );
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnGuideLevelFailed(MsgBox.PrompButtonClick btnParam)
    {
        RestartGuideLevel();
    }

    //---------------------------------------------------------------------------------------------
    private void RestartGuideLevel()
    {
        //battleGroup.RestartGuideLevel();
        if (process != null)
            process.RestartCurGuideLevel();
    }
    //---------------------------------------------------------------------------------------------
    void OnInstanceSettleResult(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("instance settle result error");
            UnLoadBattleScene(ExitInstanceType.Exit_Instance_OK);
        }
        else
        {
            GameSpeedService.Instance.SetBattleSpeed(1.0f);
            if (mUIScore == null)
            {
                mUIScore = UIMgr.Instance.OpenUI_(UIScore.ViewName) as UIScore;
            }
            int starCount = 0;
            if(mBattleResult == 0)
            {
                PB.HSInstanceSettleRet scoreInfo = msg.GetProtocolBody<PB.HSInstanceSettleRet>();
                GameEventMgr.Instance.FireEvent<int, string>(GameEventList.FinishedInstance, scoreInfo.starCount, instanceData.instanceProtoData.id);
                starCount = scoreInfo.starCount;
            }
            mUIScore.ShowScoreUI(mBattleResult, starCount, null);
            uiBattle.HideBattleUI();

            GameDataMgr.Instance.OnBattleOver(mBattleResult == 0);
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnPvpSettleResult(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("pvp settle result error");
            UnLoadBattleScene(ExitInstanceType.Exit_Instance_OK);
        }
        else
        {
            GameSpeedService.Instance.SetBattleSpeed(1.0f);
            if (mUIScore == null)
            {
                mUIScore = UIMgr.Instance.OpenUI_(UIScore.ViewName) as UIScore;
            }

            PB.HSPVPSettleRet scoreInfo = msg.GetProtocolBody<PB.HSPVPSettleRet>();
            mUIScore.ShowScoreUI(mBattleResult, 0, scoreInfo);
            uiBattle.HideBattleUI();

            GameDataMgr.Instance.OnBattleOver(mBattleResult == 0);
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnScoreReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        if (reward != null && reward.hsCode == PB.code.INSTANCE_SETTLE_C.GetHashCode())
        {
            if (mUIScore == null)
            {
                mUIScore = UIMgr.Instance.OpenUI_(UIScore.ViewName) as UIScore;
            }
            mUIScore.SetScoreInfo(reward);
        }
    }
    //---------------------------------------------------------------------------------------------
}
