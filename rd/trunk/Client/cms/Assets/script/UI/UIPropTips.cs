using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPropTips : UIBase {

    public GameObject propImage;
    public GameObject Requirement;
    public Text propName;
    public Text propType;
    public Text propRequire;
    public Text propDescription;
    public Text typeTips;
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
        Requirement.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_demand_level") + ":";
        typeTips.text = StaticDataMgr.Instance.GetTextByID("item_type");

    }
    void CloseOnClick(GameObject but)
    {
        UIMgr.Instance.DestroyUI(this);
    }
    public void PropTips(string propId)
    {
        ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(propId);
        if (ItemData.valueof(propId, 1) != null)
        {
            ItemIcon icon = ItemIcon.CreateItemIcon(ItemData.valueof(propId, 1));
            icon.transform.SetParent(propImage.transform, false);
        }
        propName.text = StaticDataMgr.Instance.GetTextByID(itemData.name);
        if (itemData.minLevel > 0)
            propRequire.text = itemData.minLevel.ToString();
        else
            Requirement.SetActive(false);
        if (itemData.type == 1)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_item");
        else if(itemData.type == 2)
            propType.text = StaticDataMgr.Instance.GetTextByID("item_type_chip");
        else if (itemData.type == 3)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_gem");
        else if (itemData.type == 4)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_box");
        else if (itemData.type == 5)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_use");
        propDescription.text = StaticDataMgr.Instance.GetTextByID(itemData.tips);
    }
}
