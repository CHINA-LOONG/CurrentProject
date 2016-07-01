using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnitData : StaticDataBase
{
    [Serializable]
	public	class RowData
	{
		public	string index;
		public	string assetID;
		public  string nickName;
		public  int    grade;
		public  bool   isEvolutionable;
		public  string evolutionID;
		public  int  property;

		public float levelUpExpRate;
		public float healthModifyRate;
		public float strengthModifyRate;
		public float intelligenceModifyRate;
		public float speedModifyRate;
		public float defenseModifyRate;
        public float enduranceModifyRate;
        public float goldNoteMinValueModifyRate;
        public float goldNoteMaxValueModifyRate;
        public float expMinValueModifyRate;
        public float expMaxValueModifyRate;
		public float recoveryRate;

		public int equip;
		public int lazy;
		public int AI;

        public string spellIDList;
        public string weakpointList;
	}

	private Dictionary<string,RowData> m_CacheDic = new Dictionary<string, RowData>();

	public UnitData.RowData getRowDataFromIndex(string monsterIndex)
	{
		if (m_CacheDic.ContainsKey (monsterIndex)) 
		{
			return m_CacheDic[monsterIndex];
		}

		int nRow = m_StaticTable.GetRowNumWithPrimaryKey ("Index", monsterIndex);
		if (nRow < 0)
		{
			Debug.LogError("Can't find UnitData index = " + monsterIndex);
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

        subField = m_StaticTable.GetCertainField(nRow, "levelUpExpRate");
        rd.levelUpExpRate = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "expMinValueModifyRate");
        rd.expMinValueModifyRate = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "expMaxValueModifyRate");
        rd.expMaxValueModifyRate = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "goldNoteMinValueModifyRate");
        rd.goldNoteMinValueModifyRate = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "goldNoteMaxValueModifyRate");
        rd.goldNoteMaxValueModifyRate = float.Parse(subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "healthModifyRate");
		rd.healthModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "strengthModifyRate");
		rd.strengthModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "intelligenceModifyRate");
		rd.intelligenceModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "speedModifyRate");
		rd.speedModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "defenseModifyRate");
		rd.defenseModifyRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "enduranceModifyRate");
        rd.enduranceModifyRate = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "recoveryRate");
        rd.recoveryRate = float.Parse(subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "equip");
        rd.equip = int.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "spellID_list");
        rd.spellIDList = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "weakpoint_list");
        rd.weakpointList = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField (nRow, "AI");
        rd.AI = int.Parse(subField.FieldValueStr);

		m_CacheDic [rd.index] = rd;

		return rd;
	}

}
