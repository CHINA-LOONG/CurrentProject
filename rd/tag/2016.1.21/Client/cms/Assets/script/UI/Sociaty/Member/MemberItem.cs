using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MemberItem : MonoBehaviour
{

    public Text nameText;
    public Text levelText;
    public Text positionText;
    public Text historyContributionText;
    public Text lastLogin;
    public GiveHuoliButton giveHuoliButton;
    public Button itemButton;

    private PB.AllianceMember memberData;
    public  static  MemberItem CreateWith(PB.AllianceMember memberData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("MemberItem");
        var item = go.GetComponent<MemberItem>();
        item.RefreshWith(memberData);
        return item;
    }

	// Use this for initialization
	void Start ()
    {
        giveHuoliButton.itemButton.onClick.AddListener(OnGiveHuoliButtonClick);
        itemButton.onClick.AddListener(OnItemButtonClick);
    }
	
    public  void    RefreshWith(PB.AllianceMember memberData)
    {
        this.memberData = memberData;
        nameText.text = memberData.name;
        levelText.text = memberData.level.ToString();
        positionText.text = GameDataMgr.Instance.SociatyDataMgrAttr.GetPositionDesc(memberData.postion);
        historyContributionText.text = memberData.contribution.ToString();
        RefreshLogOutTime();
        bool isSend = memberData.sendFatigue;
        giveHuoliButton.SetIsSend(isSend);
        if (memberData.id == GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData.id)
        {
            isSend = true;
            giveHuoliButton.HideAll();
        }
        
    }

    void RefreshLogOutTime()
    {
        string logOutMsg = "";
        int loginTime = memberData.loginTime;
        int logoutTime = memberData.logoutTime;
        if(loginTime > logoutTime)
        {
            logOutMsg = StaticDataMgr.Instance.GetTextByID("sociaty_zaixian");
        }
        else
        {
            int curTime = GameTimeMgr.Instance.GetServerTimeStamp();

            int delthaTime = curTime - logoutTime;
            if (delthaTime < 60)//1min
            {
                logOutMsg = StaticDataMgr.Instance.GetTextByID("sociaty_gang");
            }
            else if (delthaTime < 3600)//1hour
            {
                logOutMsg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_minite"), delthaTime / 60);
            }
            else if (delthaTime < 3600 * 24)//1day
            {
                logOutMsg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_hour"), delthaTime / 3600);
            }
            else
            {
                logOutMsg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_day"),delthaTime/(3600 * 24));
            }
        }
        lastLogin.text = logOutMsg;
    }

    void OnGiveHuoliButtonClick()
    {
        GameDataMgr.Instance.SociatyDataMgrAttr.RequestSendHuoli(memberData.id, OnSendHuoliFinish);
    }

    void OnSendHuoliFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        giveHuoliButton.SetIsSend(true);
        memberData.sendFatigue = true;
    }

    void OnItemButtonClick()
    {
        if(GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData.postion != 0 &&
            GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData.id != memberData.id)
        {
            MemberInfo.OpenWith(memberData);
        }
    }
}
