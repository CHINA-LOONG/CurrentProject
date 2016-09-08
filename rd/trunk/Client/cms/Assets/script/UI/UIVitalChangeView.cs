using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIVitalChangeView : MonoBehaviour
{
    //TODO: fontsettings have default mat
    public string aniName;
    public string spellNameAni;
    public string criticalAniName;
    public Font damageFont;
    public Font healFont;
    public Text vitalWnd;
    //public Image vitalBackImage;
    //public Sprite criticalSprite;
    //public Sprite criticalHealSprite;
    public Text hitResult;
    public Outline hitResultOutline;
    public Color criticalOutlineColor;
    public Color absorbOutlineColor;
    public Color interruptOutlineColor;
    public Color spellnameOutlineColor;
    public Color criticalColor;
    public Color absorbColor;
    public Color interruptColor;
    public Color spellnameColor;

    RectTransform trans;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        trans = transform as RectTransform;
    }
    //---------------------------------------------------------------------------------------------
    public void ShowVitalChange(SpellVitalChangeArgs args, RectTransform parent)
    {
        int vitalChange = args.vitalChange;

        //choose font
        trans.SetParent(parent);
        trans.localScale = Vector3.one;
        trans.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        Color curColor = Color.white;
        Color curOutlineColor = Color.white;
        if (args.vitalType == (int)VitalType.Vital_Type_Default)
        {
            vitalWnd.gameObject.SetActive(true);
            if (vitalChange < 0)
            {
                vitalWnd.font = damageFont;
                vitalChange *= -1;
                //vitalBackImage.sprite = criticalSprite;
            }
            else
            {
                vitalWnd.font = healFont;
                //vitalBackImage.sprite = criticalHealSprite;
            }
            //vitalBackImage.gameObject.SetActive(args.isCritical == true);
            hitResult.gameObject.SetActive(false);
            vitalWnd.text = vitalChange.ToString();
        }
        else if (
            args.vitalType == (int)VitalType.Vital_Type_Miss ||
            args.vitalType == (int)VitalType.Vital_Type_Critical ||
            args.vitalType == (int)VitalType.Vital_Type_Interrupt ||
            args.vitalType == (int)VitalType.Vital_Type_FirstSpell ||
            args.vitalType == (int)VitalType.Vital_Type_Absorbed ||
            args.vitalType == (int)VitalType.Vital_Type_Stun ||
            args.vitalType == (int)VitalType.Vital_Type_Immune ||
            args.vitalType == (int)VitalType.Vital_Type_NoHeal ||
            args.vitalType == (int)VitalType.Vital_Type_PhyImmune ||
            args.vitalType == (int)VitalType.Vital_Type_MagicImmune ||
            args.vitalType == (int)VitalType.Vital_Type_SpellName ||
            args.vitalType == (int)VitalType.Vital_Type_Kezhi
            )
        {
            if (args.vitalType == (int)VitalType.Vital_Type_FirstSpell)
            {
                curColor = criticalColor;
                curOutlineColor = criticalOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_first_spell");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Critical)
            {
                curColor = criticalColor;
                curOutlineColor = criticalOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_critical");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Absorbed)
            {
                curColor = absorbColor;
                curOutlineColor = absorbOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_shield");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Immune)
            {
                curColor = absorbColor;
                curOutlineColor = absorbOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_immune");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Miss)
            {
                curColor = interruptColor;
                curOutlineColor = interruptOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_miss");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Interrupt)
            {
                curColor = interruptColor;
                curOutlineColor = interruptOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_interrupt");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_NoHeal)
            {
                curColor = interruptColor;
                curOutlineColor = interruptOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_noheal");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_PhyImmune)
            {
                curColor = absorbColor;
                curOutlineColor = absorbOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_physicalImmune");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_MagicImmune)
            {
                curColor = absorbColor;
                curOutlineColor = absorbOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_magicImmune");
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_SpellName)
            {
                curColor = spellnameColor;
                curOutlineColor = spellnameOutlineColor;
                hitResult.text = args.wpNode;
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Kezhi)
            {
                curColor = criticalColor;
                curOutlineColor = criticalOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_kezhi");
            }
            else
            {
                curColor = interruptColor;
                curOutlineColor = interruptOutlineColor;
                hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_stun");
            }

            //vitalBackImage.gameObject.SetActive(false);
            vitalWnd.gameObject.SetActive(true);
            vitalWnd.text = string.Empty;
            hitResult.gameObject.SetActive(true);
            hitResult.color = curColor;
            hitResultOutline.effectColor = curOutlineColor;
        }

        //calculate pos
        BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
        if (bo != null)
        {
            Transform targetTrans = bo.transform;
            if (args.vitalType != (int)VitalType.Vital_Type_SpellName)
            {
                GameObject lifebarNode = Util.FindChildByName(bo.gameObject, BattleConst.lifeBarNode);
                if (lifebarNode != null)
                {
                    targetTrans = lifebarNode.transform;
                }

                if (
                    args.wpNode != null &&
                    args.wpNode.Length > 0
                    )
                {
                    GameObject targetNode = Util.FindChildByName(bo.gameObject, args.wpNode);
                    if (targetNode != null)
                    {
                        targetTrans = targetNode.transform;
                    }
                }
            }
            else if (args.vitalMax == -1)
            {
                hitResult.text = hitResult.text + "(" + StaticDataMgr.Instance.GetTextByID("passive") + ")";
            }
            Vector3 pt = BattleCamera.Instance.CameraAttr.WorldToScreenPoint(targetTrans.position);
            float scale = UIMgr.Instance.CanvasAttr.scaleFactor;
            trans.anchoredPosition = new Vector2(pt.x / scale, pt.y / scale);
        }

        //play ani
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            if (args.isCritical)
            {
                if (string.IsNullOrEmpty(criticalAniName) == false)
                {
                    animator.Play(criticalAniName);
                }
            }
            else if(args.vitalType == (int)VitalType.Vital_Type_SpellName)
            {
                if (string.IsNullOrEmpty(spellNameAni) == false)
                {
                    animator.Play(spellNameAni);
                }
            }
            else 
            {
                if (string.IsNullOrEmpty(aniName) == false)
                {
                    animator.Play(aniName);
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnAnimationFinish()
    {
		//Destroy(gameObject);
		//ResourceMgr.Instance.DestroyAsset(gameObject);
    }
    //---------------------------------------------------------------------------------------------
}
