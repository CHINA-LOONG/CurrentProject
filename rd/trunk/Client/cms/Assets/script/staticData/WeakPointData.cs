using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeakPointData : StaticDataBase
{
	public class RowData
	{
		public string id;
		public bool	isDamagePoint;
		public	float	damageRate;
		public int health;
		public bool isTarget;
		public int property;

		public string node;
		public int asset;
		public string  collider;
		public string mesh;
		public int initialStatus;
	}

	private Dictionary<string, RowData> m_CacheData = new Dictionary<string, RowData>();

	public RowData getRowDataWithID(string sId)
	{
		if (m_CacheData.ContainsKey (sId)) {
			return m_CacheData [sId];
		}
		
		int nRow = m_StaticTable.GetRowNumWithPrimaryKey ("id", sId);
		if (nRow < 0) {
			Debug.LogError ("Can't find weakpoint id = " + sId);
			return null;
		}
		
		RowData rd = new RowData ();
		rd.id = sId;
		
		FireEngine.FIELD subField;

		subField = m_StaticTable.GetCertainField(nRow, "isDamagePoint");
		rd.isDamagePoint = int.Parse (subField.FieldValueStr) == 1;

		subField = m_StaticTable.GetCertainField(nRow, "damageRate");
		rd.damageRate = float.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField(nRow, "health");
		rd.health = int.Parse (subField.FieldValueStr) ;

		subField = m_StaticTable.GetCertainField(nRow, "isTarget");
		rd.isTarget = int.Parse (subField.FieldValueStr) == 1;

		subField = m_StaticTable.GetCertainField(nRow, "property");
		rd.property = int.Parse (subField.FieldValueStr);


		subField = m_StaticTable.GetCertainField(nRow, "node");
		rd.node = subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField(nRow, "asset");
		rd.asset = int.Parse (subField.FieldValueStr);

		subField = m_StaticTable.GetCertainField(nRow, "collider");
		rd.collider =  subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField(nRow, "mesh");
		rd.mesh =  subField.FieldValueStr;

		subField = m_StaticTable.GetCertainField(nRow, "initialStatus");
		rd.initialStatus = int.Parse (subField.FieldValueStr);

		m_CacheData [sId] = rd;

		return rd;
	}
}
