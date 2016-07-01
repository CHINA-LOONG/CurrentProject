using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcessData : StaticDataBase
{
    public class RowData
    {
        public int id = 0;
        public string name;
        public string processAnimation;
        public string preAnimation;
        public bool isClearBuff;
        //1.敌法全部死亡
        public int endCondition = 1;

        public string state;
    }

    private Dictionary<int, RowData> m_CacheData = new Dictionary<int, RowData>();

    public RowData getRowDataFromLevel(int iId)
    {
        Logger.Log(iId);
        if (m_CacheData.ContainsKey(iId))
        {
            return m_CacheData[iId];
        }

        int nRow = m_StaticTable.GetRowNumWithPrimaryKey("id", iId.ToString());
        if (nRow < 0)
        {
            Debug.LogError("Can't find Process id = " + iId);
            return null;
        }

        RowData rd = new RowData();
        rd.id = iId;

        FireEngine.FIELD subField;

        subField = m_StaticTable.GetCertainField(nRow, "name");
        rd.name = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "processAnimation");
        rd.processAnimation = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "preAnimation");
        rd.preAnimation = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "isClearBuff");
        rd.isClearBuff = int.Parse(subField.FieldValueStr)!=0;

        subField = m_StaticTable.GetCertainField(nRow, "endCondition");
        rd.endCondition = int.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "state");
        rd.state = subField.FieldValueStr;

        m_CacheData[rd.id] = rd;

        return rd;
    }
}
