using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPet : UIBase {

    public static string ViewName = "UIPet";
    public static string AssertName = "ui/pet";

    public Button m_closeButton;
    public Button m_adjustButton;

    void Start()
    {
        EventTriggerListener.Get(m_closeButton.gameObject).onClick = CloseBagButtonDown;
        EventTriggerListener.Get(m_adjustButton.gameObject).onClick = AdjustLineUpButtonDown;
    }

    void CloseBagButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI(ViewName);
    }

    void AdjustLineUpButtonDown(GameObject go)
    {
        Debug.Log("调整阵容");
    }
}
