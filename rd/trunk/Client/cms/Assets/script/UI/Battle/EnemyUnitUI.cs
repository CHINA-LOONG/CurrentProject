using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUnitUI : MonoBehaviour 
{
	public Text unitName;
	public Text unitLevel;
	public LifeBarUI lifeBar;
    public Image propertyImage;

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
    public void Destroy()
    {
        ResourceMgr.Instance.DestroyAsset(gameObject);
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
