using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatySubTaskItem : MonoBehaviour
{
    public Image iconImage;
    public Image selectImage;
    public Image finishImage;
    public Button itemButton;

    public PB.AllianceTeamQuestInfo questInfo;

    void    Start()
    {
        itemButton.onClick.AddListener(OnItemButtonCllick);
    }
    public  static  SociatySubTaskItem CreateWith(PB.AllianceTeamQuestInfo teamQuestInfo)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatySubTaskItem");
        var item = go.GetComponent<SociatySubTaskItem>();
        item.RefreshWith(teamQuestInfo);
        return item;
    }

    public void RefreshWith(PB.AllianceTeamQuestInfo teamQuestInfo)
    {
        questInfo = teamQuestInfo;

        SociatyQuest questData = StaticDataMgr.Instance.GetSociatyQuest(questInfo.questId);
        if (null == questData)
            return;
        Sprite iconSp = ResourceMgr.Instance.LoadAssetType<Sprite>(questData.icon);
        if(null != iconSp)
        {
            iconImage.sprite = iconSp;
        }
        finishImage.gameObject.SetActive(questInfo.playerId > 0);
        SetSelect(false);
    }

    public void SetSelect(bool isselect)
    {
        selectImage.gameObject.SetActive(isselect);
    }
	
    public void SetFinish(int playerId)
    {
        questInfo.playerId = playerId;
        finishImage.gameObject.SetActive(true);
    }
    void OnItemButtonCllick()
    {
        //if(questInfo.playerId < 1)
        //{
            SociatyTaskRunning.Instance.OnSubTaskItemSelected(this);
       // }
    }
}
