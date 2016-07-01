using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleData : StaticDataBase
{
    public class RowData
    {
        public int id;
        public string name;
        public string enemy;
        public string process;

        public List<int> processList = null;
    }

    private Dictionary<int, RowData> m_CacheData = new Dictionary<int, RowData>();

    public RowData getRowDataFromLevel(int iId)
    {
        if (m_CacheData.ContainsKey(iId))
        {
            return m_CacheData[iId];
        }

        int nRow = m_StaticTable.GetRowNumWithPrimaryKey("id", iId.ToString());
        if (nRow < 0)
        {
            Debug.LogError("Can't find Battle id = " + iId);
            return null;
        }

        RowData rd = new RowData();
        rd.id = iId;

        FireEngine.FIELD subField;

        subField = m_StaticTable.GetCertainField(nRow, "name");
        rd.name = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "enemy");
        rd.enemy = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "process");
        rd.process = subField.FieldValueStr;

        if (!string.IsNullOrEmpty(rd.process))
        {
            rd.processList = new List<int>();
            string[] res = rd.process.Split(';');
            foreach (var item in res)
            {
                if (!string.IsNullOrEmpty(item))
                    rd.processList.Add(int.Parse(item.Trim()));
            }
        }

        m_CacheData[rd.id] = rd;

        return rd;
    }
}
