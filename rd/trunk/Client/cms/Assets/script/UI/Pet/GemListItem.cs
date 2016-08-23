using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface IGemListItem
{
    void OnRemoveReturn(ItemData data);
    void OnEmbedReturn(ItemData data);
    void OnChangeReturn(ItemData data);
}

public class GemListItemInfo
{
    public ItemData itemData;
    public ItemStaticData staticData;
    public enum Type
    {
        Remove,
        Embed,
        Change
    }

    public Type type;
}

public class GemListItem : MonoBehaviour
{
    public Transform transIconPos;
    private ItemIcon itemIcon;

    public Text textName;
    public Text text_Attr1;
    public Text textAttr1;
    public Text text_Attr2;
    public Text textAttr2;

    public Button btnButton;
    private Text textButton;
    public Text TextButton
    {
        get
        {
            if (textButton==null)
            {
                textButton = btnButton.GetComponentInChildren<Text>();
            }
            return textButton;
        }
    }

    public IGemListItem IGemListItemDelegate;

    public GemListItemInfo curData;


    void Start()
    {
        ScrollViewEventListener.Get(btnButton.gameObject).onClick = OnClickXiangqian;
    }

    public void OnReload(GemListItemInfo data)
    {
        curData = data;
        if (itemIcon==null)
        {
            itemIcon = ItemIcon.CreateItemIcon(curData.itemData);
            UIUtil.SetParentReset(itemIcon.transform, transIconPos);
        }
        else
        {
            itemIcon.RefreshWithItemInfo(curData.itemData);
        }
        UIUtil.SetStageColor(textName, curData.staticData);
        EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData(curData.staticData.gemId);
        UIUtil.SetDisPlayAttr(attr, text_Attr1, textAttr1, text_Attr2, textAttr2);
        switch (curData.type)
        {
            case GemListItemInfo.Type.Remove:
                TextButton.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiexia");
                break;
            case GemListItemInfo.Type.Embed:
                TextButton.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiangqian");
                break;
            case GemListItemInfo.Type.Change:
                TextButton.text = StaticDataMgr.Instance.GetTextByID("equip_Change");
                break;
        }
    }
    
    void OnClickXiangqian(GameObject go)
    {
        if (IGemListItemDelegate!=null)
        {
            switch (curData.type)
            {
                case GemListItemInfo.Type.Remove:
                    IGemListItemDelegate.OnRemoveReturn(curData.itemData);
                    break;
                case GemListItemInfo.Type.Embed:
                    IGemListItemDelegate.OnEmbedReturn(curData.itemData);
                    break;
                case GemListItemInfo.Type.Change:
                    IGemListItemDelegate.OnChangeReturn(curData.itemData);
                    break;
            }
        }
    }
    
}
