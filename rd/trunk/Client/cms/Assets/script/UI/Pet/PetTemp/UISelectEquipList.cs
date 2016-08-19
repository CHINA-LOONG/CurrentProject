using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UISelectEquipList : UIBase, 
                                 IScrollView, 
                                 IClickUsedEquip
{

    public static string ViewName = "UISelectEquipList";

    public Text textTitle;
    public Text textNotfound;
    public Button btnClose;
    public FixCountScrollView scrollView;

    private GameUnit curUnit;
    private List<EquipData> Infos = new List<EquipData>();

    void Start()
    {
        textTitle.text = StaticDataMgr.Instance.GetTextByID("");
        textNotfound.text = StaticDataMgr.Instance.GetTextByID("list_empty");
        btnClose.onClick.AddListener(OnClickCloseBtn);
    }

    public void ReloadData(GameUnit unit,PartType part)
    {
        curUnit = unit;
        UnitData petData = StaticDataMgr.Instance.GetUnitRowData(curUnit.pbUnit.id);

        Infos.Clear();
        Dictionary<long, EquipData> equipList = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.equipList;
        foreach (var item in equipList)
        {
            if (item.Value.monsterId != BattleConst.invalidMonsterID)
            {
                continue;
            }
            ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(item.Value.equipId);
            if (itemInfo.part == (int)part && itemInfo.subType == petData.equip)
            {
                Infos.Add(item.Value);
            }
        }
        textNotfound.gameObject.SetActive(Infos.Count <= 0);
        Infos.Sort(SortEquip);

        scrollView.InitContentSize(Infos.Count, this);
    }

    public void OnClickCloseBtn()
    {
        UIMgr.Instance.DestroyUI(this);
    }
    //TODO: sort by zhandouli(战斗力)
    public static int SortEquip(EquipData a, EquipData b)
    {
        return 0;
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
    
    void OnEquipDressReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipMonsterDressRet result = msg.GetProtocolBody<PB.HSEquipMonsterDressRet>();

        GameUnit monster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(result.monsterId);
        EquipData equip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(result.id);

        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        monster.SetEquipData(itemInfo.part, equip, true);

        GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.ReloadPetEquipNotify,curUnit);

        UIMgr.Instance.DestroyUI(this);
    }
    void OnEquipReplaceReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipMonsterDressRet result = msg.GetProtocolBody<PB.HSEquipMonsterDressRet>();

        GameUnit monster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(result.monsterId);
        EquipData equip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(result.id);

        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        monster.SetEquipData(itemInfo.part, equip, true);

        GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.ReloadPetEquipNotify,curUnit);

        UIMgr.Instance.DestroyUI(this);
    }

    #region ScrollView
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
    #endregion

    #region EquipListItem Interface
    public void OnSelectEquip(EquipData equip)
    {
        //throw new NotImplementedException();
    }

    public void OnUsedEquip(EquipData equip)
    {
        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        if (curUnit.equipList[itemInfo.part] == null)
        {
            PB.HSEquipMonsterDress param = new PB.HSEquipMonsterDress()
            {
                id = equip.id,
                monsterId = curUnit.pbUnit.guid
            };
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_MONSTER_DRESS_C.GetHashCode(), param);
        }
        else
        {
            PB.HSEquipMonsterReplace param = new PB.HSEquipMonsterReplace()
            {
                id = equip.id,
                monsterId = curUnit.pbUnit.guid
            };
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_MONSTER_REPLACE_C.GetHashCode(), param);
        }
    }
    #endregion
}
