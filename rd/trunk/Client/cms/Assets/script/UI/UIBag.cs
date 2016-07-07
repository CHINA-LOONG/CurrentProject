using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBag : UIBase {
    
    public static string ViewName = "UIBag";
    public static string AssertName = "ui/bag";

    public Button m_closeButton;

    void Start()
    {
        GridLayoutGroup layout = gameObject.GetComponentInChildren<GridLayoutGroup>();
        GameObject content = layout.gameObject;
        EventTriggerListener.Get(m_closeButton.gameObject).onClick = CloseBagButtonDown;

        content.GetComponent<ScrollPanel>().AddContent("ui/bag", "bagItem", 100);
    }

    void CloseBagButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI(ViewName);
    }
}

