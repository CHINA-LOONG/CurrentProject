using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public Text gonggaoLabel;
    public Text gonggaoValue;
    public Button modifyNotifyButton;

    public Text huodongLabel;
    public Button qifuButton;
    public Button taskButton;
    public Button shopButton;
    public Button jidiButton;
    public Button bossButton;

	// Use this for initialization
	void Start ()
    {
        modifyNotifyButton.onClick.AddListener(OnModifyNotifyButtonClick);
        qifuButton.onClick.AddListener(OnQifubuttonClick);
        taskButton.onClick.AddListener(OnTaskButtonClick);
        shopButton.onClick.AddListener(OnShopButtonClick);
        jidiButton.onClick.AddListener(OnJiDiButtonClick);
        bossButton.onClick.AddListener(OnBossButtonClick);
    }
	
	void    OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SYN_S.GetHashCode().ToString(), OnAllianceSynceFinish);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SYN_S.GetHashCode().ToString(), OnAllianceSynceFinish);
    }

    public override void RefreshUI()
    {
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

        PB.AllianceInfo allianceInfo = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData;

        lvlText.text = string.Format("LVL {0}", allianceInfo.level);
        nameText.text = allianceInfo.name;
        captionValue.text = allianceInfo.captaionName;
        idValue.text = allianceInfo.id.ToString();
        memberValue.text = string.Format("{0}/{1}", allianceInfo.currentPop, allianceInfo.maxPop);
        // personalContributionValue.text = allianceInfo.

        gonggaoValue.text = allianceInfo.notice;

    }

    void OnModifyNotifyButtonClick()
    {
        //todo:modify notify
    }

    void OnQifubuttonClick()
    {
        UIMgr.Instance.OpenUI_(SociatyPray.ViewName);
    }

    void OnTaskButtonClick()
    {
        UIIm.Instance.ShowSystemHints("comming Later!", (int)PB.ImType.PROMPT);
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
