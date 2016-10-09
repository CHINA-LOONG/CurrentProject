using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpMain : UIBase
{
    public static string ViewName = "PvpMain";

    public Text titleText;
    public Text seasonCountDownText;
    public Button closeButton;

    public Text defenseInformationText;
    public RectTransform[] defensePositionBgArray;
    public Button adjustButton;
    public BuildButton[] functionEntryButtons;

    public Text duanInfoText;
    public Text rankText;
    public Text competetivePointText;
    public Text honorPointText;
    public Text pvpTimesText;
    public Text pvpTimeCountDownText;
    public Button fightButton;

    public GameObject[] stageImageObject;
    public GameObject[] stageEffectObject;

    private Animator animator = null;

    private List<MonsterIcon> defenseMonsterIcon = new List<MonsterIcon>();

    bool pvpTimesIsFull = true;
    int nextRestoreTime = -1;
    int leftTime = 0;

    private PvpDataMgr  PvpDataMgrAttr
    {
        get
        {
            return GameDataMgr.Instance.PvpDataMgrAttr;
        }
    }
    public static PvpMain Open()
    {
        PvpMain pvpMain = (PvpMain)UIMgr.Instance.OpenUI_(ViewName);
        return pvpMain;
    }

    // Use this for initialization
    void Start ()
    {
        fightButton.onClick.AddListener(OnFightButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        adjustButton.onClick.AddListener(OnAdjustButtonClick);

        EventTriggerListener.Get(functionEntryButtons[0].gameObject).onClick = OnRankButtonClick;
        EventTriggerListener.Get(functionEntryButtons[1].gameObject).onClick = OnRewardButtonClick;
        EventTriggerListener.Get(functionEntryButtons[2].gameObject).onClick = OnDefenseRecordButtonClick;
        EventTriggerListener.Get(functionEntryButtons[3].gameObject).onClick = OnRuleButtonClick;
        EventTriggerListener.Get(functionEntryButtons[4].gameObject).onClick = OnPvpShopButtonClick;

        titleText.text = StaticDataMgr.Instance.GetTextByID("pvp_title");
        defenseInformationText.text = StaticDataMgr.Instance.GetTextByID("pvp_defense");

        UIUtil.SetButtonTitle(adjustButton.transform, StaticDataMgr.Instance.GetTextByID("pvp_change"));
        UIUtil.SetButtonTitle(fightButton.transform, StaticDataMgr.Instance.GetTextByID("pvp_pipei"));

        functionEntryButtons[0].nameText.text = StaticDataMgr.Instance.GetTextByID("pvp_ranking");
        functionEntryButtons[1].nameText.text = StaticDataMgr.Instance.GetTextByID("pvp_battlereward");
        functionEntryButtons[2].nameText.text = StaticDataMgr.Instance.GetTextByID("pvp_record");
        functionEntryButtons[3].nameText.text = StaticDataMgr.Instance.GetTextByID("pvp_rules");
        functionEntryButtons[4].nameText.text = StaticDataMgr.Instance.GetTextByID("pvp_shop");

        animator = GetComponent<Animator>();
    }
    public override void Init()
    {
        PvpDataMgrAttr.RequestPvpInfo(OnRequestPvpInfoFinished);
        RefreshDefensePosition();
        RefreshPvpTimes();
    }
    public override void RefreshOnPreviousUIHide()
    {
        PvpDataMgrAttr.RequestPvpInfo(OnRequestPvpInfoFinished);
        RefreshDefensePosition();
        RefreshPvpTimes();
    }
    void RefreshPvpTimes()
    {
        pvpTimesText.text = string.Format(StaticDataMgr.Instance.GetTextByID("pvp_pvpchoise"), PvpDataMgrAttr.selfPvpTiems,GameConfig.Instance.pvpFightTimesMax);

        pvpTimesIsFull = PvpDataMgrAttr.selfPvpTiems >= GameConfig.Instance.pvpFightTimesMax;
        nextRestoreTime = PvpDataMgrAttr.selfPvpTimesBeginTime + GameConfig.Instance.pvpRestorTimeNeedSecond;
        if (pvpTimesIsFull)
        {
            pvpTimeCountDownText.text = StaticDataMgr.Instance.GetTextByID("pet_detail_skill_max_point");
        }
    }

    int refreshCount = 0;
    void Update()
    {
        refreshCount++;
        if (refreshCount < 20)
            return;
        refreshCount = 0;
        if (!pvpTimesIsFull && nextRestoreTime > 0)
        {
           leftTime = nextRestoreTime - GameTimeMgr.Instance.GetServerTimeStamp();
            if (leftTime >= 0)
            {
                pvpTimeCountDownText.text = string.Format(StaticDataMgr.Instance.GetTextByID("pvp_pvptime"), leftTime / 3600, (leftTime % 3600) / 60, leftTime % 60);

            }
            else
            {
                PvpDataMgrAttr.selfPvpTiems += 1;
                PvpDataMgrAttr.selfPvpTimesBeginTime = GameTimeMgr.Instance.GetServerTimeStamp();
                RefreshPvpTimes();
            }
        }
    }

    void OnRequestPvpInfoFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            PvpErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSPVPInfoRet msgRet = message.GetProtocolBody<PB.HSPVPInfoRet>();
        PvpDataMgrAttr.SelfPvpPointAttr = msgRet.pvpPoint;
        PvpDataMgrAttr.selfPvpRank = msgRet.pvpRank;

        duanInfoText.text = PvpDataMgrAttr.GetStageNameWithId(PvpDataMgrAttr.selfPvpStage);
        if(PvpDataMgrAttr.selfPvpRank > 0)
        {
            rankText.text = string.Format(StaticDataMgr.Instance.GetTextByID("pvp_rank"), PvpDataMgrAttr.selfPvpRank);
        }
       else
        {
            rankText.text = StaticDataMgr.Instance.GetTextByID("pvp_notin");
        }
        competetivePointText.text = string.Format(StaticDataMgr.Instance.GetTextByID("pvp_points"), PvpDataMgrAttr.SelfPvpPointAttr);

        honorPointText.text = string.Format(StaticDataMgr.Instance.GetTextByID("pvp_honorpoint"), GameDataMgr.Instance.PlayerDataAttr.HonorAtr);

        RefreshSeasonCountDown(msgRet.monthRewardTimeLeft);

        for (int i = 0; i < stageImageObject.Length; ++i)
        {
            stageImageObject[i].SetActive((i + 1) == PvpDataMgrAttr.selfPvpStage);
        }
        for (int i = 0; i < stageEffectObject.Length; ++i)
        {
            stageEffectObject[i].SetActive(i == (PvpDataMgrAttr.selfPvpStage - 1) / 3);
        }
    }

    void RefreshDefensePosition()
    {
        if (defenseMonsterIcon.Count == 0)
        {
            for (int i = 0; i < defensePositionBgArray.Length; ++i)
            {
                MonsterIcon subIcon = MonsterIcon.CreateIcon();
                defenseMonsterIcon.Add(subIcon);

                subIcon.transform.SetParent(defensePositionBgArray[i]);
                RectTransform iconRt = subIcon.transform as RectTransform;
                float scale = defensePositionBgArray[i].rect.width / iconRt.rect.width;
                subIcon.transform.localScale = new Vector3(scale, scale, scale);
                subIcon.transform.localPosition = Vector3.zero;

                subIcon.gameObject.SetActive(false);
            }
        }

        string subGuid = null;
        MonsterIcon subIconItem = null;
        for (int i = 0; i < defenseMonsterIcon.Count; ++i)
        {
            subIconItem = defenseMonsterIcon[i];
            if(i < PvpDataMgrAttr.defenseTeamList.Count )
            {
                subGuid = PvpDataMgrAttr.defenseTeamList[i];
                if (!string.IsNullOrEmpty(subGuid))
                {
                    GameUnit unit = null;
                    if (!string.IsNullOrEmpty(subGuid))
                    {
                        unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(int.Parse(subGuid));
                        if(null != unit)
                        {
                            subIconItem.gameObject.SetActive(true);
                            subIconItem.SetId(subGuid);
                            subIconItem.SetMonsterStaticId(unit.pbUnit.id);
                            subIconItem.SetLevel(unit.pbUnit.level);
                            subIconItem.SetStage(unit.pbUnit.stage);
                            EventTriggerListener.Get(subIconItem.iconButton.gameObject).onClick = OnDefenseIconClick;
                            continue;
                        }
                    }
                }
            }
            subIconItem.gameObject.SetActive(false);
        }
    }

    void OnDefenseIconClick(GameObject go)
    {
        MonsterIcon itemIcon = go.GetComponentInParent<MonsterIcon>();
        int guid = int.Parse(itemIcon.Id);
        GameUnit unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(guid);
        if(null != unit)
        {
            UIMonsterInfo.Open(guid, unit.pbUnit.id, unit.pbUnit.level, unit.pbUnit.stage, unit.mBp);
        }
    }

    void RefreshSeasonCountDown(int leftsecond)
    {
        string leftTimeStr = null;
        int secondPerDay = 3600 * 24;
        int day = leftsecond / secondPerDay;
        if (day > 0)
        {
            if(leftsecond % secondPerDay > 500)
            {
                day++;
            }
            leftTimeStr = string.Format(StaticDataMgr.Instance.GetTextByID("pvp_daojishijiesuan1"), day);
        }
        else
        {
            int hour = leftsecond / 3600;
            if(hour > 0)
            {
                if(leftsecond %3600 > 60)
                {
                    hour++;
                }
                leftTimeStr = string.Format(StaticDataMgr.Instance.GetTextByID("pvp_daojishijiesuan2"), hour);
            }
            else
            {
                leftTimeStr = StaticDataMgr.Instance.GetTextByID("pvp_daojishijiesuan3");
            }
        }
        seasonCountDownText.text = string.Format("{0}{1}", StaticDataMgr.Instance.GetTextByID("pvp_daojishijiesuan"), leftTimeStr);
    }
    void OnAdjustButtonClick()
    {
        List<string> teamlist = new List<string>();
        teamlist.AddRange(PvpDataMgrAttr.defenseTeamList);
        PvpDefense.OpenWith(teamlist);
    }
    void OnRankButtonClick(GameObject go)
    {
        UIRank.OpenWith(RankType.Rank_Pvp);
    }
    void OnRewardButtonClick(GameObject go)
    {
        PvpReward.Open();
    }
    void OnDefenseRecordButtonClick(GameObject go)
    {
        PvpDefensiveRecord.Open();
    }
    void OnRuleButtonClick(GameObject go)
    {
        PvpRule.Open();
    }
    void OnPvpShopButtonClick(GameObject go)
    {
        UIShop  uiShop = UIMgr.Instance.OpenUI_(UIShop.ViewName) as UIShop;
        uiShop.RefreshShopData((int)PB.shopType.PVPSHOP);
    }
    void OnCloseButtonClick()
    {
        if(animator != null)
        {
            animator.SetTrigger("outTriger");
        }
        else
        {
            UIMgr.Instance.CloseUI_(this);
        }
    }

    void OnOutAniEnd()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnFightButtonClick()
    {
        if(!PvpDataMgrAttr.IsSelfHaveDefensePositon())
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("pvp_fangyumust"), StaticDataMgr.Instance.GetTextByID("pvp_fangyumust1"));
            return;
        }
        if(PvpDataMgrAttr.selfPvpTiems < 1)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("pvp_record_002"), (int)PB.ImType.PROMPT);
            return;
        }
        PvpDataMgrAttr.RequestSearchPvpOpponent(OnSearchOpponentFinished);
    }

    void OnSearchOpponentFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            PvpErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PvpDataMgrAttr.selfPvpTiems -= 1;
        PB.HSPVPMatchTargetRet msgRet = message.GetProtocolBody<PB.HSPVPMatchTargetRet>();
        PvpAdjustBattleTeam.OpenWith(msgRet);
    }
}
