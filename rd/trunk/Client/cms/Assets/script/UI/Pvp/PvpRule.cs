using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PvpRule : UIBase
{
    public Button closeButton;
    public Text titleText;
    public Text ruleContentText;
    public ScrollRect scrollRect;

    public static string ViewName = "PvpRule";

    public static void Open()
    {
        UIMgr.Instance.OpenUI_(ViewName);
    }
    void Start()
    {
        ruleContentText.text = "";
        closeButton.onClick.AddListener(OnCloseButtonClick);
        titleText.text = StaticDataMgr.Instance.GetTextByID("pvp_rules");
    }

    void OnEnterAnimationFinish()
    {
        ruleContentText.text = StaticDataMgr.Instance.GetTextByID("pvp_rulesneirong");
         scrollRect.normalizedPosition = Vector2.one;
    }

    void    OnCloseButtonClick()
    {
        RequestCloseUi();
    }
}
