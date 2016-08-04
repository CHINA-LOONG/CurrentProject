using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SaodangReward : MonoBehaviour {

    public Text titleText;
    public Text jinbiCountText;
    public Text expCountText;
    public ScrollView itemsScrollView;


    public static SaodangReward CreateWith(string title, PB.HSRewardInfo rewardInfo)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SaodangReward", false);
        SaodangReward reward = go.GetComponent<SaodangReward>();
        reward.InitWith(title, rewardInfo);
        return reward;
    }

   public   void    InitWith(string title, PB.HSRewardInfo rewardInfo)
    {
        titleText.text = title;
        jinbiCountText.text = "0";
        expCountText.text = "";
        itemsScrollView.ClearAllElement();

        List<PB.RewardItem> listReward = rewardInfo.RewardItems;
        PB.RewardItem subReward = null;
        for (int i = 0; i < listReward.Count; ++i)
        {
            subReward = listReward[i];
            if (subReward.type == (int)PB.itemType.PLAYER_ATTR)
            {
                if ((int)PB.changeType.CHANGE_COIN == int.Parse(subReward.itemId))
                {
                    jinbiCountText.text = subReward.count.ToString();
                }
                else if ((int)PB.changeType.CHANGE_PLAYER_EXP == int.Parse(subReward.itemId))
                {
                    expCountText.text = subReward.count.ToString();
                }
            }
            else if (subReward.type == (int)PB.itemType.EQUIP ||
                     subReward.type == (int)PB.itemType.ITEM ||
                     subReward.type == (int)PB.itemType.MONSTER)
            {
                GameObject go = RewardItemCreator.CreateRewardItem(subReward, itemsScrollView.transform,true);
                if (null != go)
                {
                    itemsScrollView.AddElement(go);
                }
            }
        }
    }
}
