using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateSociaty : UIBase
{
    public static string ViewName = "CreateSociaty";
    public Text titleText;
    public Text nameLabel;
    public Text nameRuleLabel;
    public InputField nameInputField;
    public Text gonggaoLabel;
    public InputField gonggaoInputField;
    public Text costLabel;
    public Text costValueText;
    public Button cancelButton;
    public Button conformButton;
    // Use this for initialization

    public static void Open()
    {
        UIMgr.Instance.OpenUI_(ViewName, false);
    }

	void Start ()
    {
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        conformButton.onClick.AddListener(OnConformButtonClick);

        ((Text)nameInputField.placeholder).text = "";
        ((Text)gonggaoInputField.placeholder).text = "";
    }

    public override  void Init(bool forbidGuide = false)
    {
        titleText.text = StaticDataMgr.Instance.GetTextByID("sociaty_buildtitle");
        nameLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_name");
        nameRuleLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_buildlimit");

        gonggaoLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_notice");
        costLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_reqcoin");
        costValueText.text = GameConfig.Instance.createSociatyCostCoin.ToString();
        if(GameConfig.Instance.createSociatyCostCoin > GameDataMgr.Instance.PlayerDataAttr.coin)
        {
            costValueText.color = new Color(1, 0, 0);
        }
        else
        {
            costValueText.color = new Color(251.0f/255.0f, 241.0f/255.0f, 216.0f/255.0f);
        }

        UIUtil.SetButtonTitle(cancelButton.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
        UIUtil.SetButtonTitle(conformButton.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
    }

    void    OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_C.GetHashCode().ToString(), OnRequestCreateSociatyFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_S.GetHashCode().ToString(), OnRequestCreateSociatyFinished);
    }
    
    void    OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_C.GetHashCode().ToString(), OnRequestCreateSociatyFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_S.GetHashCode().ToString(), OnRequestCreateSociatyFinished);
    }

    void OnCancelButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnConformButtonClick()
    {
        if(GameConfig.Instance.createSociatyCostCoin > GameDataMgr.Instance.PlayerDataAttr.coin)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_009"), (int)PB.ImType.PROMPT);
            return;
        }
        if(CheckInput())
        {
            RequestCreateSociaty();
        }
    }

    bool CheckInput()
    {
        string name = nameInputField.text;
        string gonggao = gonggaoInputField.text;
        if(string.IsNullOrEmpty(name))
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_005"),(int)PB.ImType.PROMPT);
            return false;
        }
        name = name.TrimStart();
        name = name.TrimEnd();
        if(Util.StringByteLength(name) > 12)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_059"), (int)PB.ImType.PROMPT);
            return false;
        }
        if (Util.StringIsAllNumber(name) )
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_057"), (int)PB.ImType.PROMPT);
            return false;
        }


        if(string.IsNullOrEmpty(gonggao))
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_007"), (int)PB.ImType.PROMPT);
            return false;
        }
        gonggao = gonggao.TrimStart();
        gonggao = gonggao.TrimEnd();
        if (Util.StringByteLength(gonggao) > 300)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_061"), (int)PB.ImType.PROMPT);
            return false;
        }

        return true;
    }

    void RequestCreateSociaty()
    {
        PB.HSAllianceCreate param = new PB.HSAllianceCreate();
        param.name = nameInputField.text;
        param.notice = gonggaoInputField.text;

        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CREATE_C.GetHashCode(), param);
    }
    void OnRequestCreateSociatyFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = msg.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }

        PB.HSAllianceCreateRet allianceRet = msg.GetProtocolBody<PB.HSAllianceCreateRet>();
        GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = allianceRet.allianeId;
        SociatyList.Close();
        UIMgr.Instance.CloseUI_(this);

        GameDataMgr.Instance.SociatyDataMgrAttr.OpenSociaty();
    }
}
