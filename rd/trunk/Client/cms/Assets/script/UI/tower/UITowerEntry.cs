using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public enum TowerType
{
    Tower_Shilian = 1,
    Tower_Juewang,
    Tower_Siwang,

    Num_Tower_Type
}
public class UITowerEntry : UIBase
{
    public static string ViewName = "UITowerEntry";
    public Button mReturnBtn;

    public Text mRemainTime;
    public Text mShilianText;
    public Text mShilinCostText;
    public Text mJuewangText;
    public Text mJuewangCostText;
    public Text mSiwangText;
    public Text mSiwangCostText;

    private MainStageController mMainStageControl;

    //---------------------------------------------------------------------------------------------
    public override void Init(bool forbidGuide = false)
    {
        base.Init(forbidGuide);
        TowerData towerShilian = StaticDataMgr.Instance.GetTowerData((int)TowerType.Tower_Shilian);
        if (towerShilian != null)
        {
            mShilianText.text = string.Format("shilian lv:{0}", towerShilian.level);
            mShilinCostText.text = GetCost(towerShilian);
        }

        TowerData towerJuewang = StaticDataMgr.Instance.GetTowerData((int)TowerType.Tower_Juewang);
        if (towerJuewang != null)
        {
            mJuewangText.text = string.Format("juewang lv:{0}", towerJuewang.level);
            mJuewangCostText.text = GetCost(towerJuewang);
        }

        TowerData towerSiwang = StaticDataMgr.Instance.GetTowerData((int)TowerType.Tower_Siwang);
        if (towerSiwang != null)
        {
            mSiwangText.text = string.Format("siwang lv:{0}", towerSiwang.level);
            mSiwangCostText.text = GetCost(towerSiwang);
        }
        RefreshTimeCountDown();
    }
    //---------------------------------------------------------------------------------------------
    private string GetCost(TowerData towerData)
    {
        int cost = 0;
        if (towerData.floorList.Count > 0)
        {
            string instanceID = towerData.floorList[0];
            InstanceEntry entry = StaticDataMgr.Instance.GetInstanceEntry(instanceID);
            if (entry != null)
            {
                cost = entry.fatigue;
            }
        }

        return cost.ToString();
    }
    //---------------------------------------------------------------------------------------------
    public void RefreshTimeCountDown()
    {
        DateTime serverDateTime = GameTimeMgr.Instance.GetServerDateTime();
        int daysInMonth = DateTime.DaysInMonth(serverDateTime.Year, serverDateTime.Month);
        if (serverDateTime.Day < daysInMonth)
        {
            mRemainTime.text = string.Format("remain:{0}d", daysInMonth - serverDateTime.Day + 1);
        }
        else
        {
            int remainHour = 24 - serverDateTime.Hour;
            if (remainHour <= 1)
            {
                mRemainTime.text = "remain:no 1h";
            }
            else
            {
                mRemainTime.text = string.Format("remain:{0}h", remainHour - 1);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public override void Clean()
    {

    }
    //---------------------------------------------------------------------------------------------
    public void SetMainStageControl(MainStageController control)
    {
        mMainStageControl = control;
    }
    //---------------------------------------------------------------------------------------------
    void OnReturn(GameObject go)
    {
        RequestCloseUi();
    }
    public override void CloseUi()
    {
        mMainStageControl.QuitSelectGroup();
    }
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(mReturnBtn.gameObject).onClick = OnReturn;
    }
    //---------------------------------------------------------------------------------------------
}
