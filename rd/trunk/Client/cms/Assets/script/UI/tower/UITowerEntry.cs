using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITowerEntry : UIBase
{
    public static string ViewName = "UITowerEntry";
    public Button mReturnBtn;

    public Text mShilianText;
    public Text mJuewangText;
    public Text mSiwangText;

    private MainStageController mMainStageControl;

    //---------------------------------------------------------------------------------------------
    public override void Init()
    {
        TowerData towerShilian = StaticDataMgr.Instance.GetTowerData("tower_shilian");
        if (towerShilian != null)
        {
            mShilianText.text = string.Format("tower:shilian lv:{0}", towerShilian.level);
        }

        TowerData towerJuewang = StaticDataMgr.Instance.GetTowerData("tower_juewang");
        if (towerJuewang != null)
        {
            mJuewangText.text = string.Format("tower:juewang lv:{0}", towerJuewang.level);
        }

        TowerData towerSiwang = StaticDataMgr.Instance.GetTowerData("tower_siwang");
        if (towerSiwang != null)
        {
            mSiwangText.text = string.Format("tower:siwang lv:{0}", towerSiwang.level);
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
    void Start()
    {
        EventTriggerListener.Get(mReturnBtn.gameObject).onClick = OnReturn;
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

    }
    //---------------------------------------------------------------------------------------------
}
