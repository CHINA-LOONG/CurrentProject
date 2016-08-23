using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyTeamMemberItem : MonoBehaviour
{
    public Image memberIcon;
    public GameObject leaderObject;
    public Text memberLevelText;
    public GameObject tipsObject;
    public Text tipsText;

    private PB.AllianceTeamMemInfo teamMemberData;
    private bool isShowTips = false;
    public static SociatyTeamMemberItem CreateWith(PB.AllianceTeamMemInfo memberData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTeamMemberItem");
        var item = go.GetComponent<SociatyTeamMemberItem>();
        item.FirstInit();
        item.RefreshWith(memberData);
        return item;
    }
   void Start()
    {
        EventTriggerListener.Get(gameObject).onEnter = OnTouchOnEnter;
        EventTriggerListener.Get(gameObject).onExit = OnTouchOnExit;
    }
    public void FirstInit()
    {
        tipsObject.SetActive(false);
        SetShowTips(false);
    }
	public void RefreshWith(PB.AllianceTeamMemInfo memberData)
    {
        this.teamMemberData = memberData;
        if(memberData.playerId == GameDataMgr.Instance.PlayerDataAttr.playerId)
        {
            memberLevelText.text = GameDataMgr.Instance.PlayerDataAttr.LevelAttr.ToString();
        }
        else
        {
            memberLevelText.text = memberData.level.ToString();
        }
        leaderObject.SetActive(memberData.isCaptain);
        tipsText.text = memberData.nickname;
    }

    public void SetShowTips(bool isShow)
    { 
        isShowTips = isShow;
    }

    void OnTouchOnEnter(GameObject go)
    {
        if(isShowTips)
        {
            tipsObject.SetActive(true);
        }
    }

    void OnTouchOnExit(GameObject go)
    {
        tipsObject.SetActive(false);
    }
}
