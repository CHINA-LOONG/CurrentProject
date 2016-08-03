using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InstanceInfo : MonoBehaviour 
{
	public	Button	backButton;
	public	Button	clearButton;
	public	Button	clearTenButton;
	public	Button	acceptButton;

	public	Text	nameText;
	public	Text	descText;
	public	Text	needFatigueText;
	public	Text	attackCountText;
	public	Text	clearCountText;

	public	Text	lbEnmeyList;
	public	Text	lbRewardList;

	public FatigueUI fatigueUi;

	public	List<Transform>	enemyGroup = new List<Transform> ();
	public	List<Transform>	rewardGroup = new List<Transform>();
	
	private InstanceEntryRuntimeData  instanceData;


	void Start ()
	{
		EventTriggerListener.Get (backButton.gameObject).onClick = OnBackButtonClick;
		EventTriggerListener.Get (clearButton.gameObject).onClick = OnClearButtonClick;
		EventTriggerListener.Get (clearTenButton.gameObject).onClick = OnClearTenButtoClick;
		EventTriggerListener.Get (acceptButton.gameObject).onClick = OnAcceptButtonClick;

		lbEnmeyList.text = StaticDataMgr.Instance.GetTextByID ("instance_difangzhenrong");
		lbRewardList.text = StaticDataMgr.Instance.GetTextByID ("instance_jiangliList");

        backButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("ui_fanhui");
		clearButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("instance_saodang");
		clearTenButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("instance_saodang10");
		acceptButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("instance_yingzhan");
	}

	public	void	SetShow(bool bshow)
	{
		gameObject.SetActive (bshow);
	}

	public void ShowWithData(InstanceEntryRuntimeData data)
	{
		instanceData = data;
		SetShow (true);
		nameText.text = data.staticData.NameAttr;
		descText.text = data.staticData.desc;
		needFatigueText.text = string.Format (StaticDataMgr.Instance.GetTextByID ("instance_tilixiaohao"), data.staticData.fatigue.ToString ());
		int allAttackCount = data.staticData.count;
		int leftCount = allAttackCount - data.countDaily;
		if (allAttackCount < 1)
		{
			attackCountText.text = "";
		} 
		else
		{
			attackCountText.text = string.Format(StaticDataMgr.Instance.GetTextByID("instance_tiaozhancishu"),leftCount,allAttackCount);
		}
		clearCountText.text =  string.Format(StaticDataMgr.Instance.GetTextByID("instance_saodangquanNumber"),-999);

		SetEnemy (data.staticData);
		SetReward (data.staticData);

		PlayerData pData = GameDataMgr.Instance.PlayerDataAttr;

		PlayerLevelAttr levelAttr = StaticDataMgr.Instance.GetPlayerLevelAttr (pData.LevelAttr);
		fatigueUi.fatigueText.text = pData.fatigue.ToString () + "/" + levelAttr.fatigue.ToString ();
	}

	void	SetEnemy(InstanceEntry entryData)
	{
		string monsterId = null;

		monsterId = entryData.enemy1;
		SetSubEnemy (0, monsterId);

		monsterId = entryData.enemy2;
		SetSubEnemy (1, monsterId);

		monsterId = entryData.enemy3;
		SetSubEnemy (2, monsterId);

		monsterId = entryData.enemy4;
		SetSubEnemy (3, monsterId);

		monsterId = entryData.enemy5;
		SetSubEnemy (4, monsterId);

		monsterId = entryData.enemy6;
		SetSubEnemy (5, monsterId);

	}

	void	SetSubEnemy(int index, string monsterId)
	{
		GameObject tempGo = enemyGroup [index].gameObject;
		if (string.IsNullOrEmpty (monsterId))
		{
			tempGo.SetActive(false);
			return;
		}
		UnitData unitData = StaticDataMgr.Instance.GetUnitRowData (monsterId);
		if (null == unitData) 
		{
			Logger.LogError("Error:instance info , monsterId config error :" + monsterId);
			tempGo.SetActive(false);
			return;
		}
		tempGo.SetActive (true);
		MonsterIcon unitIcon = tempGo.GetComponentInChildren<MonsterIcon> ();
		if (null == unitIcon)
		{
			unitIcon = MonsterIcon.CreateIcon();
			unitIcon.transform.SetParent(tempGo.transform, false);

			RectTransform rectTrans = unitIcon.transform as RectTransform;
			rectTrans.anchoredPosition = new Vector2(0,0);
			rectTrans.localScale = new Vector3(0.5f,0.5f,0.5f);
		}

		unitIcon.SetMonsterStaticId (monsterId);
		unitIcon.SetLevel (10);
		unitIcon.SetStage (1);
		if (unitData.assetID.Contains("boss_"))
		{
			unitIcon.ShowBossItem(true);
		}
		//unitIcon.SetName (unitData.nickName);
	}

	void SetReward(InstanceEntry entryData)
	{
		string  itemId = "";
		/*
		itemId = entryData.reward1;
		SetSubReward (0, itemId);
		
		itemId = entryData.reward2;
		SetSubReward (1, itemId);
		
		itemId = entryData.reward3;
		SetSubReward (2, itemId);
		
		itemId = entryData.reward4;
		SetSubReward (3, itemId);
		
		itemId = entryData.reward5;
		SetSubReward (4, itemId);
		
		itemId = entryData.reward6;
		SetSubReward (5, itemId);
        */
	}
	
	void	SetSubReward(int index, string itemId)
	{
		GameObject tempGo = rewardGroup [index].gameObject;
		
		ItemStaticData itemData = null;
		if (!string.IsNullOrEmpty (itemId))
		{
			itemData = StaticDataMgr.Instance.GetItemData (itemId);
		}
		if (null == itemData) 
		{
			tempGo.SetActive(false);
			return;
		}
		Text subText = tempGo.GetComponentInChildren<Text> ();
		if(null != subText)
			subText.text = itemData.NameAttr;
		tempGo.SetActive (true);
	}
	
	
	void	OnBackButtonClick(GameObject go)
	{
		SetShow (false);
	}

	void	OnClearButtonClick(GameObject go)
	{

	}

	void	OnClearTenButtoClick(GameObject	go)
	{
	}

	void	OnAcceptButtonClick(GameObject go)
	{
        UIAdjustBattleTeam adjustUi = UIMgr.Instance.OpenUI_(UIAdjustBattleTeam.ViewName) as UIAdjustBattleTeam;
        UIBuild uiBuild= UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
        if (uiBuild!=null)
        {
            uiBuild.uiAdjustBattleTeam = adjustUi;
        }
		adjustUi.SetData (instanceData.instanceId, instanceData.staticData.enemyList,10);
	}

}
