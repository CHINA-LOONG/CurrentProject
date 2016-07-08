using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIAdjustBattleTeam : UIBase
{
	public static string ViewName = "UIAdjustBattleTeam";
	public static string AssertName = "ui/adjustBattleTeam" ;

	public	Button	backButton;
	public	Button	battleButton;
	public	Button	cancleButton;

	public	List<MonsterIconBg>	playerTeamBg	= new	List<MonsterIconBg>();
	public	List<MonsterIconBg>	enemyTeamBg		= new	List<MonsterIconBg>();

	public ScrollView scrollView;

	private string instanceId  = null;
	private	List<string>	enemyList =null;

	private	List<string> teamList;
	private int prepareIndex = -1;//准备上阵的空位索引

	private Dictionary<string,MonsterIcon> playerAllIconDic = new Dictionary<string, MonsterIcon>();

	// Use this for initialization
	void Start () 
	{
		EventTriggerListener.Get (backButton.gameObject).onClick = OnBackButtonClick;
		EventTriggerListener.Get (battleButton.gameObject).onClick = OnBattleButtonClick;
		EventTriggerListener.Get (cancleButton.gameObject).onClick = OnCancleButtonClick;
	}

	public	void	SetData(string instanceId,List<string>enmeyList)
	{
		this.instanceId = instanceId;
		this.enemyList = enmeyList;
		StartCoroutine( RefreshUICo ());
	}
	
	IEnumerator RefreshUICo()
	{
		yield return StartCoroutine (RefreshEnmeyIcon ());
		yield return StartCoroutine (RefreshPlayerIcon ());
	}

	IEnumerator RefreshEnmeyIcon()
	{
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
				subIcon.transform.SetParent(subBg.transform,false);
				
				rectTrans = subIcon.transform as RectTransform;
				rectTrans.anchoredPosition = new Vector2(0,0);
				rectTrans.localScale  = new Vector3(0.6f,0.6f,0.6f);

				subIcon.SetMonsterStaticId(monsterId);
				
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

	IEnumerator RefreshPlayerIcon()
	{
		InitPlayerTeamIcons ();
		yield return new WaitForEndOfFrame ();
		InitAllPlayersIcons ();
	}

	void InitPlayerTeamIcons()
	{
		prepareIndex = -1;
		teamList =  BattleTeamManager.GetTeamWithKey (BattleTeamManager.TeamList.Defualt);
		
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
				subIcon.transform.SetParent(subBg.transform,false);
				
				rectTrans = subIcon.transform as RectTransform;
				rectTrans.anchoredPosition = new Vector2(0,0);
				rectTrans.localScale  = new Vector3(0.6f,0.6f,0.6f);
				
				subIcon.SetId(guid);
				subIcon.SetMonsterStaticId(unit.pbUnit.id);
				
				EventTriggerListener.Get(subIcon.iconButton.gameObject).onClick = OnPlayerTeamIconClick;
			}
		}
	}

	void InitAllPlayersIcons()
	{
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

			playerAllIconDic.Add(icon.Id,icon);

			if(teamList.Contains(subUnit.pbUnit.guid.ToString()))
			{
				icon.ShowSelectImage(true);
			}
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
				Logger.LogError("没有空位了，无法上阵。。。！");
				return;
			}
			PlayerWarehouseToBattleTeam(micon.Id);
		}
		micon.ShowSelectImage (!isSel);
	}

	void OnPlayerWarehouseIconPressEnter(GameObject go)
	{
		MonsterIcon micon = go.GetComponentInParent<MonsterIcon> ();

		string guid = micon.Id;
		UIMonsterInfo.Open (int.Parse(guid), micon.monsterId);
	}

	void	OnFriendWarehouseIconClick(GameObject go)
	{

	}

	void OnEnmeyIconClick(GameObject go)
	{
		MonsterIcon mIcon = go.GetComponentInParent<MonsterIcon> ();

		string monsterId = mIcon.monsterId;

		UIMonsterInfo.Open (-1, monsterId);
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

		//新的prepareindex
		UpdatePrepareIndex ();
	}

	//---------------------------------------------------------------------------------------------------------------------
	void BattleTeamToPlayerWarehouse(string guid)
	{
		MonsterIcon playerIcon = playerAllIconDic [guid];
		playerIcon.ShowSelectImage (false);

		MonsterIconBg subBg = null;
		for (int i =0; i<playerTeamBg.Count; ++i) 
		{
			subBg = playerTeamBg[i];
			MonsterIcon subIcon = subBg.GetComponentInChildren<MonsterIcon>();
			if(null!= subIcon && subIcon.gameObject.activeSelf)
			{
				if(guid == subIcon.Id)
				{
					DestroyImmediate(subIcon.gameObject);
					break;
				}
			}
		}
		if (prepareIndex != -1)
		{
			subBg = playerTeamBg [prepareIndex];
			subBg.SetEffectShow(false);
		}
		UpdatePrepareIndex ();

		subBg = playerTeamBg [prepareIndex];
		subBg.SetEffectShow (true);
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
		UIMgr.Instance.CloseUI (this);
	}

	void	OnBattleButtonClick(GameObject go)
	{
		bool isNeedSave = false;
		string guid = null;
		string tempGuid = null;
		MonsterIconBg subIconBg;
		MonsterIcon subIcon;
		for (int i =0; i < playerTeamBg.Count; ++i) 
		{
			subIconBg = playerTeamBg[i];
			subIcon = subIconBg.GetComponentInChildren<MonsterIcon>();
			if(null == subIcon)
			{
				guid = "";
			}
			else
			{
				guid = subIcon.Id;
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
	}

	void	OnCancleButtonClick(GameObject go)
	{
		UIMgr.Instance.CloseUI (this);
	}


}
