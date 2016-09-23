using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIAdventure : UIBase,
                            TabButtonDelegate,
                            IAdventureItem
{
    public static string ViewName = "UIAdventure";

    public Text text_Title;
    public Button btn_Close;

    public Text text_qianghua;
    public Text text_jinjie;
    public Text text_boss;

    public List<AdventureItem> adventures = new List<AdventureItem>();

    public Button btn_Expedition;
    public Text text_Expedition;

    public Text text_Tips;

    private int tabIndex = -1;
    private int selIndex = 0;

    private TabButtonGroup tabGroup;
    public TabButtonGroup TabGroup
    {
        get
        {
            if (tabGroup == null)
            {
                tabGroup = GetComponentInChildren<TabButtonGroup>();
                tabGroup.InitWithDelegate(this);
            }
            return tabGroup;
        }
    }

    private AdventureInfo curData;

    [HideInInspector]
    public UIAdventureTeams uiAdventureTeams;
    [HideInInspector]
    public UIAdventureLayout uiAdventureLayout;

    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("adventure_title");
        text_Expedition.text = StaticDataMgr.Instance.GetTextByID("adventure_teams");

        text_qianghua.text = StaticDataMgr.Instance.GetTextByID("adventure_qianghuashi");
        text_jinjie.text = StaticDataMgr.Instance.GetTextByID("adventure_jinjieshi");
        text_boss.text = StaticDataMgr.Instance.GetTextByID("adventure_boss");

        btn_Expedition.onClick.AddListener(OnClickExpeditionBtn);
        btn_Close.onClick.AddListener(OnClickCloseBtn);

        for (int i = 0; i < adventures.Count; i++)
        {
            adventures[i].IAdventureItemDelegate = this;
        }
    }

    public void Refresh(int select=-1)
    {
        selIndex = (select == -1 ? selIndex : select);

        if (tabIndex != selIndex)
        {
            TabGroup.OnChangeItem(selIndex);
        }
        else
        {
            ReLoadData(selIndex);
        }
    }

    void ReLoadData(int index)
    {
        Dictionary<int, AdventureInfo> adventureDict = AdventureDataMgr.Instance.adventureDict[index + 1];
        for (int i = 0; i < adventures.Count; i++)
        {
            adventures[i].ReloadData(adventureDict[i + 1]);
        }
    }

    void OnClickExpeditionBtn()
    {
        openAdventureTeams();
    }
    void OnClickCloseBtn()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    #region UIBase
    public override void Init()
    {
        tabIndex = -1;
        selIndex = 0;
        //临时测试代码
        Refresh();
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(uiAdventureLayout);
        UIMgr.Instance.DestroyUI(uiAdventureTeams);
    }
    #endregion
    
    #region BindListener
    void OnEnable()
    {
        BindListener();
    }
    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener(GameEventList.AdventureChange, OnAdventureChangeNotify);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.AdventureChange, OnAdventureChangeNotify);
    }

    void OnAdventureChangeNotify()
    {
        Refresh();
    }
    #endregion

    #region TabButtonDelegate
    public void OnTabButtonChanged(int index)
    {
        if (tabIndex == index)
        {
            return;
        }
        selIndex = index;
        tabIndex = selIndex;
        ReLoadData(tabIndex);
    }
    #endregion

    #region IAdventureItem
    public void openAdventureTeams()
    {
        uiAdventureTeams = UIMgr.Instance.OpenUI_(UIAdventureTeams.ViewName) as UIAdventureTeams;
    }

    public void openAdventureLayout(AdventureInfo info)
    {
        uiAdventureLayout = UIMgr.Instance.OpenUI_(UIAdventureLayout.ViewName) as UIAdventureLayout;
        uiAdventureLayout.ReloadData(info);
    }
    #endregion
}
