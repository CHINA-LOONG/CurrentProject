using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpReward : UIBase
{
    public static string ViewName = "PvpReward";
    public Text tileText;
    public Text rankText;
    public Text duanText;
    public Text tipText;

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
        tipText.text = StaticDataMgr.Instance.GetTextByID("pvp_rewardstips");

        closeButton.onClick.AddListener(OnCloseButtonClick);

        InitRankReward();
        InitDuanReward();
	}

    void InitRankReward()
    {
        List<PvpRankRewardUiStaticData> listData = StaticDataMgr.Instance.GetRankRewardUiStaticDataList();
        RankRewardItem subItem = null;
        for (int i = 0; i < listData.Count; ++i)
        {
            subItem = RankRewardItem.CreateWith(listData[i]);
            rankScrollView.AddElement(subItem.gameObject);
        }
    }

    void InitDuanReward()
    {
        List<PvpStageRewardStaticData> listData = StaticDataMgr.Instance.GetStageRewardStaticDataList();
        DuanRewardItem subItem = null;
        for (int i=0;i<listData.Count;++i)
        {
            subItem = DuanRewardItem.CreateWith(listData[i]);
            duanScrollView.AddElement(subItem.gameObject);
        }
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
}
