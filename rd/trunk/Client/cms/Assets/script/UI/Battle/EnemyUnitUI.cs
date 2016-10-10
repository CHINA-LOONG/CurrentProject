using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUnitUI : MonoBehaviour 
{
	public Text unitName;
	public Text unitLevel;
	public LifeBarUI lifeBar;
    public Image propertyImage;
    public ShakeUi shakeUi;
    public EnegyBarUI mEnergyBar;
    public Image dazhaoImage;

    public BattleObject Unit
    {
        get
        {
            return targetUnit;
        }
        set
        {
            targetUnit = value;
        }
    }
    private BattleObject targetUnit;
    private UIBuffView buffView;
	RectTransform trans;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        buffView = gameObject.GetComponent<UIBuffView>();
        buffView.Init();
		trans = transform as RectTransform;
    }
    //---------------------------------------------------------------------------------------------
    void Update()
    {
        RefreshPos();
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeBuffState(SpellBuffArgs args)
    {
        buffView.OnBuffChanged(args);
    }
    //---------------------------------------------------------------------------------------------
    public void Show(BattleObject sUnit)
    {
        buffView.SetTargetUnit(sUnit);
        lifeBar.LifeTarget = sUnit;
        Unit = sUnit;
        if (sUnit == null)
        {
            Hide();
            return;
        }
        Sprite psprite = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + sUnit.unit.property);
        if (null != psprite )
        {
            propertyImage.sprite = psprite;
        }

        if (dazhaoImage != null)
        {
            Spell dazhao = Unit.unit.GetDazhao();
            if (dazhao != null)
            {
                string icon = dazhao.spellData.icon;
                if (!string.IsNullOrEmpty(icon))
                {
                    dazhaoImage.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(icon) as Sprite;
                }
            }
        }

        unitName.text = sUnit.unit.name;
        unitLevel.text = sUnit.unit.pbUnit.level.ToString();

        gameObject.SetActive(Unit.unit.isVisible == true);
        RefreshPos();
    }
    //---------------------------------------------------------------------------------------------
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    public void StartShake()
    {
        if(null!=shakeUi)
        {
            shakeUi.SetShake();
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Destroy()
    {
        ResourceMgr.Instance.DestroyAsset(gameObject);
    }
    //---------------------------------------------------------------------------------------------
    //for pve
    public void SetEnergyBar(float fraction)
    {
        if (mEnergyBar != null)
        {
            mEnergyBar.value = fraction;
        }
    }
    //---------------------------------------------------------------------------------------------
    //for pvp
    public void SetEnergy(int currentVital)
    {
        if (mEnergyBar != null)
        {
            mEnergyBar.value = Mathf.Clamp01(currentVital / (float)BattleConst.enegyMax);
        }
    }
    //---------------------------------------------------------------------------------------------
    void RefreshPos()
    {
        if (targetUnit == null || targetUnit.unit.isBoss == true)
            return;

        Transform targetTrans = targetUnit.transform;
        GameObject headNode = Util.FindChildByName(targetUnit.gameObject, BattleConst.headNode);
        if (headNode != null)
        {
            targetTrans = headNode.transform;
        }

        Vector3 pt = BattleCamera.Instance.CameraAttr.WorldToScreenPoint(targetTrans.position);
        float scale = UIMgr.Instance.CanvasAttr.scaleFactor;
        trans.anchoredPosition = new Vector2(pt.x / scale, pt.y / scale);
    }
    //---------------------------------------------------------------------------------------------
}
