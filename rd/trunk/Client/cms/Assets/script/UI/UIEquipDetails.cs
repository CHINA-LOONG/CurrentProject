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
public class UIEquipDetails : MonoBehaviour
{   
    public Text equipNmae;//装备名称 
    public Text strengthenNum;//装备强化
    public Text equipType;//装备类型
    public Text equipPart;//装备部位
    public Text equipPower;//战斗力
    public Text lvLimit;//装备等级限制
    public GameObject equipOperation;
    public GameObject equipOperation1;
    public GameObject exit;//退出   
    public GameObject[] basicsAttribute;//基础属性列表
    public Text[] basicsAttributeNum;
    public Text[] basicsAttributePlusNum;
    public GameObject[] gemAttribute;//宝石
    public string[] part = new string[6] { "Weapon", "Waist", "Armor", "Bracelet", "Ring", "Charm" };
    public string[] equipTypeId = new string[4] { "Defend", "Physics", "Magic", "Support" };
    public static UIEquipDetails CreateEquip()
    {
        GameObject equip = ResourceMgr.Instance.LoadAsset("equipDetails");
        return equip.GetComponent<UIEquipDetails>();
    }
    public void Show(long id, EquipState Type)
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
        EquipData equipDate = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(id);
        ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(equipDate.equipId);
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
}
