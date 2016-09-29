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

    public static   PvpRankItem CreateWith()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("PvpRankItem");
        PvpRankItem item = go.GetComponent<PvpRankItem>();
        item.InitWith();
        return item;
    }
	// Use this for initialization
	void Start ()
    {
        itemButton.onClick.AddListener(OnItemButtonClick);
	}

    public void InitWith()
    {

    }
    void OnItemButtonClick()
    {
        PvpOtherDefenseInfo.OpenWith();
    }
}
