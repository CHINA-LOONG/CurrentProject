using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellStaticData : StaticDataBase
{
    private Dictionary<string, SpellProtoType> m_CacheDic = new Dictionary<string, SpellProtoType>();

    public SpellProtoType GetSpellRawData(string id)
    {
        if (m_CacheDic.ContainsKey(id))
        {
            return m_CacheDic[id];
        }

        int nRow = m_StaticTable.GetRowNumWithPrimaryKey("id", id);
        if (nRow < 0)
        {
            Debug.LogError("Can't find Spell = " + id);
            return null;
        }

        SpellProtoType spt = new SpellProtoType();
        spt.id = id;

        FireEngine.FIELD subField;
        subField = m_StaticTable.GetCertainField(nRow, "effectID");
        spt.rootEffectID = subField.FieldValueStr;

        subField = m_StaticTable.GetCertainField(nRow, "energyCost");
        spt.energyCost = int.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "energyGenerate");
        spt.energyGenerate = int.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "category");
        spt.category = int.Parse(subField.FieldValueStr); 

        subField = m_StaticTable.GetCertainField(nRow, "levelAdjust");
        spt.levelAdjust = int.Parse(subField.FieldValueStr);

        m_CacheDic[spt.id] = spt;

        return spt;
    }

}
