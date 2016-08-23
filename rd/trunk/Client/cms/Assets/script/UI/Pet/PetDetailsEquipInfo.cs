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
    void OnRemoveEquip();
}

public class PetDetailsEquipInfo : PetDetailsRight,
                                   TabButtonDelegate,
                                   IEquipInfoBase
{
    public static string ViewName = "PetDetailsEquipInfo";

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
    public EquipData CurData
    {
        get { return curData; }
    }

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

    public void Refresh(EquipData data,int select = -1)
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
        UIUtil.SetEquipType(textType, itemData.subType);
        UIUtil.SetEquipPart(textPart, itemData.part);

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
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipEmbedNotify, ReloadEquipNotify);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadEquipEmbedNotify, ReloadEquipNotify);
    }


    #region IEquipInfoBase
    public void OnClickChangeBtn(PartType part)
    {
        if (IPetDetailsEquipInfoDelegate!=null)
        {
            IPetDetailsEquipInfoDelegate.OnClickChangeEquip(part);
        }
    }

    public void OnUnloadEquip()
    {
        if (IPetDetailsEquipInfoDelegate!=null)
        {
            IPetDetailsEquipInfoDelegate.OnRemoveEquip();
        }
    }

    #endregion

}
