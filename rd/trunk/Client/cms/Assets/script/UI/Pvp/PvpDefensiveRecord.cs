using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpDefensiveRecord : UIBase, IScrollView
{
    public static string ViewName = "PvpDefensiveRecord";

    public Button closeButton;
    public Text titleText;
    public Text[] listTitleText;
    public FixCountScrollView scrollView;

    private List<int> defensiveData = new List<int>();

    public static   PvpDefensiveRecord Open()
    {
        PvpDefensiveRecord record = UIMgr.Instance.OpenUI_(ViewName) as PvpDefensiveRecord;

        return record;
    }

    // Use this for initialization
	void Start ()
    {
        //test
        for (int i = 0; i < 100; ++i)
        {
            defensiveData.Add(i+1);
        }
        closeButton.onClick.AddListener(OnCloseButtonClick);

        scrollView.InitContentSize(defensiveData.Count, this, true);
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
       // questItem quest = item.GetComponent<questItem>();
       // quest.ReLoadData(CurrentList[index]);
    }
    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        DefensiveRecordItem subItem = DefensiveRecordItem.CreateWith();
        UIUtil.SetParentReset(subItem.transform, parent);
        return subItem.transform;
    }
    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }
    #endregion
}
