using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContributionBox : MonoBehaviour
{
    public Image openBox;
    public Image openedBox;
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

    public void SetHasReword(bool hasReword)
    {
        hasGetReword = hasReword;
        openBox.gameObject.SetActive(!hasReword);
        openedBox.gameObject.SetActive(hasReword);
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
            Logger.LogError("contribution rewoard   faild!");
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.hasReceivContributionReword[index] = true;
        SociatyContentInfomation.Instance.RefreshContributionBox();
    }

}
