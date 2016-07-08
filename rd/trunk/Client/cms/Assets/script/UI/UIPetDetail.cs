using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPetDetail : UIBase{

    public static string ViewName = "UIPetDetail";
    public static string AssertName = "ui/petdetail";

    public Button m_closeButton;
  
    void Start()
    {
        EventTriggerListener.Get(m_closeButton.gameObject).onClick = CloseBagButtonDown;
    }

    void CloseBagButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI(ViewName);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
