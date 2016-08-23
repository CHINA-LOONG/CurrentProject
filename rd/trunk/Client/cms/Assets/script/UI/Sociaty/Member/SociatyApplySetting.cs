using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyApplySetting : UIBase
{
    public static string ViewName = "SociatyApplySetting";
    public Text titleText;
    public Text applyTypeLabelText;
    public Text applyTypeValueText;
    public Transform leftTypeButton;
    public Transform rightTypeButton;

    public Text levelLabelText;
    public Text levelValueText;

    public Text tipsText;
    public Button cancelButton;
    public Button conformButton;

    private bool autoJoin = false;
    private ChangeValueByHand chgValue = null;

    private int levelLimit = 1;
    private int LevelLimitAttr
    {
        set
        {
            levelLimit = value;
            levelValueText.text = levelLimit.ToString();
        }
        get
        {
            return levelLimit;
        }
    }

    public static void OpenWith(bool autoJoin,int sociatyLevelLimit)
    {
        SociatyApplySetting applySetting = (SociatyApplySetting)UIMgr.Instance.OpenUI_(ViewName);
        applySetting.InitWith(autoJoin, sociatyLevelLimit);
    }

	// Use this for initialization
	void Start ()
    {
        titleText.text = StaticDataMgr.Instance.GetTextByID("sociaty_setup");
        applyTypeLabelText.text = StaticDataMgr.Instance.GetTextByID("sociaty_type");
        levelLabelText.text = StaticDataMgr.Instance.GetTextByID("sociaty_minlevel");
        tipsText.text = StaticDataMgr.Instance.GetTextByID("sociaty_tips7");

        UIUtil.SetButtonTitle(cancelButton.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
        UIUtil.SetButtonTitle(conformButton.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));

        EventTriggerListener.Get(leftTypeButton.gameObject).onClick = OnApplyTypeLeftClick;
        EventTriggerListener.Get(rightTypeButton.gameObject).onClick = OnApplyTypeRightClick;

        cancelButton.onClick.AddListener(OnCancelButtonClick);
        conformButton.onClick.AddListener(OnConformButtonClick);
    }

    public void InitWith(bool autoJoin, int sociatyLevelLimit)
    {
        if(null == chgValue)
        {
            chgValue = GetComponent<ChangeValueByHand>();
        }

        SetAutoJoin(autoJoin);
        chgValue.ResetValue(sociatyLevelLimit, 99, 10);
        chgValue.callback = OnChangedValueByHand;
        LevelLimitAttr = sociatyLevelLimit;
    }
    void OnChangedValueByHand(int value)
    {
        LevelLimitAttr = value;
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SETTING_C.GetHashCode().ToString(), OnSettingAllianceFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SETTING_S.GetHashCode().ToString(), OnSettingAllianceFinish);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SETTING_C.GetHashCode().ToString(), OnSettingAllianceFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SETTING_S.GetHashCode().ToString(), OnSettingAllianceFinish);
    }

    void OnApplyTypeLeftClick(GameObject go)
    {
        SetAutoJoin(!this.autoJoin);
    }
    void OnApplyTypeRightClick(GameObject go)
    {
        SetAutoJoin(!this.autoJoin);
    }

    public void SetAutoJoin(bool autoJoin)
    {
        this.autoJoin = autoJoin;
        string txtid = "sociaty_type1";
        if(autoJoin)
        {
            txtid = "sociaty_type2";
        }
        applyTypeValueText.text = StaticDataMgr.Instance.GetTextByID(txtid);
    }

    void OnCancelButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnConformButtonClick()
    {
       PB.AllianceInfo allianceInfo = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData;
        
        if(allianceInfo.autoAccept == this.autoJoin && allianceInfo.minLevel == LevelLimitAttr)
        {
           // UIIm.Instance.ShowSystemHints("请先修改",(int)PB.ImType.PROMPT);
            return;
        }
        PB.HSAllianceSettion param = new PB.HSAllianceSettion();
        param.autoJoin = this.autoJoin;
        param.minLevel = this.LevelLimitAttr;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_SETTING_C.GetHashCode(), param);
    }
	
    void OnSettingAllianceFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSAllianceSettionRet retMsg = message.GetProtocolBody<PB.HSAllianceSettionRet>();
        if(null != retMsg)
        {
            PB.AllianceInfo allianceInfo = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData;
            allianceInfo.autoAccept = this.autoJoin;
            allianceInfo.minLevel = LevelLimitAttr;
            UIMgr.Instance.CloseUI_(this);
        }
    }
}
