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

    public GameObject topTipsObject;
    public Text topTipsText;

    private PB.AllianceTeamMemInfo teamMemberData;
    private bool tipsOnTop = true;
    public static SociatyTeamMemberItem CreateWith(PB.AllianceTeamMemInfo memberData,bool tipsOnTop)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTeamMemberItem");
        var item = go.GetComponent<SociatyTeamMemberItem>();
        item.FirstInit(tipsOnTop);
        item.RefreshWith(memberData);
        return item;
    }
   void Start()
    {
        EventTriggerListener.Get(gameObject).onEnter = OnTouchOnEnter;
        EventTriggerListener.Get(gameObject).onExit = OnTouchOnExit;
    }
    public void FirstInit(bool isOnTop)
    {
        this.tipsOnTop = isOnTop;
        tipsObject.SetActive(false);
        topTipsObject.SetActive(false);
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
        topTipsText.text = memberData.nickname;
    }

    void OnTouchOnEnter(GameObject go)
    {
        if(tipsOnTop)
        {
            topTipsObject.SetActive(true);
        }
        else
        {
            tipsObject.SetActive(true);
        }
    }

    void OnTouchOnExit(GameObject go)
    {
        tipsObject.SetActive(false);
        topTipsObject.SetActive(false);
    }
}
