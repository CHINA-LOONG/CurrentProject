using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum RankType
{
    Rank_Pvp
}
public class UIRank : UIBase
{
    public static string ViewName = "UIRank";
    public Text titleText;
    public Transform contentParent;
    public Button closeButton;

    PvpRank pvpRank = null;

    public static   void    OpenWith(RankType rankType)
    {
        UIMgr.Instance.OpenUI_(ViewName);
    }

	// Use this for initialization
	void Start ()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
	}

    public override void Init()
    {
        if(null ==pvpRank)
        {
            pvpRank = PvpRank.Instance;
            pvpRank.transform.SetParent(contentParent);
            pvpRank.transform.localScale = Vector3.one;
            RectTransform pvpRankRt = pvpRank.transform as RectTransform;
            pvpRankRt.anchoredPosition = new Vector2(0, 0);
        }
    }
    public override void Clean()
    {
        if(null != pvpRank)
        {
            ResourceMgr.Instance.DestroyAsset(pvpRank.gameObject);
        }
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
}
