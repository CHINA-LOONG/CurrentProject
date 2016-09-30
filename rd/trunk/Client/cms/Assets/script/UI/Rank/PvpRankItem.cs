using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PvpRankItem : MonoBehaviour
{
    public GameObject[] firstThreeObject;
    public Text indexText;
    public Text nameText;
    public Text lvlText;
    public Text gradelevelText;
    public Text competivePointText;

    public Button itemButton;
    private PB.PVPRankData pvpRankData;

    public static   PvpRankItem Create()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("PvpRankItem");
        PvpRankItem item = go.GetComponent<PvpRankItem>();
        return item;
    }
	// Use this for initialization
	void Start ()
    {
        itemButton.onClick.AddListener(OnItemButtonClick);
	}
    public void InitWith(PB.PVPRankData rankData)
    {
        pvpRankData = rankData;
        firstThreeObject[0].gameObject.SetActive(rankData.rank == 1);
        firstThreeObject[1].gameObject.SetActive(rankData.rank == 2);
        firstThreeObject[2].gameObject.SetActive(rankData.rank == 3);
        if (rankData.rank > 3)
        {
            indexText.text = rankData.rank.ToString();
        }
        else
        {
            indexText.text = "";
        }
        nameText.text = rankData.name;
        lvlText.text = rankData.level.ToString();
        int stage = GameDataMgr.Instance.PvpDataMgrAttr.GetPvpStageWithPoint(rankData.point);
        gradelevelText.text = GameDataMgr.Instance.PvpDataMgrAttr.GetStageNameWithId(stage);
        competivePointText.text = rankData.point.ToString();
    }
    void OnItemButtonClick()
    {
        GameDataMgr.Instance.PvpDataMgrAttr.RequestPlayerDefense(pvpRankData.playerId, OnRequestDefenseFinished);
    }
    void OnRequestDefenseFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            PvpErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSPVPRankDefenceRet msgRet = message.GetProtocolBody<PB.HSPVPRankDefenceRet>();
        PvpOtherDefenseInfo.OpenWith(msgRet.monsterDefence);
    }
}
