using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DuanRewardItem : MonoBehaviour
{
    public Image duanImage;
    public Transform rewardParentTrans;

    public static DuanRewardItem    CreateWith()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("DuanRewardItem");
        DuanRewardItem item = go.GetComponent<DuanRewardItem>();
        item.InitWith();
        return item;
    }
	public void InitWith()
    {

    }
}
