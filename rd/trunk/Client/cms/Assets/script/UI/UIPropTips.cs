using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPropTips : UIBase {

    public Image propImage;
    public Image propQuality;
    public GameObject Requirement;
    public Text propName;
    public Text propType;
    public Text propRequire;
    public Text propDescription;
    public GameObject mask;
    public static string ViewName = "UIPropTips";
    public static UIPropTips openPropTips(string propId)
    {
        UIPropTips propTips= UIMgr.Instance.OpenUI_(UIPropTips.ViewName,false)as UIPropTips;
        propTips.PropTips(propId);
        return propTips;
    }
    void Start() 
    {
        EventTriggerListener.Get(mask).onClick = CloseOnClick;
    }
    void CloseOnClick(GameObject but)
    {
        UIMgr.Instance.DestroyUI(this);
    }
    public void PropTips(string propId)
    {
        Sprite propAsset;
        ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(propId);
        propName.text = itemData.name;
        if (itemData.asset != "")
        {
            propAsset = ResourceMgr.Instance.LoadAssetType<Sprite>(itemData.asset);
            propImage.sprite = propAsset;
        }
        propAsset = ResourceMgr.Instance.LoadAssetType<Sprite>("grade_" + itemData.grade.ToString());
        propQuality.sprite = propAsset;
        if (itemData.minLevel > 0)
            propRequire.text = itemData.minLevel + StaticDataMgr.Instance.GetTextByID("levels can be used after");
        else
            Requirement.SetActive(false);
        if (itemData.type == 1)
            propType.text = StaticDataMgr.Instance.GetTextByID("porp");
        else if(itemData.type == 2)
            propType.text = StaticDataMgr.Instance.GetTextByID("fragment");
        else if (itemData.type == 3)
            propType.text = StaticDataMgr.Instance.GetTextByID("gem");
        else if (itemData.type == 4)
            propType.text = StaticDataMgr.Instance.GetTextByID("chest");
        else if (itemData.type == 5)
            propType.text = StaticDataMgr.Instance.GetTextByID("consumables");
        else if (itemData.type == 7)
            propType.text = StaticDataMgr.Instance.GetTextByID("equip");
        propDescription.text = itemData.tips;
    }
}
