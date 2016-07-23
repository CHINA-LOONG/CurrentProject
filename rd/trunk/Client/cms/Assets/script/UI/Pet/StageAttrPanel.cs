using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageAttrPanel : MonoBehaviour {

    public MonsterIcon icon;
    public Text text_PH;
    public Text text_STR;
    public Text text_INT;
    public Text text_DEF;
    public Text text_SPD;
    public Text textPHValue;
    public Text textSTRValue;
    public Text textINTValue;
    public Text textDEFValue;
    public Text textSPDValue;

    void Start()
    {
        text_PH.text = StaticDataMgr.Instance.GetTextByID("common_attr_health");
        text_STR.text = StaticDataMgr.Instance.GetTextByID("common_attr_strenth");
        text_INT.text = StaticDataMgr.Instance.GetTextByID("common_attr_intelligence");
        text_DEF.text = StaticDataMgr.Instance.GetTextByID("common_attr_defence");
        text_SPD.text = StaticDataMgr.Instance.GetTextByID("common_attr_speed");
    }


    public void ReloadData(GameUnit unit, int stage)
    {
        int health, strength, inteligence, defence, speed;
        UIUtil.GetAttrValue(unit, stage, out health, out strength, out inteligence, out defence, out speed);

        textPHValue.text = health.ToString();
        textSTRValue.text =  strength.ToString();
        textDEFValue.text =  defence.ToString();
        textINTValue.text =  inteligence.ToString();
        textSPDValue.text = speed.ToString();

        icon.Init();
        icon.SetId(unit.pbUnit.guid.ToString());
        icon.SetMonsterStaticId(unit.pbUnit.id);
        icon.SetStage(stage);
        icon.iconButton.gameObject.SetActive(false);
    }

    public IEnumerator ReloadUI(GameUnit unit, int stage)
    {
        yield return new WaitForEndOfFrame();


    }
}
