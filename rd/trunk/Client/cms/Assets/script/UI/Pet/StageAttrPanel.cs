using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageAttrPanel : MonoBehaviour {

    public MonsterIcon icon;
    public Text  phyValue;
    public Text  strengthValue;
    public Text  inteligenceValue;
    public Text  defenceValue;
    public Text  speedValue;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    { 
    }

    public void ReloadData(GameUnit unit, int stage)
    {
        int grade = StaticDataMgr.Instance.GetUnitRowData(unit.pbUnit.id).grade;
        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(stage);
        UnitData unitData = StaticDataMgr.Instance.GetUnitRowData(unit.pbUnit.id);
        int health = (int)((1 + unitStageData.modifyRate) * unitData.healthModifyRate * (unit.health + unitStageData.health));
        int strength = (int)((1 + unitStageData.modifyRate) * unitData.strengthModifyRate * (unit.health + unitStageData.strength));
        int inteligence = (int)((1 + unitStageData.modifyRate) * unitData.intelligenceModifyRate * (unit.health + unitStageData.intelligence));
        int defence = (int)((1 + unitStageData.modifyRate) * unitData.defenseModifyRate * (unit.health + unitStageData.defense));
        int speed = (int)((1 + unitStageData.modifyRate) * unitData.speedModifyRate * (unit.health + unitStageData.speed));

        phyValue.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrHealth), health);
        strengthValue.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrDefence), strength);
        defenceValue.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrSpeed), defence);
        inteligenceValue.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrStrenth), inteligence);
        speedValue.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrIntelligence), speed);

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
