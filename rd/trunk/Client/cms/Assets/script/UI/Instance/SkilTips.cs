using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkilTips : MonoBehaviour
{
	public	Text	skillTypeText;
	public	Text	skillDescText;
	

	// Use this for initialization
	void Start () 
	{
	
	}

	public	void	SetSpellId(string spellId,int level)
	{
		SpellProtoType spell = StaticDataMgr.Instance.GetSpellProtoData (spellId);
		string spellTypeName = "";
		string spellDesc = "";
		if (spell != null) 
		{
			spellTypeName = GetCategoryDesc(spell.category);
			spellDesc = spell.GetTips(level);
		}
		skillTypeText.text = spellTypeName;
		skillDescText.text = spellDesc;
	}

	public string GetCategoryDesc(int category)
	{
		switch (category) 
		{
		case (int)SpellType.Spell_Type_PhyAttack:
			return "物理技能";
			break;
		case (int)SpellType.Spell_Type_MgicAttack:
		case (int)SpellType.Spell_Type_Cure:
			return "法术技能";
			break;
		case (int)SpellType.Spell_Type_Passive:
			return "被动技能";
			break;
		case (int)SpellType.Spell_Type_Beneficial:
		case (int)SpellType.Spell_Type_Negative:
		case (int)SpellType.Spell_Type_Dot:
			return "buff技能";
			
		case (int)SpellType.Spell_Type_PhyDaZhao:
		case (int)SpellType.Spell_Type_MagicDazhao:
			return "大招技能";
		default:
			return "";
			break;
		}
	}
}
