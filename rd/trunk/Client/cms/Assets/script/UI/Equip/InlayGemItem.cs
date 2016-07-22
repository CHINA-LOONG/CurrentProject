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
    private MosaicType type;
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
    public Button btnSelect;
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
        text_UnlockDesc.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_unlockTips");
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
        if (gem == null)
        {
            imgItem.sprite = null;
            imgItem.color = Color.red;
            Type = MosaicType.Lock;
            return;
        }
        if (gem.gemId.Equals(BattleConst.invalidGemID))
        {
            imgItem.sprite = null;
            imgItem.color = Color.blue;
            Type = MosaicType.Unlock;
            return;
        }

        ItemStaticData gemItem = StaticDataMgr.Instance.GetItemData(gem.gemId);
        if (gemItem!=null)
        {
            //TODO:
            imgItem.sprite = null;
            imgItem.color = Color.green;
            textGemName.text = gemItem.name;
            EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData(gemItem.gemId);
            UIUtil.SetDisPlayAttr(attr, text_Attr1, textAttr1, text_Attr2, textAttr2);
            Type = MosaicType.Mosaic;
        }
        else
        {
            Logger.LogError("mosaic gem error");
        }
    }

    public void SetSelectState(bool select)
    {
        if (select)
        {
            GetComponent<Image>().color = Color.blue;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
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
