using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIVitalChangeView : MonoBehaviour
{
    public string aniName;
    public Font damageFont;
    public Font healFont;
    public Text vitalWnd;
    public Image vitalBackImage;
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
        if (vitalChange == 0)
            return;

        //choose font
        trans.SetParent(parent);
        trans.localScale = Vector3.one;
        trans.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        if (vitalChange < 0)
        {
            vitalWnd.font = damageFont;
            vitalChange *= -1;
        }
        else
        {
            vitalWnd.font = healFont;
        }
        vitalWnd.text = vitalChange.ToString();

        //calculate pos
        BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
        if (bo != null)
        {
            //TODO; use a child node
            Vector3 pt = BattleCamera.Instance.CameraAttr.WorldToScreenPoint(bo.gameObject.transform.position);
            float scale = UIMgr.Instance.CanvasAttr.scaleFactor;
            trans.anchoredPosition = new Vector2(pt.x / scale, pt.y / scale);
        }

        vitalBackImage.gameObject.SetActive(args.isCritical == true);

        //play ani
        if (aniName == null || aniName.Length == 0)
            return;
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(aniName);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnAnimationFinish()
    {
        Destroy(gameObject);
    }
    //---------------------------------------------------------------------------------------------
}
