using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class UITower : UIBase
{
    public static string ViewName = "UITower";
    public static UITower OpenTower(int towerType)
    {
        GameDataMgr.Instance.mTowerRefreshed = false;
        UITower tower = UIMgr.Instance.OpenUI_(UITower.ViewName) as UITower;
        tower.ShowTower(towerType);
        return tower;
    }
    public GameObject rewardButton;
    public GameObject closeRewardButton;
    public GameObject towerReward;//奖励
	public Text rewardButtonName;
    public GameObject exitTowerButton;
    public Text rewardTitle;
    public Text towerStageNum;
	//外面拖的 代码不用new和add
    public List<GameObject> towerItems;
    public Transform fatherBox;
    public GameObject towerItemOne;
	public Text towerNext;
    public GameObject towerNextImage;
    public GameObject towerItemTwo;
    public Transform startVec = null;
    public Transform middleVec = null;
    public Transform endVec = null;
    List<GameObject> towerRewardItems = new List<GameObject>();
    Tweener towerTw1 = null;
    Tweener towerTw2 = null;
    int towerID;//塔ID
    TowerData towerData;
    int loopNum = 0;
    int currLoopNum = 1;
    float oneTime = 0.2f;
    float twoTime = 0.4f;
    //---------------------------------------------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(rewardButton).onClick = StageRewardClick;
        EventTriggerListener.Get(closeRewardButton).onClick = StageRewardClick;
        EventTriggerListener.Get(exitTowerButton).onClick = ExitTowerClick;
        rewardTitle.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_reward");
        rewardButtonName.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_reward");
        towerReward.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
	void StageRewardClick(GameObject btn)
	{		
        if (btn != closeRewardButton)
        {
            towerReward.SetActive(true);
        }
        else
        {
            towerReward.SetActive(false);
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    void ShowTower(int towerType)
    {
        int currentFloor = 0;
        switch (towerType)
        {
            case (int)TowerType.Tower_Shilian:
                currentFloor = GameDataMgr.Instance.curTowerShilianFloor;
                break;
            case (int)TowerType.Tower_Juewang:
                currentFloor = GameDataMgr.Instance.curTowerJuewangFloor;
                break;
            case (int)TowerType.Tower_Siwang:
                currentFloor = GameDataMgr.Instance.curTowerSiwangFloor;
                break;
        }
        towerData = StaticDataMgr.Instance.GetTowerData(towerType);
        int towerItemNum = 0;
        TowerItemData towerItemData;
        Sprite towerStateImage = null;
        if (currentFloor >= (towerData.floorList.Count - 5))
        {
            towerNextImage.SetActive(false);
            towerNext.text = "FINISH";
        }       
        int stageTower = (currentFloor) / 5;
        if (currentFloor == towerData.floorList.Count)
            --stageTower;
        towerStageNum.text = "STAGE " + (stageTower + 1);
        //first entry
        loopNum = 0;
       	
        for (int i = 0; i < towerData.floorList.Count; i++)
        {
            if ((i / 5) == stageTower || (i / 5) == (stageTower + 1))
            {
                EventTriggerListener.Get(towerItems[towerItemNum]).onClick = TowerItemClick;
                towerItemData = towerItems[towerItemNum].GetComponent<TowerItemData>();
                towerItemData.towerNum.text = (i + 1).ToString();
                towerItemData.itemTowerID = towerData.floorList[i];
                if (currentFloor == towerData.floorList.Count)
                {
                    towerStageNum.text = "STAGE " + stageTower;
                    towerItemData.currType = TowerItemType.Item_Type_ok;
                    towerStateImage = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_cleared");
                }
                else if ((currentFloor - 1) >= i)
                {
                    towerItemData.currType = TowerItemType.Item_Type_ok;
                    towerStateImage = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_cleared");
                }
                else if (i == currentFloor)
                {
                    towerItemData.currType = TowerItemType.Item_Type_Curr;
                    towerItemData.selectedImage.SetActive(true);
                    towerStateImage = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_ready");
                }
                else if ((towerItemNum + 1) % 5 == 0)
                {
                    towerItemData.currType = TowerItemType.Item_Type_end;
                    towerStateImage = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_boss");
                }
                else if ((currentFloor - 1) < i)
                {
                    towerItemData.currType = TowerItemType.Item_Type_not;
                    towerStateImage = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_lock");
                }
                towerItemData.towerImage.sprite = towerStateImage;
                towerItemData.towerImage.SetNativeSize();
                towerItemNum++;
            }
        }
        if ((int)GameDataMgr.Instance.curTowerType != towerType)
        {
            GameDataMgr.Instance.lastStage = -1;
        }
        else
        {
            if (GameDataMgr.Instance.lastStage != -1)
            {
                if (GameDataMgr.Instance.lastStage != stageTower && stageTower > GameDataMgr.Instance.lastStage)
                {
                    loopNum = stageTower - GameDataMgr.Instance.lastStage;
                    SlideTowerItem();
                }
            }
        }        
        GameDataMgr.Instance.lastStage = stageTower;

        if (towerID != towerType)
        {
            towerID = towerType;
            ShowReward(towerID);
        }
        GameDataMgr.Instance.curTowerType = (TowerType)towerType;
    }
	//---------------------------------------------------------------------------------------------------------------------------------------    
    void SlideTowerItem()
    {
		towerTw1 = towerItemOne.transform.DOLocalMoveY(endVec.localPosition.y, oneTime);
        towerTw1.SetEase(Ease.Linear);
        towerTw1.OnComplete(SlideTowerItemOne);
        towerTw2 = towerItemTwo.transform.DOLocalMoveY(endVec.localPosition.y, twoTime);
        towerTw2.SetEase(Ease.Linear);
        towerTw2.OnComplete(SlideTowerItemTwo);
    }
	//---------------------------------------------------------------------------------------------------------------------------------------
    void SlideTowerItemOne()
    {
        if (currLoopNum == loopNum)
        {
            towerItemOne.transform.localPosition = startVec.transform.localPosition;
			towerTw1 = towerItemOne.transform.DOLocalMoveY(middleVec.localPosition.y, oneTime);
            towerTw1.SetEase(Ease.Linear);
        }
        else
        {
            towerItemOne.transform.localPosition = startVec.transform.localPosition;
			towerTw1 = towerItemOne.transform.DOLocalMoveY(endVec.localPosition.y, twoTime);
            towerTw1.SetEase(Ease.Linear);
            towerTw1.OnComplete(SlideTowerItemOne);          
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    void SlideTowerItemTwo()
    {
        if (currLoopNum == loopNum)
        {
            towerItemTwo.transform.localPosition = startVec.transform.localPosition;
            return;
        }
        towerItemTwo.transform.localPosition = startVec.transform.localPosition;
        towerTw2 = towerItemTwo.transform.DOLocalMoveY(endVec.localPosition.y, twoTime);
        towerTw2.SetEase(Ease.Linear);
        towerTw2.OnComplete(SlideTowerItemTwo);
        currLoopNum++;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    #region stage奖励展示
    void ShowReward(int towerType)
    {
        TowerRewardItem towerItem;
        ItemStaticData itemData;
        InstanceEntry instanceEntry;
        RewardData rewardData;
		ItemIcon icon = null;
        towerData = StaticDataMgr.Instance.GetTowerData(towerType);
        int rewardNum = 0;
        for (int i = 0; i < towerData.floorList.Count; i++)
        {
            if ((i + 1) % 5 == 0)
            {
                instanceEntry = StaticDataMgr.Instance.GetInstanceEntry(towerData.floorList[i].ToString());
                rewardData = StaticDataMgr.Instance.GetRewardData(instanceEntry.reward);
                towerRewardItems.Add(ResourceMgr.Instance.LoadAsset("TowerItemReward"));
                towerRewardItems[rewardNum].transform.parent = fatherBox;
                towerRewardItems[rewardNum].transform.localScale = fatherBox.localScale;
                towerItem = towerRewardItems[rewardNum].GetComponent<TowerRewardItem>();
                itemData = StaticDataMgr.Instance.GetItemData(rewardData.itemList[0].protocolData.itemId);
				if (rewardData.itemList[0].protocolData.type == (int)PB.itemType.ITEM) 
				{
					icon = ItemIcon.CreateItemIcon(ItemData.valueof(rewardData.itemList[0].protocolData.itemId, 1), false);
				}
				else if (rewardData.itemList[0].protocolData.type == (int)PB.itemType.EQUIP)
				{
					EquipData equipData = EquipData.valueof(
						rewardData.itemList[0].protocolData.id,rewardData.itemList[0].protocolData.itemId,
						rewardData.itemList[0].protocolData.stage,rewardData.itemList[0].protocolData.level,BattleConst.invalidMonsterID, null);
					icon = ItemIcon.CreateItemIcon(equipData,false);
                }
                icon.transform.SetParent(towerItem.rewardImage.transform, false);

                towerItem.rewardName.text = itemData.NameAttr + "x" + rewardData.itemList[0].protocolData.count;
                towerItem.RewardStage.text = "STAGE " + (rewardNum + 1);
                rewardNum++;
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    void CloseReward()
    {
        for (int i = 0; i < towerRewardItems.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(towerRewardItems[i]);
        }
        towerRewardItems.Clear();
        towerReward.SetActive(false);
    }
    #endregion
    //---------------------------------------------------------------------------------------------------------------------------------------
    void TowerItemClick(GameObject item)
    {
        if (GameDataMgr.Instance.mTowerRefreshed == true)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("towerBoss_record_004"), (int)PB.ImType.PROMPT);
            return;
        }

        TowerItemData towerItemData = item.GetComponent<TowerItemData>();
		if (towerItemData.currType == TowerItemType.Item_Type_Curr)
        {
            int curTowerFloor = int.Parse(towerItemData.towerNum.text);
            UIAdjustBattleTeam.OpenWith(towerItemData.itemTowerID, 0,false, InstanceType.Tower, curTowerFloor);
        }
		else if(towerItemData.currType == TowerItemType.Item_Type_not || towerItemData.currType == TowerItemType.Item_Type_end)
		{
			UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("towerBoss_record_003"),  (int)PB.ImType.PROMPT);
		}
		else if (towerItemData.currType == TowerItemType.Item_Type_ok) 
		{
			UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("towerBoss_record_002"),  (int)PB.ImType.PROMPT);
		}
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    void ExitTowerClick(GameObject btn)
    {
        CloseReward();
        UIMgr.Instance.DestroyUI(transform.GetComponent<UITower>());
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    //TODO: save current towid and laststage to playerdatamanager
    //---------------------------------------------------------------------------------------------------------------------------------------
}
