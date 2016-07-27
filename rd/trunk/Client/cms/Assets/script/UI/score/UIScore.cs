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
        PB.HSRewardInfo rewardInfo = mInstanceSettleResult.reward;
        if (rewardInfo != null)
        {
            int count = rewardInfo.RewardItems.Count;
            for (int i = 0; i < count; ++i)
            {
                PB.RewardItem item = rewardInfo.RewardItems[i];
                if (item.type == (int)PB.itemType.MONSTER)
                {
                    AddGainMonster(item.itemId);
                    return;
                }
            }
        }

        ShowEndBattleUI();
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
        PB.HSRewardInfo rewardInfo = mInstanceSettleResult.reward;
        PlayerData mainPlayer = GameDataMgr.Instance.PlayerDataAttr;
        PlayerLevelAttr originalAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(mainPlayer.level);

        //success
        if (rewardInfo != null)
        {
            //show player info;
            SetInitPlayerInfo(originalAttr, mainPlayer);
            PB.SynPlayerAttr playerAttr = rewardInfo.playerAttr;
            List<PB.RewardItem> rewardItemList = rewardInfo.RewardItems;
            int count = rewardItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                PB.RewardItem item = rewardItemList[i];
                if (item.type == (int)PB.itemType.PLAYER_ATTR)
                {
                    if ((int)PB.changeType.CHANGE_COIN == int.Parse(item.itemId))
                    {
                        mPlayerGainGold.text = "+" + item.count.ToString();
                        //GameDataMgr.Instance.mainPlayer.coin += item.count;
                        //GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, playerAttr.coin);
                    }
                    else if ((int)PB.changeType.CHANGE_PLAYER_EXP == int.Parse(item.itemId))
                    {
                        mPlayerGainExp.text = "+" + item.count.ToString();
                    }
                }
            }
            if (playerAttr.level > 0)
            {
                PlayerLevelAttr curAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(playerAttr.level);
                mPlayerLvl.text = "LVL" + playerAttr.level.ToString();
                mPlayerProgress.SetLoopCount(playerAttr.level - mainPlayer.level);
                mPlayerProgress.SetCurrrentRatio(mainPlayer.exp / (float)originalAttr.exp);
                mPlayerProgress.SetTargetRatio(playerAttr.exp / (float)curAttr.exp);
                //TODO:Sysnc player info here?
                if (mainPlayer.level != playerAttr.level)
                {
                    mainPlayer.level = playerAttr.level;
                    GameEventMgr.Instance.FireEvent<int>(GameEventList.LevelChanged, mainPlayer.level);
                }
                mainPlayer.exp = playerAttr.exp;
            }


            //show monster info
            mMonsterExpList.gameObject.SetActive(true);
            List<PB.SynMonsterAttr> monsterInfoList = rewardInfo.monstersAttr;
            count = monsterInfoList.Count;
            if (count > 0)
            {
                Dictionary<long, UIMonsterIconExp> uiMonsterExpList = new Dictionary<long,UIMonsterIconExp>();
                //set monster info
                for (int i = 0; i < count; ++i)
                {
                    GameUnit originalMonster = mainPlayer.GetPetWithKey(monsterInfoList[i].monsterId);
                    if (originalMonster == null)
                    {
                        Logger.LogError("Score error, no this monster");
                        continue;
                    }

                    UIMonsterIconExp monsterIconExp = UIMonsterIconExp.Create();
                    monsterIconExp.transform.SetParent(mMonsterExpList.transform, false);
                    monsterIconExp.SetMonsterIconExpInfo(
                        originalMonster.pbUnit.id,
                        originalMonster.currentExp,
                        monsterInfoList[i].exp,
                        originalMonster.pbUnit.level,
                        monsterInfoList[i].level
                        );
                    mainPlayer.mainUnitList[i].unit.RefreshUnitLvl(monsterInfoList[i].level, monsterInfoList[i].exp);
                    uiMonsterExpList.Add(originalMonster.pbUnit.guid, monsterIconExp);
                }
                //set exp gain
                count = rewardItemList.Count;
                for (int i = 0; i < count; ++i)
                {
                    PB.RewardItem item = rewardItemList[i];
                    if (item.type == (int)PB.itemType.MONSTER_ATTR)
                    {
                        if ((int)PB.changeType.CHANGE_MONSTER_EXP == int.Parse(item.itemId))
                        {
                            UIMonsterIconExp curMonsterExp = null;
                            if (uiMonsterExpList.TryGetValue(item.id, out curMonsterExp) == true)
                            {
                                curMonsterExp.SetExpGain(item.count);
                            }
                        }
                    }
                }
            }
            else
            {
                count = mainPlayer.mainUnitList.Count;
                for (int i = 0; i < count; ++i)
                {
                    UIMonsterIconExp monsterIconExp = UIMonsterIconExp.Create();
                    monsterIconExp.transform.SetParent(mMonsterExpList.transform, false);
                    GameUnit curUnit = mainPlayer.mainUnitList[i].unit;
                    monsterIconExp.SetMonsterIconExpInfo(
                        curUnit.pbUnit.id,
                        curUnit.currentExp,
                        curUnit.currentExp,
                        curUnit.pbUnit.level,
                        curUnit.pbUnit.level,
                        0
                        );
                }
            }

            //show item drop info
            count = rewardItemList.Count;
            mItemGainList.gameObject.SetActive(true);
            for (int i = 0; i < count; ++i)
            {
                PB.RewardItem item = rewardItemList[i];
                if (item.type == (int)PB.itemType.ITEM)
                {
                    ItemIcon icon = ItemIcon.CreateItemIcon(ItemData.valueof(item.itemId, item.count));
                    icon.transform.SetParent(mItemGainList.transform);
                    icon.transform.localScale = Vector3.one;
                }
                else if (item.type == (int)PB.itemType.EQUIP)
                {
                    EquipData equipData = new EquipData()
                    {
                        id = item.id,
                        equipId = item.itemId,
                        stage = item.stage,
                        level = item.level
                    };
                    ItemIcon icon = ItemIcon.CreateItemIcon(equipData);
                    icon.transform.SetParent(mItemGainList.transform);
                    icon.transform.localScale = Vector3.one;
                }
            }
        }
        //failed
        else
        {
            SetInitPlayerInfo(originalAttr, mainPlayer);
        }

        //show button
        mRetryBtn.gameObject.SetActive(true);
        mNextLevelBtn.gameObject.SetActive(mIsSuccess);
        mConfirmBtn.gameObject.SetActive(true);
    }
    //---------------------------------------------------------------------------------------------
    private void SetInitPlayerInfo(PlayerLevelAttr playerAttr, PlayerData playerData)
    {
        //TODO: duplicate code
        mPlayerInfoRoot.SetActive(true);
        mPlayerGainGold.text = "+0";
        mPlayerGainExp.text = "+0";
        mPlayerLvl.text = "LVL" + playerAttr.level.ToString();
        float curExpRatio = playerData.exp / (float)playerAttr.exp;
        mPlayerProgress.SetCurrrentRatio(curExpRatio);
        mPlayerProgress.SetTargetRatio(curExpRatio);
    }
    //---------------------------------------------------------------------------------------------
}
