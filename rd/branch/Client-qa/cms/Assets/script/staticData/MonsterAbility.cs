using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterAbility : StaticDataBase
{
	public	class RowData
	{
		public	string index;
		public	string assetID;
		public  string nickName;
		public  int    grade;
		public  bool   isEvolutionable;
		public  string evolutionID;
		public  int  property;

		public float initLife;
		public float experienceModifyRate;
		public float healthModifyRate;
		public float strengthModifyRate;
		public float intelligenceModifyRate;
		public float speedModifyRate;
		public float resistanceModifyRate;
		public float enduranceModifyRate;
		public float hitRateModifyVal;
		public float criticalRateModifyVal;
		public float criticalFactorModifyVal;
		public float goldNoteValueModifyRate;
		public float expValueModifyRate;
		public float recoveryRate;

		public int equip;
		public string physical_skill;	
		public string magic_skill;
		public string buff_skill;
		public string powerful_skill;
		public string protect_skill;
		public string lazy_skill;
		public string captain_skill;

		public int lazy;
		public int AI;

	}

	private Dictionary<string,RowData> m_CacheDic = new Dictionary<string, RowData>();

	public MonsterAbility.RowData getRowDataFromIndex(string monsterIndex)
	{
		if (m_CacheDic.ContainsKey (monsterIndex)) 
		{
			return m_CacheDic[monsterIndex];
		}

		int nRow = m_StaticTable.GetRowNumWithPrimaryKey ("Index", monsterIndex);
		if (nRow < 0)
		{
			Debug.LogError("Can't find MonsterAbility index = " + monsterIndex);
			return null;
		}
		
		RowData rd = new RowData ();
		rd.index = monsterIndex;
		
		FireEngine.FIELD subField;

		subField = m_StaticTable.GetCertainField (nRow, "assetID");
		rd.assetID = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "nickname");
		rd.nickName = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "grade");
		rd.grade = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "evolutionable");
		rd.isEvolutionable = int.Parse (subField.FieldValueStr) != 0;

		subField = m_StaticTable.GetCertainField (nRow, "evolutionID");
		rd.evolutionID = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "property");
		rd.property = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "initLife");
		rd.initLife = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "experienceModifyRate");
		rd.experienceModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "healthModifyRate");
		rd.healthModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "strengthModifyRate");
		rd.strengthModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "intelligenceModifyRate");
		rd.intelligenceModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "speedModifyRate");
		rd.speedModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "resistanceModifyRate");
		rd.resistanceModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "enduranceModifyRate");
		rd.enduranceModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "hitRateModifyVal");
		rd.hitRateModifyVal = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "criticalRateModifyVal");
		rd.criticalRateModifyVal = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "criticalFactorModifyVal");
		rd.criticalFactorModifyVal = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "goldNoteValueModifyRate");
		rd.goldNoteValueModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "expValueModifyRate");
		rd.expValueModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "recoveryRate");
		rd.recoveryRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "equip");
		rd.equip = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "physical_skill");
		rd.physical_skill = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "magic_skill");
		rd.magic_skill = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "buff_skill");
		rd.buff_skill = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "powerful_skill");
		rd.powerful_skill = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "protect_skill");
		rd.nickName = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "lazy_skill");
		rd.nickName = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "captain_skill");
		rd.captain_skill = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "lazy");
		rd.lazy = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "AI");
		rd.AI = int.Parse (subField.FieldValueStr);

		m_CacheDic [rd.index] = rd;

		return rd;
	}

}
