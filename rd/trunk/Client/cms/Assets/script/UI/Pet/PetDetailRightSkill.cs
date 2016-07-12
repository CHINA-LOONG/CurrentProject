using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PetDetailRightSkill : PetDetailRightBase, SkillElementDelegate
{
    public SkillElement[] skillElements;
    public Text point;
    public Text status;

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
                MsgBox.PromptMsg.Open("提示", "技能点不足", "确定");
            }

            return;
        }

        PB.HSMonsterSkillUpRet skillUpResponse = msg.GetProtocolBody<PB.HSMonsterSkillUpRet>();
        m_unit.spellList[m_currentSpellId].level += 1;
        StatisticsDataMgr.Instance.ResetSkillPointState(skillUpResponse.skillPoint, skillUpResponse.skillPointTimeStamp);
        ReloadElement(m_currentSpellId, m_unit.spellList[m_currentSpellId], m_unit);

    }

    override public void ReloadData(GameUnit unit)
    {
        m_unit = unit;
        StopAllCoroutines();
        StartCoroutine(IncreasePoint());
        skillElements[(int)PetViewConst.SkillIndex.SKILL_BUFF_INDEX].gameObject.SetActive(false);
        foreach (KeyValuePair<string, Spell> element in unit.spellList)
        {
            ReloadElement(element.Key, element.Value, unit);
        }
    }

    void ReloadElement(string spellId, Spell spell, GameUnit unit)
    {
        if (spell.spellData.category == (int)SpellType.Spell_Type_PhyAttack)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_PHY_INDEX].ReloadData(spellId, spell, unit);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_MgicAttack || spell.spellData.category == (int)SpellType.Spell_Type_Cure)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_MAGIC_INDEX].ReloadData(spellId, spell, unit);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_Dot)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_BUFF_INDEX].gameObject.SetActive(true);
            skillElements[(int)PetViewConst.SkillIndex.SKILL_BUFF_INDEX].ReloadData(spellId, spell, unit);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_Passive)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_PASSIVE_INDEX].ReloadData(spellId, spell, unit);
        }
        else if (spell.spellData.category == (int)SpellType.Spell_Type_PhyDaZhao || spell.spellData.category == (int)SpellType.Spell_Type_MagicDazhao)
        {
            skillElements[(int)PetViewConst.SkillIndex.SKILL_DAZHAO_INDEX].ReloadData(spellId, spell, unit);
        }
    }

    IEnumerator IncreasePoint()
    {
        while (true)
        {
            point.text = StatisticsDataMgr.Instance.GetSkillPoint().ToString();
            if (StatisticsDataMgr.Instance.isMaxPoint() == false)
            {
                status.text = string.Format("{0:D2} : {1:D2}", StatisticsDataMgr.Instance.GetSkillPointLeftTime() / 60, StatisticsDataMgr.Instance.GetSkillPointLeftTime() % 60);
            }
            else
            {
                status.text = @"技能点已满";
            }

            yield return new WaitForSeconds(1);
        }
    }

}
