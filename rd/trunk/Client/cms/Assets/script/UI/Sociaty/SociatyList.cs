using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyList : UIBase
{
    public static string ViewName = "SociatyList";

    public Button closeButton;
    public Button createButton;
    public Button searchButton;
    public InputField searchInputField;

    public Text title;
    public Text[] contentTitle;

    public Text gonggaoDescText;
    public Text gonggaoContentText;
    public Text pageText;
    public Button leftButton;
    public Button rightButton;
    public ScrollView scrollView;

    public static SociatyList Instance = null;

    private int curPage = 1;//start with 1
    private int totalPage = 1;
    private int requestPage = 1;
    private List<PB.AllianceSimpleInfo> listAllianceInfo = new List<PB.AllianceSimpleInfo>();

    private AllianceInfoItem lastSelItem = null;

    private List<AllianceInfoItem> allianceItemCatch = new List<AllianceInfoItem>();

    public  static  void    OpenWith(string search)
    {
        SociatyList thislist = (SociatyList)UIMgr.Instance.OpenUI_(ViewName);
        thislist.InitWithSearch(search);
    }

	void Start ()
    {
        Instance = this;
        closeButton.onClick.AddListener(OnCloseButtonClick);
        createButton.onClick.AddListener(OnCreateButtonClick);
        searchButton.onClick.AddListener(OnSearchButtonClick);
        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);

        contentTitle[0].text = StaticDataMgr.Instance.GetTextByID("sociaty_id");
        contentTitle[1].text = StaticDataMgr.Instance.GetTextByID("sociaty_name");
        contentTitle[2].text = StaticDataMgr.Instance.GetTextByID("sociaty_level");
        contentTitle[3].text = StaticDataMgr.Instance.GetTextByID("sociaty_chairman");
        contentTitle[4].text = StaticDataMgr.Instance.GetTextByID("sociaty_peoplenum");
        contentTitle[5].text = StaticDataMgr.Instance.GetTextByID("sociaty_qiliveness");
        contentTitle[6].text = StaticDataMgr.Instance.GetTextByID("sociaty_applylevel");
        contentTitle[7].text = StaticDataMgr.Instance.GetTextByID("sociaty_operate");
    }

    public  override void Clean()
    {
        foreach(var subItem in allianceItemCatch)
        {
            ResourceMgr.Instance.DestroyAsset(subItem.gameObject);
        }
        allianceItemCatch.Clear();
    }
   
    void    OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SEARCH_C.GetHashCode().ToString(), OnRequestSearchFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SEARCH_S.GetHashCode().ToString(), OnRequestSearchFinished);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LIST_C.GetHashCode().ToString(), OnRequestPageFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LIST_S.GetHashCode().ToString(), OnRequestPageFinished);
    }

    void    OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SEARCH_C.GetHashCode().ToString(), OnRequestSearchFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SEARCH_S.GetHashCode().ToString(), OnRequestSearchFinished);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LIST_C.GetHashCode().ToString(), OnRequestPageFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LIST_S.GetHashCode().ToString(), OnRequestPageFinished);
    }

    public  void    InitWithSearch(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            RequestPage(1);
        }
        else
        {
            RequestSearch(search);
        }

    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
    
    void    OnCreateButtonClick()
    {
        CreateSociaty.Open();
    }

    void OnSearchButtonClick()
    {
        string search = searchInputField.text.Trim();
        InitWithSearch(search);
    }

    void OnLeftButtonClick()
    {
        if(curPage > 1)
        {
            RequestPage(curPage - 1);
        }
    }

    void OnRightButtonClick()
    {
        if (curPage < totalPage)
        {
            RequestPage(curPage + 1);
        }
    }

    public  void OnAllianceInfoItemClick(AllianceInfoItem item)
    {
        if(null != lastSelItem)
        {
            lastSelItem.SetSelect(false);
        }
        lastSelItem = item;
        lastSelItem.SetSelect(true);
        gonggaoContentText.text = item.itemInfoData.notice;
    }

    void RequestSearch(string search)
    {
        PB.HSAllianceSearch param = new PB.HSAllianceSearch();
        param.nameOrId = search;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_SEARCH_C.GetHashCode(), param);
    }

    void OnRequestSearchFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();
        PB.HSAllianceSearchRet searchRet = msg.GetProtocolBody<PB.HSAllianceSearchRet>();
        if(searchRet != null && searchRet.result != null && searchRet.result.Count > 0)
        {
            listAllianceInfo.Clear();
            listAllianceInfo.AddRange(searchRet.result);
            curPage = 1;
            totalPage = 1;
            RefreshUi();
            return;
        }
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_001"), (int)PB.ImType.PROMPT);
    }

    void RequestPage(int index)
    {
        requestPage = index;
        PB.HSAllianceList param = new PB.HSAllianceList();
        param.reqPage = index;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_LIST_C.GetHashCode(), param);
    }
    
    void OnRequestPageFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSAllianceListRet listRet = msg.GetProtocolBody<PB.HSAllianceListRet>();
        listAllianceInfo.Clear();
        listAllianceInfo.AddRange(listRet.allianceList);
        totalPage = listRet.totalPage;
        curPage = requestPage;

        RefreshUi();
    }

    void RefreshUi()
    {
        gonggaoContentText.text = "";

        PB.AllianceSimpleInfo itemData = null;
        AllianceInfoItem itemUi = null;
        ResetCacheItem();
       for (int i =0;i <listAllianceInfo.Count; ++i)
        {
            itemData = listAllianceInfo[i];
            itemUi = null;
            if (i < allianceItemCatch.Count)
            {
                itemUi = allianceItemCatch[i];
                itemUi.InitWith(itemData);
                itemUi.gameObject.SetActive(true);
            }
            if(null == itemUi)
            {
                itemUi = AllianceInfoItem.CreateWith(itemData);
                allianceItemCatch.Add(itemUi);
                scrollView.AddElement(itemUi.gameObject);
            }
        }
        pageText.text = string.Format("{0}/{1}", curPage, totalPage);
    }

    void ResetCacheItem()
    {
        foreach(var subitem in allianceItemCatch)
        {
            subitem.SetSelect(false);
            subitem.gameObject.SetActive(false);
        }
    }
}
