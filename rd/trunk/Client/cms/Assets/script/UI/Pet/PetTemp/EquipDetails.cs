using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class EquipInfoBase : MonoBehaviour
{
    public IEquipInfoBase IEquipInfoBaseDelegate;
    public abstract void ReloadData(EquipData data);
}
public interface IEquipInfoBase : IEquipDetails
{
}

//*************************
//装备详情
//************************
public interface IEquipDetails
{
    void OnClickChangeBtn(PartType part);
    void OnUnloadEquip();
}

public class EquipDetails : EquipInfoBase
{
    public Transform BasePos;
    public Transform GemsPos;

    public Button btnRemove;
    public Button btnChange;

    public Text textBaseAttr;
    public Text textGemsAttr;
    public Text textNotSlot;

    private ShowAttributesItem[] AttrList = new ShowAttributesItem[5];
    private GemSlotItem[] mosaicItems = new GemSlotItem[BattleConst.maxGemCount];

    public IEquipDetails IEquipDetailsDelegate { get { return IEquipInfoBaseDelegate; } }
    public EquipData curData;

    void Start()
    {
        btnRemove.onClick.AddListener(OnClickRemoveBtn);
        btnChange.onClick.AddListener(OnClickChangeBtn);
        btnRemove.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiexia");
        btnChange.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("equip_Change");

        textBaseAttr.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_attr");
        textGemsAttr.text = StaticDataMgr.Instance.GetTextByID("equip_gem_casting");
        textNotSlot.text = StaticDataMgr.Instance.GetTextByID("equip_gem_NotMent");
    }

    public override void ReloadData(EquipData data)
    {
        curData = data;
        #region 显示属性
        for (int i = 0; i < AttrList.Length; i++)
        {
            if (AttrList[i] != null)
            {
                AttrList[i].gameObject.SetActive(false);
            }
        }
        Action<int, string, float, float> action = (index, name, value, change) =>
         {
             if (AttrList[index] == null)
             {
                 GameObject go = ResourceMgr.Instance.LoadAsset("ShowAttributesItem");
                 UIUtil.SetParentReset(go.transform, BasePos);
                 AttrList[index] = go.GetComponent<ShowAttributesItem>();
             }
             else
             {
                 AttrList[index].gameObject.SetActive(true);
             }
             AttrList[index].SetValue(name, (int)value, (int)change);
         };
        int count = 0;
        if (curData.health + curData.healthStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_health"), curData.health + curData.healthStrengthen, 0);
            count++;
        }
        if (curData.strength + curData.strengthStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_strenth"), curData.strength + curData.strengthStrengthen, 0);
            count++;
        }
        if (curData.intelligence + curData.intelligenceStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_intelligence"), curData.intelligence + curData.intelligenceStrengthen, 0);
            count++;
        }
        if (curData.defense + curData.defenseStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_defence"), curData.defense + curData.defenseStrengthen, 0);
            count++;
        }
        if (curData.speed + curData.speedStrengthen > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_speed"), curData.speed + curData.speedStrengthen, 0);
            count++;
        }

        #endregion


        int canOpenCount = curData.stage - (BattleConst.minGemStage - 1);
        textNotSlot.gameObject.SetActive(canOpenCount <= 0);

        Action<int> SetGemSlot = (index) =>
          {
              if (mosaicItems[index] != null)
              {
                  mosaicItems[index].gameObject.SetActive(true);
              }
              else
              {
                  GameObject go = ResourceMgr.Instance.LoadAsset("GemSlotItem");
                  UIUtil.SetParentReset(go.transform, GemsPos);
                  (go.transform as RectTransform).SetAsLastSibling();
                  mosaicItems[index] = go.GetComponent<GemSlotItem>();
              }
          };
        for (int i = 0; i < mosaicItems.Length; i++)
        {
            if (canOpenCount <= i)
            {
                if (mosaicItems[i] != null)
                {
                    mosaicItems[i].gameObject.SetActive(false);
                }
                continue;
            }
            SetGemSlot(i);
            if (curData.gemList.Count > i)
            {
                mosaicItems[i].ReloadData(curData.gemList[i]);
            }
            else
            {
                mosaicItems[i].ReloadData(null);
            }
        }
    }

    void OnClickChangeBtn()
    {
        ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(curData.equipId);
        if (IEquipDetailsDelegate != null)
        {
            IEquipDetailsDelegate.OnClickChangeBtn((PartType)itemData.part);
        }
    }

    void OnClickRemoveBtn()
    {
        PB.HSEquipMonsterUndress param = new PB.HSEquipMonsterUndress();
        param.id = curData.id;
        GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_MONSTER_UNDRESS_C.GetHashCode(), param);
    }
    void OnEquipUnloadReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
            return;

        PB.HSEquipMonsterDressRet result = msg.GetProtocolBody<PB.HSEquipMonsterDressRet>();

        GameUnit monster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(result.monsterId);
        EquipData equip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(result.id);

        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        monster.SetEquipData(itemInfo.part, null, true);

        GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.ReloadPetEquipNotify, monster);

        if (IEquipDetailsDelegate!=null)
        {
            IEquipDetailsDelegate.OnUnloadEquip();
        }
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_UNDRESS_C.GetHashCode().ToString(), OnEquipUnloadReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_UNDRESS_S.GetHashCode().ToString(), OnEquipUnloadReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_UNDRESS_C.GetHashCode().ToString(), OnEquipUnloadReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_MONSTER_UNDRESS_S.GetHashCode().ToString(), OnEquipUnloadReturn);
    }

}
