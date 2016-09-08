using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class AbilitieOptionItem : MonoBehaviour
{
    public Transform iconPos;
    private SpellIcon spellIcon;
    public Image imageSelect;
    public Image imageEffect;
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
        InitEffect();
        curData = spell;
        if (spellIcon==null)
        {
            spellIcon = SpellIcon.CreateWith(iconPos);
        }
        spellIcon.SetData(curData.level, curData.spellData.id);
        IsSelect = false;
    }
    public void PlayEffect()
    {
        imageEffect.gameObject.SetActive(true);
        Tweener tweener1 = imageEffect.transform.DOScale(2.0f, 0.5f).
                            SetEase(Ease.Linear);
        Tweener tweener2 = imageEffect.DOFade(0.0f, 0.5f).
                            SetEase(Ease.Linear).
                            OnComplete(delegate () { InitEffect(); });
    }
    void InitEffect()
    {
        imageEffect.transform.localScale = Vector3.one;
        imageEffect.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        imageEffect.gameObject.SetActive(false);
    }

}
