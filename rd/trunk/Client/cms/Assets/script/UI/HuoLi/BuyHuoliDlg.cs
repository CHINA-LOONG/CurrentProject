using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuyHuoliDlg : UIBase
{
    public Text huafeiDesc;
    public Text buyItemDesc;

    public Button cancelButton;
    public Button conformButton;
    public Button closeButton;
    public Toggle nextToggle;

    private MsgBox.PromptMsg.PrompDelegate callBack;

    public static string ViewName = "BuyHuoliDlg";
    public static void    OpenWith(string desc1,string desc2, MsgBox.PromptMsg.PrompDelegate callBack)
    {
        BuyHuoliDlg huoliDlg = (BuyHuoliDlg)UIMgr.Instance.OpenUI_(ViewName);
        huoliDlg.InitWith(desc1, desc2, callBack);
    }
    
    void    Start()
    {
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        closeButton.onClick.AddListener(OnCancelButtonClick);
        conformButton.onClick.AddListener(OnConformButtonClick);
    }

    public  void InitWith(string desc1, string desc2, MsgBox.PromptMsg.PrompDelegate callBack)
    {
        this.callBack = callBack;
        huafeiDesc.text = desc1;
        buyItemDesc.text = desc2;
        nextToggle.isOn = !HuoLiDataMgr.Instance.showHuoliBuyDlg;
    }
    void    OnCancelButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void    OnConformButtonClick()
    {
        if(callBack != null)
        {
            callBack(MsgBox.PrompButtonClick.OK);
        }
        HuoLiDataMgr.Instance.showHuoliBuyDlg = !nextToggle.isOn;
        UIMgr.Instance.CloseUI_(this);
    }

}
