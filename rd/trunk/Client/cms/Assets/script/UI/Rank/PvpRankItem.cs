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
        gradelevelText.text = GameDataMgr.Instance.PvpDataMgrAttr.GetPvpStageWithPoint(rankData.point).ToString();
        competivePointText.text = rankData.point.ToString();
    }
    void OnItemButtonClick()
    {
        PvpOtherDefenseInfo.OpenWith();
    }
}
