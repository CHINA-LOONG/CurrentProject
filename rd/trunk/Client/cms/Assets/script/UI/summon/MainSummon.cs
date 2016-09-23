using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSummon : UIBase
{
    public static string ViewName = "UISummon1";
    UISummon uiSummon = null;
    bool consume;
    public Text jinbiSumName;
    public Text zuanshiSumName;
    public Text sumIntroduce1;
    public Text sumIntroduce2;
    public GameObject jinbiButton;
    public GameObject zuanshiButton;
    public GameObject freeImage1;
    public GameObject freeImage2;
    public GameObject jinbi;
    public GameObject zuanshi;
    public GameObject close;
    private MainStageController mMainStageControl;
    //---------------------------------------------------------------------------------------------
    public void SetMainStageControl(MainStageController control)
    {
        mMainStageControl = control;
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
    }
    //---------------------------------------------------------------------------
    void Exit(GameObject go)
    {
        UIMgr.Instance.CloseUI_(MainSummon.ViewName);
         mMainStageControl.QuitSelectGroup();
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
	void Start ()
    {
        EventTriggerListener.Get(jinbiButton).onClick = JinbiSummon;
        EventTriggerListener.Get(zuanshiButton).onClick = ZhuanshiSummon;
        EventTriggerListener.Get(close).onClick = Exit;
        jinbiSumName.text = StaticDataMgr.Instance.GetTextByID("summon_titlecoin");
        zuanshiSumName.text = StaticDataMgr.Instance.GetTextByID("summon_titlegold");
        sumIntroduce1.text = StaticDataMgr.Instance.GetTextByID("summon_tipscoin");
        sumIntroduce2.text = StaticDataMgr.Instance.GetTextByID("summon_tipsgold");
        freeImage1.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("summon_free");
        freeImage2.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("summon_free");
        if ((GameDataMgr.Instance.summonJinbi + GameConfig.Instance.jinBiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
        {
            SetFree(true, freeImage1, jinbi);
        }
        else
        {
            SetFree(false, freeImage1, jinbi);
        }
        if ((GameDataMgr.Instance.summonZuanshi + GameConfig.Instance.zuanShiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
        {
            SetFree(true, freeImage2, zuanshi);
        }
        else
        {
            SetFree(false, freeImage2, zuanshi);
        }
	}
    //---------------------------------------------------------------------------
}
