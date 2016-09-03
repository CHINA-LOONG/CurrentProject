using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnterInstanceParam
{
		
	public	List<int> playerTeam;
	//public	string	friendTeam;
	public	PB.HSInstanceEnterRet instanceData;
}

public enum InstanceType
{
    Normal = PB.InstanceType.INSTANCE_STORY,
    Hole = PB.InstanceType.INSTANCE_HOLE,
    Tower = PB.InstanceType.INSTANCE_TOWER,
    Guild = PB.InstanceType.INSTANCE_GUILD
}

public class UIAdjustBattleTeam : UIBase
{
    public enum SaodangState
    {
        OK =0,
        HuoliNotEnough,
        BattleTimesNotEnough,
        SaodangquanNotEnough,
        UnKonw
    }
	public static string ViewName = "UIAdjustBattleTeam";

	public	Button	backButton;
	public	Button	battleButton;
    public Button rapid1Button;
    public Button rapid10Button;
    public Button resetTimesButton;
    public Text battleTimesText;
    public Text customHuoliText;
    public Text dropInfoText;

	public	Text	lbMyZhenrong;
	public  Text	lbEnemyZhenrong;
	public	Text	lbMyShangzhen;
	public	Text	lbMyHoubei;

    public SpellIcon buffIcon;
    public Text nameText;
    public Transform[] szStar;
    public Transform[] szGrewStar;
    public SkilTips skillTips;

    public float monsterIconScale = 1.0f;
    public float bossMonsterIconScale = 1.16f;
    public List<MonsterIconBg> playerTeamBg = new List<MonsterIconBg>();
    private List<MonsterIcon> playerIcons = new List<MonsterIcon>();

    public List<MonsterIconBg> enemyTeamBg = new List<MonsterIconBg>();
    private List<MonsterIcon> enemyIcons = new List<MonsterIcon>();

    public List<Transform> dropList = new List<Transform>();
    private List<GameObject> dropObjectList = new List<GameObject>();

	public ScrollView scrollView;


	private string instanceId  = null;
    private InstanceEntry instanceEntryData;
	private	List<string>	enemyList =null;
	private int enemyLevel = 1;
    private int star = 0;
    private InstanceType instanceType;
    private int towerFloor;
    private int teamMax = 1;
    private int resetInstanceCost = 0;

	private	List<string> teamList;
	private int prepareIndex = -1;//准备上阵的空位索引
    private bool isOpenSaodangOneTimes = false;
    private bool isOpenSaodangTenTimes = false;
    private bool isFirst = true;
    private List<GameUnit> mPetList = new List<GameUnit>();

    private Dictionary<string,MonsterIcon> playerAllIconDic = new Dictionary<string, MonsterIcon>();

	EnterInstanceParam enterInstanceParam  = new EnterInstanceParam();
    
    public  static void OpenWith(string instanceId,int star, InstanceType insType = InstanceType.Normal, int towerFloor = 0)
    {
        UIAdjustBattleTeam uiadust = (UIAdjustBattleTeam)UIMgr.Instance.OpenUI_(ViewName);
        uiadust.FirstInit();
        uiadust.towerFloor = towerFloor;
        uiadust.SetData(instanceId, star, insType);

        UIBuild uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
        if (uiBuild != null)
        {
            uiBuild.uiAdjustBattleTeam = uiadust;
        }
    }

   public void FirstInit()
    {
        if (!isFirst)
            return;
        isFirst = true;
        EventTriggerListener.Get(backButton.gameObject).onClick = OnBackButtonClick;
        EventTriggerListener.Get(battleButton.gameObject).onClick = OnBattleButtonClick;
        EventTriggerListener.Get(resetTimesButton.gameObject).onClick = OnResetInstanceTimesClick;
        EventTriggerListener.Get(rapid1Button.gameObject).onClick = OnRapid1ButtonClick;
        EventTriggerListener.Get(rapid10Button.gameObject).onClick = OnRapid10ButtonClick;
        EventTriggerListener.Get(buffIcon.iconButton.gameObject).onEnter = OnPointerEnterBuffIcon;
        EventTriggerListener.Get(buffIcon.iconButton.gameObject).onExit = OnPointerExitBuffIcon;

        lbEnemyZhenrong.text = StaticDataMgr.Instance.GetTextByID("instance_difangzhenrong");
        lbMyHoubei.text = StaticDataMgr.Instance.GetTextByID("instance_houbei");
        lbMyShangzhen.text = StaticDataMgr.Instance.GetTextByID("instance_shangzhen");
        lbMyZhenrong.text = StaticDataMgr.Instance.GetTextByID("instance_wofangzhenrong");
        dropInfoText.text = StaticDataMgr.Instance.GetTextByID("instance_jiangliList");

        battleButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("instance_kaishizhandou");
        UIUtil.SetButtonTitle(resetTimesButton.transform, StaticDataMgr.Instance.GetTextByID("arrayselect_chongzhi_anniu"));
        UIUtil.SetButtonTitle(rapid1Button.transform, StaticDataMgr.Instance.GetTextByID("instance_saodang"));
        skillTips.gameObject.SetActive(false);
    }
    public override void Clean()
    {
        //TODO: destroy MonsterIcon
        CleanAllPlayersIcons();
    }

