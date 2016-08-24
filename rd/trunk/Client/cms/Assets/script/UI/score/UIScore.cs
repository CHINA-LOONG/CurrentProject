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
    public GameObject mPlayerLvlUp;
    public GameObject mScoreBack;
    
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
    private PB.HSRewardInfo mInstanceSettleResult;
    private Dictionary<long, UIMonsterIconExp> mUIMonsterExpList = new Dictionary<long, UIMonsterIconExp>();
    private bool mSkipEnable = false;
    private Tweener mBattleTitleTw;
    private int mOriginalPlayerLvl;
    private int mCurrentPlayerLvl;
    private int mCurrentHuoli;
    private int mHuoliBeginTime;
    private bool mCheckCoin;
    private int mStarCount;
    private float mWaitStarTime;
    //---------------------------------------------------------------------------------------------
    public static void AddResourceRequest()
    {
        ResourceMgr resMgr = ResourceMgr.Instance;
        resMgr.AddAssetRequest(new AssetRequest("monsterExpIcon"));
        resMgr.AddAssetRequest(new AssetRequest("endBattle"));
        resMgr.AddAssetRequest(new AssetRequest("star_ui"));
        resMgr.AddAssetRequest(new AssetRequest(ViewName));
        //TODO: always keep this
        ResourceMgr.Instance.AddAssetRequest(new AssetRequest("monsterIcon"));
    }
    //---------------------------------------------------------------------------------------------
    void Start()
    {
        EventTriggerListener.Get(mRetryBtn.gameObject).onClick = OnRetry;
        EventTriggerListener.Get(mNextLevelBtn.gameObject).onClick = OnNextLevel;
        EventTriggerListener.Get(mConfirmBtn.gameObject).onClick = OnConfirm;
        EventTriggerListener.Get(mScoreBack.gameObject).onClick = SkipScoreAni;
        mWaitStarTime = 0.0f;
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
        if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Normal)
        {
            BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_Retry);
        }
        else if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Tower)
        {
            if (GameDataMgr.Instance.mTowerRefreshed == false)
            {
                if (mIsSuccess == true)
                {
                    BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_Next);
                }
                else
                {
                    BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_Retry);
                }
            }
            else
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("towerBoss_record_004"), (int)PB.ImType.PROMPT);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnNextLevel(GameObject go)
    {
        if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Normal)
        {
            BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_Next);
        }
        else if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Hole)
        {
            BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_OK);
        }
        else if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Guild)
        {
            BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_OK);
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnConfirm(GameObject go)
    {
        BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_OK);
    }
    //---------------------------------------------------------------------------------------------
    void SkipScoreAni(GameObject go)
    {
        if (mSkipEnable == true)
        {
            mSkipEnable = false;
            if (mBattleTitleTw != null)
            {
                mBattleTitleTw.Complete();
            }
            mPlayerProgress.SkipAnimation();

            var itor = mUIMonsterExpList.GetEnumerator();
            while (itor.MoveNext())
            {
                itor.Current.Value.SkipAnimation();
            }

            if (mIsSuccess == true)
            {
                EndBattleUI endBattleComponent = mEndBattleUI.GetComponent<EndBattleUI>();
                endBattleComponent.SkipShowStarAni();
                mWaitStarTime = 0.0f;
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        gameObject.SetActive(false);
        mOriginalPlayerLvl = mCurrentPlayerLvl = 0;
    }
    //---------------------------------------------------------------------------------------------
    public override void Init()
    {
        base.Init();

        mSkipEnable = false;
        mUIMonsterExpList.Clear();
        mPlayerInfoRoot.SetActive(false);
        mMonsterExpList.gameObject.SetActive(false);
        mItemGainList.gameObject.SetActive(false);
        mRetryBtn.gameObject.SetActive(false);
        mNextLevelBtn.gameObject.SetActive(false);
        mConfirmBtn.gameObject.SetActive(false);
        mBackground.SetActive(false);
        mLineMonsterItem.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    public override void Clean()
    {
        base.Clean();
        mUIMonsterExpList.Clear();
        mBattleTitleTw = null;
        ResourceMgr.Instance.DestroyAsset(mEndBattleUI);
    }
    //---------------------------------------------------------------------------------------------
    public void ShowScoreUI(bool success, int starCount)
    {
        mStarCount = starCount;
        if (UIIm.Instance != null)
        {
            UIIm.Instance.SetLevelVisible(false);
            //UIIm.Instance.HideChat();
        }
        mIsSuccess = success;
        gameObject.SetActive(true);
        if (mInstanceSettleResult != null)
        {
            int count = mInstanceSettleResult.RewardItems.Count;
            for (int i = 0; i < count; ++i)
            {
                PB.RewardItem item = mInstanceSettleResult.RewardItems[i];
                if (item.type == (int)PB.itemType.MONSTER)
                {
                    UnitData unitRowData = StaticDataMgr.Instance.GetUnitRowData(item.itemId);
                    PB.HSMonster monster = item.monster;
                    if (unitRowData != null)
                    {
                        //add monster icon
                        MonsterIcon icon = MonsterIcon.CreateIcon();
                        icon.transform.SetParent(mItemGainList.transform, false);
                        icon.SetMonsterStaticId(monster.cfgId);
                        icon.SetLevel(monster.level);
                        icon.SetStage(monster.stage);
                        if (unitRowData.rarity >= 3)
                        {
                            AddGainMonster(monster.cfgId, monster.level, monster.stage);
                            return;
                        }
                    }

                    break;
                }
            }
        }

        ShowEndBattleUI();
    }
    //---------------------------------------------------------------------------------------------
    private void AddGainMonster(string monsterID, int level, int stage)
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
        SetScoreInternal();
        mEndBattleUI = ResourceMgr.Instance.LoadAsset("endBattle");
        mEndBattleUI.transform.SetParent(transform, false);
        mEndBattleUI.transform.localPosition = mCenterPos.localPosition;
        EndBattleUI endBattleUI = mEndBattleUI.GetComponent<EndBattleUI>();
        endBattleUI.SetSuccess(mIsSuccess);
        if (mIsSuccess == true)
        {
            endBattleUI.SetStarCount(mStarCount);
        }

        mBattleTitleTw = mEndBattleUI.transform.DOLocalMove(mTopPos.localPosition, BattleConst.scoreTitleUpTime);
        mBattleTitleTw.OnComplete(ShowStar);
        mBattleTitleTw.SetDelay(BattleConst.scoreTitleStayTime);
        
        mSkipEnable = true;
    }
    //---------------------------------------------------------------------------------------------
    public void SetScoreInfo(PB.HSRewardInfo scoreInfo)
    {
        mInstanceSettleResult = scoreInfo;
    }
    //---------------------------------------------------------------------------------------------
    private void ShowStar()
    {
        //Logger.LogFormat("get star {0}", mInstanceSettleResult.starCount);
        EndBattleUI endBattleUI = mEndBattleUI.GetComponent<EndBattleUI>();
        endBattleUI.SetStarVisiblebool(mIsSuccess);
        if (mIsSuccess == true)
        {
            endBattleUI.ShowStar();
        }

        mWaitStarTime = mStarCount * BattleConst.scoreStarInterval;
        StartCoroutine(ShowScoreInfo());
    }
    //---------------------------------------------------------------------------------------------
    private void SetScoreInternal()
    {
        mCheckCoin = false;
        PlayerData mainPlayer = GameDataMgr.Instance.PlayerDataAttr;
        PlayerLevelAttr originalAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(mainPlayer.LevelAttr);

        //success
        if (mInstanceSettleResult != null)
        {
            //show player info;
            SetInitPlayerInfo(originalAttr, mainPlayer);
            PB.SynPlayerAttr playerAttr = mInstanceSettleResult.playerAttr;
            List<PB.RewardItem> rewardItemList = mInstanceSettleResult.RewardItems;
            int count = rewardItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                PB.RewardItem item = rewardItemList[i];
                if (item.type == (int)PB.itemType.PLAYER_ATTR)
                {
                    if ((int)PB.changeType.CHANGE_COIN == int.Parse(item.itemId))
                    {
                        mCheckCoin = true;
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
            if (playerAttr != null && playerAttr.level > 0)
            {
                PlayerLevelAttr curAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(playerAttr.level);
                mPlayerProgress.SetLoopCount(playerAttr.level - mainPlayer.LevelAttr);
                mPlayerProgress.SetCurrrentRatio(mainPlayer.ExpAttr / (float)originalAttr.exp);
                if (playerAttr.level >= GameConfig.MaxPlayerLevel)
                {
                    mPlayerLvl.text = "MAX LVL";
                    mPlayerProgress.SetTargetRatio(0.0f);
                    mPlayerGainExp.text = "+0";
                }
                else
                {
                    mPlayerLvl.text = "LVL " + playerAttr.level.ToString();
                    mPlayerProgress.SetTargetRatio(playerAttr.exp / (float)curAttr.exp);
                }
                mPlayerLvlUp.SetActive(mainPlayer.LevelAttr != playerAttr.level);
                mOriginalPlayerLvl = mainPlayer.LevelAttr;
                mCurrentPlayerLvl = playerAttr.level;
                mCurrentHuoli = BattleController.Instance.mHuoliBeforeScore;
                mHuoliBeginTime = playerAttr.fatigueBeginTime;
                //TODO:Sysnc player info here?
                if (mainPlayer.LevelAttr != playerAttr.level)
                {
                    mainPlayer.LevelAttr = playerAttr.level;
                    //GameEventMgr.Instance.FireEvent<int>(GameEventList.LevelChanged, mainPlayer.LevelAttr);
                }
                mainPlayer.ExpAttr = playerAttr.exp;
            }

            //show monster info
            List<PB.SynMonsterAttr> monsterInfoList = mInstanceSettleResult.monstersAttr;
            count = monsterInfoList.Count;
            if (count > 0)
            {
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
                        monsterInfoList[i].level,
                        originalMonster.pbUnit.stage
                        );
                    mainPlayer.mainUnitList[i].unit.RefreshUnitLvl(monsterInfoList[i].level, monsterInfoList[i].exp);
                    if (UIUtil.CheckPetIsMaxLevel(monsterInfoList[i].level) == 0)
                    {
                        mUIMonsterExpList.Add(originalMonster.pbUnit.guid, monsterIconExp);
                    }
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
                            if (mUIMonsterExpList.TryGetValue(item.id, out curMonsterExp) == true)
                            {
                                curMonsterExp.SetExpGain("+" + item.count.ToString());
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
                        curUnit.pbUnit.stage,
                        0
                        );
                }
            }

            //show item drop info
            count = rewardItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                PB.RewardItem item = rewardItemList[i];
                if (item.type == (int)PB.itemType.ITEM)
                {
                    ItemIcon icon = ItemIcon.CreateItemIcon(
                        ItemData.valueof(item.itemId, item.count),
                        true,
                        false
                        );
                    icon.transform.SetParent(mItemGainList.transform);
                    icon.transform.localScale = Vector3.one;
                    icon.ShowTips = true;
                }
                else if (item.type == (int)PB.itemType.EQUIP)
                {
                    EquipData equipData = EquipData.valueof(item.id, item.itemId, item.stage, item.level, BattleConst.invalidMonsterID, null);
                    ItemIcon icon = ItemIcon.CreateItemIcon(equipData,true,false);
                    icon.transform.SetParent(mItemGainList.transform);
                    icon.transform.localScale = Vector3.one;
                    //icon.ShowTips = true;
                }
            }
        }
        //failed
        else
        {
            SetInitPlayerInfo(originalAttr, mainPlayer);
        }
    }
    //---------------------------------------------------------------------------------------------
    private IEnumerator ShowScoreInfo()
    {
        while (mWaitStarTime > 0.0f)
        {
            mWaitStarTime -= Time.unscaledDeltaTime;
            yield return null;
        }
        mRetryBtn.gameObject.SetActive(false);
        mNextLevelBtn.gameObject.SetActive(false);
        mConfirmBtn.gameObject.SetActive(false);
        //TODO: use enum
        //0:normal 1:hole 2:tower
        switch (GameDataMgr.Instance.curInstanceType)
        {
            case (int)InstanceType.Normal:
                mRetryText.text = StaticDataMgr.Instance.GetTextByID("ui_battle_again");
                mNextLevelText.text = StaticDataMgr.Instance.GetTextByID("ui_battle_next");
                mConfirmText.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
                break;
            case (int)InstanceType.Hole:
                mNextLevelText.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
                break;
            case (int)InstanceType.Tower:
                break;
            case (int)InstanceType.Guild:
                mNextLevelText.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
                break;
        }

        mBackground.SetActive(true);
        mLineMonsterItem.SetActive(true);
        //show player info
        mPlayerInfoRoot.SetActive(true);
        //show monster info
        mMonsterExpList.gameObject.SetActive(true);
        //show item drop info
        mItemGainList.gameObject.SetActive(true);
        //show button
        //normal instance
        if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Normal)
        {
            if (mIsSuccess == true)
            {
                EnterInstanceParam curInstance = BattleController.Instance.GetCurrentInstance();
                if (curInstance != null)
                {
                    InstanceEntryRuntimeData curData = InstanceMapService.Instance.GetNextRuntimeInstance(curInstance.instanceData.instanceId);
                    mNextLevelBtn.gameObject.SetActive(curData != null);
                }
            }
            else
            {
                mNextLevelBtn.gameObject.SetActive(false);
            }
            mRetryBtn.gameObject.SetActive(true);
            mConfirmBtn.gameObject.SetActive(true);
        }
        //hole instance
        else if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Hole)
        {
            mNextLevelBtn.gameObject.SetActive(true);
        }
        //tower instance
        else if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Tower)
        {
            if (mIsSuccess == true)
            {
                string nextTowerFloor;
                int nextFloor;
                GameDataMgr.Instance.GetNextTowerFloor(out nextTowerFloor, out nextFloor);
                if (nextTowerFloor != null)
                {
                    mRetryText.text = StaticDataMgr.Instance.GetTextByID("ui_battle_next");
                    mRetryBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                mRetryText.text = StaticDataMgr.Instance.GetTextByID("ui_battle_again");
                mRetryBtn.gameObject.SetActive(true);
            }

            mConfirmBtn.gameObject.SetActive(true);
        }
        //guild instance
        else if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Guild)
        {
            mNextLevelBtn.gameObject.SetActive(true);
        }

        if (mCheckCoin)
        {
            GameDataMgr.Instance.CheckCoinFull();
        }

        if (mOriginalPlayerLvl != mCurrentPlayerLvl)
        {
            PlayerData mainPlayer = GameDataMgr.Instance.PlayerDataAttr;
            LevelUp.OpenWith(mOriginalPlayerLvl, mCurrentPlayerLvl, mCurrentHuoli, mainPlayer.HuoliAttr);
        }
    }
    //---------------------------------------------------------------------------------------------
    private void SetInitPlayerInfo(PlayerLevelAttr playerAttr, PlayerData playerData)
    {
        //TODO: duplicate code
        //mPlayerInfoRoot.SetActive(true);
        mPlayerGainGold.text = "+0";
        mPlayerGainExp.text = "+0";
        mPlayerProgress.SetLoopCount(0);
        float curExpRatio = playerData.ExpAttr / (float)playerAttr.exp;
        mPlayerProgress.SetCurrrentRatio(curExpRatio);
        if (playerAttr.level >= GameConfig.MaxPlayerLevel)
        {
            mPlayerLvl.text = "MAX LVL";
            mPlayerProgress.SetTargetRatio(0.0f);
        }
        else
        {
            mPlayerLvl.text = "LVL " + playerAttr.level.ToString();
            mPlayerProgress.SetTargetRatio(curExpRatio);
        }
        mPlayerLvlUp.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
}
