using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyTeamMemberItem : MonoBehaviour
{
    public Image memberIcon;
    public GameObject leaderObject;
    public Text memberLevelText;

    private PB.AllianceTeamMemInfo teamMemberData;

    public static SociatyTeamMemberItem CreateWith(PB.AllianceTeamMemInfo memberData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTeamMemberItem");
        var item = go.GetComponent<SociatyTeamMemberItem>();
        item.RefreshWith(memberData);
        return item;
    }

	public void RefreshWith(PB.AllianceTeamMemInfo memberData)
    {
        this.teamMemberData = memberData;
        memberLevelText.text = memberData.level.ToString();
        leaderObject.SetActive(memberData.isCaptain);
    }
}
