using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MsgBox;

public class PrompMsgAdventureConfirm : UIBase
{
    public static string ViewName = "PrompMsgAdventureConfirm";

    public Text textTitle;

    public Text textStep;
    public Text textMsg1;

    public GameObject objMsg2;
    public Text textMsg2;
    public Text costText;

    public Button btnCancel;
    public Button btnConfirm;


    private PromptMsg.PrompDelegate callBack;
    private bool autoClose;

    public static PrompMsgAdventureConfirm Open(string title, string msg1, int step, string msg2, int cost = 0, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
    {
        PrompMsgAdventureConfirm mInfo = UIMgr.Instance.OpenUI_(ViewName, false) as PrompMsgAdventureConfirm;
        mInfo.SetData(title,msg1,step, msg2, cost, callback, autoClose);
        return mInfo;
    }

    void Start()
    {
        btnCancel.onClick.AddListener(OnClickCancelBtn);
        btnConfirm.onClick.AddListener(OnClickConfirmBtn);

        UIUtil.SetButtonTitle(btnCancel.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
        UIUtil.SetButtonTitle(btnConfirm.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
    }

    public void SetData(string title, string msg1,int step, string msg2, int cost = 0, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
    {
        this.callBack = callback;
        this.autoClose = autoClose;

        textTitle.text = title;
        textStep.text = step.ToString() + "%";
        textMsg1.text = msg1;

        if (string.IsNullOrEmpty(msg2))
        {
            objMsg2.SetActive(false);
        }
        else
        {
            objMsg2.SetActive(true);
            textMsg2.text = msg2;
            costText.text = cost.ToString();
        }
    }

    public void Close()
    {
        UIMgr.Instance.DestroyUI(this);
    }
    void OnClickCancelBtn()
    {
        if (autoClose)
        {
            Close();
        }
        if (callBack != null)
        {
            callBack(PrompButtonClick.Cancle);
        }
    }
    void OnClickConfirmBtn()
    {
        if (autoClose)
        {
            Close();
        }
        if (callBack != null)
        {
            callBack(PrompButtonClick.OK);
        }
    }
}
