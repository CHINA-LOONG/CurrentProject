using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SociatyContenType:int
{
    Infomation = 0,//公会信息
    Member,
    Technology,
   // Log,
    OtherSociaty,
    Count
}


public class SociatyMain : UIBase, TabButtonDelegate
{
    public static string ViewName = "SociatyMain";
    public Text title;
    public Button closeButton;
    public TabButtonGroup tabBtnGroup;
    public RectTransform contentParent;

    public static SociatyMain Instance = null;
    bool isFirst = true;
    SociatyContenType contentType = SociatyContenType.Count;
    SociatyContentBase[] contentPages = new SociatyContentBase[(int)SociatyContenType.Count];

    private string searchOtherSociaty = null;

    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LEVEL_CHANGE_N_S.GetHashCode().ToString(), OnLevelChange);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LEAVE_N_S.GetHashCode().ToString(), OnAllianceLeave_N_S);
    }
    //---------------------------------------------------------------------------------------------
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LEVEL_CHANGE_N_S.GetHashCode().ToString(), OnLevelChange);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LEAVE_N_S.GetHashCode().ToString(), OnAllianceLeave_N_S);
    }
    //---------------------------------------------------------------------------------------------
    public static void OpenWith(SociatyContenType contentType = SociatyContenType.Infomation,string search = null)
    {
        SociatyMain main = (SociatyMain)UIMgr.Instance.OpenUI_(ViewName);
        main.SetContentPage(contentType,search);
    }
    //---------------------------------------------------------------------------------------------
    public override void Init()
    {
        if (isFirst)
        {
            isFirst = false;
            FirsInit();
        }
    }
    //---------------------------------------------------------------------------------------------
    void FirsInit()
    {
        Instance = this;
        tabBtnGroup.InitWithDelegate(this);
        title.text = StaticDataMgr.Instance.GetTextByID("sociaty_title");

        tabBtnGroup.tabButtonList[0].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_information"));
        tabBtnGroup.tabButtonList[1].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_member"));
        tabBtnGroup.tabButtonList[2].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_technology"));
        //tabBtnGroup.tabButtonList[3].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_log"));
        tabBtnGroup.tabButtonList[3].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_other"));

        closeButton.onClick.AddListener(OnCloseButtonClick);
    }
    //---------------------------------------------------------------------------------------------
    public override void RefreshOnPreviousUIHide()
    {
       if( GameDataMgr.Instance.SociatyDataMgrAttr.allianceID == 0)
        {
            Close();
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("sociaty_beiqingli"));
            return;
        }
        
        if (contentType == SociatyContenType.Infomation)
        {
            contentPages[(int)SociatyContenType.Infomation].RefreshUI();
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnTabButtonChanged(int index)
    {
       // if((int)contentType != index)
      //  {
            RefreshUi((SociatyContenType)index);
      //  }
    }
    //---------------------------------------------------------------------------------------------
    public void    SetContentPage(SociatyContenType contentType,string search = null)
    {
        this.searchOtherSociaty = search;
        tabBtnGroup.OnChangeItem((int)contentType);
    }
    //---------------------------------------------------------------------------------------------
    private void RefreshUi(SociatyContenType contentType)
    {
        this.contentType = contentType;
        for(int i = 0; i < contentPages.Length; ++i)
        {
            SociatyContentBase subPage = contentPages[i];
            if(i == (int)contentType)
            {
                if (subPage == null)
                {
                    subPage = SociatyContentBase.CreateWith(contentType, contentParent);
                    contentPages[i] = subPage;
                }
                subPage.gameObject.SetActive(true);
                if(contentType == SociatyContenType.OtherSociaty)
                {
                    if(!string.IsNullOrEmpty(searchOtherSociaty))
                    {
                        ((SociatyContentOther)subPage).SetSearch(searchOtherSociaty);
                        searchOtherSociaty = null;
                    }
                }
                subPage.RefreshUI();
            }
            else
            {
                if (subPage != null)
                {
                    subPage.gameObject.SetActive(false);
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
    //---------------------------------------------------------------------------------------------
    void OnLevelChange(ProtocolMessage msg)
    {
        PB.AllianceInfo allianceData = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData;
        PB.HSLevelChangeNotify lvlChangeData = msg.GetProtocolBody<PB.HSLevelChangeNotify>();
        allianceData.contribution = lvlChangeData.contribution;
        if (lvlChangeData != null)
        {
            switch ((SociatyTecEnum)lvlChangeData.type)
            {
                case SociatyTecEnum.Sociaty_Tec_Lvl:
                    allianceData.level = lvlChangeData.level;
                    break;
                case SociatyTecEnum.Sociaty_Tec_Member:
                    allianceData.memLevel = lvlChangeData.level;
                    break;
                case SociatyTecEnum.Sociaty_Tec_Coin:
                    allianceData.coinLevel = lvlChangeData.level;
                    break;
                case SociatyTecEnum.Sociaty_Tec_Exp:
                    allianceData.expLevel = lvlChangeData.level;
                    break;
            }

            SetTechnologyInternal(lvlChangeData.type);
        }
    }

    void OnAllianceLeave_N_S(ProtocolMessage message)
    {
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = 0;
        Close();
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("sociaty_beiqingli"));
    }
    //---------------------------------------------------------------------------------------------
    private void SetTechnologyInternal(int type)
    {
        for (int i = 0; i < contentPages.Length; ++i)
        {
            if (i == (int)SociatyContenType.Technology)
            {
                if (contentPages[i] != null && contentPages[i].gameObject.activeSelf)
                {
                    SociatyContentTechnology technologyUI = contentPages[i] as SociatyContentTechnology;
                    technologyUI.RefreshTechnologyInfo(type);
                }
                break;
            }
        }
    }
    //---------------------------------------------------------------------------------------------

    public void Close()
    {
        UIMgr.Instance.CloseUI_(this);
    }
}

