using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum HoleType
{
    Hole_Exp = 1,
    Hole_Jingbi,

    Num_Hole_Type
}

public class UIHoleEntry : UIBase
{
    public static string ViewName = "UIHoleEntry";
    public Button mReturnBtn;
    //jinbi
    public Text mJinbiStartTimeText;
    public Text mJinbiRemainCountText;
    public Text mJinbiNameText;
    public Text mJinbiDrop;
    public RectTransform mJinbiDropList;
    //jinyan
    public Text mExpStartTimeText;
    public Text mExpRemainCountText;
    public Text mExpNameText;
    public Text mExpDrop;
    public RectTransform mExpDropList;

    public Text mTitle;


    private HoleData mHoleJinbiData;
    private HoleData mHoleExpData;
    private MainStageController mMainStageControl;

    //---------------------------------------------------------------------------------------------
    public void RefreshUI()
    {
        //mHoleJinbiData = StaticDataMgr.Instance.GetHoleData((int)HoleType.Hole_Jingbi);
        //if (mHoleJinbiData != null)
        //{
        //    mJinbiStartTimeText.text = mHoleJinbiData.openId;
        //    mJinbiNameText.text = mHoleJinbiData.id.ToString();
        //    int finishCount = GameDataMgr.Instance.GetHoleDailyCount((int)HoleType.Hole_Jingbi);
        //    mJinbiRemainCountText.text = finishCount.ToString() + "/" + mHoleJinbiData.count.ToString();
        //}

        //mHoleExpData = StaticDataMgr.Instance.GetHoleData((int)HoleType.Hole_Exp);
        //if (mHoleExpData != null)
        //{
        //    mExpStartTimeText.text = mHoleExpData.openId;
        //    mExpNameText.text = mHoleExpData.id.ToString();
        //    int finishCount = GameDataMgr.Instance.GetHoleDailyCount((int)HoleType.Hole_Exp);
        //    mExpRemainCountText.text = finishCount.ToString() + "/" + mHoleExpData.count.ToString();
        //}
    }
    //---------------------------------------------------------------------------------------------
    public override void Init(bool forbidGuide = false)
    {
        base.Init();
        RefreshUI();
    }
    //---------------------------------------------------------------------------------------------
    public override void Clean()
    {

    }
    //---------------------------------------------------------------------------------------------
    public bool IsDailyCountFull(HoleType holeType)
    {
        if (holeType == HoleType.Hole_Exp)
        {
            return mHoleExpData.count <= GameDataMgr.Instance.GetHoleDailyCount((int)holeType);
        }
        else if (holeType == HoleType.Hole_Jingbi)
        {
            return mHoleJinbiData.count <= GameDataMgr.Instance.GetHoleDailyCount((int)holeType);
        }
        return true;
    }
    //---------------------------------------------------------------------------------------------
    public void SetMainStageControl(MainStageController control)
    {
        mMainStageControl = control;
    }
    //---------------------------------------------------------------------------------------------
    void OnReturn(GameObject go)
    {
        base.RequestCloseUi(true);
    }
    //---------------------------------------------------------------------------------------------
    public override void CloseUi()
    {
        mMainStageControl.QuitSelectGroup();
    }
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(mReturnBtn.gameObject).onClick = OnReturn;
        mTitle.text = StaticDataMgr.Instance.GetTextByID("tower_instance_title");
        mJinbiNameText.text = StaticDataMgr.Instance.GetTextByID("tower_gold_name");
        mExpNameText.text = StaticDataMgr.Instance.GetTextByID("tower_exp_name");

        mHoleJinbiData = StaticDataMgr.Instance.GetHoleData((int)HoleType.Hole_Jingbi);
        if (mHoleJinbiData != null)
        {
            mJinbiStartTimeText.text = StaticDataMgr.Instance.GetTextByID(mHoleJinbiData.openId);
            mJinbiDrop.text = StaticDataMgr.Instance.GetTextByID("tower_instance_drop");
            int finishCount = GameDataMgr.Instance.GetHoleDailyCount((int)HoleType.Hole_Jingbi);
            mJinbiRemainCountText.text = string.Format(StaticDataMgr.Instance.GetTextByID("tower_instance_count"), finishCount.ToString());
            DropProp(mHoleJinbiData, false);
        }

        mHoleExpData = StaticDataMgr.Instance.GetHoleData((int)HoleType.Hole_Exp);
        if (mHoleExpData != null)
        {
            mExpStartTimeText.text = StaticDataMgr.Instance.GetTextByID(mHoleExpData.openId);
            mExpDrop.text = StaticDataMgr.Instance.GetTextByID("tower_instance_drop");
            int finishCount = GameDataMgr.Instance.GetHoleDailyCount((int)HoleType.Hole_Exp);
            mExpRemainCountText.text = string.Format(StaticDataMgr.Instance.GetTextByID("tower_instance_count"), finishCount.ToString());
            DropProp(mHoleExpData, true);
        }
    }
    //---------------------------------------------------------------------------------------------
    void DropProp(HoleData data, bool isExp)
    {
        RewardData rewardData;
        RewardItemData rewardItemData;
        GameObject go = null;
        rewardData = StaticDataMgr.Instance.GetRewardData(data.dropId);
        if (rewardData != null)
        {
            for (int j = 0; j < rewardData.itemList.Count; j++)
            {
                rewardItemData = rewardData.itemList[j];
                if (isExp)
                    go = RewardItemCreator.CreateRewardItem(rewardItemData.protocolData, mExpDropList, false, false);
                else
                    go = RewardItemCreator.CreateRewardItem(rewardItemData.protocolData, mJinbiDropList, false, false);
                if (go != null)
                    go.transform.localScale = mExpDropList.localScale;
            }
        }   
    }
}
