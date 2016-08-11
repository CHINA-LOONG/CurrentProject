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
        lastLogin.text = memberData.loginTime.ToString();//todo
    }
    void OnGiveHuoliButtonClick()
    {

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
