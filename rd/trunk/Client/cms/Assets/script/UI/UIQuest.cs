using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIQuest : UIBase
{
    public static string ViewName = "UIQuest";

    public Button btn_Close;
    public Text text_Title;

    void Start()
    {
        //TODO
        OnLanguageChanged();
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

    void OnLanguageChanged()
    {
        //TODO: change language
        text_Title.text = StaticDataMgr.Instance.GetTextByID("quest_title");
    }

}
