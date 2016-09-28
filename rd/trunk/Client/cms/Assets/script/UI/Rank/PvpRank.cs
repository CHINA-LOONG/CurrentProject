using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpRank : MonoBehaviour, IScrollView
{
    public Text[] titlesText;
    public FixCountScrollView scrollView;

    private static PvpRank instance = null;
    public static PvpRank Instance
    {
        get
        {
            if(null == instance)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("PvpRank");
                instance = go.GetComponent<PvpRank>();
            }
            return instance;
        }
    }
    
    // Use this for initialization
	void Start ()
    {
        scrollView.InitContentSize(60, this, true);
    }

    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
        // questItem quest = item.GetComponent<questItem>();
        // quest.ReLoadData(CurrentList[index]);
    }
    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        PvpRankItem subItem = PvpRankItem.CreateWith();
        UIUtil.SetParentReset(subItem.transform, parent);
        return subItem.transform;
    }
    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }
    #endregion
}
