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
        int hornorGet,
        PB.HSPVPSettleRet pvpScore,
        PvpFightParam pvpParam
        )
    {
        //mBp.text = bp.ToString();
        if (isself == true)
        {
            mNickname.text = GameDataMgr.Instance.PlayerDataAttr.nickName;
            if (pvpScore.rewardPoint >= 0)
            {
                mPvpScore.text = string.Format("+{0}", pvpScore.rewardPoint);
            }
            else
            {
                mPvpScore.text = pvpScore.rewardPoint.ToString();
            }
            mPvpMoney.gameObject.SetActive(true);
            if (hornorGet >= 0)
            {
                mPvpMoney.text = string.Format("+{0}", hornorGet);
            }
            else
            {
                mPvpMoney.text = string.Format("-{0}", hornorGet);
            }
            mBp.text = pvpParam.myBp.ToString();
        }
        else
        {
            mNickname.text = pvpParam.targetData.name;
            mPvpMoney.gameObject.SetActive(false);
            if (pvpScore.rewardPoint >= 0)
            {
                mPvpScore.text = string.Format("-{0}", pvpScore.rewardPoint);
            }
            else
            {
                mPvpScore.text = string.Format("+{0}", pvpScore.rewardPoint * -1);
            }
            mBp.text = pvpParam.enemyBp.ToString();
        }
    }
}
