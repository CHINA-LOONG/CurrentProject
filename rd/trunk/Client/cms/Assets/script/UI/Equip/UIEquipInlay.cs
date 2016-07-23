using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface IEquipInlayCallBack
{
    void OnSelectReturn(int tabIndex,int selIndex);
    void OnMosaicReturn(int tabIndex, int selIndex);
}

public class UIEquipInlay : UIBase, TabButtonDelegate, IEquipPanelBaseCallBack
{
    //public enum State
    //{
    //    PetUI,
    //    Setting
    //}

    public const string AssetName = "UIEquipInlay";

    public static UIEquipInlay CreateEquipInlay()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset(AssetName);
        UIEquipInlay uiEquipInlay = go.GetComponent<UIEquipInlay>();
        return uiEquipInlay;
    }

    public IEquipInlayCallBack ICallBackDelegate;

    public List<EquipPanelBase> uiPanel;
    public GameObject uiInlayPanel;

    public Text tabBuild;
    public Text tabMosaic;

    public enum State
    {
        PetUI,
        Setting
    }
    private State type;
    public State uiType
    {
        get { return type; }
        set 
        { 
            type = value;
            if (type==State.PetUI)
            {
                TabGroup.gameObject.SetActive(true); 
                uiInlayPanel.SetActive(true);
            }
            else
            {
                TabGroup.gameObject.SetActive(false);
                uiInlayPanel.SetActive(false);
            }
        }
    }

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
    private int selIndex = -1;
    public EquipData equipData;

    void Start()
    {
        tabBuild.text = StaticDataMgr.Instance.GetTextByID("equip_forge_title");
        tabMosaic.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_title");
        uiPanel.ForEach(delegate(EquipPanelBase item) { item.ICallBack = this; });
    }

    public void Refresh(EquipData data, int index, int select,State type)
    {
        equipData = data;
        selIndex = (select == -1 ? selIndex : select);
        uiType = type;
        if (tabIndex != index)
        {
            TabGroup.OnChangeItem(index);
        }
        else
        {
            uiPanel[index].ReloadData(equipData,uiType, selIndex);
        }
    }

    #region 实现接口

    public void OnTabButtonChanged(int index)
    {
        tabIndex = index;
        uiPanel.ForEach(delegate(EquipPanelBase item) { item.gameObject.SetActive(false); });
        uiPanel[index].gameObject.SetActive(true);
        uiPanel[index].ReloadData(equipData,uiType, selIndex);
    }

    public void OnBuildEquipReture()
    {
        if (ICallBackDelegate != null)
        {
            ICallBackDelegate.OnMosaicReturn(tabIndex, selIndex);
        }
    }

    public void OnSelectReturn(int selIndex)
    {
        if (ICallBackDelegate != null)
        {
            ICallBackDelegate.OnSelectReturn(tabIndex, selIndex);
        }
    }

    public void OnMosaicReturn(int selIndex)
    {
        if (ICallBackDelegate != null)
        {
            ICallBackDelegate.OnMosaicReturn(tabIndex, selIndex);
        }
    }
    #endregion

}
