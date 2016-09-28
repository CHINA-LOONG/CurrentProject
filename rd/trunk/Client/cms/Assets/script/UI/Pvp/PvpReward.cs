using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PvpReward : UIBase
{
    public static string ViewName = "PvpReward";
    public Text tileText;
    public Text rankText;
    public Text duanText;

    public Button closeButton;

    public ScrollView rankScrollView;
    public ScrollView duanScrollView;

    public  static  void    Open()
    {
        UIMgr.Instance.OpenUI_(ViewName);
    }
    public override void Clean()
    {
        rankScrollView.ClearAllElement();
        duanScrollView.ClearAllElement();
    }
    // Use this for initialization
    void Start ()
    {
        tileText.text = StaticDataMgr.Instance.GetTextByID("pvp_battlereward");
        rankText.text = StaticDataMgr.Instance.GetTextByID("pvp_rankreward");
        duanText.text = StaticDataMgr.Instance.GetTextByID("pvp_duanweireward");

        closeButton.onClick.AddListener(OnCloseButtonClick);

        InitRankReward();
        InitDuanReward();
	}

    void InitRankReward()
    {

    }

    void InitDuanReward()
    {

    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
}
