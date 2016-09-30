using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BattlePositionAdjust))]
public class PvpDefense : UIBase
{
    public static string ViewName = "PvpDefense";

    public Text titleText;
    public Text bpLabelText;
    public Text bpValueText;
    public Text myPetLabelText;

    public Button saveButton;
    public Button closeButton;

    List<string> defenseTeamList = new List<string>();
    BattlePositionAdjust battlePositionAdjust = null;
    BattlePositionAdjust BattlePositionAdjustAttr
    {
        get
        {
            if (battlePositionAdjust == null)
            {
                battlePositionAdjust = GetComponent<BattlePositionAdjust>();
            }
            return battlePositionAdjust;
        }
    }
    private PvpDataMgr PvpDataMgrAttr
    {
        get
        {
            return GameDataMgr.Instance.PvpDataMgrAttr;
        }
    }
    public static void OpenWith(List<string> teamList)
    {
        if (null == teamList || teamList.Count != 5)
        {
            Logger.LogError("battlePositon must have 5 positon");
            return;
        }
        PvpDefense defenseUi = UIMgr.Instance.OpenUI_(ViewName) as PvpDefense;
        defenseUi.InitWith(teamList);
    }

    public void InitWith(List<string> teamList)
    {
        defenseTeamList.Clear();
        defenseTeamList.AddRange(teamList);
        BattlePositionAdjustAttr.InitWithDefaultPosition(ref defenseTeamList, BattlePositionAdjust.AdjustType.PvpDefensse, OnPositionChanged);
        OnPositionChanged();
    }

    void Start()
    {
        saveButton.onClick.AddListener(OnSaveButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        titleText.text = StaticDataMgr.Instance.GetTextByID("pvp_defense");
        bpLabelText.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");
        myPetLabelText.text = StaticDataMgr.Instance.GetTextByID("instance_wofangzhenrong");
    }
    void OnPositionChanged()
    {
        bpValueText.text = PvpDataMgrAttr.GetBpWithGuidList(defenseTeamList).ToString();
    }
    void OnSaveButtonClick()
    {
        int teamNumber = GetTeamNumbers();
        if (0 == teamNumber)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("pvp_record_003"), (int)PB.ImType.PROMPT);
            return;
        }
        if( 5 > teamNumber)
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("pvp_buzufive"), OnConformSaveSelect);
        }
        else
        {
            RequestSave();
        }
    }
    void OnConformSaveSelect(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            RequestSave();
        }
    }
    int GetTeamNumbers()
    {
        int count = 0;
        for(int i =0;i<defenseTeamList.Count;++i)
        {
            if(!string.IsNullOrEmpty(defenseTeamList[i]))
            {
                ++count;
            }
        }
        return count;
    }
    void RequestSave()
    {
        PvpDataMgrAttr.RequestSaveDefensePosition(defenseTeamList, OnSaveDefensePositionFinished);
    }

    void OnSaveDefensePositionFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            PvpErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PvpDataMgrAttr.defenseTeamList.Clear();
        PvpDataMgrAttr.defenseTeamList.AddRange(defenseTeamList);
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("pvp_record_001"), (int)PB.ImType.PROMPT);
    }
    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

   
}
