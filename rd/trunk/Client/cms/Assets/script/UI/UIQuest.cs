using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIQuest : UIBase
{
    public static string ViewName = "UIQuest";
    public static string AssertName = "ui/quest";


    public Button btn_Close;
    public Text text_Title;

    void Start()
    {
        EventTriggerListener.Get(btn_Close.gameObject).onClick = ClickCloseButton;
    }


    void ClickCloseButton(GameObject go)
    {
        Logger.Log("click UIQuest close button");
#if XL_DEBUG
        Destroy(gameObject);
#else
        UIMgr.Instance.CloseUI(this);
#endif
    }
}
