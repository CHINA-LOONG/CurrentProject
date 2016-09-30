using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PvpScoreInfo : MonoBehaviour {

    public Text mNickname;
    public Image mHeadImg;
    public Text mBp;
    public Text mPvpScore;
    public Text mPvpMoney;
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPvpInfo(
        string nickname,
        int bp,
        bool isself,
        PB.HSPVPSettleRet pvpScore
        )
    {
        mNickname.text = nickname;
        mBp.text = bp.ToString();
        if (isself == true)
        {
            mPvpScore.text = pvpScore.point.ToString();
            mPvpMoney.gameObject.SetActive(true);
            mPvpMoney.text = pvpScore.rewardPoint.ToString();
        }
        else
        {
            mPvpMoney.gameObject.SetActive(false);
            mPvpScore.text = (pvpScore.point * -1).ToString();
        }

        //TODO: rank
    }
}
