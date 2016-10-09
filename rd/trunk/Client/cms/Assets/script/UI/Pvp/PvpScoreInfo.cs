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
        bool isself,
        int scoreGet,
        PB.HSPVPSettleRet pvpScore,
        PvpFightParam pvpParam
        )
    {
        //mBp.text = bp.ToString();
        if (isself == true)
        {
            mNickname.text = GameDataMgr.Instance.PlayerDataAttr.nickName;
            if (scoreGet >= 0)
            {
                mPvpScore.text = string.Format("+{0}", scoreGet);
            }
            else
            {
                mPvpScore.text = scoreGet.ToString();
            }
            mPvpMoney.gameObject.SetActive(true);
            mPvpMoney.text = string.Format("+{0}", pvpScore.rewardPoint);
            mBp.text = pvpParam.myBp.ToString();
        }
        else
        {
            mNickname.text = pvpParam.targetData.name;
            mPvpMoney.gameObject.SetActive(false);
            if (scoreGet >= 0)
            {
                mPvpScore.text = string.Format("-{0}", scoreGet);
            }
            else
            {
                mPvpScore.text = string.Format("+{0}", scoreGet * -1);
            }
            mBp.text = pvpParam.enemyBp.ToString();
        }
    }
}
