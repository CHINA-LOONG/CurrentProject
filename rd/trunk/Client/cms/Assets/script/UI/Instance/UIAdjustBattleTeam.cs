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
    Normal = 0,
    Tower
}

public class UIAdjustBattleTeam : UIBase
{
	public static string ViewName = "UIAdjustBattleTeam";

	public	Button	backButton;
	public	Button	battleButton;
    public Button rapid1Button;
    public Button rapid10Button;
    public Button resetTimesButton;
    public Text battleTimesText;
    public Text customHuoliText;

	public	Text	lbMyZhenrong;
	public  Text	lbEnemyZhenrong;
	public	Text	lbMyShangzhen;
	public	Text	lbMyHoubei;

    public Image buffImage;
    public Text nameText;
    public Transform[] szStar;
    public Transform[] szGrewStar;

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
    private int teamMax = 1;

	private	List<string> teamList;
	private int prepareIndex = -1;//准备上阵的空位索引

	private Dictionary<string,MonsterIcon> playerAllIconDic = new Dictionary<string, MonsterIcon>();

	EnterInstanceParam enterInstanceParam  = new EnterInstanceParam();
    
    public  static void OpenWith(string instanceId,int star, InstanceType insType = InstanceType.Normal)
    {
        UIAdjustBattleTeam uiadust = (UIAdjustBattleTeam)UIMgr.Instance.OpenUI_(ViewName);
        uiadust.SetData(instanceId, star, insType);

        UIBuild uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
        if (uiBuild != null)
        {
            uiBuild.uiAdjustBattleTeam = uiadust;
        }
    }

	// Use this for initialization
	void Start () 
	{
		EventTriggerListener.Get (backButton.gameObject).onClick = OnBackButtonClick;
		EventTriggerListener.Get (battleButton.gameObject).onClick = OnBattleButtonClick;
        EventTriggerListener.Get(resetTimesButton.gameObject).onClick = OnResetTimesClick;

		lbEnemyZhenrong.text = StaticDataMgr.Instance.GetTextByID ("instance_difangzhenrong");
		lbMyHoubei.text = StaticDataMgr.Instance.GetTextByID ("instance_houbei");
		lbMyShangzhen.text = StaticDataMgr.Instance.GetTextByID ("instance_shangzhen");
		lbMyZhenrong.text = StaticDataMgr.Instance.GetTextByID ("instance_wofangzhenrong");

		battleButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID ("instance_kaishizhandou");
        resetTimesButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("arrayselect_chongzhi_anniu");
    }

    public override void Init()
    {

    }
    public override void Clean()
    {
        //TODO: destroy MonsterIcon
        CleanAllPlayersIcons();
    }

    void OnEnable()
    {
        BindListener();
    }

