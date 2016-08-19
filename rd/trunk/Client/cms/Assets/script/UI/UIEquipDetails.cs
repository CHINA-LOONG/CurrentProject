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

public class UIEquipDetails : UIBase
{
    //界面
    public static string ViewName = "equipDetails";
    public Text equipTypeTip;
    public Text equipPowerTip;
    public Text equipLimitTip;
    public Text attributeTextTip;
    public Text gemTextTip;
    public Text equipUninstallTip;
    public Text equipInstallTip;
    public Text StrengthenTip;
    public Text gemSetTip;
    public IEquipCallBack equipCallBack;
    public Text equipNmae;//装备名称 
    public Text equipType;//装备类型
    public Text equipPart;//装备部位
    public Text equipPower;//战斗力
    public Text lvLimit;//装备等级限制
    public GameObject equipOperation;
    public GameObject equipOperation1;
    public GameObject[] basicsAttribute;//基础属性列表
    public Text[] basicsAttributeNum;
    public Text[] basicsAttributePlusNum;
    public GameObject gemPrompt;//宝石提示
    public GameObject[] gemAttribute;//宝石
    GameObject gemName;//宝石名
    GameObject[] gemAttrList = new GameObject[2];
    string[] part = new string[6] { "equip_Weapon", "equip_Helmet", "equip_Armor", "equip_Bracer", "equip_Ring", "equip_Accessory" };
    string[] equipTypeId = new string[4] { "common_type_defence", "common_type_physical", "common_type_magic", "common_type_support" };
    public GameObject reinforcedButton;//强化
    public GameObject inlayButton;//镶嵌
    public GameObject unloadButton;//卸下
    public GameObject reloadButton;//更换
    public GameObject mask;//遮罩
    EquipData equipDate;
    GameUnit unitDate;
    ItemStaticData itemData;
    public static UIEquipDetails CreateEquip()
    {
        GameObject equip = ResourceMgr.Instance.LoadAsset("equipDetails");
        return equip.GetComponent<UIEquipDetails>();
    }
    public static UIEquipDetails openEquipTips(EquipData equipData)
    {
        UIEquipDetails equipTips = UIMgr.Instance.OpenUI_(UIEquipDetails.ViewName, false) as UIEquipDetails;
        equipTips.Show(equipData,null, EquipState.hide,true);
        return equipTips;
    }

