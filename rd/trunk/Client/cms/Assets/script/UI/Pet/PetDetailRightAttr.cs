using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PetDetailRightAttr : PetDetailRightBase
{

    public Text text_Title;

    public Text text_STA;
    public Text textSTA;
    public Text text_STR;
    public Text textSTR;
    public Text text_INT;
    public Text textINT;
    public Text text_DEF;
    public Text textDEF;
    public Text text_SPD;
    public Text textSPD;
    public Text text_EDR;
    public Text textEDR;
    public Text text_REC;
    public Text textREC;
    public Text text_HP;
    public Text textHP;
    public Text text_PhysATK;
    public Text textPhysATK;
    public Text text_MagicATK;
    public Text textMagicATK;
    public Text text_INJ;
    public Text textINJ;

    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("detail_title");
        text_STA.text = StaticDataMgr.Instance.GetTextByID("common_attr_health");
        text_STR.text = StaticDataMgr.Instance.GetTextByID("common_attr_strenth");
        text_INT.text = StaticDataMgr.Instance.GetTextByID("common_attr_intelligence");
        text_DEF.text = StaticDataMgr.Instance.GetTextByID("common_attr_defence");
        text_SPD.text = StaticDataMgr.Instance.GetTextByID("common_attr_speed");
        text_EDR.text = StaticDataMgr.Instance.GetTextByID("detail_endurance");
        text_REC.text = StaticDataMgr.Instance.GetTextByID("detail_blood");
        text_HP.text = StaticDataMgr.Instance.GetTextByID("detail_life");
        text_PhysATK.text = StaticDataMgr.Instance.GetTextByID("detail_physatk");
        text_MagicATK.text = StaticDataMgr.Instance.GetTextByID("detail_magicatk");
        text_INJ.text = StaticDataMgr.Instance.GetTextByID("detail_injuryratio");
    }

    public override void ReloadData(PetRightParamBase obj)
    {
        GameUnit unit = obj.unit;

        textSTA.text = unit.health.ToString();
        textSTR.text = unit.strength.ToString();
        textINT.text = unit.intelligence.ToString();
        textDEF.text = unit.defense.ToString();
        textSPD.text = unit.speed.ToString();
        textEDR.text = unit.endurance.ToString();
        textREC.text = unit.recovery.ToString();
        textHP.text = unit.maxLife.ToString();//((int)(unit.health * SpellConst.healthToLife)).ToString();
        textPhysATK.text = unit.phyAttack.ToString(); //((int)(unit.strength * SpellConst.strengthToAttack)).ToString();
        textMagicATK.text = unit.magicAttack.ToString(); //((int)(unit.intelligence * SpellConst.intelligenceToAttack)).ToString();
        //受伤比计算 max(1/(1+(守方总防御力-攻方防御穿透)/I(min(lv1,lv2))),25%)
        float injuryRatio = 1.0f / (1.0f + (unit.defense * 1.0f) / SpellFunctions.GetInjuryAdjustNum(unit.pbUnit.level, unit.pbUnit.level));
        injuryRatio = injuryRatio < 0.25f ? 0.25f : injuryRatio;
        textINJ.text = injuryRatio.ToString("P");
    }
}
