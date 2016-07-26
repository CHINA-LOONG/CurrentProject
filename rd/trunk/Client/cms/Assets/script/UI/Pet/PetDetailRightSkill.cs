using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PetDetailRightSkill : PetDetailRightBase, SkillElementDelegate
{
    public SkillElement[] skillElements;
    public Text point;
    public Text status;
    public Text pointLabel;
    public Image scrollIcon;
    public ScrollRect scrollrect;

    GameUnit m_unit;
    string m_currentSpellId;

    // Use this for initialization
    void Start()
    {
        BindLisetener();

        foreach (SkillElement element in skillElements)
        {
            element.eventDelegate = this;
        }
    }

    void OnDestroy()
    {
        UnBindListener();
    }

    void BindLisetener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_S.GetHashCode().ToString(), OnMonsterSkillUpReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_C.GetHashCode().ToString(), OnMonsterSkillUpReturn);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_S.GetHashCode().ToString(), OnMonsterSkillUpReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_C.GetHashCode().ToString(), OnMonsterSkillUpReturn);
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollrect != null && scrollrect.normalizedPosition.y > 0.01)
        {
            scrollIcon.gameObject.SetActive(true);
        }
        else
        {
            scrollIcon.gameObject.SetActive(false);
        }
    }

    public void OnLevelButtonDown(string spellId)
    {
        m_currentSpellId = spellId;
        PB.HSMonsterSkillUp request = new PB.HSMonsterSkillUp();
        request.monsterId = m_unit.pbUnit.guid;
        request.skillId = spellId;
        GameApp.Instance.netManager.SendMessage(ProtocolMessage.Create(PB.code.MONSTER_SKILL_UP_C.GetHashCode(), request));
    }

    void OnMonsterSkillUpReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();

            if (error.errCode == (int)PB.monsterError.SKILL_POINT_NOT_ENOUGH)
            {
				MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,
				                      StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillPointNotEnough));
            }

            return;
        }

        PB.HSMonsterSkillUpRet skillUpResponse = msg.GetProtocolBody<PB.HSMonsterSkillUpRet>();
        m_unit.spellList[m_currentSpellId].level += 1;
        StatisticsDataMgr.Instance.ResetSkillPointState(skillUpResponse.skillPoint, skillUpResponse.skillPointTimeStamp);
        ReloadElement(m_currentSpellId, m_unit.spellList[m_currentSpellId], m_unit);
    }

    public override void ReloadData(PetRightParamBase param)
    {
        m_unit = param.unit;

        StopAllCoroutines();
        StartCoroutine(IncreasePoint());

        pointLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillCurrentPoint);
        skillElements[(int)PetViewConst.SkillIndex.SKILL_BUFF_INDEX].gameObject.SetActive(false);
        foreach (KeyValuePair<string, Spell> element in m_unit.spellList)
        {
            ReloadElement(element.Key, element.Value, m_unit);
        }
    }

    void ReloadElement(string spellId, Spell spell, GameUnit unit)
    {
        if (spell.spellData.category == (int)SpellType.Spell_Type_PhyAttack)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_PHY_INDEX].ReloadData(spellId, spell, unit, true);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_MgicAttack || spell.spellData.category == (int)SpellType.Spell_Type_Cure)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_MAGIC_INDEX].ReloadData(spellId, spell, unit, true);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_Dot || spell.spellData.category == (int)SpellType.Spell_Type_Beneficial || spell.spellData.category == (int)SpellType.Spell_Type_Negative)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_BUFF_INDEX].gameObject.SetActive(true);
            skillElements[(int)PetViewConst.SkillIndex.SKILL_BUFF_INDEX].ReloadData(spellId, spell, unit, spell.spellData.category == (int)SpellType.Spell_Type_Dot ? true : false);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_Passive)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_PASSIVE_INDEX].ReloadData(spellId, spell, unit, false);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_PhyDaZhao || spell.spellData.category == (int)SpellType.Spell_Type_MagicDazhao)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_DAZHAO_INDEX].ReloadData(spellId, spell, unit, true);
        }
    }

    IEnumerator IncreasePoint()
    {
        while (true)
        {
            point.text = StatisticsDataMgr.Instance.GetSkillPoint().ToString();
            if (StatisticsDataMgr.Instance.isMaxPoint() == false)
            {
                status.text = string.Format("({0:D2} : {1:D2})", StatisticsDataMgr.Instance.GetSkillPointLeftTime() / 60, StatisticsDataMgr.Instance.GetSkillPointLeftTime() % 60);
            }
            else
            {
                status.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailSkillMaxPoint);
            }

            yield return new WaitForSeconds(1);
        }
    }

}
