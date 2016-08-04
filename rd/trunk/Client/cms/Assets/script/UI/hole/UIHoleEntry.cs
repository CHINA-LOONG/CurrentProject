using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    private MainStageController mMainStageControl;

    //---------------------------------------------------------------------------------------------
    public override void Init()
    {
        HoleData holeJinbi = StaticDataMgr.Instance.GetHoleData("hole_jinbi");
        if (holeJinbi != null)
        {
            mJinbiStartTimeText.text = holeJinbi.openId;
            mJinbiNameText.text = holeJinbi.id;
            mJinbiRemainCountText.text = holeJinbi.count.ToString();
        }

        HoleData holeExp = StaticDataMgr.Instance.GetHoleData("hole_exp");
        if (holeExp != null)
        {
            mExpStartTimeText.text = holeExp.openId;
            mExpNameText.text = holeExp.id;
            mExpRemainCountText.text = holeExp.count.ToString();
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
        mMainStageControl.QuitSelectGroup();
    }
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start ()
    {
        EventTriggerListener.Get(mReturnBtn.gameObject).onClick = OnReturn;
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {

    }
    //---------------------------------------------------------------------------------------------
}
