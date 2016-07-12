using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface SkillElementDelegate
{
    void OnLevelButtonDown(string spellId);
}

public class SkillElement : MonoBehaviour {

    public Image icon;
    public Text type;
    public Text level;
    public Text cost;
    public Text currentTips;
    public Text nextTips;
    public Button levelBtn;
    public SkillElementDelegate eventDelegate = null;

    public Text levelLabel;
    public Text currentLevelLabel;
    public Text nextlevelLabel;

    string m_spellId = null ;
    Spell m_spell = null;

	// Use this for initialization
	void Start () {        
	}
	
	// Update is called once per frame
	void Update () {
      
        if (cost != null && m_spell != null)
        {
            if (GameDataMgr.Instance.PlayerDataAttr.coin < StaticDataMgr.Instance.GetSPellLevelPrice(m_spell.level + 1))
            {
                cost.color = Color.red;
            }
            else
            {
                cost.color = Color.black;
            }
        }   
	}

    public void LevelButtonDown(GameObject go)
    {
        if(eventDelegate != null)
        {
            eventDelegate.OnLevelButtonDown(m_spellId);
        }
    }

    public void ReloadData(string spellId, Spell spell, GameUnit unit)
    {
        m_spellId = spellId;
        m_spell = spell;

        levelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillLevel);
        currentLevelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillCurrentLeve);
        nextlevelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillNextLeve);

        type.text = spell.spellData.GetTypeName();
        level.text = string.Format("( {0} / {1})", spell.level, unit.pbUnit.level);
        
        currentTips.text = spell.spellData.GetTips(spell.level);
        if (spell.level == GameConfig.MaxMonsterLevel)
        {
            nextTips.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillMaxLeve);
            cost.text = "";
        }
        else
        {
            nextTips.text = spell.spellData.GetTips(spell.level + 1);
            cost.text = StaticDataMgr.Instance.GetSPellLevelPrice(spell.level + 1).ToString();
        }

        if (spell.level >= unit.pbUnit.level)
        {
            levelBtn.interactable = false;
            EventTriggerListener.Get(levelBtn.gameObject).onClick = null;
        }
        else
        {
            levelBtn.interactable = true;
            EventTriggerListener.Get(levelBtn.gameObject).onClick = LevelButtonDown;
        }
    }
}
