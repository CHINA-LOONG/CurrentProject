using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public enum EquipState
{
    show,
    showGem,
    hide
}
public interface IEquipCallBack
{
    void Reinforced(EquipData data);
    void Inlay(EquipData data);
    void Unload();//卸载
    void Replacement(GameUnit unit,int equipPart);//换装
}

public class UIEquipDetails : MonoBehaviour
{
    public IEquipCallBack equipCallBack;

    public Text equipNmae;//装备名称 
    public Text strengthenNum;//装备强化
    public Text equipType;//装备类型
    public Text equipPart;//装备部位
    public Text equipPower;//战斗力
    public Text lvLimit;//装备等级限制
    public GameObject equipOperation;
    public GameObject equipOperation1;
    public GameObject[] basicsAttribute;//基础属性列表
    public Text[] basicsAttributeNum;
    public Text[] basicsAttributePlusNum;
    public GameObject[] gemAttribute;//宝石
    public string[] part = new string[6] { "Weapon", "Waist", "Armor", "Bracelet", "Ring", "Charm" };
    public string[] equipTypeId = new string[4] { "Defend", "Physics", "Magic", "Support" };
    public GameObject reinforcedButton;//强化
    public GameObject inlayButton;//镶嵌
    public GameObject unloadButton;//卸下
    public GameObject reloadButton;//更换
    EquipData equipDate;
    GameUnit unitDate;
    ItemStaticData itemData;
    public static UIEquipDetails CreateEquip()
    {
        GameObject equip = ResourceMgr.Instance.LoadAsset("equipDetails");
        return equip.GetComponent<UIEquipDetails>();
    }
    public void Show(EquipData equip, GameUnit unit, EquipState Type)
    {
        Hide();
        int w = 0;
        if (Type == EquipState.show)
        {
            equipOperation.SetActive(true);
            equipOperation1.SetActive(true);
        }
        else if (Type == EquipState.showGem)
        {
            equipOperation.SetActive(false);
            equipOperation1.SetActive(true);
        }
        else if (Type == EquipState.hide)
        {
            equipOperation.SetActive(false);
            equipOperation1.SetActive(false);
        }
        equipDate = equip;
        unitDate = unit;
        itemData = StaticDataMgr.Instance.GetItemData(equipDate.equipId);
        equipNmae.text = itemData.name;
        lvLimit.text = itemData.minLevel.ToString();
        if (itemData.part > -1&&itemData.part < 6)//装备超出
            equipPart.text = StaticDataMgr.Instance.GetTextByID(part[itemData.part]);

        if (itemData.subType > -1 && itemData.part < 4)//装备类型超出
            equipType.text = StaticDataMgr.Instance.GetTextByID(equipTypeId[itemData.subType]); 
        
        if (equipDate.level > 0) { strengthenNum.enabled = true; strengthenNum.text = equipDate.level.ToString(); }
        if (equipDate.health > 0)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("health");
            basicsAttributeNum[w].text = equipDate.health.ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = equipDate.healthStrengthen.ToString();
            }
            w++;
        }
        if (equipDate.strength > 0)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("strength");
            basicsAttributeNum[w].text = equipDate.strength.ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = equipDate.strengthStrengthen.ToString();
            }
            ++w;
        }
        if (equipDate.intelligence > 0)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("intelligence");
            basicsAttributeNum[w].text = equipDate.intelligence.ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = equipDate.intelligenceStrengthen.ToString();
            }
            w++;
        }
        if (equipDate.defense > 0)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("defense");
            basicsAttributeNum[w].text = equipDate.defense.ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = equipDate.defenseStrengthen.ToString();
            }
            w++;
        }
        if (equipDate.speed > 0)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("speed");
            basicsAttributeNum[w].text = equipDate.speed.ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = equipDate.speedStrengthen.ToString();
            }
            w++;
        }
    }
    void Hide()
    {
        for (int i = 0; i < basicsAttribute.Length; i++)
        {
            basicsAttribute[i].SetActive(false);
            basicsAttributePlusNum[i].enabled = false;                
        }
        for (int i = 0; i < gemAttribute.Length; i++)
        {
            gemAttribute[i].SetActive(false);
        }        
        strengthenNum.enabled = false;
    }

    void OnClick(GameObject go)
    {
        if (go.name == reinforcedButton.name)
        {
            equipCallBack.Reinforced(equipDate);  
        }
        else if (go.name == inlayButton.name)
        {
            equipCallBack.Inlay(equipDate);
        }
        else if (go.name == unloadButton.name)
        {
            OnUnloadEquip(equipDate);
        }
        else if (go.name == reloadButton.name)
        {            
           equipCallBack.Replacement(unitDate,itemData.part);
        }
    }

    public void OnUnloadEquip(EquipData equip)
    {
        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
            PB.HSEquipMonsterUndress param = new PB.HSEquipMonsterUndress()
            {
                id = equip.id,
            };
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_MONSTER_UNDRESS_C.GetHashCode(), param);
    }

    void OnEquipUnloadReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipMonsterUndressRet result = msg.GetProtocolBody<PB.HSEquipMonsterUndressRet>();
        GameUnit monster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(result.monsterId);
        EquipData equip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(result.id);
        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        monster.equipList[itemInfo.part] = null;
        equip.monsterId = BattleConst.invalidMonsterID;
        equipCallBack.Unload();
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
    
    void Start()
    { 
        EventTriggerListener.Get(reinforcedButton).onClick = OnClick;
        EventTriggerListener.Get(inlayButton).onClick = OnClick;
        EventTriggerListener.Get(unloadButton).onClick = OnClick;
        EventTriggerListener.Get(reloadButton).onClick = OnClick;
    }
}
