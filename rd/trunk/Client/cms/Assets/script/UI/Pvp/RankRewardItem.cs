using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankRewardItem : MonoBehaviour
{
    public Image[] firstThreeImageArray;
    public Text indexText;
    public Transform rewardParentTrans;

    public static RankRewardItem CreateWith()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("RankRewardItem");
        RankRewardItem item = go.GetComponent<RankRewardItem>();
        item.InitWith();
        return item;
    }
    public void InitWith()
    {

    }
	
}
