using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InstanceItem : MonoBehaviour
{
    public Text nameText;
    public Text huoliText;
    public RectTransform[] szStar = new RectTransform[3];
    public RectTransform[] szGrewStar = new RectTransform[3];
    public Image itemButton;
    public RectTransform lockPanel;
    public Image zhuangshiImage;

    private InstanceEntryRuntimeData instanceData;

	// Use this for initialization
	void Start ()
    {
        ScrollViewEventListener.Get(itemButton.gameObject).onClick = OnItemClicked;
	}

    public  void    RefreshWith(InstanceEntryRuntimeData insData)
    {
        instanceData = insData;
        nameText.text = instanceData.staticData.NameAttr;
        lockPanel.gameObject.SetActive(!instanceData.isOpen);

        if(insData.isOpen)
        {
            huoliText.text = string.Format(StaticDataMgr.Instance.GetTextByID("{0}疲劳值"), instanceData.staticData.fatigue);
        }
        else
        {
            huoliText.text = "";
        }
        szStar[0].gameObject.SetActive(instanceData.star > 0);
        szStar[1].gameObject.SetActive(instanceData.star > 1);
        szStar[2].gameObject.SetActive(instanceData.star > 2);

        szGrewStar[0].gameObject.SetActive(false);
        szGrewStar[1].gameObject.SetActive(instanceData.star == 1);
        szGrewStar[2].gameObject.SetActive(instanceData.star == 2);

        Sprite sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(insData.staticData.bgzhuangshi);
        if(null != sprite)
        {
            zhuangshiImage.sprite = sprite;
        }

    }

    void OnItemClicked(GameObject go)
    {
       // GameEventMgr.Instance.FireEvent<int, string>(GameEventList.FinishedInstance, 3, instanceData.instanceId);
        UIAdjustBattleTeam.OpenWith(instanceData.instanceId, instanceData.staticData.enemyList, 1);
        InstanceList.Close();
    }
}