    void OnEnable()
    {
        BindListener();
        isBattleClick = false;
    }

    void OnDisable()
    {
        UnBindListener();
        isBattleClick = false;
    }

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.INSTANCE_ENTER_S.GetHashCode ().ToString(), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.HOLE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.TOWER_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        //公会战
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.GUILD_INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        //GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_S.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        //GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_SWEEP_C.GetHashCode().ToString(), OnSaodangFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_SWEEP_S.GetHashCode().ToString(), OnSaodangFinished);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_RESET_COUNT_C.GetHashCode().ToString(), RequestResetInstanceFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_RESET_COUNT_S.GetHashCode().ToString(), RequestResetInstanceFinished);

        GameEventMgr.Instance.AddListener(GameEventList.RefreshSaodangTimes, RefreshSaodangTimes);
    }
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.INSTANCE_ENTER_S.GetHashCode ().ToString (), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.HOLE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.TOWER_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
        //公会战
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.GUILD_INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_SWEEP_C.GetHashCode().ToString(), OnSaodangFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_SWEEP_S.GetHashCode().ToString(), OnSaodangFinished);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_RESET_COUNT_C.GetHashCode().ToString(), RequestResetInstanceFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_RESET_COUNT_S.GetHashCode().ToString(), RequestResetInstanceFinished);

        GameEventMgr.Instance.RemoveListener(GameEventList.RefreshSaodangTimes, RefreshSaodangTimes);
    }

	public void SetData(string instanceId,int star, InstanceType insType = InstanceType.Normal)
    {
        GameDataMgr.Instance.curInstanceType = (int)insType;
        this.instanceId = instanceId;
        instanceEntryData = StaticDataMgr.Instance.GetInstanceEntry(instanceId);
        this.enemyList = instanceEntryData.enemyList;
        InstanceData instanceData = StaticDataMgr.Instance.GetInstanceData(instanceId);
        this.enemyLevel = instanceData.instanceProtoData.level;
        this.star = star;
        instanceType = insType;
        teamMax = 3;
        if(IsFirstBackupOpen())
        {
            playerTeamBg[3].SetAsLocked(false);
            teamMax++;
        }
        else
        {
            playerTeamBg[3].SetAsLocked(true,FirstBackupClicked);
        }

        if(IsSecondBackupOpen())
        {
            playerTeamBg[4].SetAsLocked(false);
            teamMax++;
        }
        else
        {
            playerTeamBg[4].SetAsLocked(true,SecondBackupClicked);
        }

        isOpenSaodangOneTimes = false;
        isOpenSaodangTenTimes = false;
        if(star >  2)
        {
            if(GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.saodangOpenLevelForOneTimes)
            {
                isOpenSaodangOneTimes = true;
            }
            if(GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.saodangOpenLevelForTenTimes)
            {
                isOpenSaodangTenTimes = true;
            }
        }
        RefreshUICo();
    }

    void    FirstBackupClicked()
    {
        string msg = string.Format(StaticDataMgr.Instance.GetTextByID("arrayselect_count_004"),
            GameConfig.Instance.FirstBackupOpenNeedLevel);
        UIIm.Instance.ShowSystemHints(msg, (int)PB.ImType.PROMPT);
    }

    void SecondBackupClicked()
    {
        string msg = string.Format(StaticDataMgr.Instance.GetTextByID("arrayselect_count_004"),
            GameConfig.Instance.SecondBackupOpenNeedLevel);
        UIIm.Instance.ShowSystemHints(msg, (int)PB.ImType.PROMPT);
    }

    bool IsFirstBackupOpen()
    {
        return GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.FirstBackupOpenNeedLevel;
    }

    bool IsSecondBackupOpen()
    {
        return GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.SecondBackupOpenNeedLevel;
    }

    void RefreshUICo()
	{
        ClearAllCo();
        RefreshEnmeyIcons();
        RefreshPlayerIcons();
        RefreshDropInfo();
        RefreshTitleAndOthers();
    }

    void ClearAllCo()
    {
        CleanAllEnemyIcons();//清空敌方怪物列表
        CleanAllPlayersIcons();
        ClearAllDropObject();
        nameText.text = "";
        buffIcon.gameObject.SetActive(false);
        rapid1Button.gameObject.SetActive(false);
        rapid10Button.gameObject.SetActive(false);
        resetTimesButton.gameObject.SetActive(false);
    }


	void RefreshEnmeyIcons()
	{
        CleanAllEnemyIcons();//清空敌方怪物列表
        int enmeyCount = enemyList.Count;
		string monsterId = null;
		MonsterIconBg subBg = null;
		RectTransform rectTrans = null;
    
		for (int i = 0; i<enemyTeamBg.Count; ++ i)
		{
			subBg = enemyTeamBg[i];
			if(i < enmeyCount)
			{
				monsterId  = enemyList[i];

				MonsterIcon subIcon = MonsterIcon.CreateIcon();
                enemyIcons.Add(subIcon);        //加入列表用于维护

				subIcon.transform.SetParent(subBg.transform,false);
				
				rectTrans = subIcon.transform as RectTransform;
				rectTrans.anchoredPosition = new Vector2(0,0);
				rectTrans.localScale  = new Vector3(monsterIconScale, monsterIconScale, monsterIconScale);

				subIcon.SetMonsterStaticId(monsterId);
                subIcon.SetStage(instanceEntryData.GetEnemyState(monsterId));
				subIcon.SetLevel(enemyLevel);

				UnitData unitRow = StaticDataMgr.Instance.GetUnitRowData(monsterId);
				if(unitRow != null)
				{
					if(unitRow.assetID.Contains("boss"))
					{
						//subIcon.ShowBossItem(true);
						RectTransform rt = subBg.transform as RectTransform;
						Vector2 oldPivot = rt.pivot;
						Vector2 newPivot = new Vector2(0.5f,0.5f);
						Vector2 newpos = new Vector2(0,0);
						newpos.x = rt.anchoredPosition.x - (oldPivot.x - newPivot.x)*rt.sizeDelta.x;
						newpos.y = rt.anchoredPosition.y -(oldPivot.y -newPivot.y)*rt.sizeDelta.y;

						rt.pivot = newPivot;
						rt.anchoredPosition = newpos;
						float scale = GameConfig.Instance.BossEnemyIconScale;
						subBg.transform.localScale = new Vector3(bossMonsterIconScale, bossMonsterIconScale, bossMonsterIconScale);
					}
				}

				ScrollViewEventListener.Get(subIcon.iconButton.gameObject).onPressEnter = OnEnmeyIconClick;
				subBg.gameObject.SetActive(true);
			}
			else
			{
				subBg.gameObject.SetActive(false);
			}
		}
	}
    void CleanAllEnemyIcons()
    {
        for (int i = 0; i < enemyIcons.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(enemyIcons[i].gameObject);
        }
        enemyIcons.Clear();
    }

	void RefreshPlayerIcons()
	{
		InitPlayerTeamIcons ();
		InitAllPlayerPetsIcons ();
	}

	void InitPlayerTeamIcons()
	{
		prepareIndex = -1;
		teamList =  BattleTeamManager.GetTeamWithKey (BattleTeamManager.TeamList.Defualt);
        CleanAllPlayersIcons();

        MonsterIconBg subBg = null;
		RectTransform rectTrans = null;
		for (int i = 0; i<teamMax; ++ i)
		{
			subBg = playerTeamBg[i];
			subBg.SetEffectShow(false);
			
			string guid = teamList[i];
			GameUnit unit = null;
			
			if(!string.IsNullOrEmpty(guid))
			{
				unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(int.Parse(guid));
			}
			if(unit == null)
			{
				if(prepareIndex ==-1)
				{
					prepareIndex = i;
					subBg.SetEffectShow(true);
				}
			}
			else
			{
				MonsterIcon subIcon = MonsterIcon.CreateIcon();
                playerIcons.Add(subIcon);//添加列表用于维护

				subIcon.transform.SetParent(subBg.transform,false);
				
				rectTrans = subIcon.transform as RectTransform;
				rectTrans.anchoredPosition = new Vector2(0,0);
                rectTrans.localScale = new Vector3(monsterIconScale, monsterIconScale, monsterIconScale);
				
				subIcon.SetId(guid);
				subIcon.SetMonsterStaticId(unit.pbUnit.id);
				subIcon.SetLevel(unit.pbUnit.level);
                subIcon.SetStage(unit.pbUnit.stage);
				
				EventTriggerListener.Get(subIcon.iconButton.gameObject).onClick = OnPlayerTeamIconClick;
			}
		}
	}
    void CleanAllPlayersIcons()
    {
        for (int i = 0; i < playerIcons.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(playerIcons[i].gameObject);
        }
        playerIcons.Clear();
    }

	void InitAllPlayerPetsIcons()
	{
        //清理所有宠物Icon
        CleanAllPlayerPetsIcons();

		GameDataMgr.Instance.PlayerDataAttr.GetAllPet (ref mPetList);
        mPetList.Sort();
		GameUnit subUnit = null;
		for (int i =0; i< mPetList.Count; ++i) 
		{
            subUnit = mPetList[i];
			string monsterId = subUnit.pbUnit.id;

			MonsterIcon icon = MonsterIcon.CreateIcon();
			//EventTriggerListener.Get(icon.iconButton.gameObject).onClick = OnPlayerWarehouseIconClick;
			ScrollViewEventListener.Get(icon.iconButton.gameObject).onClick = OnPlayerWarehouseIconClick;
			ScrollViewEventListener.Get(icon.iconButton.gameObject).onPressEnter = OnPlayerWarehouseIconPressEnter;
			scrollView.AddElement(icon.gameObject);
			icon.SetMonsterStaticId(monsterId);
			icon.SetId(subUnit.pbUnit.guid.ToString());
			icon.SetLevel(subUnit.pbUnit.level);
			icon.SetStage(subUnit.pbUnit.stage);

			playerAllIconDic.Add(icon.Id,icon);

			if(teamList.Contains(subUnit.pbUnit.guid.ToString()))
			{
				icon.ShowSelectImage(true);
				icon.ShowMaskImage();
			}
		}
	}
    void CleanAllPlayerPetsIcons()
    {
        foreach (var item in playerAllIconDic)
        {
            ResourceMgr.Instance.DestroyAsset(item.Value.gameObject);
        }
        playerAllIconDic.Clear();
    }

    void RefreshDropInfo()
    {
        ClearAllDropObject();
        string rewardId = instanceEntryData.reward;

        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardId);
        if (rewardData == null || rewardData.itemList == null)
            return;

        for(int i =0; i< rewardData.itemList.Count;++i)
        {
            if (i > 5)
                break;

            var subRewardData = rewardData.itemList[i];
            GameObject go = RewardItemCreator.CreateRewardItem(subRewardData.protocolData, dropList[i],true,true);
            if(null != go)
            {
                dropObjectList.Add(go);
            }
        }
    }

    void ClearAllDropObject()
    {
        for(int i =0;i<dropObjectList.Count;++i)
        {
            ResourceMgr.Instance.DestroyAsset(dropObjectList[i]);
        }
        dropObjectList.Clear();
    }

   void RefreshTitleAndOthers()
    {
        //buffImage
        string spellId = instanceEntryData.instanceSpell;
        if(string.IsNullOrEmpty(spellId))
        {
            buffIcon.gameObject.SetActive(false);
        }
        else
        {
            buffIcon.gameObject.SetActive(true);
            buffIcon.SetData(1, spellId);

            skillTips.SetSpellId(spellId, 1);
            skillTips.gameObject.SetActive(false);
        }
        

        //instance name
        nameText.text = instanceEntryData.NameAttr;

        szStar[0].gameObject.SetActive(star > 0);
        szStar[1].gameObject.SetActive(star > 1);
        szStar[2].gameObject.SetActive(star > 2);
        szGrewStar[0].gameObject.SetActive(false);
        szGrewStar[1].gameObject.SetActive(star == 1);
        szGrewStar[2].gameObject.SetActive(star > 0  && star < 3);

        bool isNormalInstance = instanceType == InstanceType.Normal;
        rapid10Button.gameObject.SetActive(isNormalInstance && isOpenSaodangTenTimes);
        rapid1Button.gameObject.SetActive(isNormalInstance && isOpenSaodangOneTimes);
        battleTimesText.gameObject.SetActive(isNormalInstance);
        resetTimesButton.gameObject.SetActive(false);
        if (isNormalInstance)
        {
            InstanceEntryRuntimeData realData = InstanceMapService.Instance.GetRuntimeInstance(instanceId);
            if(realData.countDaily < realData.staticData.count)
            {
                battleTimesText.text = string.Format(StaticDataMgr.Instance.GetTextByID("instance_tiaozhancishu"), realData.staticData.count - realData.countDaily, realData.staticData.count);
            }
            else
            {
                battleTimesText.text = "";
                resetTimesButton.gameObject.SetActive(true);
            }
            RefreshSaodangTimes();
        }
        RefreshBattleButton();
    }

    void RefreshBattleButton()
    {
        InstanceEntry stData = StaticDataMgr.Instance.GetInstanceEntry(instanceId);

        customHuoliText.text = stData.fatigue.ToString();
        if(stData.fatigue > GameDataMgr.Instance.PlayerDataAttr.HuoliAttr)
        {
            customHuoliText.color = new Color(1, 0, 0);
        }
        else
        {
            customHuoliText.color = new Color(96.0f / 255.0f,76.0f / 255.0f, 51.0f / 255.0f);
        }
    }
    //---------------------------------------------------------------------------------------------------------------------

    void	OnPlayerTeamIconClick(GameObject go)
	{
		MonsterIcon subIcon = go.GetComponentInParent<MonsterIcon> ();

		string guid = subIcon.Id;
		BattleTeamToPlayerWarehouse (guid);
	}

	void	OnPlayerWarehouseIconClick(GameObject go)
	{

		MonsterIcon micon = go.GetComponentInParent<MonsterIcon> ();
		bool isSel = micon.IsSelected ();

		if (isSel) 
		{
			BattleTeamToPlayerWarehouse (micon.Id);
		}
		else
		{
			if (prepareIndex == -1) 
			{
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("arrayselect_count_003"), (int)PB.ImType.PROMPT);
				return;
			}
			PlayerWarehouseToBattleTeam(micon.Id);
		}
		micon.ShowSelectImage (!isSel);
		micon.ShowMaskImage (!isSel);
	}

	void OnPlayerWarehouseIconPressEnter(GameObject go)
	{
		MonsterIcon micon = go.GetComponentInParent<MonsterIcon> ();

		int guid = int.Parse(micon.Id);
		GameUnit unit = null;

		GameDataMgr.Instance.PlayerDataAttr.allUnitDic.TryGetValue (guid, out unit);
        UIMonsterInfo.Open(guid, micon.monsterId, unit.pbUnit.level, unit.pbUnit.stage);
	}

	void OnEnmeyIconClick(GameObject go)
	{
		MonsterIcon mIcon = go.GetComponentInParent<MonsterIcon> ();

		string monsterId = mIcon.monsterId;

        UIMonsterInfo.Open(-1, monsterId, enemyLevel, instanceEntryData.GetEnemyState(monsterId));
	}

	#region 上阵 --下阵
	//---------------------------------------------------------------------------------------------------------------------
	void PlayerWarehouseToBattleTeam(string guid)
	{
		var iconBg = playerTeamBg[prepareIndex];
		iconBg.SetEffectShow (false);

		MonsterIcon subIcon = iconBg.GetComponentInChildren<MonsterIcon> ();
		if (null == subIcon) 
		{
			subIcon = MonsterIcon.CreateIcon();
            playerIcons.Add(subIcon);
			subIcon.transform.SetParent(iconBg.transform,false);
			
			RectTransform rectTrans = subIcon.transform as RectTransform;
			rectTrans.anchoredPosition = new Vector2(0,0);
			rectTrans.localScale  = new Vector3(monsterIconScale, monsterIconScale, monsterIconScale);
			EventTriggerListener.Get(subIcon.iconButton.gameObject).onClick = OnPlayerTeamIconClick;
		}
		subIcon.gameObject.SetActive (true);
		subIcon.SetId (guid);

		GameUnit unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey (int.Parse (guid));
		subIcon.SetMonsterStaticId (unit.pbUnit.id);
		subIcon.SetLevel (unit.pbUnit.level);
        subIcon.SetStage(unit.pbUnit.stage);

		//新的prepareindex
		UpdatePrepareIndex ();
	}

	//---------------------------------------------------------------------------------------------------------------------
	void BattleTeamToPlayerWarehouse(string guid)
	{
		MonsterIcon playerIcon = playerAllIconDic [guid];
		playerIcon.ShowSelectImage (false);
		playerIcon.ShowMaskImage (false);

		MonsterIconBg subBg = null;
		for (int i =0; i<teamMax; ++i) 
		{
			subBg = playerTeamBg[i];
			MonsterIcon subIcon = subBg.GetComponentInChildren<MonsterIcon>();
			if(null!= subIcon && subIcon.gameObject.activeSelf)
			{
				if(guid == subIcon.Id)
				{
                    playerIcons.Remove(subIcon);
                    ResourceMgr.Instance.DestroyAsset(subIcon.gameObject);
					break;
				}
			}
		}
        StartCoroutine(_UpdatePrepareIndex());
	}
    IEnumerator _UpdatePrepareIndex()
    {
        yield return new WaitForEndOfFrame();
        MonsterIconBg subBg = null;
        if (prepareIndex != -1)
        {
            subBg = playerTeamBg[prepareIndex];
            subBg.SetEffectShow(false);
        }
        UpdatePrepareIndex();

        subBg = playerTeamBg[prepareIndex];
        subBg.SetEffectShow(true);
    }

	//---------------------------------------------------------------------------------------------------------------------
	void UpdatePrepareIndex()
	{
		prepareIndex = -1;
		MonsterIconBg subIconBg = null;
		for (int i =0 ;i < teamMax; ++i)
		{
			subIconBg = playerTeamBg[i];
			MonsterIcon tempIcon = subIconBg.GetComponentInChildren<MonsterIcon>();
			if(null== tempIcon  || !tempIcon.gameObject.activeSelf)
			{
				prepareIndex = i ;
				subIconBg.SetEffectShow(true);
				break;
			}
		}
	}
    #endregion
    //---------------------------------------------------------------------------------------------------------------------
    #region 场地bufftips
    void OnPointerEnterBuffIcon(GameObject go)
    {
        buffIcon.SetMask(true);
        skillTips.gameObject.SetActive(true);
    }

    void OnPointerExitBuffIcon(GameObject go)
    {
        buffIcon.SetMask(false);
        skillTips.gameObject.SetActive(false);
    }
    #endregion

    #region 重置副本
    void OnResetInstanceTimesClick(GameObject go)
    {
        if (isBattleClick) return;
        ResetInstance();
    }

    void ResetInstance()
    {
        string msg = StaticDataMgr.Instance.GetTextByID("arrayselect_chongzhi");
        int times = InstanceMapService.Instance.instanceResetTimes;
        InstanceReset insReset = StaticDataMgr.Instance.GetInstanceReset("1");
        resetInstanceCost = insReset.GetBaseZuanshiWithTime(times + 1);

        string optionMsg = string.Format(StaticDataMgr.Instance.GetTextByID("arrayselect_chongzhiTimes"), times);

        MsgBox.PrompCostMsg.Open(resetInstanceCost, msg, optionMsg, OnInstanceRestPrompClick);
    }

    void OnInstanceRestPrompClick(MsgBox.PrompButtonClick sel)
    {
        if(sel == MsgBox.PrompButtonClick.OK)
        {
            if(resetInstanceCost > GameDataMgr.Instance.PlayerDataAttr.gold)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            }
            else
            {
                RequestResetInstance();
            }
        }
    }

    void RequestResetInstance()
    {
        PB.HSInstanceResetCount param = new PB.HSInstanceResetCount();
        param.instanceId = instanceId;
        GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_RESET_COUNT_C.GetHashCode(), param);
    }

    void RequestResetInstanceFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
            
            return;
        }
       // PB.HSInstanceResetCountRet retData = message.GetProtocolBody<PB.HSInstanceResetCountRet>();
        InstanceMapService.Instance.instanceResetTimes++;
        InstanceMapService.Instance.ResetCountDaily(instanceId);
        RefreshTitleAndOthers();
    }
