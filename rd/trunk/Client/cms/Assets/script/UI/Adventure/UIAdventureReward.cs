using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIAdventureReward:UIBase
{
    public static string ViewName = "UIAdventureReward";

    public Text text_Title;
    public Text text_Base;
    public Text text_Extra;
    public Transform basePos;
    private List<GameObject> baseList = new List<GameObject>();
    public Transform extraPos;
    private List<GameObject> extraList = new List<GameObject>();

    public Button btnClose;

    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("adventure_successful");
        text_Base.text = StaticDataMgr.Instance.GetTextByID("adventure_foundationreward");
        text_Extra.text = StaticDataMgr.Instance.GetTextByID("adventure_extrareward");
        UIUtil.SetButtonTitle(btnClose.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));

        btnClose.onClick.AddListener(OnClickClose);
    }

    public void ReloadData(List<PB.RewardItem> baseRewards,List<PB.RewardItem> extraRewards)
    {
        RefreshRewardItem(ref baseList, basePos, baseRewards);
        RefreshRewardItem(ref extraList, extraPos, extraRewards);
    }
    
    void RefreshRewardItem(ref List<GameObject> list, Transform parent, List<PB.RewardItem> rewards)
    {
        for (int i = 0; i < list.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(list[i]);
        }
        list.Clear();
        for (int i = 0; i < rewards.Count; i++)
        {
            GameObject go = RewardItemCreator.CreateRewardItem(rewards[i], parent, true, false);
            list.Add(go);
        }
    }

    void OnClickClose()
    {
        UIMgr.Instance.DestroyUI(this);
    }
}
