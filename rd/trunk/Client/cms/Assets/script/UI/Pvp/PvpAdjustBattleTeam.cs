using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(BattlePositionAdjust))]
public class PvpAdjustBattleTeam : UIBase
{
    public static string ViewName = "PvpAdjustBattleTeam";

    public Text enemyNameText;
    public Text enemyLableText;
    public Text enemyBpLabelText;
    public Text enemBpValueText;
    public Text myPetLabelText;
    public Text myBpLabelText;
    public Text myBpValueText;

    public Text dropTitleLableText;
    public Text victoryDropLabelText;
    public Text loseDropLabelText;
    public RectTransform victoryDropParent;
    public RectTransform loseDropParent;

    public Text refreshCostCoinText;
    public Button refreshButton;
    public Button fightButton;
    public Button closeButton;

    private List<string> pvpFightTeam = new List<string>();
    private BattlePositionAdjust battlePositionAdjust = null;
    private BattlePositionAdjust BattlePositionAdjustAttr
    {
        get
        {
            if(null == battlePositionAdjust)
            {
                battlePositionAdjust = GetComponent<BattlePositionAdjust>();
            }
            return battlePositionAdjust;
        }
    }

    public static void OpenWith()
    {
        PvpAdjustBattleTeam adjustUi = UIMgr.Instance.OpenUI_(ViewName) as PvpAdjustBattleTeam;
        adjustUi.InitWith();
    }
    public void InitWith()
    {
        List<string> pvpTeamList = BattleTeamManager.GetTeamWithKey(BattleTeamManager.TeamList.Pvp);
        pvpFightTeam.Clear();
        pvpFightTeam.AddRange(pvpTeamList);
        OnFightTeamChanged();
        BattlePositionAdjustAttr.InitWithDefaultPosition(ref pvpFightTeam, BattlePositionAdjust.AdjustType.PvpBattle, OnFightTeamChanged);
    }

	void Start ()
    {
        enemyLableText.text = StaticDataMgr.Instance.GetTextByID("instance_difangzhenrong");
        enemyBpLabelText.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");
        myPetLabelText.text = StaticDataMgr.Instance.GetTextByID("instance_wofangzhenrong");
        myBpLabelText.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");

        dropTitleLableText.text = StaticDataMgr.Instance.GetTextByID("pvp_rewardyulan");
        victoryDropLabelText.text = StaticDataMgr.Instance.GetTextByID("pvp_winreward");
        loseDropLabelText.text = StaticDataMgr.Instance.GetTextByID("pvp_losereward");

        UIUtil.SetButtonTitle(refreshButton.transform, StaticDataMgr.Instance.GetTextByID("shop_refresh"));
        UIUtil.SetButtonTitle(fightButton.transform, StaticDataMgr.Instance.GetTextByID("pvp_battle"));

        fightButton.onClick.AddListener(OnFightButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
	}

    void OnFightTeamChanged()
    {
        myBpValueText.text = GameDataMgr.Instance.PvpDataMgrAttr.GetBpWithGuidList(pvpFightTeam).ToString();
    }

    void OnFightButtonClick()
    {

    }
    void OnCloseButtonClick()
    {
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("pvp_exit"),
            StaticDataMgr.Instance.GetTextByID("pvp_exitsure"), OnConformExit);
    }
    void OnConformExit(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            UIMgr.Instance.CloseUI_(this);
        }
    }
	
}
