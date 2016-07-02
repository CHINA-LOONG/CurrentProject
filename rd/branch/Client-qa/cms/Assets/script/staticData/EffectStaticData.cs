using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/*

public class EffectStaticData 
{
    private Dictionary<string, EffectPrototype> m_CacheDic = new Dictionary<string, EffectPrototype>();

    public EffectPrototype GetEffectRawData(string id)
    {
        if (m_CacheDic.ContainsKey(id))
        {
            return m_CacheDic[id];
        }

        int nRow = m_StaticTable.GetRowNumWithPrimaryKey("id", id);
        if (nRow < 0)
        {
            Debug.LogError("Can't find effect = " + id);
            return null;
        }

        EffectPrototype spt = null;
        FireEngine.FIELD subField;
        subField = m_StaticTable.GetCertainField(nRow, "type");
        EffectType et = (EffectType)(int.Parse(subField.FieldValueStr));
        switch (et)
        {
            case EffectType.Effect_Type_Set:
                {
                    spt = new EffectSetPrototype();
                    EffectSetPrototype setPt = spt as EffectSetPrototype;
                    subField = m_StaticTable.GetCertainField(nRow, "effectList");
                    string [] effects = subField.FieldValueStr.Split(';');
                    for (int i = 0; i < effects.Length; ++i)
                    {
                        setPt.effectList.Add(effects[i]);
                    }
                }
                break;
            case EffectType.Effect_Type_Search:
                {
                    spt = new EffectSearchPrototype();
                    EffectSearchPrototype searchPt = spt as EffectSearchPrototype;
                    
                    subField = m_StaticTable.GetCertainField(nRow, "count");
                    searchPt.count = int.Parse(subField.FieldValueStr);

                    subField = m_StaticTable.GetCertainField(nRow, "camp");
                    searchPt.camp = int.Parse(subField.FieldValueStr);

                    subField = m_StaticTable.GetCertainField(nRow, "searchEffect");
                    searchPt.effectID = subField.FieldValueStr;
                }
                break;
            case EffectType.Effect_Type_Persistent:
                {
                    spt = new EffectPersistentProtoType();
                    EffectPersistentProtoType persistPt = spt as EffectPersistentProtoType;

                    subField = m_StaticTable.GetCertainField(nRow, "effectStartID");
                    persistPt.effectStartID = subField.FieldValueStr;

                    subField = m_StaticTable.GetCertainField(nRow, "startDelay");
                    persistPt.startDelayTime = float.Parse(subField.FieldValueStr);

                    subField = m_StaticTable.GetCertainField(nRow, "periodEffectList");
                    string []effectList = subField.FieldValueStr.Split(';');
                    for (int i = 0; i < effectList.Length; ++i)
                    {
                        string []effectKV = effectList[i].Split('|');
                        if (effectKV.Length != 2)
                            continue;

                        persistPt.effectList.Add(new KeyValuePair<float, string>(float.Parse(effectKV[0]), effectKV[1]));
                    }
                }
                break;
            case EffectType.Effect_Type_Damage:
                {
                    spt = new EffectDamageProtoType();
                    EffectDamageProtoType damagePt = spt as EffectDamageProtoType;

                    subField = m_StaticTable.GetCertainField(nRow, "damageType");
                    damagePt.damageType = int.Parse(subField.FieldValueStr);

                    subField = m_StaticTable.GetCertainField(nRow, "attackFactor");
                    damagePt.attackFactor = float.Parse(subField.FieldValueStr);

                    subField = m_StaticTable.GetCertainField(nRow, "isHeal");
                    damagePt.isHeal = (int.Parse(subField.FieldValueStr)) > 0;

                    subField = m_StaticTable.GetCertainField(nRow, "damageProp");
                    damagePt.damageProperty = int.Parse(subField.FieldValueStr);
                }
                break;
            case EffectType.Effect_Type_Buff:
                {
                    spt = new EffectApplyBuffPrototype();
                    EffectApplyBuffPrototype buffPt = spt as EffectApplyBuffPrototype;
                    
                    subField = m_StaticTable.GetCertainField(nRow, "buffID");
                    buffPt.buffID = subField.FieldValueStr;
                }
                break;
        }

        spt.id = id;
        spt.effectType = et;

        subField = m_StaticTable.GetCertainField(nRow, "targetType");
        spt.targetType = int.Parse(subField.FieldValueStr);

        subField = m_StaticTable.GetCertainField(nRow, "casterType");
        spt.casterType = int.Parse(subField.FieldValueStr);

        m_CacheDic[spt.id] = spt;

        return spt;
    }

}
*/
