using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BaoxiangReward : MonoBehaviour
{
	public	static	BaoxiangReward	CreateWith(int boxIndex, PB.HSRewardInfo rewardInfo)
	{
		GameObject go =  ResourceMgr.Instance.LoadAsset ("BaoxiangReward", false);
		BaoxiangReward boxReward = go.GetComponent<BaoxiangReward> ();
		boxReward.InitWith (boxIndex, rewardInfo);
		return	boxReward;
	}

	public	Text	boxIndexText;
	public	Text	jinbiCountText;
	public	Transform	subItemsParent;


	public	void	InitWith(int	boxIndex, PB.HSRewardInfo rewardInfo)
	{
		boxIndexText.text = boxIndex.ToString ();
		jinbiCountText.text = "0";

		List<PB.RewardItem> listReward = rewardInfo.RewardItems;
		PB.RewardItem subReward = null;
		for (int i = 0; i < listReward.Count; ++i)
		{
			subReward = listReward[i];
			if(subReward.type == (int)PB.itemType.PLAYER_ATTR)
			{
				if ((int)PB.changeType.CHANGE_COIN == int.Parse(subReward.itemId))
				{
					jinbiCountText.text =  subReward.count.ToString();
				}
			}
			else if (subReward.type == (int)PB.itemType.EQUIP ||
			         subReward.type == (int)PB.itemType.ITEM ||
			         subReward.type == (int)PB.itemType.MONSTER)
			{
				BaoxiangSubReward boxSubReward = BaoxiangSubReward.CreateWith(subReward);
				if(null != boxSubReward)
				{
					boxSubReward.transform.SetParent(subItemsParent);
				}
			}
		}

	}

}
