using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankRewardItem : MonoBehaviour
{
    public Image[] firstThreeImageArray;
    public Text indexText;
    public Transform rewardParentTrans;

    public static RankRewardItem CreateWith(PvpRankRewardUiStaticData rankData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("RankRewardItem");
        RankRewardItem item = go.GetComponent<RankRewardItem>();
        item.InitWith(rankData);
        return item;
    }
    public void InitWith(PvpRankRewardUiStaticData rankData)
    {
        if(rankData.id > 3)
        {
            indexText.text = rankData.rank;
        }
        else
        {
            indexText.text = "";
        }
        for(int i =0;i<firstThreeImageArray.Length;++i)
        {
            firstThreeImageArray[i].gameObject.SetActive(i == rankData.id - 1);
        }
        InitReward(rankData.reward);
    }
    void InitReward(string rewardid)
    {
        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardid);
        if (rewardData == null || rewardData.itemList == null)
            return;
        foreach (var itemData in rewardData.itemList)
        {
            GameObject go = RewardItemCreator.CreateRewardItem(itemData.protocolData, rewardParentTrans, true, false);
            if (null != go)
            {
                go.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
