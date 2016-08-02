using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropInfo : UIBase
{
    public static string ViewName = "DropInfo";
    public Text titleText;
    public ScrollView scrollView;
    public GameObject closeButton;

    public  static  void    OpenWith(string dropId)
    {
        DropInfo dinfo = (DropInfo) UIMgr.Instance.OpenUI_(ViewName);
        dinfo.RefreshWith(dropId);
    }

	// Use this for initialization
	void Start ()
    {
        EventTriggerListener.Get(closeButton).onClick = OnCloseButtonClick;
        titleText.text = StaticDataMgr.Instance.GetTextByID("drop_info");
    }

    void    OnCloseButtonClick(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

     public void    RefreshWith(string dropId)
    {
        scrollView.ClearAllElement();

        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(dropId);
        if (null == rewardData || rewardData.itemList == null)
            return;
       
        foreach(var subItemData in rewardData.itemList)
        {
            var subItem = DropItem.CreateWith(subItemData);
            if(null != subItem)
            {
                scrollView.AddElement(subItem.gameObject);
            }
        }
    }
}
