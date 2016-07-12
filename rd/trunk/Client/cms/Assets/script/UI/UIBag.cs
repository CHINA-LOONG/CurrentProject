using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBag : UIBase {
    
    public static string ViewName = "UIBag";

    public Button m_closeButton;

    void Start()
    {
        ScrollViewContainer container = gameObject.GetComponent<ScrollViewContainer>();
        var go = ResourceMgr.Instance.LoadAsset("bagItem");
        for (int i = 0; i < 25; ++i)
        {
            GameObject bagItem = GameObject.Instantiate(go);
            if (bagItem != null)
            {
                bagItem.transform.SetParent(gameObject.transform, false);
                bagItem.transform.localScale = Vector3.one;
                bagItem.transform.localPosition = Vector3.zero;
                container.AddElement(bagItem);
            }
        }

        EventTriggerListener.Get(m_closeButton.gameObject).onClick = CloseBagButtonDown;
    }

    void CloseBagButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI(ViewName);
    }
}

