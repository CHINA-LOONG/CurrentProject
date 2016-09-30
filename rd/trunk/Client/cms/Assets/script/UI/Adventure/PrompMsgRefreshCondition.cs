using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MsgBox;

public class PrompMsgRefreshCondition : UIBase
{
    public static string ViewName = "PrompMsgRefreshCondition";

    public Text textTitle;
    public Text textDesc;
    public GameObject objGold;
    public Text costText;

    public Button btnCancel;
    public Button btnConfirm;

    private string content;
    private TimeEventWrap timeWrap;
    public TimeEventWrap TimeWrap
    {
        get { return timeWrap; }
        set
        {
            if (TimeWrap != null)
            {
                TimeWrap.RemoveUpdateEvent(OnUpdateTime);
            }
            timeWrap = value;
        }
    }
    private int changeAmount;
    private PromptMsg.PrompDelegate callBack;
    private bool autoClose;


    public static PrompMsgRefreshCondition Open(string title, int cost=0, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
    {
        PrompMsgRefreshCondition mInfo = UIMgr.Instance.OpenUI_(ViewName, false) as PrompMsgRefreshCondition;
        mInfo.SetData(title, cost, callback, autoClose);
        return mInfo;
    }

    void Start()
    {
        btnCancel.onClick.AddListener(OnClickCancelBtn);
        btnConfirm.onClick.AddListener(OnClickConfirmBtn);

        UIUtil.SetButtonTitle(btnCancel.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
        UIUtil.SetButtonTitle(btnConfirm.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
    }

    public void SetData(string title, int cost, PromptMsg.PrompDelegate callback, bool autoClose)
    {
        this.callBack = callback;
        this.autoClose = autoClose;

        textTitle.text = title;
        OnAdventureConditionCountChangeNotify();

        objGold.SetActive(cost != 0);
        costText.text = cost.ToString();
    }

    void OnAdventureConditionCountChangeNotify()
    {
        TimeWrap = AdventureDataMgr.Instance.ConditionTimeEvent;
        changeAmount = AdventureDataMgr.Instance.AdventureChange;
        if (TimeWrap!=null)
        {
            TimeWrap.AddUpdateEvent(OnUpdateTime);
        }
        else if (changeAmount >= GameConfig.MaxAdventurePoint)
        {
            textDesc.text = StaticDataMgr.Instance.GetTextByID("adventure_tipsmax");
        }
        else
        {
            Logger.LogError("刷新条件次数计时器错误");
        }
    }

    void OnUpdateTime(int time)
    {
        if (time <= 0)
        {
            OnClickConfirmBtn();
        }
        textDesc.text = string.Format(StaticDataMgr.Instance.GetTextByID("adventure_tipshuifu"),changeAmount, UIUtil.Convert_hh_mm_ss(time));
    }

    public void Close()
    {
        TimeWrap = null;
        UIMgr.Instance.DestroyUI(this);
    }

    void OnClickCancelBtn()
    {
        if (autoClose)
        {
            Close();
        }
        if (callBack != null)
        {
            callBack(PrompButtonClick.Cancle);
        }
    }
    void OnClickConfirmBtn()
    {
        if (autoClose)
        {
            Close();
        }
        if (callBack != null)
        {
            callBack(PrompButtonClick.OK);
        }
    }


    void OnEnable()
    {
        GameEventMgr.Instance.AddListener(GameEventList.AdventureConditionCountChange, OnAdventureConditionCountChangeNotify);
    }
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.AdventureConditionCountChange, OnAdventureConditionCountChangeNotify);
    }
}
