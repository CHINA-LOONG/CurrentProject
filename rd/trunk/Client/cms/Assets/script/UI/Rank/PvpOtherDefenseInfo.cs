using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpOtherDefenseInfo : UIBase
{
    public static string ViewName = "PvpOtherDefenseInfo";

    public GameObject closeGameObj;
    public Text titleText;
    public Text bpLabelText;
    public Text bpValueText;

    public RectTransform[] petParentArray;

    public static void OpenWith()
    {
        PvpOtherDefenseInfo defenseInfo = UIMgr.Instance.OpenUI_(ViewName) as PvpOtherDefenseInfo;
        defenseInfo.InitWith();
    }

    public void InitWith()
    {
    }

	// Use this for initialization
	void Start ()
    {
        titleText.text = StaticDataMgr.Instance.GetTextByID("pvp_defense");
        bpLabelText.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");
        EventTriggerListener.Get(closeGameObj).onClick = OnCloseImgClick;
	}
    void OnCloseImgClick (GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }
}
