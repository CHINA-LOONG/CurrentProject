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
        UITower tower = UIMgr.Instance.OpenUI_(UITower.ViewName) as UITower;
        tower.ShowTower(towerType);
        return tower;
    }
    public GameObject rewardButton;
    public GameObject closeRewardButton;
    public GameObject towerReward;//奖励
    public GameObject exitTowerButton;
    public Text rewardTitle;
    public Text towerStageNum;
	//外面拖的 代码不用new和add
    public List<GameObject> towerItems;
    public Transform fatherBox;
    public GameObject towerItemOne;
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
    int currLoopNum = -1;
	float oneTime = 1f;
	float twoTime = 2f;
    int lastStage = -1;
    //---------------------------------------------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(rewardButton).onClick = StageRewardClick;
        EventTriggerListener.Get(closeRewardButton).onClick = StageRewardClick;
        EventTriggerListener.Get(exitTowerButton).onClick = ExitTowerClick;
        rewardTitle.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_reward");
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

        int towerItemNum = 0;
        TowerItemData towerItemData;

        Sprite towerStateImage = null;
        int stageTower = (currentFloor) / 5;
        towerStageNum.text = "STAGE " + (stageTower + 1);
        //first entry
        loopNum = 0;
        if (towerID != 0)
        {
            if (lastStage != stageTower)
            {
                loopNum = stageTower - lastStage;
                SlideTowerItem();
            }
        }
        towerData = StaticDataMgr.Instance.GetTowerData(towerType);
        for (int i = 0; i < towerData.floorList.Count; i++)
        {
            if ((i / 5) == stageTower || (i / 5) == (stageTower + 1))
            {
                EventTriggerListener.Get(towerItems[towerItemNum]).onClick = TowerItemClick;
                towerItemData = towerItems[towerItemNum].GetComponent<TowerItemData>();
                towerItemData.towerNum.text = (towerItemNum + 1).ToString();
                towerItemData.itemTowerID = towerData.floorList[i];
                if ((currentFloor - 1) >= i)
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
        lastStage = stageTower;

        if (towerID != towerType)
        {
            towerID = towerType;
            ShowReward(towerID);
        }
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

                ItemIcon icon = ItemIcon.CreateItemIcon(ItemData.valueof(rewardData.itemList[0].protocolData.itemId, 1), false);
                icon.transform.SetParent(towerItem.rewardImage.transform, false);

                towerItem.rewardName.text = StaticDataMgr.Instance.GetTextByID(itemData.name) + "x" + rewardData.itemList[0].protocolData.count;
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
        TowerItemData towerItemData = item.GetComponent<TowerItemData>();
		if (towerItemData.currType == TowerItemType.Item_Type_Curr)
        {
            UIAdjustBattleTeam.OpenWith(towerItemData.itemTowerID, 0, InstanceType.Tower);
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    void ExitTowerClick(GameObject btn)
    {
        CloseReward();
        UIMgr.Instance.DestroyUI(transform.GetComponent<UITower>());
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    public void OpenAdjustTeam()
    {
        string nextInstance = GameDataMgr.Instance.GetNextTowerFloor();
        if (nextInstance != null)
        {
            UIAdjustBattleTeam.OpenWith(nextInstance, 0, InstanceType.Tower);
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    //TODO: save current towid and laststage to playerdatamanager
    //---------------------------------------------------------------------------------------------------------------------------------------
}
