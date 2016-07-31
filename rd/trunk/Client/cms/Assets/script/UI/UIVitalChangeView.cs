using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIVitalChangeView : MonoBehaviour
{
    //TODO: fontsettings have default mat
    public string aniName;
    public string criticalAniName;
    public Font damageFont;
    public Font healFont;
    public Text vitalWnd;
    public Image vitalBackImage;
    public Sprite criticalSprite;
    public Sprite criticalHealSprite;
    //modify: xuelong 2015-8-31 11:50:51  由于图片中心点不同。不能直接设置
    //public Sprite missSprite;
    //public Sprite interruptSprite;
    public Text hitResult;
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
        if (args.vitalType == (int)VitalType.Vital_Type_Default)
        {
            vitalWnd.gameObject.SetActive(true);
            if (vitalChange < 0)
            {
                vitalWnd.font = damageFont;
                vitalChange *= -1;
                vitalBackImage.sprite = criticalSprite;
            }
            else
            {
                vitalWnd.font = healFont;
                vitalBackImage.sprite = criticalHealSprite;
            }
            vitalBackImage.gameObject.SetActive(args.isCritical == true);
            hitResult.gameObject.SetActive(false);
            vitalWnd.text = vitalChange.ToString();
        }
        else if (args.vitalType == (int)VitalType.Vital_Type_Miss)
        {
            //vitalBackImage.sprite = missSprite;
            vitalBackImage.gameObject.SetActive(false);
            vitalWnd.gameObject.SetActive(true);
            vitalWnd.text = string.Empty;
            hitResult.gameObject.SetActive(true);
            hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_miss");
        }
        else if (args.vitalType == (int)VitalType.Vital_Type_Interrupt)
        {
            //vitalBackImage.sprite = interruptSprite;
            vitalBackImage.gameObject.SetActive(false);
            vitalWnd.gameObject.SetActive(true);
            vitalWnd.text = string.Empty;
            hitResult.gameObject.SetActive(true);
            hitResult.text = StaticDataMgr.Instance.GetTextByID("spell_hit_interrupt");
        }
        else if (
            args.vitalType == (int)VitalType.Vital_Type_FirstSpell ||
            args.vitalType == (int)VitalType.Vital_Type_Absorbed ||
            args.vitalType == (int)VitalType.Vital_Type_Stun ||
            args.vitalType == (int)VitalType.Vital_Type_Immune
            )
        {
            string textID;
            if(args.vitalType == (int)VitalType.Vital_Type_FirstSpell)
            {
                textID = "spell_first_spell";
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Absorbed)
            {
                textID = "spell_shield";
            }
            else if (args.vitalType == (int)VitalType.Vital_Type_Immune)
            {
                textID = "spell_hit_immune";
            }
            else 
            {
                textID = "spell_stun";
            }
            vitalBackImage.gameObject.SetActive(false);
            vitalWnd.gameObject.SetActive(true);
            vitalWnd.text = string.Empty;
            hitResult.gameObject.SetActive(true);
            hitResult.text = StaticDataMgr.Instance.GetTextByID(textID);
        }

        //calculate pos
        BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
        if (bo != null)
        {
            Transform targetTrans = bo.transform;
            GameObject lifebarNode = Util.FindChildByName(bo.gameObject, BattleConst.lifeBarNode);
            if (lifebarNode != null)
            {
                targetTrans = lifebarNode.transform;
            }

            if (args.wpNode != null && args.wpNode.Length > 0)
            {
                GameObject targetNode = Util.FindChildByName(bo.gameObject, args.wpNode);
                if (targetNode != null)
                {
                    targetTrans = targetNode.transform;
                }
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
		ResourceMgr.Instance.DestroyAsset(gameObject);
    }
    //---------------------------------------------------------------------------------------------
}
