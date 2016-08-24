using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyContentInfomation : SociatyContentBase
{
    public Text lvlText;
    public Text nameText;
    public Text captionLabel;
    public Text captionValue;
    public Text idLabel;
    public Text idValue;

    public Text memberLabel;
    public Text memberValue;

    public Text personalContributionLabel;
    public Text personalContributionValue;
    public Text sociatyContributionLabel;
    public Text sociatyContributionValue;
    public Text todayContributionText;
    public ContributionBox[] contributionBoxArray;
    public UIProgressbar progressBar;
    public Button recruitButton;

    public Text gonggaoLabel;
    public Text gonggaoValue;
    public Button modifyNotifyButton;

    public Text huodongLabel;
    public Button qifuButton;
    public Button taskButton;
    public Button shopButton;
    public Button jidiButton;
    public Button bossButton;

    private string newNotify;
    private PB.AllianceInfo allianceInfo;
    private PB.AllianceMember selfInfo;

    public static SociatyContentInfomation Instance = null;
	// Use this for initialization
	void Start ()
    {
        Instance = this;
        recruitButton.onClick.AddListener(OnRecruitButtonClick);
        modifyNotifyButton.onClick.AddListener(OnModifyNotifyButtonClick);
        qifuButton.onClick.AddListener(OnQifubuttonClick);
        taskButton.onClick.AddListener(OnTaskButtonClick);
        shopButton.onClick.AddListener(OnShopButtonClick);
        jidiButton.onClick.AddListener(OnJiDiButtonClick);
        bossButton.onClick.AddListener(OnBossButtonClick);

        captionLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_chairmanname");
        idLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_id1");
        memberLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_peoplenum1");
        personalContributionLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_onecontribution");
        sociatyContributionLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_comcontribution");
        todayContributionText.text = StaticDataMgr.Instance.GetTextByID("sociaty_comcontribution");

        contributionBoxArray[0].SetRewordValue(0,GameConfig.Instance.contributionRewordLevel1);
        contributionBoxArray[1].SetRewordValue(1,GameConfig.Instance.contributionRewordLevel2);
        contributionBoxArray[2].SetRewordValue(2,GameConfig.Instance.contributionRewordLevel3);
    }
	
	void    OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SYN_S.GetHashCode().ToString(), OnAllianceSynceFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnContributionReward);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SYN_S.GetHashCode().ToString(), OnAllianceSynceFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnContributionReward);
    }

    public override void RefreshUI()
    {
        modifyNotifyButton.gameObject.SetActive(false);
        RequestAllianceSync();
    }

    void RequestAllianceSync()
    {
        PB.HSAllianceSyn param = new PB.HSAllianceSyn();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_SYN_C.GetHashCode(), param);
    }

    void OnAllianceSynceFinish(ProtocolMessage msg)
    {
        UINetRequest.Close();

         allianceInfo = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData;
         selfInfo = GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData;

        lvlText.text = string.Format("LVL {0}", allianceInfo.level);
        nameText.text = allianceInfo.name;
        captionValue.text = allianceInfo.captaionName;
        idValue.text = allianceInfo.id.ToString();
        memberValue.text = string.Format("{0}/{1}", allianceInfo.currentPop, allianceInfo.maxPop);
        personalContributionValue.text = selfInfo.contribution.ToString();
        sociatyContributionValue.text = allianceInfo.contribution.ToString();

        float ratio = (float)allianceInfo.contributionToday / (float)GameConfig.Instance.maxContributionToday;
        progressBar.SetTargetRatio(ratio);
        progressBar.mProgressText.text = string.Format("{0}/{1}", allianceInfo.contributionToday, GameConfig.Instance.maxContributionToday);
        progressBar.SkipAnimation();

        gonggaoValue.text = allianceInfo.notice;
        modifyNotifyButton.gameObject.SetActive(selfInfo.postion == 2);

        RefreshContributionBox();
    }

    void OnContributionReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        if (reward == null || reward.hsCode != PB.code.ALLIANCE_CONTRI_REWARD_C.GetHashCode())
            return;

        List<PB.HSRewardInfo> listRewardForContribution = new List<PB.HSRewardInfo>();
        listRewardForContribution.Add(reward);

        OpenBaoxiangResult.OpenWith(listRewardForContribution);
    }

    public void RefreshContributionBox()
    {
        for (int i = 0; i < 3; ++i)
        {
            contributionBoxArray[i].SetHasReword(GameDataMgr.Instance.SociatyDataMgrAttr.hasReceivContributionReword[i]);
        }
    }

    void OnRecruitButtonClick()
    {
        MsgBox.InputConform.OpenWith(StaticDataMgr.Instance.GetTextByID("sociaty_shouren"), StaticDataMgr.Instance.GetTextByID("sociaty_shourenbianji"), false, UserConformRecuruitCallback);
    }

    void UserConformRecuruitCallback(MsgBox.PrompButtonClick buttonClick,string msg)
    {
        if (buttonClick == MsgBox.PrompButtonClick.OK)
        {
            if(!string.IsNullOrEmpty(msg))
            {
                if(Util.StringByteLength(msg) > 100)
                {
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_062"), (int)PB.ImType.PROMPT);
                    return;
                }
            }

            string sendMsg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_gonghuiyaoqing"), allianceInfo.name, msg);
            bool issend = UIIm.Instance.OnSendMsg(sendMsg, ImMessageType.Msg_Type_Recruit, GameDataMgr.Instance.SociatyDataMgrAttr.allianceID.ToString());
            if(issend)
            {
                MsgBox.InputConform.Close();
            }
        }
    }

    void OnModifyNotifyButtonClick()
    {
        MsgBox.InputConform.OpenWith(StaticDataMgr.Instance.GetTextByID("sociaty_revisenotice"), allianceInfo.notice, false, UserConformModifyCallback);
    }

    void UserConformModifyCallback(MsgBox.PrompButtonClick buttonClick, string newNotify)
    {
        if (buttonClick == MsgBox.PrompButtonClick.OK)
        {
            if (string.IsNullOrEmpty(newNotify))
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_007"), (int)PB.ImType.PROMPT);
                return ;
            }
            if (Util.StringByteLength(newNotify) > 300)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_061"), (int)PB.ImType.PROMPT);
                return ;
            }
            this.newNotify = newNotify;
            GameDataMgr.Instance.SociatyDataMgrAttr.RequestModifyNotify(newNotify, OnModifyNotifyFinished);
  
        }
    }

    void    OnModifyNotifyFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("modify   faild!");
            PB.HSErrorCode errorCode = msg.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        MsgBox.InputConform.Close();
        gonggaoValue.text = newNotify;
        allianceInfo.notice = newNotify;
    }

    void OnQifubuttonClick()
    {
        UIMgr.Instance.OpenUI_(SociatyPray.ViewName);
    }

    void OnTaskButtonClick()
    {
        UISociatyTask.Open(SociatyTaskContenType.MyTeam,null);
       // UIIm.Instance.ShowSystemHints("comming Later!", (int)PB.ImType.PROMPT);
    }

    void OnShopButtonClick()
    {
        FoundMgr.Instance.GoToUIShop(PB.shopType.ALLIANCESHOP);
    }

    void OnJiDiButtonClick()
    {
        UIIm.Instance.ShowSystemHints("comming Later!", (int)PB.ImType.PROMPT);
    }

    void OnBossButtonClick()
    {
        UIIm.Instance.ShowSystemHints("comming Later!", (int)PB.ImType.PROMPT);
    }
}
