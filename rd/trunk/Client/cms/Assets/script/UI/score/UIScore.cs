using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UIScore : UIBase 
{
    public static string ViewName = "UIScore";

    public GameObject mPlayerInfoRoot;
    public Text mPlayerLvl;
    public UIProgressbar mPlayerProgress;
    public Text mPlayerGainExp;
    public Text mPlayerGainGold;
    public Sprite mVictorySprite;
    public Sprite mFailedSprite;
    
    //internal use only
    public RectTransform mCenterPos;
    public RectTransform mTopPos;

    public Button mRetryBtn;
    public Button mNextLevelBtn;
    public Button mConfirmBtn;
    public Text mRetryText;
    public Text mNextLevelText;
    public Text mConfirmText;

    public RectTransform mMonsterExpList;
    public RectTransform mItemGainList;

    public GameObject mBackground;
    public GameObject mLineMonsterItem;

    private UIGainPet mGainPetUI;
    private GameObject mEndBattleUI;

    private bool mIsSuccess;
    private PB.HSInstanceSettleRet mInstanceSettleResult;

    //---------------------------------------------------------------------------------------------
    void Start()
    {
        EventTriggerListener.Get(mRetryBtn.gameObject).onClick = OnRetry;
        EventTriggerListener.Get(mNextLevelBtn.gameObject).onClick = OnNextLevel;
        EventTriggerListener.Get(mConfirmBtn.gameObject).onClick = OnConfirm;
    }
    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
    }
    //---------------------------------------------------------------------------------------------
    void OnDisable()
    {
    }
    //---------------------------------------------------------------------------------------------
    void OnRetry(GameObject go)
    {
        BattleController.Instance.UnLoadBattleScene(2);
    }
    //---------------------------------------------------------------------------------------------
    void OnNextLevel(GameObject go)
    {
        BattleController.Instance.UnLoadBattleScene(1);
    }
    //---------------------------------------------------------------------------------------------
    void OnConfirm(GameObject go)
    {
        BattleController.Instance.UnLoadBattleScene(0);
    }
    //---------------------------------------------------------------------------------------------
    void OnSkipAni(GameObject go)
    {

    }
    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    public override void Init()
    {
        base.Init();

        mPlayerInfoRoot.SetActive(false);
        mMonsterExpList.gameObject.SetActive(false);
        mItemGainList.gameObject.SetActive(false);
        mRetryBtn.gameObject.SetActive(false);
        mNextLevelBtn.gameObject.SetActive(false);
        mConfirmBtn.gameObject.SetActive(false);
        mBackground.SetActive(false);
        mLineMonsterItem.SetActive(false);
        mRetryText.text = StaticDataMgr.Instance.GetTextByID("ui_battle_again");
        mNextLevelText.text = StaticDataMgr.Instance.GetTextByID("ui_battle_next");
        mConfirmText.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
    }
    //---------------------------------------------------------------------------------------------
    public override void Clean()
    {
        base.Clean();
        ResourceMgr.Instance.DestroyAsset(mEndBattleUI);
    }
    //---------------------------------------------------------------------------------------------
    public void ShowScoreUI(bool success)
    {
        mIsSuccess = success;
        gameObject.SetActive(true);
        if (false)
        {
            AddGainMonster("xgXiyiren");
        }
        else
        {
            ShowEndBattleUI();
        }
    }
    //---------------------------------------------------------------------------------------------
    private void AddGainMonster(string monsterID)
    {
        mGainPetUI = UIMgr.Instance.OpenUI_(UIGainPet.ViewName) as UIGainPet;
        mGainPetUI.transform.SetParent(transform, false);
        mGainPetUI.ShowGainPet(monsterID);
        mGainPetUI.SetConfirmCallback(ConfirmGainPet);
    }
    //---------------------------------------------------------------------------------------------
    private void ConfirmGainPet(GameObject go)
    {
        UIMgr.Instance.DestroyUI(mGainPetUI);
        ShowEndBattleUI();
    }
    //---------------------------------------------------------------------------------------------
    private void ShowEndBattleUI()
    {
        mEndBattleUI = ResourceMgr.Instance.LoadAsset("endBattle");
        mEndBattleUI.transform.SetParent(transform, false);
        mEndBattleUI.transform.localPosition = mCenterPos.localPosition;
        Image endImage = mEndBattleUI.GetComponent<Image>();
        if (mIsSuccess)
        {
            endImage.sprite = mVictorySprite;
        }
        else
        {
            endImage.sprite = mFailedSprite;
        }
        endImage.SetNativeSize();
        Tweener battleTitleTw = mEndBattleUI.transform.DOLocalMove(mTopPos.localPosition, BattleConst.scoreTitleUpTime);
        battleTitleTw.OnComplete(ShowStar);
        battleTitleTw.SetDelay(BattleConst.scoreTitleStayTime);
    }
    //---------------------------------------------------------------------------------------------
    public void SetScoreInfo(PB.HSInstanceSettleRet scoreInfo)
    {
        mInstanceSettleResult = scoreInfo;
    }
    //---------------------------------------------------------------------------------------------
    private void ShowStar()
    {
        //Logger.LogFormat("get star {0}", mInstanceSettleResult.starCount);
        ShowScoreInfo();
    }
    //---------------------------------------------------------------------------------------------
    private void ShowScoreInfo()
    {
        mBackground.SetActive(true);
        mLineMonsterItem.SetActive(true);
        //PB.HSRewardInfo rewardInfo = mInstanceSettleResult.reward;

        //show player info;
        mPlayerInfoRoot.SetActive(true);
        //PB.SynPlayerAttr playerAttr = rewardInfo.playerAttr;
        //PlayerData mainPlayer = GameDataMgr.Instance.PlayerDataAttr;
        //int expGain = playerAttr.exp - mainPlayer.exp
        //mPlayerLvl.text = playerAttr.level.ToString();
        //mPlayerProgress.SetLoopCount(playerAttr.level - mainPlayer.level);
        //mPlayerProgress.SetCurrrentRatio(mainPlayer.exp / (mainPlayer.level * 10 + 100));
        //mPlayerProgress.SetTargetRatio(playerAttr.exp / (playerAttr.level * 10 + 100));
        //mPlayerGainGold.text = (playerAttr.gold - mainPlayer.gold).ToString();
        //mPlayerGainExp.text = expGain.ToString();

        //show monster info
        mMonsterExpList.gameObject.SetActive(true);
        //List<PB.SynMonsterAttr> monsterInfoList = rewardInfo.monstersAttr;
        //int count = monsterInfoList.Count;
        //GameUnit originalMonster;
        //for (int i = 0; i < monsterInfoList.Count; ++i)
        //{
        //    originalMonster = mainPlayer.GetPetWithKey(monsterInfoList[i].monsterId);
        //    if (originalMonster == null)
        //    {
        //        Logger.LogError("Score error, no this monster");
        //        continue;
        //    }

        //    UIMonsterIconExp monsterInfo = UIMonsterIconExp.Create();
        //    monsterInfo.transform.SetParent(mMonsterExpList.transform, false);
        //    monsterInfo.SetMonsterIconExpInfo(
        //        monsterInfoList[i].monsterId,
        //        expGain,
        //        originalMonster.pbUnit.level,
        //        monsterInfoList[i].level
        //        );
        //}

        //show item drop info
        mItemGainList.gameObject.SetActive(true);
        //List<PB.RewardItem>rewardItemList = rewardInfo.RewardItems;
        //count = rewardItemList.Count;
        //for (int i = 0; i < count; ++i)
        //{
        //    PB.RewardItem item = rewardItemList[i];
        //    if (item.type == (int)PB.itemType.ITEM)
        //    {
        //        ItemIcon icon = ItemIcon.CreateItemIcon(ItemData.valueof(item.itemId, item.count));
        //        icon.transform.SetParent(mItemGainList.transform);
        //    }
        //    else if (item.type == (int)PB.itemType.EQUIP)
        //    {
        //        EquipData equipData = new EquipData()
        //        {
        //            id = item.id,
        //            equipId = item.itemId,
        //            stage = item.stage,
        //            level = item.level
        //        };
        //        ItemIcon icon = ItemIcon.CreateItemIcon(equipData);
        //        icon.transform.SetParent(mItemGainList.transform);
        //    }
        //}

        //show button
        mRetryBtn.gameObject.SetActive(true);
        mNextLevelBtn.gameObject.SetActive(mIsSuccess);
        mConfirmBtn.gameObject.SetActive(true);
    }
    //---------------------------------------------------------------------------------------------
}
