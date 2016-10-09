using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIEquipTips : UIBase
{
    public static string ViewName = "UIEquipTips";

    public Transform iconPos;
    private ItemIcon EquipIcon;
    public Text textName;
    public Text textType;
    public Text textPart;
    public Text textBase;
    public Transform BasePos;
    public Transform GemsPos;
    public Text textNotSlot;
    public GameObject mask;

    public GameObject objGetBy;
    public Text text_GetBy;
    public Transform content;

    private string[] partLanguageId = new string[6] { "equip_Weapon", "equip_Helmet", "equip_Armor", "equip_Bracer", "equip_Ring", "equip_Accessory" };
    private string[] typeLanguageId = new string[4] { "common_type_defence", "common_type_physical", "common_type_magic", "common_type_support" };

    private EquipData curData;
    private ShowAttributesItem[] AttrList = new ShowAttributesItem[5];
    private GemSlotItem[] mosaicItems = new GemSlotItem[BattleConst.maxGemCount];


    public static UIEquipTips OpenEquipTips(EquipData data, bool getBy = false)
    {
        UIEquipTips equipTips = UIMgr.Instance.OpenUI_(UIEquipTips.ViewName, false) as UIEquipTips;
        equipTips.ReloadData(data, getBy);
        return equipTips;
    }

    void Start()
    {
        EventTriggerListener.Get(mask).onClick = CloseOnClick;
        textBase.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_attr");
        textNotSlot.text = StaticDataMgr.Instance.GetTextByID("equip_gem_NotMent");
        text_GetBy.text = StaticDataMgr.Instance.GetTextByID("handbook_huodeway");
    }

    void ReloadData(EquipData data, bool getBy = false)
    {
        curData = data;
        
        ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(curData.equipId);
        UIUtil.SetStageColor(textName, itemData.NameAttr, curData.stage, curData.level);
        textType.text = StaticDataMgr.Instance.GetTextByID(typeLanguageId[itemData.subType - 1]);
        textPart.text = StaticDataMgr.Instance.GetTextByID(partLanguageId[itemData.part - 1]);

        if (EquipIcon == null)
        {
            EquipIcon = ItemIcon.CreateItemIcon(curData,false);
            UIUtil.SetParentReset(EquipIcon.transform, iconPos);
        }
        else
        {
            EquipIcon.RefreshWithEquipInfo(curData);
        }

        #region 显示属性
        for (int i = 0; i < AttrList.Length; i++)
        {
            if (AttrList[i] != null)
            {
                AttrList[i].gameObject.SetActive(false);
            }
        }
        Action<int, string, float, float> action = (index, name, value, change) =>
        {
            if (AttrList[index] == null)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("ShowAttributesItem");
                UIUtil.SetParentReset(go.transform, BasePos);
                AttrList[index] = go.GetComponent<ShowAttributesItem>();
            }
            else
            {
                AttrList[index].gameObject.SetActive(true);
            }
            AttrList[index].SetValue(name, (int)value, (int)change, 20);
        };
        int count = 0;
        if (curData.health + curData.healthStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_health"), curData.health + curData.healthStrengthen, 0);
            count++;
        }
        if (curData.strength + curData.strengthStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_strenth"), curData.strength + curData.strengthStrengthen, 0);
            count++;
        }
        if (curData.intelligence + curData.intelligenceStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_intelligence"), curData.intelligence + curData.intelligenceStrengthen, 0);
            count++;
        }
        if (curData.defense + curData.defenseStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_defence"), curData.defense + curData.defenseStrengthen, 0);
            count++;
        }
        if (curData.speed + curData.speedStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_speed"), curData.speed + curData.speedStrengthen, 0);
            count++;
        }

        #endregion

        #region 宝石属性
        int canOpenCount = curData.stage - (BattleConst.minGemStage - 1);
        textNotSlot.gameObject.SetActive(canOpenCount <= 0);

        Action<int> SetGemSlot = (index) =>
        {
            if (mosaicItems[index] != null)
            {
                mosaicItems[index].gameObject.SetActive(true);
            }
            else
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("GemSlotItem");
                UIUtil.SetParentReset(go.transform, GemsPos);
                (go.transform as RectTransform).SetAsLastSibling();
                mosaicItems[index] = go.GetComponent<GemSlotItem>();
            }
        };
        for (int i = 0; i < mosaicItems.Length; i++)
        {
            if (canOpenCount <= i)
            {
                if (mosaicItems[i] != null)
                {
                    mosaicItems[i].gameObject.SetActive(false);
                }
                continue;
            }
            SetGemSlot(i);
            if (curData.gemList.Count > i)
            {
                mosaicItems[i].ReloadData(curData.gemList[i]);
            }
            else
            {
                mosaicItems[i].ReloadData(null);
            }
        }
        #endregion

        #region 获取途径
        if (!getBy || itemData.FoundList == null || itemData.FoundList.Count <= 0)
        {
            objGetBy.SetActive(false);
        }
        else
        {
            objGetBy.SetActive(true);
            List<List<string>> list = itemData.FoundList;
            for (int i = 0; i < list.Count; i++)
            {
                FoundItem found = FoundItem.CreateItem(list[i], this);
                UIUtil.SetParentReset(found.transform, content);
            }
        }
        #endregion
    }

    void CloseOnClick(GameObject go)
    {
        UIMgr.Instance.DestroyUI(this);
    }
}
