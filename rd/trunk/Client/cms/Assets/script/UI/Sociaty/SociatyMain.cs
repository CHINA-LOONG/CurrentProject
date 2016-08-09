using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SociatyContenType:int
{
    Infomation = 0,//公会信息
    Member,
    Technology,
    Log,
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


    bool isFirst = true;
    SociatyContenType contentType = SociatyContenType.Count;
    SociatyContentBase[] contentPages = new SociatyContentBase[(int)SociatyContenType.Count];

    public static void OpenWith(SociatyContenType contentType = SociatyContenType.Infomation)
    {
        SociatyMain main = (SociatyMain)UIMgr.Instance.OpenUI_(ViewName);
        main.SetContentPage(contentType);
    }
    public override void Init()
    {
        if (isFirst)
        {
            isFirst = false;
            FirsInit();
        }
    }
    void FirsInit()
    {
        tabBtnGroup.InitWithDelegate(this);
        title.text = StaticDataMgr.Instance.GetTextByID("sociaty_title");

        tabBtnGroup.tabButtonList[0].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_information"));
        tabBtnGroup.tabButtonList[1].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_member"));
        tabBtnGroup.tabButtonList[2].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_technology"));
        tabBtnGroup.tabButtonList[3].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_log"));
        tabBtnGroup.tabButtonList[4].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_other"));

        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    public void OnTabButtonChanged(int index)
    {
        if((int)contentType != index)
        {
            RefreshUi((SociatyContenType)index);
        }
    }

    public  void    SetContentPage(SociatyContenType contentType)
    {
        tabBtnGroup.OnChangeItem((int)contentType);
    }

    private  void RefreshUi(SociatyContenType contentType)
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

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
}

