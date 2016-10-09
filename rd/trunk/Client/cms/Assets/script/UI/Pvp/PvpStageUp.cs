using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PvpStageUp : UIBase {

    public static string ViewName = "UIPvpStageUp";
    public Text mTitleText;
    public Text mStageText;
    public float mAutoDeleteTime;
    //TODO: duplicate code as pvpmain
    public GameObject[] mStageImage;
    public GameObject[] mStageEffect;
    private float mAutoEndTime;
    //---------------------------------------------------------------------------------------------
    void Start () {
        mAutoEndTime = Time.time + mAutoDeleteTime;
        //refresh stage info
        mTitleText.text = StaticDataMgr.Instance.GetTextByID("pvp_titleshengduan");
        PvpDataMgr pvpDataMgr = GameDataMgr.Instance.PvpDataMgrAttr;
        mStageText.text = pvpDataMgr.GetStageNameWithId(pvpDataMgr.selfPvpStage);
        string stageIcon = string.Format("pvp_duanweixiao_{0}", pvpDataMgr.selfPvpStage);

        //TODO: duplicate code as pvpmain
        for (int i = 0; i < mStageImage.Length; ++i)
        {
            mStageImage[i].SetActive((i+1) == pvpDataMgr.selfPvpStage);
        }
        for (int i = 0; i < mStageImage.Length; ++i)
        {
            mStageEffect[i].SetActive(i == (pvpDataMgr.selfPvpStage - 1) / 3);
        }
    }
    //---------------------------------------------------------------------------------------------
    void Update () {
        if (Time.time >= mAutoEndTime)
        {
            UIMgr.Instance.DestroyUI(this);
        }
    }
    //---------------------------------------------------------------------------------------------
}
