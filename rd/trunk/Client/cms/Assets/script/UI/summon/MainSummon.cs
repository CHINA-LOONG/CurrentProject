using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSummon : UIBase,GuideBase
{
    public static string ViewName = "UISummon1";
    UISummon uiSummon = null;
    bool consume;
    public Text jinbiSumName;
    public Text zuanshiSumName;
    public Text sumIntroduce1;
    public Text sumIntroduce2;
    public Text jinbiText;
    public Text zuanshiText;
    public GameObject jinbiButton;
    public GameObject zuanshiButton;
    public GameObject freeImage1;
    public GameObject freeImage2;
    public GameObject jinbi;
    public GameObject zuanshi;
    public GameObject close;
    private MainStageController mMainStageControl;
    static MainSummon mInst = null;
    public static MainSummon Instance
    {
        get
        {
            return mInst;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetMainStageControl(MainStageController control)
    {
        mMainStageControl = control;
        //GuideManager.Instance.RequestGuide(this);
    }
    //---------------------------------------------------------------------------------------------
    void OnReturn(GameObject go)
    {
        mMainStageControl.QuitSelectGroup();
    }
    //---------------------------------------------------------------------------
    void JinbiSummon(GameObject go)
    {
        consume = true;
        ShowSummon();
    }
	//---------------------------------------------------------------------------
    void ZhuanshiSummon(GameObject go)
    {
        consume = false;
        ShowSummon();
    }
    //---------------------------------------------------------------------------
    void ShowSummon()
    {
        uiSummon = UIMgr.Instance.OpenUI_(UISummon.ViewName) as UISummon;
        uiSummon.OpenUISummon(consume);
		uiSummon. SetFreeTime();
        UIIm.Instance.HideIm(true);
    }
    //---------------------------------------------------------------------------
    void Exit(GameObject go)
    {
        RequestCloseUi();
         
    }
    public override void Init(bool forbidGuide = false)
    {
        base.Init(forbidGuide);
        if(!forbidGuide)
        {
            GuideManager.Instance.RequestGuide(this);
        }
    }
    public override void RefreshOnPreviousUIHide()
    {
        base.RefreshOnPreviousUIHide();
        GuideManager.Instance.RequestGuide(this);

    }
    public override void CloseUi()
    {
        base.CloseUi();
        mMainStageControl.QuitSelectGroup();
        UIIm.Instance.HideIm(false);
    }
    //---------------------------------------------------------------------------
    void SetFree(bool isfree,GameObject free1,GameObject free2)
    {
        if (isfree)
        {
            free1.SetActive(true);
            free2.SetActive(false);
        }
        else
        {
            free1.SetActive(false);
            free2.SetActive(true);
        }
    }
    //---------------------------------------------------------------------------
    public void SetReset()
    {
        if (GameDataMgr.Instance.freeJinbiSumNum < 5)
        {
            if ((GameDataMgr.Instance.summonJinbi + GameConfig.Instance.jinBiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
                SetFree(true, freeImage1, jinbi);
            else
                SetFree(false, freeImage1, jinbi);
        }
        else
            SetFree(false, freeImage1, jinbi);
        if ((GameDataMgr.Instance.summonZuanshi + GameConfig.Instance.zuanShiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
            SetFree(true, freeImage2, zuanshi);
        else
            SetFree(false, freeImage2, zuanshi);
        if (GameDataMgr.Instance.PlayerDataAttr.gold < GameConfig.Instance.zuanShiSum)
            zuanshiText.color = Color.red;
        else
            zuanshiText.color = ColorConst.system_color_black;
        if (GameDataMgr.Instance.PlayerDataAttr.coin < GameConfig.Instance.jinBiSum)
            jinbiText.color = Color.red;
        else
            jinbiText.color = ColorConst.system_color_black;
    }
    //---------------------------------------------------------------------------
    void Start()
    {
        mInst = this;
        EventTriggerListener.Get(jinbiButton).onClick = JinbiSummon;
        EventTriggerListener.Get(zuanshiButton).onClick = ZhuanshiSummon;
        EventTriggerListener.Get(close).onClick = Exit;
        jinbiSumName.text = StaticDataMgr.Instance.GetTextByID("summon_titlecoin");
        zuanshiSumName.text = StaticDataMgr.Instance.GetTextByID("summon_titlegold");
        sumIntroduce1.text = StaticDataMgr.Instance.GetTextByID("summon_tipscoin");
        sumIntroduce2.text = StaticDataMgr.Instance.GetTextByID("summon_tipsgold");
        freeImage1.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("summon_free");
        freeImage2.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("summon_free");
        jinbiText.text = GameConfig.Instance.jinBiSum.ToString();
        zuanshiText.text = GameConfig.Instance.zuanShiSum.ToString();
        SetReset();
    }
    //---------------------------------------------------------------------------
    void OnCoinChanged(long coin)
    {
        SetReset();
    }
    //---------------------------------------------------------------------------
    void OnZuanshiChanged(int zuanshi)
    {
        SetReset();
    }
    //---------------------------------------------------------------------------
    void OnEnable()
    {
        BindListener();
        GuideListener(true);
    }
    //---------------------------------------------------------------------------
    void OnDisable()
    {
        UnBindListener();
        GuideListener(false);
    }
    //---------------------------------------------------------------------------
    void BindListener()
    {
        GameEventMgr.Instance.AddListener<long>(GameEventList.CoinChanged, OnCoinChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.ZuanshiChanged, OnZuanshiChanged);
    }
    //---------------------------------------------------------------------------
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<long>(GameEventList.CoinChanged, OnCoinChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.ZuanshiChanged, OnZuanshiChanged);
    }
    protected override void OnGuideMessageCallback(string message)
    {
        if(message.Equals("gd_summon1_zuanshi"))
        {
            ZhuanshiSummon(null);
        }
    }
}
