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

public class UIAdjustBattleTeam : UIBase
{
	public static string ViewName = "UIAdjustBattleTeam";

	public	Button	backButton;
	public	Button	battleButton;
	public	Button	cancleButton;

	public	Text	lbMyZhenrong;
	public  Text	lbEnemyZhenrong;
	public	Text	lbMyShangzhen;
	public	Text	lbEnemyShangzhen;
	public	Text	lbMyHoubei;
	public	Text	lbYongyou;
	public	Text	lbFriend;
	public  Text	lbZhenrongTiaozheng;

    public List<MonsterIconBg> playerTeamBg = new List<MonsterIconBg>();
    private List<MonsterIcon> playerIcons = new List<MonsterIcon>();
    public List<MonsterIconBg> enemyTeamBg = new List<MonsterIconBg>();
    private List<MonsterIcon> enemyIcons = new List<MonsterIcon>();

	public ScrollView scrollView;

	private string instanceId  = null;
	private	List<string>	enemyList =null;
	private int enemyLevel = 1;

	private	List<string> teamList;
	private int prepareIndex = -1;//准备上阵的空位索引

	private Dictionary<string,MonsterIcon> playerAllIconDic = new Dictionary<string, MonsterIcon>();

	EnterInstanceParam enterInstanceParam  = new EnterInstanceParam();
	// Use this for initialization
	void Start () 
	{
		EventTriggerListener.Get (backButton.gameObject).onClick = OnBackButtonClick;
		EventTriggerListener.Get (battleButton.gameObject).onClick = OnBattleButtonClick;
		EventTriggerListener.Get (cancleButton.gameObject).onClick = OnCancleButtonClick;

		lbEnemyShangzhen.text = StaticDataMgr.Instance.GetTextByID ("instance_shangzhen");
		lbEnemyZhenrong.text = StaticDataMgr.Instance.GetTextByID ("instance_difangzhenrong");
		lbFriend.text = StaticDataMgr.Instance.GetTextByID ("instance_haoyou");
		lbMyHoubei.text = StaticDataMgr.Instance.GetTextByID ("instance_houbei");
		lbMyShangzhen.text = StaticDataMgr.Instance.GetTextByID ("instance_shangzhen");
		lbMyZhenrong.text = StaticDataMgr.Instance.GetTextByID ("instance_wofangzhenrong");
		lbYongyou.text = StaticDataMgr.Instance.GetTextByID ("instance_yongyou");
		lbZhenrongTiaozheng.text = StaticDataMgr.Instance.GetTextByID ("instance_zhenrongtiaozheng");

        backButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("instance_back");
		battleButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID ("instance_kaishizhandou");
		cancleButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID ("instance_quxiaozhandou");
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

	public void SetData(string instanceId,List<string>enmeyList,int enemyLevel)
	{
		this.instanceId = instanceId;
		this.enemyList = enmeyList;
		this.enemyLevel = enemyLevel;
		StartCoroutine( RefreshUICo ());
	}
	
	IEnumerator RefreshUICo()
	{
		yield return StartCoroutine (RefreshEnmeyIcons());
		yield return StartCoroutine (RefreshPlayerIcons());
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
				rectTrans.localScale  = new Vector3(0.6f,0.6f,0.6f);

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
						subBg.transform.localScale = new Vector3(scale, scale, scale);
					}
				}
				
				EventTriggerListener.Get(subIcon.iconButton.gameObject).onClick = OnEnmeyIconClick;
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
				rectTrans.localScale  = new Vector3(0.6f,0.6f,0.6f);
				
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
			subIcon.transform.SetParent(iconBg.transform,false);
			
			RectTransform rectTrans = subIcon.transform as RectTransform;
			rectTrans.anchoredPosition = new Vector2(0,0);
			rectTrans.localScale  = new Vector3(0.6f,0.6f,0.6f);
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
		for (int i =0; i<playerTeamBg.Count; ++i) 
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
		for (int i =0 ;i < playerTeamBg.Count; ++i)
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
		UIMgr.Instance.CloseUI_(this);
	}

	void	OnCancleButtonClick(GameObject go)
	{
		UIMgr.Instance.CloseUI_(this);
	}

	void	OnBattleButtonClick(GameObject go)
	{
		List<int> battleTeam = SaveBattleTeam ();
		if (null == battleTeam || battleTeam.Count < 1) 
		{
			MsgBox.PromptMsg.Open(StaticDataMgr.Instance.GetTextByID("ui_tishi"),
			                      StaticDataMgr.Instance.GetTextByID("tip_zhenrongError"),
			                      StaticDataMgr.Instance.GetTextByID("ui_queding"));
		} 
		else
		{
			enterInstanceParam.playerTeam = battleTeam;
			RequestEnterInstance ();
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
		PB.HSInstanceEnter param = new PB.HSInstanceEnter ();
		param.instanceId = instanceId;

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
