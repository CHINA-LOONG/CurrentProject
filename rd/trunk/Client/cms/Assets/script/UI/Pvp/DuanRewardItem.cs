using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DuanRewardItem : MonoBehaviour
{
    public Text nameText;
    public Image duanImage;
    public Transform rewardParentTrans;

    public static DuanRewardItem    CreateWith(PvpStageRewardStaticData stageData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("DuanRewardItem");
        DuanRewardItem item = go.GetComponent<DuanRewardItem>();
        item.InitWith(stageData);
        return item;
    }
	public void InitWith(PvpStageRewardStaticData stageData)
    {
        nameText.text = GameDataMgr.Instance.PvpDataMgrAttr.GetStageNameWithId(stageData.grade);

        Sprite stageSp = ResourceMgr.Instance.LoadAssetType<Sprite>(string.Format("pvp_duanweixiao_{0}", stageData.grade));
        if(null != stageData)
        {
            duanImage.sprite = stageSp;
        }
        InitReward(stageData.reward2);
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
