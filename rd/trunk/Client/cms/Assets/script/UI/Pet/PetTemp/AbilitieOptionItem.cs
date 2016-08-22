using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilitieOptionItem : MonoBehaviour
{
    public Transform iconPos;
    private SpellIcon spellIcon;
    public Image imageSelect;
    public bool IsSelect
    {
        set
        {
            imageSelect.gameObject.SetActive(value);
        }
    }

    public Spell curData;

    public void ReloadData(Spell spell)
    {
        curData = spell;
        if (spellIcon==null)
        {
            spellIcon = SpellIcon.CreateWith(iconPos);
        }
        spellIcon.SetData(curData.level, curData.spellData.id);
        IsSelect = false;
    }
    
}
