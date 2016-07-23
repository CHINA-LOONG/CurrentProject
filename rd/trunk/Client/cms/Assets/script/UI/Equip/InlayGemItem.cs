using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface IMosaicCallBack
{
    void OnSelectItem(int selIndex);
    void OnClickMosaic(int selIndex);
}


public class InlayGemItem : MonoBehaviour
{
    public enum MosaicType
    {
        Lock,
        Unlock,
        Mosaic
    }
    private MosaicType type=MosaicType.Lock;
    public MosaicType Type
    {
        get { return type; }
        set 
        {
            if (value!=type)
            {
                type = value;
                OnChangeType(type);
            }
        }
    }
    public GameObject[] typeItem = new GameObject[3]; 
    //common
    public Image imgItem;
    public Transform gemPos;
    private ItemIcon gemIcon;
    public Button btnSelect;
    public Image imgSelect;
    //lock
    public Text text_LockDesc;
    //unlock
    public Text text_UnlockDesc;
    //mosaic
    public Text text_Attr1;
    public Text text_Attr2;
    public Text textGemName;
    public Text textAttr1;
    public Text textAttr2;
    public Button btnXiezai;
    public Text text_Xiezai;

    [HideInInspector]
    public int Index;
    public IMosaicCallBack mosaicDelegate;

    void Start()
    {
        text_LockDesc.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_lockTips");
        text_Xiezai.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiexia");

        ScrollViewEventListener.Get(btnSelect.gameObject).onClick = OnSelectItem;
        ScrollViewEventListener.Get(btnXiezai.gameObject).onClick = OnClickXiezai;
    }
    void OnChangeType(MosaicType type)
    {
        for (int i = 0; i < typeItem.Length; i++)
        {
            typeItem[i].SetActive((int)Type == i);
        }
    }

    public void Reload(GemInfo gem)
    {
        if (gemIcon != null)
        {
            gemIcon.gameObject.SetActive(false);
        }

        if (gem == null)//未开孔
        {
            imgItem.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>("baoshi_lock");
            Type = MosaicType.Lock;
            return;
        }
        else //开孔
        {
            imgItem.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>("baoshiditu_" + gem.type);
            text_UnlockDesc.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_unlockTips");

            if (gem.gemId.Equals(BattleConst.invalidGemID))//未镶嵌
            {
                Type = MosaicType.Unlock;
            }
            else //镶嵌
            {
                ItemStaticData gemItem = StaticDataMgr.Instance.GetItemData(gem.gemId);
                if (gemItem != null)
                {
                    Type = MosaicType.Mosaic;

                    EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData(gemItem.gemId);
                    textGemName.text = StaticDataMgr.Instance.GetTextByID(gemItem.name);
                    UIUtil.SetDisPlayAttr(attr, text_Attr1, textAttr1, text_Attr2, textAttr2);
                    ItemData itemData = new ItemData() { itemId = gem.gemId, count = 0 };
                    if (gemIcon == null)
                    {
                        gemIcon = ItemIcon.CreateItemIcon(itemData, false);
                        UIUtil.SetParentReset(gemIcon.transform, gemPos.transform);
                        gemIcon.HideExceptIcon();
                    }
                    else
                    {
                        gemIcon.gameObject.SetActive(true);
                        gemIcon.RefreshWithItemInfo(itemData,false);
                        gemIcon.HideExceptIcon();
                    }
                }
                else
                {
                    Logger.LogError("宝石镶嵌出现错误");
                }
            }
        }

    }

    public void SetSelectState(bool select)
    {
        imgSelect.enabled = select;
        if (select)
        {
            text_UnlockDesc.text = StaticDataMgr.Instance.GetTextByID("equip_select_gem");
        }
        else
        {
            text_UnlockDesc.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_unlockTips");
        }
    }


    void OnSelectItem(GameObject go)
    {
        if (Type==MosaicType.Lock)
        {
            return;
        }
        if (mosaicDelegate != null)
        {
            mosaicDelegate.OnSelectItem(Index);
        }
    }

    void OnClickXiezai(GameObject go)
    {
        if (mosaicDelegate != null)
        {
            mosaicDelegate.OnClickMosaic(Index);
        }
    }


}
