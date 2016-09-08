using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContributionBox : MonoBehaviour
{
    public BaoxiangState baoxiangState;
    public Text contributionText;
    // Use this for initialization

    private int needContribution = 0;
    private bool hasGetReword = false;
    private int index = 0;
	void Start ()
    {
        EventTriggerListener.Get(gameObject).onClick = OnBoxClick;	
	}
	
    public void SetRewordValue(int index, int needContribution)
    {
        this.needContribution = needContribution;
        contributionText.text = needContribution.ToString();
        this.index = index;
    }

    public void SetHasReword(bool hasRewarded)
    {
        hasGetReword = hasRewarded;
        if (hasGetReword)
        {
            baoxiangState.SetState(BaoxiangState.State.YiLingqu);
            return;
        }

        int sociatyContirbution = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData.contributionToday;
        if (sociatyContirbution >= needContribution)
        {
            baoxiangState.SetState(BaoxiangState.State.Kelingqu);
        }
        else
        {
            baoxiangState.SetState(BaoxiangState.State.BukeLingqu);
        }
    }

    void OnBoxClick(GameObject go)
    {
        if(hasGetReword)
        {
            return;
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(""), (int)PB.ImType.PROMPT);
        }

        int sociatyContirbution = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData.contributionToday;
        if(sociatyContirbution >= needContribution)
        {
            //请求领取奖励
            GameDataMgr.Instance.SociatyDataMgrAttr.RequestContributionReward(index, OnRequestContributionRewordFinish);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_011"), (int)PB.ImType.PROMPT);
        }
    }

    void OnRequestContributionRewordFinish(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = msg.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.hasReceivContributionReword[index] = true;
        SociatyContentInfomation.Instance.RefreshContributionBox();
    }

}
