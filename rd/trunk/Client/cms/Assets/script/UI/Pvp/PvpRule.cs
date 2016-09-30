using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PvpRule : UIBase
{
    public Button closeButton;
    public Text titleText;
    public Text ruleContentText;

    public static string ViewName = "PvpRule";

    public static void Open()
    {
        UIMgr.Instance.OpenUI_(ViewName);
    }

    void Start()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        titleText.text = StaticDataMgr.Instance.GetTextByID("pvp_record");
        ruleContentText.text = StaticDataMgr.Instance.GetTextByID("pvp_rulesneirong");
    }

    void    OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
}
