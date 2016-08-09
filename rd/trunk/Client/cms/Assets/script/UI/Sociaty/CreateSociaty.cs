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
	}

    public override  void Init()
    {
        titleText.text = StaticDataMgr.Instance.GetTextByID("sociaty_buildtitle");
        nameLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_name");
        nameRuleLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_buildlimit");

        gonggaoLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_notice");
        costLabel.text = StaticDataMgr.Instance.GetTextByID("sociaty_reqcoin");
        costValueText.text = GameConfig.Instance.createSociatyCostZuanshi.ToString();

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
        if(GameConfig.Instance.createSociatyCostZuanshi > GameDataMgr.Instance.PlayerDataAttr.gold)
        {
            GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            return;
        }
        if(CheckInput())
        {
            RequestCreateSociaty();
        }
    }

    bool CheckInput()
    {
        if(string.IsNullOrEmpty(nameInputField.text))
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("名字不能为空 "),(int)PB.ImType.PROMPT);
            return false;
        }
        if(string.IsNullOrEmpty(gonggaoInputField.text))
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("公告内容不能为空 "), (int)PB.ImType.PROMPT);
            return false;
        }
      //  UIIm.Instance.ShowSystemHints(nameInputField.text.Length.ToString(), (int)PB.ImType.PROMPT);
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
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = msg.GetProtocolBody<PB.HSErrorCode>();
           // switch(errorCode)
           // {
               // case PB.allianceError.ALLIANCE_ALREADY_APPLY
            //}
            return;
        }

        PB.HSAllianceCreateRet allianceRet = msg.GetProtocolBody<PB.HSAllianceCreateRet>();
        GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = allianceRet.allianeId;
        InstanceList.Close();
        UIMgr.Instance.CloseUI_(this);

        GameDataMgr.Instance.SociatyDataMgrAttr.OpenSociaty();
    }
}
