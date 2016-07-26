using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IUsedExpCallBack
{
    void OnUseExp(ItemStaticData itemData, System.Action callback);
    void OnSendMsg(ItemStaticData itemData,int count);
}


public class EXPListItem : MonoBehaviour
{
    public Transform transIconPos;
    private ItemIcon itemIcon;

    public Text textName;
    public Text textCount;
    public Text textExp;

    public Button btnUsed;
    public IUsedExpCallBack usedExpDelegate;

    private int useCount;

    private ItemStaticData itemData;
    private ItemData itemInfo;

    private bool isMaxlevel = false;
    public bool IsMaxlevel
    {
        get { return isMaxlevel; }
        set 
        { 
            isMaxlevel = value;
            SetButton();
        }
    }
    private bool isNone = false;
    public bool IsNone
    {
        get{return isNone;}
        set 
        {
            isNone = value;
            //SetButton();
        }
    }
    void SetButton()
    {
        bool isActive = !(IsMaxlevel/* || IsNone*/);
        btnUsed.interactable = isActive;
        if (isActive)
        {
            ScrollViewEventListener.Get(btnUsed.gameObject).onDown = OnButtonDown;
            ScrollViewEventListener.Get(btnUsed.gameObject).onUp = OnButtonUp;
            ScrollViewEventListener.Get(btnUsed.gameObject).onExit = OnButtonExit;
        }
        else
        {
            ScrollViewEventListener.Get(btnUsed.gameObject).onDown = null;
            ScrollViewEventListener.Get(btnUsed.gameObject).onUp = null;
            ScrollViewEventListener.Get(btnUsed.gameObject).onExit = null;
        }
    }


    public void OnReload(ItemStaticData staticData,bool isMaxleve)
    {
        itemData = staticData;
        itemInfo = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(itemData.id);
        useCount = 0;

        ItemData tempInfo = new ItemData() { itemId = itemData.id, count = 0 };
        if (itemIcon==null)
        {
            itemIcon = ItemIcon.CreateItemIcon(tempInfo);
            UIUtil.SetParentReset(itemIcon.transform, transIconPos);
        }
        else
        {
            itemIcon.RefreshWithItemInfo(tempInfo);
        }

        UIUtil.SetStageColor(textName, itemData);
        UpdateCount(useCount);
        textExp.text = string.Format("{0}+{1}",StaticDataMgr.Instance.GetTextByID("Exp"),itemData.addAttrValue);
    }

    void UpdateCount(int useCount)
    {
        int curCount = (itemInfo == null ? 0 : itemInfo.count)- useCount;
        textCount.text = "×" + curCount;
        IsNone = (curCount <= 0);
    }

    void UsedExp()
    {
        if (itemInfo==null)
        {
			//TODO:提示处理
            return;
        }
        else if (useCount >= itemInfo.count)
        {
            OnButtonUp(null);
            return;
        }
        else
        {
            useCount++;
            UpdateCount(useCount);
            if (usedExpDelegate != null)
            {
                usedExpDelegate.OnUseExp(itemData, OnMaxLevelBack);
            }
        }
    }

    void OnSendMsg()
    {
        if (useCount > 0 && usedExpDelegate != null)
        {
            usedExpDelegate.OnSendMsg(itemData, useCount);
            useCount = 0;
        }
    }

    void OnMaxLevelBack()
    {
        CancelInvoke("UsedExp");
        OnSendMsg();
    }

    void OnButtonDown(GameObject go)
    {
        InvokeRepeating("UsedExp", 1.0f, 0.1f);
        UsedExp();
    }
    void OnButtonUp(GameObject go)
    {
        CancelInvoke("UsedExp");
        OnSendMsg();
    }
    void OnButtonExit(GameObject go)
    {
        CancelInvoke("UsedExp");
        OnSendMsg();
    }


}
