using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public abstract class PetDetailsRight:MonoBehaviour
{
    public IPetDetailsRight IPetDetailsRight;
}

public interface IPetDetailsRight : IPetDetailsEquipInfo { }


public interface IPetDetailsEquipInfo
{
    void OnClickChangeEquip(PartType part);
}

public class PetDetailsEquipInfo : PetDetailsRight,
                                   TabButtonDelegate,
                                   IEquipInfoBase
{
    public Transform iconPos;
    private ItemIcon EquipIcon;
    public Text textName;
    public Text textType;
    public Text textPart;

    private TabButtonGroup tabGroup;
    public TabButtonGroup TabGroup
    {
        get
        {
            if (tabGroup == null)
            {
                tabGroup = GetComponentInChildren<TabButtonGroup>();
                tabGroup.InitWithDelegate(this);
            }
            return tabGroup;
        }
    }

    string[] partLanguageId = new string[6] { "equip_Weapon", "equip_Helmet", "equip_Armor", "equip_Bracer", "equip_Ring", "equip_Accessory" };
    string[] typeLanguageId = new string[4] { "common_type_defence", "common_type_physical", "common_type_magic", "common_type_support" };
    private int tabIndex = -1;
    private int selIndex = 0;

    public Text tabTitle1;
    public Text tabTitle2;
    public Text tabTitle3;

    public EquipDetails equipDetails;
    public EquipForge equipForge;
    public EquipEmbed equipEmbed;
    public IPetDetailsEquipInfo IPetDetailsEquipInfoDelegate { get { return IPetDetailsRight; } }
    public EquipInfoBase[] tabPanel=new EquipInfoBase[3];

    private EquipData curData;

    void Start()
    {
        tabTitle1.text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_detail_attr");
        tabTitle2.text = StaticDataMgr.Instance.GetTextByID("equip_forge_dazao");
        tabTitle3.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiangqian");
    }

    public void OnTabButtonChanged(int index)
    {
        if (tabIndex == index)
        {
            return;
        }
        selIndex = index;
        tabIndex = selIndex;
        ReloadData(tabIndex);
    }

    public void Refresh(EquipData data, int select = -1)
    {
        curData = data;
        selIndex = (select == -1 ? selIndex : select);
        if (tabIndex != selIndex)
        {
            TabGroup.OnChangeItem(selIndex);
        }
        else
        {
            ReloadData(tabIndex);
        }
    }
    void ReloadData(int index)
    {
        ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(curData.equipId);
        
        if (EquipIcon == null)
        {
            EquipIcon = ItemIcon.CreateItemIcon(curData);
            UIUtil.SetParentReset(EquipIcon.transform, iconPos);
        }
        else
        {
            EquipIcon.RefreshWithEquipInfo(curData);
        }

        UIUtil.SetStageColor(textName, itemData.name, curData.stage, curData.level);
        textType.text = StaticDataMgr.Instance.GetTextByID(typeLanguageId[itemData.subType - 1]);
        textPart.text = StaticDataMgr.Instance.GetTextByID(partLanguageId[itemData.part - 1]);

        for (int i = 0; i < tabPanel.Length; i++)
        {
            tabPanel[i].gameObject.SetActive(i == index);
            if (i == index)
            {
                tabPanel[i].IEquipInfoBaseDelegate = this;
                tabPanel[i].ReloadData(curData);
            }
        }
    }


    void ReloadEquipNotify(EquipData equipData)
    {
        if (curData == equipData)
        {
            Refresh(curData);
        }
    }
    void OnEnable()
    {
        BindListener();
    }
    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipNotify);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipNotify);
    }


    #region IEquipInfoBase
    public void OnClickChangeBtn(PartType part)
    {
        if (IPetDetailsEquipInfoDelegate!=null)
        {
            IPetDetailsEquipInfoDelegate.OnClickChangeEquip(part);
        }
    }

    #endregion

}