#endregion
    #region   扫荡相关
    void OnRapid1ButtonClick(GameObject go)
    {
        if (isBattleClick) return;
        int saodTime = 1;
        SaodangState sstate = GetSaodangState(out saodTime);
        if(sstate == SaodangState.OK)
        {
            RequestSaodang(1);
        }
        else
        {
            CanotSaodang(sstate);
        }
    }

    void OnRapid10ButtonClick(GameObject go)
    {
        if (isBattleClick) return;
        int saodTime = 1;
        SaodangState sstate = GetSaodangState(out saodTime);
        if (sstate == SaodangState.OK)
        {
            RequestSaodang(saodTime);
        }
        else
        {
            CanotSaodang(sstate);
        }
    }

    void CanotSaodang(SaodangState state)
    {
        if(state == SaodangState.HuoliNotEnough)
        {
            UseHuoLi.Open();
        }
        else if(state == SaodangState.BattleTimesNotEnough)
        {
            ResetInstance();
        }
        else if (state == SaodangState.SaodangquanNotEnough)
        {
            BuyItem.BuyItemParam param = new BuyItem.BuyItemParam();
            param.itemId = GameConfig.Instance.saodangQuanId;
            param.defaultbuyCount = 1;
            param.maxCount = GameConfig.Instance.maxBuyItemCount;
            param.isShowCoinButton = true;
            BuyItem.OpenWith(param);
        }
    }

    void RefreshSaodangTimes()
    {
        if (instanceType == InstanceType.Normal && isOpenSaodangTenTimes)
        {
            int saodTimes = 1;
            GetSaodangState(out saodTimes);
            UIUtil.SetButtonTitle(rapid10Button.transform, string.Format(StaticDataMgr.Instance.GetTextByID("instance_saodang10"), saodTimes));
        }
        RefreshBattleButton();
    }

    SaodangState GetSaodangState(out int iTimes)
    {
        InstanceEntryRuntimeData realInstanceData = InstanceMapService.Instance.GetRuntimeInstance(instanceId);
        if(null == realInstanceData)
        {
            iTimes = 1;
            return SaodangState.HuoliNotEnough;
        }
        int saodangMax = realInstanceData.staticData.count;
        if (saodangMax > 10)
        {
            saodangMax = 10;
        }
        int leftTimes = realInstanceData.staticData.count - realInstanceData.countDaily;
        int huoliTimes = GameDataMgr.Instance.PlayerDataAttr.HuoliAttr / realInstanceData.staticData.fatigue;
        int saodangquanCount = 0;
        ItemData itemSaodang = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(GameConfig.Instance.saodangQuanId);
        if(null != itemSaodang)
        {
            saodangquanCount = itemSaodang.count;
        }
        iTimes = saodangMax;
        SaodangState retState;
        if(huoliTimes ==0)
        {
            retState = SaodangState.HuoliNotEnough;
        }
        else if (leftTimes == 0)
        {
            retState = SaodangState.BattleTimesNotEnough;
        }
        else if (saodangquanCount ==0)
        {
            retState = SaodangState.SaodangquanNotEnough;
        }
        else
        {
            retState = SaodangState.OK;
            iTimes = huoliTimes > leftTimes ? leftTimes : huoliTimes;
            if(iTimes > saodangquanCount)
            {
                iTimes = saodangquanCount;
            }
            if(iTimes > saodangMax)
            {
                iTimes = saodangMax;
            }
        }

        return retState ;
    }

    void RequestSaodang(int times)
    {
        PB.HSInstanceSweep param = new PB.HSInstanceSweep();
        param.instanceId = instanceId;
        param.count = times;
        GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_SWEEP_C.GetHashCode(), param);
    }

    void OnSaodangFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
            return;
        }

        PB.HSInstanceSweepRet sweetRet = message.GetProtocolBody<PB.HSInstanceSweepRet>();

        InstanceMapService.Instance.AddCountDaily(instanceId, sweetRet.completeReward.Count);
        RefreshTitleAndOthers();
        List<PB.HSRewardInfo> listReward = new List<PB.HSRewardInfo>();
        listReward.AddRange(sweetRet.completeReward);
        listReward.Add(sweetRet.sweepReward);
        //扫荡结果
        SaodangResult.OpenWith(listReward);
    }
