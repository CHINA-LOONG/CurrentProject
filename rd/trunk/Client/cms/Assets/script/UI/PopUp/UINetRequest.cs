using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINetRequest : UIBase
{

    public static string ViewName = "UINetRequest";
    public static UINetRequest Inst = null;

    public Button closeButton;

    public static void Open()
    {
        if (Inst != null)
        {
            UIMgr.Instance.ShowUI(Inst);
        }
        else
        {
            Inst = UIMgr.Instance.OpenUI_(UINetRequest.ViewName) as UINetRequest;
        }
    }

    public static void Close()
    {
        if (Inst != null)
        {
            UIMgr.Instance.HideUI(Inst);
        }
        //UIMgr.Instance.CloseUI_(UINetRequest.ViewName);
    }

    public override void Init(bool forbidGuide = false)
    {
        //前置

    }
    public override void Clean()
    {
    }

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(closeButton.gameObject).onClick = OnCloseButtonCliced;
    }

    void OnCloseButtonCliced(GameObject go)
    {
        UINetRequest.Close();
    }
    
}
