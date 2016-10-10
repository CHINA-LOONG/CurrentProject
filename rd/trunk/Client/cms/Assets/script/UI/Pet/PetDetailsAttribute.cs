//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PetDetailsAttribute : PetDetailsRight
{
    public static string ViewName = "PetDetailsAttribute";

    public Text text_Title;

    public Text text_BaseAttr;
    public Text text_STR;
    public Text textSTR;
    public Text text_STA;
    public Text textSTA;
    public Text text_INT;
    public Text textINT;
    public Text text_DEF;
    public Text textDEF;
    public Text text_SPD;
    public Text textSPD;

    public Text text_BattleAttr;
    public Text text_PhysATK;
    public Text textPhysATK;
    public Text text_HP;
    public Text textHP;
    public Text text_MagicATK;
    public Text textMagicATK;
    public Text text_ENG;
    public Text textENG;
    public Text text_CRate;
    public Text textCRate;
    public Text text_CDMG;
    public Text textCDMG;
    public Text text_AMNT;
    public Text textAMNT;


    private GameUnit curData;

    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage");
        text_BaseAttr.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_attr");
        text_STR.text = StaticDataMgr.Instance.GetTextByID("common_attr_strenth");
        text_STA.text = StaticDataMgr.Instance.GetTextByID("common_attr_health");
        text_INT.text = StaticDataMgr.Instance.GetTextByID("common_attr_intelligence");
        text_DEF.text = StaticDataMgr.Instance.GetTextByID("common_attr_defence");
        text_SPD.text = StaticDataMgr.Instance.GetTextByID("common_attr_speed");

        text_BattleAttr.text = StaticDataMgr.Instance.GetTextByID("detail_title2");
        text_PhysATK.text = StaticDataMgr.Instance.GetTextByID("detail_physatk");
        text_HP.text = StaticDataMgr.Instance.GetTextByID("detail_life");
        text_MagicATK.text = StaticDataMgr.Instance.GetTextByID("detail_magicatk");
        text_ENG.text = StaticDataMgr.Instance.GetTextByID("common_attr_energy");
        text_CRate.text = StaticDataMgr.Instance.GetTextByID("common_attr_critical_ratio");
        text_CDMG.text = StaticDataMgr.Instance.GetTextByID("common_attr_critical_dmg");
        text_AMNT.text = StaticDataMgr.Instance.GetTextByID("common_attr_heal_ratio");
    }

    public void ReloadData(GameUnit unit)
    {
        curData = unit;

        textSTR.text = ((int)curData.strength).ToString();
        textSTA.text = ((int)curData.health).ToString();
        textINT.text = ((int)curData.intelligence).ToString();
        textDEF.text = ((int)curData.defense).ToString();
        textSPD.text = ((int)curData.speed).ToString();

        textPhysATK.text = ((int)curData.phyAttack).ToString();
        textHP.text = ((int)curData.maxLife).ToString();
        textMagicATK.text = ((int)curData.magicAttack).ToString();
        textENG.text = ((int)curData.additionEnergy).ToString();
        textCRate.text = (curData.criticalRatio * 100).ToString("f1") + "%";
        textCDMG.text = (curData.criticalDamageRatio * 100).ToString("f1") + "%";
        textAMNT.text = (curData.additionHealRatio * 100).ToString("f1") + "%";
    }


    void OnEnable()
    {
        BindListerner();
    }
    void OnDisable()
    {
        UnBindListerner();
    }
    void BindListerner()
    {
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetLevelNotify, PetAttributeChangeNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadPetEquipNotify, PetAttributeChangeNotify);
    }
    void UnBindListerner()
    {
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetLevelNotify, PetAttributeChangeNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadPetEquipNotify, PetAttributeChangeNotify);
    }

    void PetAttributeChangeNotify(GameUnit unit)
    {
        if (curData==unit)
        {
            ReloadData(curData);
        }
    }
    void PetAttributeChangeNotify(EquipData equip)
    {
        if (equip != null && equip.monsterId == curData.pbUnit.guid)
        {
            ReloadData(curData);
        }
    }

}
