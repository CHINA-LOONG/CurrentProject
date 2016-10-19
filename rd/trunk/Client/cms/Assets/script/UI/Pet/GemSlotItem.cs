using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface IGemSlotItem
{
    void OnClickGemSlot(GemInfo info);
}

public class GemSlotItem : MonoBehaviour
{
    public GameObject objGem;
    public Image imageType;
    public Transform iconPos;
    private ItemIcon gemIcon;
    public Text textUnlock;
    public Text text_Attr1;
    public Text text_Attr2;
    public Text textAttr1;
    public Text textAttr2;
    public GameObject button;

    public IGemSlotItem IGemSlotItemDelegate;
    private GemInfo curInfo;
    
    public enum SlotType
    {
        Locked,
        Unlock,
        Metagems
    }
    private SlotType type;
    public SlotType Type
    {
        get { return type; }
        set
        {
            type = value;
            for (int i = 0; i < typeObject.Length; i++)
            {
                typeObject[i].SetActive((int)Type == i);
            }
        }
    }

    public GameObject[] typeObject = new GameObject[3];


    public void ReloadData(GemInfo info, bool Interactable = false)
    {
        curInfo = info;

        objGem.gameObject.SetActive(curInfo != null);
        if (curInfo == null)  //未解锁
        {
            Type = SlotType.Locked;
        }
        else   //为开孔
        {
            imageType.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(UIUtil.GetBaoshidituByType(curInfo.type));
            if (curInfo.gemId.Equals(BattleConst.invalidGemID))
            {
                Type = SlotType.Unlock;
                textUnlock.text = Interactable ? StaticDataMgr.Instance.GetTextByID("equip_inlay_unlockTips") :
                                                StaticDataMgr.Instance.GetTextByID("equip_gem_NotSet");
                if (gemIcon!=null)
                {
                    gemIcon.gameObject.SetActive(false);
                }
            }
            else
            {
                ItemStaticData gemItem = StaticDataMgr.Instance.GetItemData(curInfo.gemId);
                if (gemItem == null)
                {
                    Logger.LogError("镶嵌宝石错误");
                    return;
                }
                Type = SlotType.Metagems;
                EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData(gemItem.gemId);
                UIUtil.SetDisPlayAttr(attr, text_Attr1, textAttr1, text_Attr2, textAttr2);
                ItemData itemData = new ItemData() { itemId = curInfo.gemId, count = 0 };
                if (gemIcon == null)
                {
                    gemIcon = ItemIcon.CreateItemIcon(itemData, false);
                    UIUtil.SetParentReset(gemIcon.transform, iconPos.transform);
                    gemIcon.HideExceptIcon();
                }
                else
                {
                    gemIcon.gameObject.SetActive(true);
                    gemIcon.RefreshWithItemInfo(itemData, false);
                    gemIcon.HideExceptIcon();
                }
            }

            if (Interactable&& Type != SlotType.Locked)
            {
                ScrollViewEventListener.Get(button).onClick = OnClickThisSlot;
            }
            else
            {
                ScrollViewEventListener.Get(button).onClick = null;
            }
        }
    }

    void OnClickThisSlot(GameObject go)
    {
        if (Type==SlotType.Locked)//锁定
        {
            return;
        }

        if (IGemSlotItemDelegate!=null)
        {
            IGemSlotItemDelegate.OnClickGemSlot(curInfo);
        }
    }

}
