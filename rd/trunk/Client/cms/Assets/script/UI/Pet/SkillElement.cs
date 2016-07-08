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

    private string m_spellId = null ;

	// Use this for initialization
	void Start () {        
	}
	
	// Update is called once per frame
	void Update () {
	
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

        type.text = spell.spellData.GetTypeName();
        level.text = string.Format("( {0} / {1})", spell.level, unit.pbUnit.level);
        currentTips.text = spell.spellData.GetTips(spell.level);
        nextTips.text = spell.spellData.GetTips(spell.level + 1);
        cost.text = StaticDataMgr.Instance.GetSPellLevelPrice(spell.level + 1).ToString();

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
