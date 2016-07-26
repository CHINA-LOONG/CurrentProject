using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface SkillElementDelegate
{
    void OnLevelButtonDown(string spellId);
}

public class SkillElement : MonoBehaviour
{

    public Transform iconPos;
    private SpellIcon spellIcon;
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

    string m_spellId = null;
    Spell m_spell = null;

    // Update is called once per frame
    void Update()
    {

        if (cost != null && m_spell != null)
        {
            if (GameDataMgr.Instance.PlayerDataAttr.coin < StaticDataMgr.Instance.GetSPellLevelPrice(m_spell.level + 1))
            {
                cost.color = ColorConst.text_color_nReq;
            }
            else
            {
                cost.color = ColorConst.system_color_white;
            }
        }
    }

    public void LevelButtonDown(GameObject go)
    {
        if (eventDelegate != null)
        {
            eventDelegate.OnLevelButtonDown(m_spellId);
        }
    }

    public void ReloadData(string spellId, Spell spell, GameUnit unit, bool canUpgrade)
    {
        m_spellId = spellId;
        m_spell = spell;

        if (spellIcon == null)
        {
            spellIcon = SpellIcon.CreateWith(iconPos);
        }
        spellIcon.SetData(m_spell.level, m_spellId);
        levelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillLevel);
        currentLevelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillCurrentLeve);
        nextlevelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillNextLeve);

        type.text = spell.spellData.GetTypeName();
        level.text = string.Format("({0} / {1})", spell.level, GameConfig.MaxMonsterLevel);

        currentTips.text = spell.spellData.GetTips(spell.level);
        int needCost = 0;
        if (canUpgrade == true)
        {
            if (spell.level == GameConfig.MaxMonsterLevel)
            {
                nextTips.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillMaxLeve);
                needCost = 0;
            }
            else
            {
                nextTips.text = spell.spellData.GetTips(spell.level + 1);
                needCost = StaticDataMgr.Instance.GetSPellLevelPrice(spell.level + 1);
            }

            levelBtn.gameObject.SetActive(true);

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
        else
        {
            nextTips.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillNoUpgrade);
            levelBtn.gameObject.SetActive(false);
            needCost = 0;
        }
        if (GameDataMgr.Instance.PlayerDataAttr.coin < needCost)
        {
            cost.color = ColorConst.text_color_nReq;
        }
        else
        {
            cost.color = ColorConst.system_color_white;
        }
        cost.text = needCost.ToString();
    }
}
