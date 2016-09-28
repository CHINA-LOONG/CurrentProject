using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    }
	
    void OnFightButtonClick()
    {

    }
    void OnAdjustButtonClick()
    {

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

    }
    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
}