    public void Show(EquipData equip, GameUnit unit, EquipState Type, bool isTips = false)
    {
        Hide();
        if (!isTips)
            mask.SetActive(false);
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
        if (unit != null)
            unitDate = unit;        
        itemData = StaticDataMgr.Instance.GetItemData(equipDate.equipId);
        equipNmae.text = StaticDataMgr.Instance.GetTextByID(itemData.name);
        if (equipDate.level > 0)
        {
            equipNmae.text = StaticDataMgr.Instance.GetTextByID(itemData.name) + "+ " + equipDate.level.ToString();
        }        
        Outline outline = equipNmae.GetComponent<Outline>();
        outline.effectColor = ColorConst.GetStageOutLineColor(equip.stage);
        equipNmae.color = ColorConst.GetStageTextColor(equip.stage);

        lvLimit.text = itemData.minLevel.ToString();
        if (itemData.part > -1 && itemData.part < 6)//装备超出
            equipPart.text = "<" + StaticDataMgr.Instance.GetTextByID(part[itemData.part]) + ">";

        if (itemData.subType > -1 && itemData.subType < 4)//装备类型超出
            equipType.text = StaticDataMgr.Instance.GetTextByID(equipTypeId[itemData.subType]);
      
        if (equipDate.health > 0.0f)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_health");
            basicsAttributeNum[w].text = ((int)equipDate.health).ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = "+" + ((int)equipDate.healthStrengthen).ToString();
            }
            w++;
        }
        if (equipDate.strength > 0.0f)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_strenth");
            basicsAttributeNum[w].text = ((int)equipDate.strength).ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = "+" + ((int)equipDate.strengthStrengthen).ToString();
            }
            ++w;
        }
        if (equipDate.intelligence > 0.0f)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_intelligence");
            basicsAttributeNum[w].text = ((int)equipDate.intelligence).ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = "+" + ((int)equipDate.intelligenceStrengthen).ToString();
            }
            w++;
        }
        if (equipDate.defense > 0.0f)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_defence");
            basicsAttributeNum[w].text = ((int)equipDate.defense).ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = "+" + ((int)equipDate.defenseStrengthen).ToString();
            }
            w++;
        }
        if (equipDate.speed > 0.0f)
        {
            basicsAttribute[w].SetActive(true);
            basicsAttribute[w].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_speed");
            basicsAttributeNum[w].text = ((int)equipDate.speed).ToString();
            if (equipDate.level > 0)
            {
                basicsAttributePlusNum[w].enabled = true;
                basicsAttributePlusNum[w].text = "+" + ((int)equipDate.speedStrengthen).ToString();
            }
            w++;
        }
        if (equip.stage < 3) //装备品级三阶以下不可镶嵌
        {            
            gemPrompt.SetActive(true);
            gemPrompt.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("equip_gem_NotMent");
            inlayButton.SetActive(false);
        }
        else
        {
            gemPrompt.SetActive(false);
			//TODO: use managed memory
            Sprite gemBox;
            if (equip.gemList != null && equip.gemList.Count > 0)
            {
                showGem(equip.stage, null, null, false);
                EquipLevelData gemAttr;
                for (int i = 0; i < equip.gemList.Count; i++)
                {
                    gemBox = ResourceMgr.Instance.LoadAssetType<Sprite>("baoshiditu_" + equip.gemList[i].type);
                    gemAttribute[i].GetComponent<Image>().sprite = gemBox;
                    if (equip.gemList[i].gemId == "0")
                    {
                        gemAttribute[i].GetComponent<Image>().enabled = true;
                        gemAttribute[i].transform.FindChild("noGem").gameObject.SetActive(true);
                        gemPrompt.SetActive(false);
                        gemAttribute[i].transform.FindChild("noGem").GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("equip_gem_NotSet");
                    }
                    else
                    {
                        itemData = StaticDataMgr.Instance.GetItemData(equip.gemList[i].gemId);
                        gemAttr = StaticDataMgr.Instance.GetEquipLevelData(itemData.gemId);
                        showGem(0, gemAttr, gemAttribute[i], true, equip.gemList[i].gemId);
                    }
                }
            }
            else
            {
                showGem(equip.stage, null, null, false);
            }
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
            gemAttribute[i].transform.GetComponent<Image>().enabled = false;
            gemAttribute[i].transform.FindChild("noGem").gameObject.SetActive(false);
            gemAttribute[i].transform.FindChild("gemName").gameObject.SetActive(false);
            gemAttribute[i].transform.FindChild("gemAttr1").gameObject.SetActive(false);
            gemAttribute[i].transform.FindChild("gemAttr2").gameObject.SetActive(false);
            gemAttribute[i].transform.FindChild("gemParent").gameObject.SetActive(false);  
            gemAttribute[i].SetActive(false);     
        }
        inlayButton.SetActive(true);
    }
    void showGem(int equipStage, EquipLevelData gemAttr, GameObject gem, bool isLoad,string gemId = "0")
    {
        if (!isLoad)
        {
            int j = 0;
            //判断装备品级3阶有一个孔,4阶有两个孔,5阶有三个孔,6阶有四个孔
            if (equipStage == 3) j = 1;
            else if (equipStage == 4) j = 2;
            else if (equipStage == 5) j = 3;
            else if (equipStage == 6) j = 4;
            for (int i = 0; i < j; i++)
            {
                gemAttribute[i].SetActive(true);
                gemPrompt.SetActive(true);
                gemPrompt.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("equip_gem_NotKong");
            }
        }
        else
        {
            gemPrompt.SetActive(false);
            gemName = gem.transform.FindChild("gemName").gameObject;
            gemName.gameObject.SetActive(true);
            gem.transform.FindChild("noGem").gameObject.SetActive(false);
            gem.transform.GetComponent<Image>().enabled = true;

            GameObject gemParent = Util.FindChildByName(gem, "gemParent");
            gemParent.SetActive(true);
            ItemIcon icon = ItemIcon.CreateItemIcon(ItemData.valueof(gemId, 1));
            icon.transform.SetParent(gemParent.transform, false);
            icon.HideExceptIcon();

            gemName.GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID(itemData.name);
            gemName.GetComponent<Text>().color = ColorConst.GetStageTextColor(itemData.grade);
            gemAttrList[0] = gem.transform.FindChild("gemAttr1").gameObject;
            gemAttrList[1] = gem.transform.FindChild("gemAttr2").gameObject;
            int gemNum = 0;
            //TODO: use UIUtil-》SetDisPlayAttr（）
            if (gemAttr.energy != 0)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);//属性   +99999
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_energy") + "   +" + (int)gemAttr.energy;
                gemNum++;
            }
            if (gemAttr.criticalRatio != 0)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);//属性   +99999
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_critical_ratio") + "   +" + (int)gemAttr.criticalRatio;
                gemNum++;
            }
            if (gemAttr.criticalDmg != 0)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);//属性   +99999
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_critical_dmg") + "   +" + (int)gemAttr.criticalRatio;
                gemNum++;
            }
            if (gemAttr.healRatio != 0)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);//属性   +99999
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_heal_ratio") + "   +" + (int)gemAttr.healRatio;
                gemNum++;
            }
            if (gemAttr.health != 0.0f)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);//属性   +99999
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_health") + "   +" + (int)gemAttr.health;
                gemNum++;
            }
            if (gemAttr.strength != 0.0f)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_strenth") + "   +" + (int)gemAttr.strength;
                gemNum++;
            }
            if (gemAttr.intelligence != 0.0f)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_intelligence") + "   +" + (int)gemAttr.intelligence;
                gemNum++;
            }
            if (gemAttr.defense != 0.0f)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_defence") + "   +" + (int)gemAttr.defense;
                gemNum++;
            }
            if (gemAttr.speed != 0.0f)
            {
                if (gemNum > 1) return;
                gemAttrList[gemNum].SetActive(true);
                gemAttrList[gemNum].GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("common_attr_speed") + "   +" + (int)gemAttr.speed;
                gemNum++;
            }
        }
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
        else if (go.name == mask.name)
        {
            UIMgr.Instance.DestroyUI(this);
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
            return;
        PB.HSEquipMonsterUndressRet result = msg.GetProtocolBody<PB.HSEquipMonsterUndressRet>();
        GameUnit monster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(result.monsterId);
        EquipData equip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(result.id);
        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
        monster.SetEquipData(itemInfo.part, null, true);
        GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.ReloadPetEquipNotify,unitDate);
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
        EventTriggerListener.Get(mask).onClick = OnClick;
        equipTypeTip.text = StaticDataMgr.Instance.GetTextByID("equip_List_zhuangbeileixing")+":";
        equipPowerTip.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli");
        equipLimitTip.text = StaticDataMgr.Instance.GetTextByID("equip_List_xianzhidengji") + ":";
        attributeTextTip.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_attr");
        gemTextTip.text = StaticDataMgr.Instance.GetTextByID("equip_gem_casting");
        equipUninstallTip.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiexia");
        StrengthenTip.text = StaticDataMgr.Instance.GetTextByID("equip_forge_dazao");
        gemSetTip.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiangqian");
        equipInstallTip.text = StaticDataMgr.Instance.GetTextByID("equip_Change");
    }
}
