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
    public Text mShilianName;
    public Text mShilinCostText;
    public Text mShilinFloor;
    public Text mJuewangText;
    public Text mJuewangName;
    public Text mJuewangCostText;
    public Text mJuewangFloor;
    public Text mSiwangText;
    public Text mSiwangName;
    public Text mSiwangCostText;
    public Text mSiwangFloor;
    public Text mTitle;
    public Text mTowerStoreName;
    public GameObject mTowerStore;
    private MainStageController mMainStageControl;

    //---------------------------------------------------------------------------------------------
    public override void Init(bool forbidGuide = false)
    {
        base.Init(forbidGuide);
        TowerData towerShilian = StaticDataMgr.Instance.GetTowerData((int)TowerType.Tower_Shilian);
        if (towerShilian != null)
        {
            mShilianText.text = string.Format("Lv:{0}", towerShilian.level);
            mShilinCostText.text = GetCost(towerShilian);
        }

        TowerData towerJuewang = StaticDataMgr.Instance.GetTowerData((int)TowerType.Tower_Juewang);
        if (towerJuewang != null)
        {
            mJuewangText.text = string.Format("Lv:{0}", towerJuewang.level);
            mJuewangCostText.text = GetCost(towerJuewang);
        }

        TowerData towerSiwang = StaticDataMgr.Instance.GetTowerData((int)TowerType.Tower_Siwang);
        if (towerSiwang != null)
        {
            mSiwangText.text = string.Format("Lv:{0}", towerSiwang.level);
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
            mRemainTime.text = string.Format(StaticDataMgr.Instance.GetTextByID("towerBoss_instance_time1"), daysInMonth - serverDateTime.Day + 1);
        }
        else
        {
            int remainHour = 24 - serverDateTime.Hour;
            if (remainHour <= 1)
            {
                mRemainTime.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_time3");
            }
            else
            {
                mRemainTime.text = string.Format(StaticDataMgr.Instance.GetTextByID("towerBoss_instance_time2"), remainHour - 1);
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
    void OpenTowerStore(GameObject go)
    {
        FoundMgr.Instance.GoToUIShop(PB.shopType.TOWERSHOP);
    }
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(mReturnBtn.gameObject).onClick = OnReturn;
        EventTriggerListener.Get(mTowerStore).onClick = OpenTowerStore;
        mTitle.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_title");
        mShilianName.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_name1");
        mJuewangName.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_name2");
        mSiwangName.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_name3");
        mTowerStoreName.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_shop");
        mShilinFloor.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_floor1") + GameDataMgr.Instance.curTowerShilianFloor;
        mJuewangFloor.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_floor2") + GameDataMgr.Instance.curTowerJuewangFloor;
        mSiwangFloor.text = StaticDataMgr.Instance.GetTextByID("towerBoss_instance_floor3") + GameDataMgr.Instance.curTowerSiwangFloor;
    }
    //---------------------------------------------------------------------------------------------
}
