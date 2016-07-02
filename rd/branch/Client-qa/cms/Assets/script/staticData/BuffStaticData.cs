using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuffStaticData : StaticDataBase
{
    private Dictionary<string, BuffPrototype> m_CacheDic = new Dictionary<string, BuffPrototype>();

    public BuffPrototype GetBuffRawData(string id)
    {
        if (m_CacheDic.ContainsKey(id))
        {
            return m_CacheDic[id];
        }

        int nRow = m_StaticTable.GetRowNumWithPrimaryKey("id", id);
        if (nRow < 0)
        {
            Debug.LogError("Can't find buff = " + id);
            return null;
        }

        BuffPrototype bpt = new BuffPrototype();
        bpt.id = id;

        FireEngine.FIELD subField;
        subField = m_StaticTable.GetCertainField(nRow, "category");
        bpt.category = int.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "periodEffect");
        bpt.periodEffectID = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "duration");
        bpt.duration = int.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "stun");
        bpt.stun = int.Parse(subField.FieldValueStr) > 0;

        subField = m_StaticTable.GetCertainField(nRow, "invincible");
        bpt.invincible = int.Parse(subField.FieldValueStr) > 0;

        subField = m_StaticTable.GetCertainField(nRow, "strengthRatio");
        bpt.strengthRatio = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "intelligenceRatio");
        bpt.intelligenceRatio = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "defenseRatio");
        bpt.defenseRatio = float.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "speedRatio");
        bpt.speedRatio = float.Parse(subField.FieldValueStr);

        m_CacheDic[bpt.id] = bpt;

        return bpt;
    }

}
