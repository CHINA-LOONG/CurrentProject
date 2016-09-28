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
    public RectTransform mJinbiDropList;
    //jinyan
    public Text mExpStartTimeText;
    public Text mExpRemainCountText;
    public Text mExpNameText;
    public RectTransform mExpDropList;

    private HoleData mHoleJinbiData;
    private HoleData mHoleExpData;
    private MainStageController mMainStageControl;

    //---------------------------------------------------------------------------------------------
    public void RefreshUI()
    {
        mHoleJinbiData = StaticDataMgr.Instance.GetHoleData((int)HoleType.Hole_Jingbi);
        if (mHoleJinbiData != null)
        {
            mJinbiStartTimeText.text = mHoleJinbiData.openId;
            mJinbiNameText.text = mHoleJinbiData.id.ToString();
            int finishCount = GameDataMgr.Instance.GetHoleDailyCount((int)HoleType.Hole_Jingbi);
            mJinbiRemainCountText.text = finishCount.ToString() + "/" + mHoleJinbiData.count.ToString();
        }

        mHoleExpData = StaticDataMgr.Instance.GetHoleData((int)HoleType.Hole_Exp);
        if (mHoleExpData != null)
        {
            mExpStartTimeText.text = mHoleExpData.openId;
            mExpNameText.text = mHoleExpData.id.ToString();
            int finishCount = GameDataMgr.Instance.GetHoleDailyCount((int)HoleType.Hole_Exp);
            mExpRemainCountText.text = finishCount.ToString() + "/" + mHoleExpData.count.ToString();
        }
    }
    //---------------------------------------------------------------------------------------------
    public override void Init()
    {
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
        mMainStageControl.QuitSelectGroup();
    }
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start ()
    {
        EventTriggerListener.Get(mReturnBtn.gameObject).onClick = OnReturn;
    }
    //---------------------------------------------------------------------------------------------
}
