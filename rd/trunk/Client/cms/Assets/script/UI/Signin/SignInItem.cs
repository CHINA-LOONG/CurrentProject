//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public enum SignInType
{
    YiQianDao,
    KeQianDao,
    KeBuQian,
    WeiQianDao
}
public class SignInItemInfo
{
    private SignInType type;
    public SignInType Type
    {
        get{return type;}
        set
        {
            type = value;
        }
    }
    
    public PB.RewardItem protocolData;
}
public class SignInItem : MonoBehaviour
{
    public Transform iconPos;
    private ItemIcon itemIcon;
    private MonsterIcon monsterIcon;
    
    public GameObject objBackground;
    public GameObject objMask;
    public GameObject objRetroactive;
    public Text textRetroactive;

    private SignInItemInfo curData;
    public SignInItemInfo CurData
    {
        get { return curData; }
        set
        {
            curData = value;
        }
    }

    private System.Action SigninAction;
    void Start()
    {
        textRetroactive.text = StaticDataMgr.Instance.GetTextByID("monthlyevent_anniu");
        ScrollViewEventListener.Get(gameObject).onClick = OnClickItem;
    }

    public void InitItem(System.Action signin)
    {
        SigninAction = signin;
    }
    public void ReloadData(SignInItemInfo data)
    {
        CurData = data;
        if (CurData.protocolData.type == (int)PB.itemType.ITEM)
        {
            ItemData itemData = new ItemData() { itemId = CurData.protocolData.itemId, count = (int)CurData.protocolData.count };
            if (itemIcon == null)
            {
                itemIcon = ItemIcon.CreateItemIcon(itemData);
                UIUtil.SetParentReset(itemIcon.transform, iconPos);
            }
            else
            {
                itemIcon.gameObject.SetActive(true);
                itemIcon.RefreshWithItemInfo(itemData);
            }
            if (monsterIcon!=null)
            {
                monsterIcon.gameObject.SetActive(false);
            }
        }
        else if (CurData.protocolData.type == (int)PB.itemType.MONSTER)
        {
            if (monsterIcon==null)
            {
                monsterIcon = MonsterIcon.CreateIcon();
                UIUtil.SetParentReset(monsterIcon.transform, iconPos);
            }
            else
            {
                monsterIcon.gameObject.SetActive(true);
                monsterIcon.Init();
            }
            if (itemIcon!=null)
            {
                itemIcon.gameObject.SetActive(false);
            }
            monsterIcon.SetId("0");
            monsterIcon.SetMonsterStaticId(CurData.protocolData.itemId);
            monsterIcon.SetStage(CurData.protocolData.stage);
            monsterIcon.SetLevel(CurData.protocolData.level, false);
            monsterIcon.iconButton.gameObject.SetActive(false);
        }
        else
        {
            Logger.LogError("配置奖励类型不正确");
        }
        UpdateType();
    }

    public void UpdateType()
    {
        objRetroactive.gameObject.SetActive(false);
        objBackground.gameObject.SetActive(false);
        objMask.gameObject.SetActive(false);
        switch (CurData.Type)
        {
            case SignInType.YiQianDao:
                objMask.gameObject.SetActive(true);
                break;
            case SignInType.KeQianDao:
                //TODO:设置特效;
                break;
            case SignInType.KeBuQian:
                objRetroactive.gameObject.SetActive(true);
                objBackground.gameObject.SetActive(true);
                break;
            case SignInType.WeiQianDao:
                break;
            default:
                break;
        }
    }

    void OnClickItem(GameObject go)
    {
        switch (CurData.Type)
        {
            case SignInType.KeQianDao:
                if (SigninAction != null)
                {
                    SigninAction();
                }
                break;
            case SignInType.YiQianDao:
            case SignInType.KeBuQian:
            case SignInType.WeiQianDao:
                if (CurData.protocolData.type == (int)PB.itemType.ITEM)
                {
                    itemIcon.OnClickIconBtn(itemIcon.iconButton.gameObject);
                }
                break;
            default:
                break;
        }
    }


}
