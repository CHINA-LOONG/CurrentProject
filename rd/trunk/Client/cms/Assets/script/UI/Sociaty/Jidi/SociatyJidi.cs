using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SociatyJidiContenType : int
{
    MyJidi = 0,//我的驻兵
    JidiMembers,//基地列兵
    Count
}
public class SociatyJidi : UIBase, TabButtonDelegate
{
    public static string ViewName = "SociatyJidi";
    public Text title;
    public Text contributionText;
    public Button closeButton;
    public TabButtonGroup tabBtnGroup;
    public RectTransform contentParent;

    private SociatyMyJidi myJidi;
    private SociatyJidiMembers jidiMembers;

    private SociatyJidiContenType contentType = SociatyJidiContenType.Count;

    public static SociatyJidi Instance;
    public static void Open(SociatyJidiContenType contentType = SociatyJidiContenType.MyJidi)
    {
        SociatyJidi jidiUi = (SociatyJidi)UIMgr.Instance.OpenUI_(ViewName);
        jidiUi.InitType((int)contentType);
    }
    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LEAVE_N_S.GetHashCode().ToString(), OnAllianceLeave_N_S);
    }
    //---------------------------------------------------------------------------------------------
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LEAVE_N_S.GetHashCode().ToString(), OnAllianceLeave_N_S);
    }

    public override void Clean()
    {
        if (null != myJidi)
        {
            myJidi.Clear();
            ResourceMgr.Instance.DestroyAsset(myJidi.gameObject);
            myJidi = null;
        }
        if (null != jidiMembers)
        {
            jidiMembers.Clear();
            ResourceMgr.Instance.DestroyAsset(jidiMembers.gameObject);
            jidiMembers = null;
        }
    }

    // Use this for initialization
    void Start()
    {
        Instance = this;
        title.text = StaticDataMgr.Instance.GetTextByID("sociaty_jidi");

        tabBtnGroup.tabButtonList[0].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_mybing"));
        tabBtnGroup.tabButtonList[1].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_otherbing"));
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    public void InitType(int initType)
    {
        tabBtnGroup.InitWithDelegate(this);
        tabBtnGroup.OnChangeItem(initType);
    }

    public void OnTabButtonChanged(int index, TabButtonGroup tab)
    {
        SetJidiContentType((SociatyJidiContenType)index);
    }

    public void SetJidiContentType(SociatyJidiContenType type)
    {
        contentType = type;
        if (contentType == SociatyJidiContenType.MyJidi)
        {
            ShowMyJidi(true);
            ShowJidiMembers(false);
        }
        else if (contentType == SociatyJidiContenType.JidiMembers)
        {
            ShowMyJidi(false);
            ShowJidiMembers(true);
        }
        contributionText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_lishi"),
            GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData.totalContribution);
    }

    void ShowMyJidi(bool bshow)
    {
        if (!bshow)
        {
            if (myJidi != null)
            {
                myJidi.gameObject.SetActive(false);
            }
            return;
        }
        if (null == myJidi)
        {
            myJidi = SociatyMyJidi.Instance;
            myJidi.transform.SetParent(contentParent);
            RectTransform rt = myJidi.transform as RectTransform;
            rt.anchoredPosition = new Vector2(0, 0);
            rt.localScale = new Vector3(1, 1, 1);
        }
        myJidi.gameObject.SetActive(true);
        myJidi.RequestJidiInfo();
    }
    void ShowJidiMembers(bool bshow)
    {
        if (!bshow)
        {
            if (jidiMembers != null)
            {
                jidiMembers.gameObject.SetActive(false);
            }
            return;
        }
        if (null == jidiMembers)
        {
            jidiMembers = SociatyJidiMembers.Instance;
            jidiMembers.transform.SetParent(contentParent);
            RectTransform rt = jidiMembers.transform as RectTransform;
            rt.anchoredPosition = new Vector2(0, 0);
            rt.localScale = new Vector3(1, 1, 1);
        }
        jidiMembers.gameObject.SetActive(true);
        jidiMembers.RequestData();
    }

    void OnCloseButtonClick()
    {
        RequestCloseUi();
    }
    void OnAllianceLeave_N_S(ProtocolMessage message)
    {
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = 0;
        UIMgr.Instance.CloseUI_(this);
    }
}
