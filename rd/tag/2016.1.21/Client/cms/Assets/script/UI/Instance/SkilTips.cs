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
		if (null == spell)
			return;

		skillTypeText.text = spell.NameAttr;

		string spellDesc = "";
		spellDesc = spell.GetTips(level);
		skillDescText.text = spellDesc;
	}

	public static	string GetCategoryDesc(int category)
	{
		switch (category) 
		{
		case (int)SpellType.Spell_Type_PhyAttack:
			return StaticDataMgr.Instance.GetTextByID("spell_wuli_name");
			break;
		case (int)SpellType.Spell_Type_MgicAttack:
		case (int)SpellType.Spell_Type_Cure:
			return StaticDataMgr.Instance.GetTextByID("spell_magic_name");
			break;
		case (int)SpellType.Spell_Type_Passive:
			return StaticDataMgr.Instance.GetTextByID("spell_beidong_name");
			break;
		case (int)SpellType.Spell_Type_Beneficial:
		case (int)SpellType.Spell_Type_Negative:
		case (int)SpellType.Spell_Type_Dot:
			return StaticDataMgr.Instance.GetTextByID("spell_buff_name");
			
		case (int)SpellType.Spell_Type_PhyDaZhao:
		case (int)SpellType.Spell_Type_MagicDazhao:
			return StaticDataMgr.Instance.GetTextByID("spell_dazhao_name");
		default:
			return "";
			break;
		}
	}
}
