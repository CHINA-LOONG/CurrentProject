using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnitBaseData : StaticDataBase
{
    [Serializable]
	public	class RowData
	{
		public int level;
		public int experience;
		public int health;
		public int strength;
		public int intelligence;
		public int speed;
		public int defense;
		public int endurance;	
		//public float hitRate;
		//public float criticalRate;
		//public float criticalFactor;
		public int recovery;
		public int goldNoteValue;
		public int goldNoteMin;
		public int goldNoteMax;
		public int expMin;
		public int expMax;
		public int output;	
	}

	private Dictionary<int,UnitBaseData.RowData> m_CacheData = new Dictionary<int, UnitBaseData.RowData>();

	public UnitBaseData.RowData getRowDataFromLevel(int iLevel)
	{
		if (m_CacheData.ContainsKey (iLevel))
		{
			return m_CacheData[iLevel];
		}

		int nRow = m_StaticTable.GetRowNumWithPrimaryKey ("level", iLevel.ToString ());
		if (nRow < 0)
		{
			Debug.LogError("Can't find UnitBaseData level = " +  iLevel);
			return null;
		}
		
		RowData rd = new RowData ();
		rd.level = iLevel;
		
		FireEngine.FIELD subField;

		subField = m_StaticTable.GetCertainField (nRow, "experience");
		rd.experience = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "health");
		rd.health = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "strength");
		rd.strength = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "intelligence");
		rd.intelligence = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "speed");
		rd.speed = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "defense");
		rd.defense = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "endurance");
		rd.endurance = int.Parse (subField.FieldValueStr);

        //subField = m_StaticTable.GetCertainField (nRow, "hitRate");
        //rd.hitRate = float.Parse (subField.FieldValueStr);

        //subField = m_StaticTable.GetCertainField (nRow, "criticalRate");
        //rd.criticalRate = float.Parse (subField.FieldValueStr);

        //subField = m_StaticTable.GetCertainField (nRow, "criticalFactor");
        //rd.criticalFactor = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "recovery");
		rd.recovery = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "goldNoteMin");
		rd.goldNoteMin = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "goldNoteMax");
		rd.goldNoteMax = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "expMin");
		rd.expMin = int.Parse (subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "expMax");
        rd.expMax = int.Parse(subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField (nRow, "output");
		rd.output = int.Parse (subField.FieldValueStr);

		m_CacheData [rd.level] = rd;

		return rd;
	}
}