#endregion
    #region      取消战斗
    void OnBackButtonClick(GameObject go)
    {
        if (isBattleClick) return;

        GameEventMgr.Instance.FireEvent<string>(GameEventList.ShowInstanceList, instanceId);
        UIMgr.Instance.CloseUI_(this);
    }
    #endregion

    #region 开始战斗--战斗请求----保存阵容
    bool isBattleClick = false;
    void OnBattleButtonClick(GameObject go)
    {
        if (isBattleClick) return;
        isBattleClick = true;
        List<int> battleTeam = SaveBattleTeam();
        if (null == battleTeam || battleTeam.Count < 1)
        {
           // MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("tip_zhenrongError"));
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("tip_zhenrongError"), (int)PB.ImType.PROMPT);
            isBattleClick = false;
        }
        else if (instanceEntryData.fatigue > GameDataMgr.Instance.PlayerDataAttr.HuoliAttr)
        {
            UseHuoLi.Open();
            isBattleClick = false;
        }
        else
        {
            if(instanceType == InstanceType.Normal)
            {
                InstanceEntryRuntimeData realInstanceData = InstanceMapService.Instance.GetRuntimeInstance(instanceId);
                int leftTimes = realInstanceData.staticData.count - realInstanceData.countDaily;
                if(leftTimes == 0)
                {
                    ResetInstance();
                    isBattleClick = false;
                    return;
                }
            }
            

            enterInstanceParam.playerTeam = battleTeam;
            RequestEnterInstance();
        }
    }

    List<int> SaveBattleTeam()
	{
		List<int> battleTeam = new List<int> ();
		bool isNeedSave = false;
		string guid = null;
		string tempGuid = null;
		MonsterIconBg subIconBg;
		MonsterIcon subIcon;

		bool lastIndexIsNull = false;

		for (int i =0; i < playerTeamBg.Count; ++i) 
		{
			subIconBg = playerTeamBg[i];
			subIcon = subIconBg.GetComponentInChildren<MonsterIcon>();
			if(null == subIcon)
			{
				guid = "";
				lastIndexIsNull = true;
			}
			else
			{
				if(lastIndexIsNull)
				{
					return null;//不允许前面有空位
				}
				guid = subIcon.Id;
				battleTeam.Add(int.Parse(guid));
			}
			
			tempGuid = teamList[i];
			if(tempGuid != guid)
			{
				teamList[i] = guid;
				isNeedSave = true;
			}
		}
		
		if (isNeedSave)
		{
			BattleTeamManager.SetTeam(teamList,BattleTeamManager.TeamList.Defualt);
		}

		return battleTeam;
	}

	void RequestEnterInstance()
    {
        //GameDataMgr.Instance.curInstanceType = (int)instanceType;
        if (instanceType == InstanceType.Normal)
        {
            PB.HSInstanceEnter param = new PB.HSInstanceEnter();
            param.instanceId = instanceId;
            int count = enterInstanceParam.playerTeam.Count;
            for (int i = 0; i < count; ++i)
            {
                param.battleMonsterId.Add(enterInstanceParam.playerTeam[i]);
            }

            GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_ENTER_C.GetHashCode(), param);
        }
        else if (instanceType == InstanceType.Hole)
        {
            PB.HSHoleEnter param = new PB.HSHoleEnter();
            param.instanceId = instanceId;
            int count = enterInstanceParam.playerTeam.Count;
            UIHole hole = UIMgr.Instance.GetUI(UIHole.ViewName) as UIHole;
            param.holeId = hole.GetSelectHoleType();
            GameDataMgr.Instance.curHoleType = (HoleType)param.holeId;
            for (int i = 0; i < count; ++i)
            {
                param.battleMonsterId.Add(enterInstanceParam.playerTeam[i]);
            }

            GameApp.Instance.netManager.SendMessage(PB.code.HOLE_ENTER_C.GetHashCode(), param);
        }
        else if (instanceType ==  InstanceType.Tower)
        {
            PB.HSTowerEnter param = new PB.HSTowerEnter();
            int count = enterInstanceParam.playerTeam.Count;
            for (int i = 0; i < count; ++i)
            {
                param.battleMonsterId.Add(enterInstanceParam.playerTeam[i]);
            }
            param.towerId = (int)GameDataMgr.Instance.curTowerType;
            param.floor = towerFloor;

            GameApp.Instance.netManager.SendMessage(PB.code.TOWER_ENTER_C.GetHashCode(), param);
        }
        else if (instanceType == InstanceType.Guild)
        {
            PB.HSGuildInstanceEnter  param = new PB.HSGuildInstanceEnter();
            param.instanceId = instanceId;
            int count = enterInstanceParam.playerTeam.Count;
            for (int i = 0; i < count; ++i)
            {
                param.battleMonsterId.Add(enterInstanceParam.playerTeam[i]);
            }

            GameApp.Instance.netManager.SendMessage(PB.code.GUILD_INSTANCE_ENTER_C.GetHashCode(), param);
        }
        else
        {
            isBattleClick = false;
        }
	}

	void OnRequestEnterInstanceFinished(ProtocolMessage msg)
	{
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();

            string errorMsg;
            if (error.errCode == (int)PB.instanceError.INSTANCE_NOT_OPEN)
            {
                errorMsg = StaticDataMgr.Instance.GetTextByID("tower_record_004");
                UIIm.Instance.ShowSystemHints(errorMsg, (int)PB.ImType.PROMPT);
            }
            else if (error.errCode == (int)PB.instanceError.TOWER_FLOOR)
            {
                errorMsg = StaticDataMgr.Instance.GetTextByID("towerBoss_record_004");
                UIIm.Instance.ShowSystemHints(errorMsg, (int)PB.ImType.PROMPT);
            }
            else if (error.errCode == (int)PB.instanceError.INSTANCE_FATIGUE)
            {
            }
            else if (error.errCode == (int)PB.instanceError.INSTANCE_COUNT)
            {

            }
            isBattleClick = false;
            return;
        }

        if(instanceEntryData.chapter == 1)
        {
            GameConfig.Instance.RecoveryMirrorEnegyUnit = GameConfig.Instance.RecoveryMirrorEnegyUnitOne;
            GameConfig.Instance.ConsumMirrorEnegyUnit = GameConfig.Instance.ConsumMirrorEnegyUnitOne;
        }
        else if (instanceEntryData.chapter == 2)
        {
            GameConfig.Instance.RecoveryMirrorEnegyUnit = GameConfig.Instance.RecoveryMirrorEnegyUnitTwo;
            GameConfig.Instance.ConsumMirrorEnegyUnit = GameConfig.Instance.ConsumMirrorEnegyUnitTwo;
        }
        
		var responseData =  msg.GetProtocolBody<PB.HSInstanceEnterRet> ();
		enterInstanceParam.instanceData = responseData;
        GameDataMgr.Instance.OnBattleStart();
        //TODO:
        //------------------------------------------------------------------------
        //InstanceData instanceData = StaticDataMgr.Instance.GetInstanceData(enterInstanceParam.instanceData.instanceId);

        GameMain.Instance.LoadBattleLevel(enterInstanceParam);
        //GameEventMgr.Instance.FireEvent(GameEventList.SwitchLevel, slArgs);
        //切换场景
        //ResourceMgr.Instance.LoadLevelAsyn(instanceData.instanceProtoData.sceneID, false, OnSceneLoaded);
        //------------------------------------------------------------------------
        //GameMain.Instance.ChangeModule<BattleModule>(enterInstanceParam);
        //GameEventMgr.Instance.FireEvent(GameEventList.StartBattle, proto);
    }
    //---------------------------------------------------------------------------------------------
    #endregion
}