    void OnDisable()
    {
        UnBindListener();
    }

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.INSTANCE_ENTER_S.GetHashCode ().ToString(), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
    }
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.INSTANCE_ENTER_S.GetHashCode ().ToString (), OnRequestEnterInstanceFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_C.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
    }

	public void SetData(string instanceId,int star, InstanceType insType = InstanceType.Normal)
	{
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
            playerTeamBg[3].SetAsLocked(true);
        }

        if(IsSecondBackupOpen())
        {
            playerTeamBg[4].SetAsLocked(false);
            teamMax++;
        }
        else
        {
            playerTeamBg[4].SetAsLocked(true);
        }

        StartCoroutine( RefreshUICo ());
	}

    bool IsFirstBackupOpen()
    {
        return GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.FirstBackupOpenNeedLevel;
    }

    bool IsSecondBackupOpen()
    {
        return GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.Instance.SecondBackupOpenNeedLevel;
    }

    IEnumerator RefreshUICo()
	{
		yield return StartCoroutine (RefreshEnmeyIcons());
		yield return StartCoroutine (RefreshPlayerIcons());
        yield return StartCoroutine(RefreshDropInfo());
        yield return StartCoroutine(RefreshTitleAndOthers());
	}


	IEnumerator RefreshEnmeyIcons()
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
				subIcon.SetStage(1);
				subIcon.SetLevel(enemyLevel);

				UnitData unitRow = StaticDataMgr.Instance.GetUnitRowData(monsterId);
				if(unitRow != null)
				{
					if(unitRow.assetID.Contains("boss_"))
					{
						subIcon.ShowBossItem(true);
						RectTransform rt = subBg.transform as RectTransform;
						Vector2 oldPivot = rt.pivot;
						Vector2 newPivot = new Vector2(0,1);
						Vector2 newpos = new Vector2(0,0);
						newpos.x = rt.anchoredPosition.x - (oldPivot.x - newPivot.x)*rt.sizeDelta.x;
						newpos.y = rt.anchoredPosition.y -(oldPivot.y -newPivot.y)*rt.sizeDelta.y;

						rt.pivot = newPivot;
						rt.anchoredPosition = newpos;
						float scale = GameConfig.Instance.BossEnemyIconScale;
						subBg.transform.localScale = new Vector3(bossMonsterIconScale, bossMonsterIconScale, bossMonsterIconScale);
					}
				}

				ScrollViewEventListener.Get(subIcon.iconButton.gameObject).onClick = OnEnmeyIconClick;
				ScrollViewEventListener.Get(subIcon.iconButton.gameObject).onPressEnter = OnEnmeyIconClick;
				subBg.gameObject.SetActive(true);
			}
			else
			{
				subBg.gameObject.SetActive(false);
			}
		}
		yield return new WaitForEndOfFrame ();
	}
    void CleanAllEnemyIcons()
    {
        for (int i = 0; i < enemyIcons.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(enemyIcons[i].gameObject);
        }
        enemyIcons.Clear();
    }

	IEnumerator RefreshPlayerIcons()
	{
		InitPlayerTeamIcons ();
		yield return new WaitForEndOfFrame ();
		InitAllPlayerPetsIcons ();
	}

	void InitPlayerTeamIcons()
	{
		prepareIndex = -1;
		teamList =  BattleTeamManager.GetTeamWithKey (BattleTeamManager.TeamList.Defualt);
        CleanAllPlayersIcons();

        MonsterIconBg subBg = null;
		RectTransform rectTrans = null;
		for (int i = 0; i<playerTeamBg.Count; ++ i)
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

		List<GameUnit> listUnit = GameDataMgr.Instance.PlayerDataAttr.GetAllPet ();

		GameUnit subUnit = null;
		for (int i =0; i< listUnit.Count; ++i) 
		{
            subUnit = listUnit[i];
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

    IEnumerator RefreshDropInfo()
    {
        yield return new WaitForEndOfFrame();
        ClearAllDropObject();

        string rewardId = instanceEntryData.reward;

        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardId);
        if (rewardData == null || rewardData.itemList == null)
            yield break;

        for(int i =0; i< rewardData.itemList.Count;++i)
        {
            if (i > 5)
                break;

            var subRewardData = rewardData.itemList[i];
            GameObject go = RewardItemCreator.CreateRewardItem(subRewardData, dropList[i]);
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

   IEnumerator RefreshTitleAndOthers()
    {
        yield return new WaitForEndOfFrame();

        //buffImage
        string spellId = instanceEntryData.instanceSpell;
        SpellProtoType spell = StaticDataMgr.Instance.GetSpellProtoData(spellId);
        if(null != spell)
        {
            Sprite spellSp = ResourceMgr.Instance.LoadAssetType<Sprite>(spell.icon);
            if(null != spellSp)
            {
                buffImage.sprite = spellSp;
            }
        }

        //instance name
        nameText.text = instanceEntryData.NameAttr;

        szStar[0].gameObject.SetActive(star > 0);
        szStar[1].gameObject.SetActive(star > 1);
        szStar[2].gameObject.SetActive(star > 2);
        szGrewStar[0].gameObject.SetActive(false);
        szGrewStar[1].gameObject.SetActive(star == 1);
        szGrewStar[2].gameObject.SetActive(star == 2);

        bool isNormalInstance = instanceType == InstanceType.Normal;
        rapid10Button.gameObject.SetActive(isNormalInstance);
        rapid1Button.gameObject.SetActive(isNormalInstance);
        battleTimesText.gameObject.SetActive(isNormalInstance);
        resetTimesButton.gameObject.SetActive(false);
        if (isNormalInstance)
        {
            InstanceEntryRuntimeData realData = InstanceMapService.Instance.GetRuntimeInstance(instanceId);
            if(realData.countDaily < realData.staticData.count)
            {
                battleTimesText.text = string.Format(StaticDataMgr.Instance.GetTextByID("instance_tiaozhancishu"), realData.countDaily, realData.staticData.count);
            }
            else
            {
                battleTimesText.text = "";
                resetTimesButton.gameObject.SetActive(true);
            }
        }
        InstanceEntry stData = StaticDataMgr.Instance.GetInstanceEntry(instanceId);
        customHuoliText.text = stData.fatigue.ToString();
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
				Logger.LogError("没有空位了，无法上阵。。。！");
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

	void	OnFriendWarehouseIconClick(GameObject go)
	{

	}

	void OnEnmeyIconClick(GameObject go)
	{
		MonsterIcon mIcon = go.GetComponentInParent<MonsterIcon> ();

		string monsterId = mIcon.monsterId;

		UIMonsterInfo.Open (-1, monsterId,enemyLevel,1);
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
	void	OnBackButtonClick(GameObject go)
	{
        GameEventMgr.Instance.FireEvent<string>(GameEventList.ShowInstanceList, instanceId);
        UIMgr.Instance.CloseUI_(this);
    }

	void	OnCancleButtonClick(GameObject go)
	{
        GameEventMgr.Instance.FireEvent<string>(GameEventList.ShowInstanceList,instanceId);
        UIMgr.Instance.CloseUI_(this);
    }

	void	OnBattleButtonClick(GameObject go)
	{
		List<int> battleTeam = SaveBattleTeam ();
		if (null == battleTeam || battleTeam.Count < 1) 
		{
			MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,StaticDataMgr.Instance.GetTextByID("tip_zhenrongError"));
		} 
		else
		{
			enterInstanceParam.playerTeam = battleTeam;
			RequestEnterInstance ();
		}
	}

    void OnResetTimesClick(GameObject go)
    {

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
		PB.HSInstanceEnter param = new PB.HSInstanceEnter ();
		param.instanceId = instanceId;
        int count = enterInstanceParam.playerTeam.Count;
        for (int i = 0; i < count; ++i)
        {
            param.battleMonsterId.Add(enterInstanceParam.playerTeam[i]);
        }

		GameApp.Instance.netManager.SendMessage (PB.code.INSTANCE_ENTER_C.GetHashCode (), param);
	}

	void OnRequestEnterInstanceFinished(ProtocolMessage msg)
	{
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        
		var responseData =  msg.GetProtocolBody<PB.HSInstanceEnterRet> ();
		enterInstanceParam.instanceData = responseData;
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
}
