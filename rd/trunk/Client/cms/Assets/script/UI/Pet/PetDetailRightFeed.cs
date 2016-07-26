using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetDetailRightFeed : PetDetailRightBase,IUsedExpCallBack
{

    public Transform content;

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
    GameUnit m_unit;
    UnitData s_unit;
    TempUnit t_unit = new TempUnit();


    private List<ItemStaticData> infos = new List<ItemStaticData>();

    private List<EXPListItem> items = new List<EXPListItem>();
    private List<EXPListItem> itemPool = new List<EXPListItem>();


    public override void ReloadData(PetRightParamBase param)
    {


        m_unit = param.unit;
        t_unit.getData(m_unit);
        s_unit = StaticDataMgr.Instance.GetUnitRowData(m_unit.pbUnit.id);

        StaticDataMgr.Instance.GetItemData(PB.changeType.CHANGE_MONSTER_EXP, ref infos);
        infos.Sort(SortEXP);
        RemoveAllElement();
        for (int i = 0; i < infos.Count; i++)
        {
            EXPListItem item = GetElement();
            item.OnReload(infos[i],UIUtil.CheckPetIsMaxLevel(t_unit.level));
            item.transform.SetAsLastSibling();
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
        items.ForEach(delegate(EXPListItem item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(items);
        items.Clear();
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

    void AddTempExp(int exp, System.Action callback)
    {
        if (UIUtil.CheckPetIsMaxLevel(t_unit.level))
        {
            t_unit.level = GameConfig.MaxMonsterLevel;
            t_unit.exp = 0;
            Logger.LogError("达到最大等级");
            //TODO：
            callback();
            items.ForEach(delegate(EXPListItem item) { item.IsMaxlevel = true; });
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
            ParentNode.SetUnitLevelExp(t_unit.level, t_unit.exp);
        }
    }

    public void OnUseExp(ItemStaticData itemData, System.Action callback)
    {
        AddTempExp(itemData.addAttrValue,callback);
    }

    public void OnSendMsg(ItemStaticData itemData, int count)
    {
        PB.HSItemUse param = new PB.HSItemUse()
        {
            targetID=m_unit.pbUnit.guid,
            itemId=itemData.id,
            itemCount=count
        };
        GameApp.Instance.netManager.SendMessage(PB.code.ITEM_USE_C.GetHashCode(), param);
    }

    void OnUseExpReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
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
        ParentNode.ReloadLeftData();
        ParentNode.ReloadRigthData(PetViewConst.UIPetFeedAssetName);
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

}
