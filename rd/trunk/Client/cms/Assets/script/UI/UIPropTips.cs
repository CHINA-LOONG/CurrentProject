using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPropTips : UIBase {

    public static string ViewName = "UIPropTips";

    public Transform iconPos;
    public Text propName;
    public Text typeTips;
    public Text propType;
    public Text Requirement;
    public Text propRequire;
    public Text propDescription;
    public GameObject mask;

    public GameObject objGetBy;
    public Text text_GetBy;
    public Transform content;

    public static UIPropTips openPropTips(string propId, bool getBy = false)
    {
        UIPropTips propTips = UIMgr.Instance.OpenUI_(UIPropTips.ViewName, false) as UIPropTips;
        propTips.PropTips(propId, getBy);
        return propTips;
    }
    void Start() 
    {
        EventTriggerListener.Get(mask).onClick = CloseOnClick;
        Requirement.text = StaticDataMgr.Instance.GetTextByID("item_demandLevel");
        typeTips.text = StaticDataMgr.Instance.GetTextByID("item_type");

        text_GetBy.text = StaticDataMgr.Instance.GetTextByID("handbook_huodeway");

    }
    void PropTips(string propId, bool getBy)
    {
        ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(propId);
        if (ItemData.valueof(propId, 1) != null)
        {
            ItemIcon icon = ItemIcon.CreateItemIcon(ItemData.valueof(propId, 1), false);
            icon.transform.SetParent(iconPos, false);
        }
        propName.text = StaticDataMgr.Instance.GetTextByID(itemData.name);
        Outline outline = propName.GetComponent<Outline>();
        outline.effectColor = ColorConst.GetStageOutLineColor(itemData.grade);
        propName.color = ColorConst.GetStageTextColor(itemData.grade);
        if (itemData.minLevel > 0)
            propRequire.text = itemData.minLevel.ToString();
        else
            Requirement.gameObject.SetActive(false);
        if (itemData.type == 1)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_item");
        else if (itemData.type == 2)
            propType.text = StaticDataMgr.Instance.GetTextByID("item_type_chip");
        else if (itemData.type == 3)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_gem");
        else if (itemData.type == 4)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_box");
        else if (itemData.type == 5)
            propType.text = StaticDataMgr.Instance.GetTextByID("bag_tag_use");
        propDescription.text = StaticDataMgr.Instance.GetTextByID(itemData.tips);

        //get by
        if (!getBy || itemData.FoundList.Count <= 0)
        {
            objGetBy.SetActive(false);
        }
        else
        {
            objGetBy.SetActive(true);
            List<List<string>> list = itemData.FoundList;
            for (int i = 0; i < list.Count; i++)
            {
                FoundItem found = FoundItem.CreateItem(list[i],this);
                UIUtil.SetParentReset(found.transform, content);
            }
        }

    }
    void CloseOnClick(GameObject but)
    {
        UIMgr.Instance.DestroyUI(this);
    }
}
