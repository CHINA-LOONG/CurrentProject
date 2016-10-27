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
    public Image huoliImage;

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
            huoliImage.gameObject.SetActive(true);
            huoliText.text = string.Format("{0}", instanceData.staticData.fatigue);
            if(instanceData.staticData.fatigue > GameDataMgr.Instance.PlayerDataAttr.HuoliAttr)
            {
                huoliText.color = new Color(1, 0, 0);
            }
            else
            {
                huoliText.color = new Color(96/255.0f, 76/255.0f, 51/255.0f);
            }
        }
        else
        {
            huoliImage.gameObject.SetActive(false);
            huoliText.text = "";
        }
        szStar[0].gameObject.SetActive(instanceData.star > 0);
        szStar[1].gameObject.SetActive(instanceData.star > 1);
        szStar[2].gameObject.SetActive(instanceData.star > 2);

        szGrewStar[0].gameObject.SetActive(false);
        szGrewStar[1].gameObject.SetActive(instanceData.star == 1);
        szGrewStar[2].gameObject.SetActive(instanceData.star > 0 && instanceData.star < 3);

        Sprite sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(insData.staticData.bgzhuangshi);
        if(null != sprite)
        {
            zhuangshiImage.sprite = sprite;
        }

    }

   public void OnItemClicked(GameObject go)
    {
        if(instanceData.isOpen)
        {
            if(GameDataMgr.Instance.PlayerDataAttr.HuoliAttr >= instanceData.staticData.fatigue)
            {
                UIAdjustBattleTeam.OpenWith(instanceData.instanceId, instanceData.star,true);
            }
           else
            {
                UseHuoLi.Open();
            }
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("instanceselect_open_003"), (int)PB.ImType.PROMPT);
        }
    }
}
