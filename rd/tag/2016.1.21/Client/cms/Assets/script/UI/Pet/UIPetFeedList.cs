using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IUIPetFeedList
{
    void SetUnitLevelExp(GameUnit unit, int level, int exp);
    void UsedExpFinish(GameUnit unit);
}

public class UIPetFeedList : UIBase,
                             IUsedExpCallBack
{

    public static string ViewName = "UIPetFeedList";

    public Text text_Title;
    public Button btnClose;

    public Text text_Current;
    public Text text_Target;
    public Transform content;
    public Text text_Tips2;

    public Text textExp;
    public Slider progressExp;

    public IUIPetFeedList IUIPetFeedListDelegate;
    GameUnit m_unit;
    UnitData s_unit;
    TempUnit t_unit = new TempUnit();
    public class TempUnit
    {
        public int level;
        public int exp;

        public void getData(GameUnit unit)
        {
            this.level = unit.pbUnit.level;
            this.exp = unit.pbUnit.curExp;
        }
    }

    private List<ItemStaticData> infos = new List<ItemStaticData>();

    private List<EXPListItem> items = new List<EXPListItem>();
    private List<EXPListItem> itemPool = new List<EXPListItem>();
    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("exp_title");
        text_Tips2.text = StaticDataMgr.Instance.GetTextByID("exp_changan");

        btnClose.onClick.AddListener(OnClickCloseBtn);

    }

    public void ReloadData(GameUnit unit)
    {
        m_unit = unit;
        t_unit.getData(m_unit);
        s_unit = StaticDataMgr.Instance.GetUnitRowData(m_unit.pbUnit.id);

        text_Target.text = StaticDataMgr.Instance.GetTextByID("exp_level") + GameDataMgr.Instance.PlayerDataAttr.LevelAttr.ToString();
        RefreshLevelExp(m_unit.pbUnit.level, m_unit.pbUnit.curExp);
        
        StaticDataMgr.Instance.GetItemData(PB.changeType.CHANGE_MONSTER_EXP, ref infos);
        infos.Sort(SortEXP);
        RemoveAllElement();

        for (int i = 0; i < infos.Count; i++)
        {
            EXPListItem item = GetElement();
            item.OnReload(infos[i], UIUtil.CheckPetIsMaxLevel(t_unit.level) > 0);
            item.transform.SetAsLastSibling();
        }
    }
    public void RefreshLevelExp(int level, int exp)
    {
        text_Current.text = StaticDataMgr.Instance.GetTextByID("exp_monsterlevel") + level;
        int maxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(level).experience * s_unit.levelUpExpRate);
        switch (UIUtil.CheckPetIsMaxLevel(level))
        {
            case 0:
                textExp.text = exp + "/" + maxExp;
                progressExp.value = (float)exp / (float)maxExp;
                break;
            case 1:
                textExp.text = "MAX LVL";
                progressExp.value = 0.0f;
                break;
            case 2:
                textExp.text = 0 + "/" + maxExp;
                progressExp.value = 0.0f;
                break;
        }
    }
    public EXPListItem GetElement()
    {
        EXPListItem item = null;
        if (itemPool.Count <= 0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("EXPListItem");
            if (go != null)
            {
                UIUtil.SetParentReset(go.transform, content);
                item = go.GetComponent<EXPListItem>();
                item.usedExpDelegate = this;
            }
        }
        else
        {
            item = itemPool[itemPool.Count - 1];
            item.gameObject.SetActive(true);
            itemPool.Remove(item);
        }
        items.Add(item);
        return item;
    }
    public void RemoveElement(EXPListItem item)
    {
        if (items.Contains(item))
        {
            item.gameObject.SetActive(false);
            items.Remove(item);
            itemPool.Add(item);
        }
    }
    public void RemoveAllElement()
    {
        items.ForEach(delegate (EXPListItem item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(items);
        items.Clear();
    }
    //sort reward item
    public static int SortEXP(ItemStaticData a, ItemStaticData b)
    {
        int result = 0;
        if (a.addAttrValue != b.addAttrValue)
        {
            if (a.addAttrValue < b.addAttrValue)
            {
                result = -1;
            }
            else
            {
                result = 1;
            }
        }
        return result;
    }

    void AddTempExp(int exp, Action callback)
    {
        if (UIUtil.CheckPetIsMaxLevel(t_unit.level) > 0)
        {
            t_unit.level = (GameConfig.MaxMonsterLevel < GameDataMgr.Instance.PlayerDataAttr.LevelAttr ?
                           GameConfig.MaxMonsterLevel :
                           GameDataMgr.Instance.PlayerDataAttr.LevelAttr);
            t_unit.exp = 0;
            Logger.LogError("达到最大等级");
            callback();
            items.ForEach(delegate (EXPListItem item) { item.IsMaxlevel = true; });
            return;
        }
        int maxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(t_unit.level).experience * s_unit.levelUpExpRate);
        if (exp >= (maxExp - t_unit.exp))
        {
            exp -= (maxExp - t_unit.exp);
            t_unit.level++;
            t_unit.exp = 0;
            AddTempExp(exp, callback);
        }
        else
        {
            t_unit.exp += exp;
            if (IUIPetFeedListDelegate != null)
            {
                IUIPetFeedListDelegate.SetUnitLevelExp(m_unit, t_unit.level, t_unit.exp);
                RefreshLevelExp(t_unit.level, t_unit.exp);
            }
        }
    }

    void OnClickCloseBtn()
    {
        UIMgr.Instance.DestroyUI(this);
    }

    void OnUseExpReturn(ProtocolMessage msg)
    {
        //UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("使用失败");
            //TODO： 
            return;
        }
        else
        {
            Logger.Log("使用成功");
        }
        if (IUIPetFeedListDelegate!=null)
        {
            IUIPetFeedListDelegate.UsedExpFinish(m_unit);
        }
        ReloadData(m_unit);
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_USE_C.GetHashCode().ToString(), OnUseExpReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseExpReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_USE_C.GetHashCode().ToString(), OnUseExpReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseExpReturn);
    }
    
    #region Item Interface
    public void OnUseExp(ItemStaticData itemData, Action callback)
    {
        AddTempExp(itemData.addAttrValue, callback);
    }

    public void OnSendMsg(ItemStaticData itemData, int count)
    {
        PB.HSItemUse param = new PB.HSItemUse()
        {
            targetID = m_unit.pbUnit.guid,
            itemId = itemData.id,
            itemCount = count
        };
        GameApp.Instance.netManager.SendMessage(PB.code.ITEM_USE_C.GetHashCode(), param, false);
    }
    #endregion
}
