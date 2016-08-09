using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public interface IUIEquipListCallBack
{
    void OnSelectEquip(GameUnit unit, EquipData equip);
    void OnEquipDressOrReplace();
}

public class UIEquipList : MonoBehaviour,IClickUsedEquip,IScrollView
{
    public const string AssetName = "UIEquipList";
    public static UIEquipList CreateEquipList()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset(AssetName);
        UIEquipList uiEquipList = go.GetComponent<UIEquipList>();
        return uiEquipList;
    }

    public IUIEquipListCallBack listDelegate;
    
    public FixCountScrollView scrollView;
    
    private GameUnit m_unit;
    private List<EquipData> Infos=new List<EquipData>();

    private List<EquipListItem> items = new List<EquipListItem>();
    private List<EquipListItem> itemPool = new List<EquipListItem>();

    public void Refresh(GameUnit unit,int part)
    {
        m_unit = unit;
        UnitData petData = StaticDataMgr.Instance.GetUnitRowData(m_unit.pbUnit.id);

        Infos.Clear();
        Dictionary<long, EquipData> equipList = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.equipList;
        foreach (var item in equipList)
        {
            if (item.Value.monsterId != BattleConst.invalidMonsterID)
            {
                continue;
            }
            ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(item.Value.equipId);
            if (itemInfo.part == part && itemInfo.subType == petData.equip)
            {
                Infos.Add(item.Value);
            }
        }
        Infos.Sort(SortEquip);
        
        scrollView.InitContentSize(Infos.Count, this);
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
        monster.SetEquipData(itemInfo.part, equip, true);
        //equip.monsterId = result.monsterId;
        //monster.equipList[itemInfo.part] = equip;

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
        //EquipData equip2 = monster.equipList[itemInfo.part];
        //equip2.monsterId = BattleConst.invalidMonsterID;
        monster.SetEquipData(itemInfo.part, equip, true);
        //equip.monsterId = result.monsterId;
        //monster.equipList[itemInfo.part] = equip;

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

    public void ReloadData(Transform item, int index)
    {
        EquipListItem equip = item.GetComponent<EquipListItem>();
        equip.ReloadData(Infos[index]);
    }

    public Transform CreateData(Transform parent, int index = 0)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("EquipListItem");
        if (go != null)
        {
            UIUtil.SetParentReset(go.transform, parent);
            EquipListItem item = go.GetComponent<EquipListItem>();
            item.ClickDelegate = this;
            return go.transform;
        }
        return null;
    }

    public void CleanData(List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
        itemList.Clear();
    }
}
