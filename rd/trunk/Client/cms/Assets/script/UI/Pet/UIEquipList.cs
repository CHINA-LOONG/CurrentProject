using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public interface IUIEquipListCallBack
{
    void OnSelectEquip(GameUnit unit, EquipData equip);
    void OnEquipDressOrReplace();
}

public class UIEquipList : MonoBehaviour,IClickUsedEquip
{
    public const string AssetName = "UIEquipList";
    public static UIEquipList CreateEquipList()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset(AssetName);
        UIEquipList uiEquipList = go.GetComponent<UIEquipList>();
        return uiEquipList;
    }

    public IUIEquipListCallBack listDelegate;

    public ScrollRect scrollView;

    public Transform content;


    private GameUnit m_unit;
    private List<EquipData> list1=new List<EquipData>();
    private List<EquipListItem> items = new List<EquipListItem>();
    private List<EquipListItem> itemPool = new List<EquipListItem>();

    public void Refresh(GameUnit unit,int part)
    {
        m_unit = unit;
        UnitData petData = StaticDataMgr.Instance.GetUnitRowData(m_unit.pbUnit.id);

        list1.Clear();
        Dictionary<long, EquipData> equipList = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.equipList;
        foreach (var item in equipList)
        {
            if (item.Value.monsterId != BattleConst.invalidMonsterID)
            {
                continue;
            }
            ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(item.Value.equipId);
            if (itemInfo.minLevel <= m_unit.pbUnit.level && itemInfo.part == part && itemInfo.subType == petData.equip)
            {
                list1.Add(item.Value);
            }
        }
        list1.Sort(SortEquip);
        RemoveAllElement();

        for (int i = 0; i < list1.Count; i++)
        {
            EquipListItem item = GetElement();
            item.Refresh(list1[i]);
            item.transform.SetAsLastSibling();
        }
    }

    public EquipListItem GetElement()
    {
        EquipListItem item = null;
        if (itemPool.Count<=0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("EquipListItem");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(content, false);
                item = go.GetComponent<EquipListItem>();
                item.ClickDelegate=this;
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
    public void RemoveElement(EquipListItem item)
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
        items.ForEach(delegate(EquipListItem item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(items);
        items.Clear();
    }
    //TODO: sort by zhandouli(战斗力)
    public static int SortEquip(EquipData a, EquipData b)
    {
        return 0;
    }

    public void OnSelectEquip(EquipData equip)
    {
        listDelegate.OnSelectEquip(m_unit, equip);
    }

    public void OnUsedEquip(EquipData equip)
    {
        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        if (m_unit.equipList[itemInfo.part] == null)
        {
            PB.HSEquipMonsterDress param = new PB.HSEquipMonsterDress()
            {
                id = equip.id,
                monsterId = m_unit.pbUnit.guid
            };
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_MONSTER_DRESS_C.GetHashCode(), param);
        }
        else
        {
            PB.HSEquipMonsterReplace param = new PB.HSEquipMonsterReplace()
            {
                id=equip.id,
                monsterId=m_unit.pbUnit.guid
            };
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_MONSTER_REPLACE_C.GetHashCode(), param);
        }
    }
    
    void OnEquipDressReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null||msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipMonsterDressRet result = msg.GetProtocolBody<PB.HSEquipMonsterDressRet>();

        GameUnit monster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(result.monsterId);
        EquipData equip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(result.id);

        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        equip.monsterId = result.monsterId;
        monster.equipList[itemInfo.part] = equip;

        GameEventMgr.Instance.FireEvent(GameEventList.ReloadPetEquipNotify);
        listDelegate.OnEquipDressOrReplace();
    }

    void OnEquipReplaceReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg==null||msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipMonsterDressRet result = msg.GetProtocolBody<PB.HSEquipMonsterDressRet>();

        GameUnit monster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(result.monsterId);
        EquipData equip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(result.id);

        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        EquipData equip2 = monster.equipList[itemInfo.part];
        equip2.monsterId = BattleConst.invalidMonsterID;
        equip.monsterId = result.monsterId;
        monster.equipList[itemInfo.part] = equip;

        GameEventMgr.Instance.FireEvent(GameEventList.ReloadPetEquipNotify);
        listDelegate.OnEquipDressOrReplace();
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_DRESS_C.GetHashCode().ToString(), OnEquipDressReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_DRESS_S.GetHashCode().ToString(), OnEquipDressReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_REPLACE_C.GetHashCode().ToString(), OnEquipReplaceReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_REPLACE_S.GetHashCode().ToString(), OnEquipReplaceReturn);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_DRESS_C.GetHashCode().ToString(), OnEquipDressReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_DRESS_S.GetHashCode().ToString(), OnEquipDressReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_REPLACE_C.GetHashCode().ToString(), OnEquipReplaceReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_REPLACE_S.GetHashCode().ToString(), OnEquipReplaceReturn);
    }


}
