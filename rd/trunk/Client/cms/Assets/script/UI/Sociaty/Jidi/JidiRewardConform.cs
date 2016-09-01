using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JidiRewardConform : UIBase
{
    public static string ViewName = "JidiRewardConform";

    public Text zhuzhaLabelText;
    public Text zhuzhaValueText;
    public Text maoxianLabelText;
    public Text maoxianValueText;

    public Button conformButton;

    public static void OpenWith(int zhushouReward, int maoxianReward)
    {
        JidiRewardConform conformUi = (JidiRewardConform)UIMgr.Instance.OpenUI_(ViewName);
        conformUi.RefreshWith(zhushouReward, maoxianReward);
    }
    // Use this for initialization
    void Start ()
    {
        conformButton.onClick.AddListener(OnConformButtonClick);
	}

    public void RefreshWith(int zhushouReward,int maoxianReward)
    {
        zhuzhaValueText.text = zhushouReward.ToString();
        maoxianValueText.text = maoxianReward.ToString();
    }

	void OnConformButtonClick ()
    {
        UIMgr.Instance.CloseUI_(this);
	}
}
