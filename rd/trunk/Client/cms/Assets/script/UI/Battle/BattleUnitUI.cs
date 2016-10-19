using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleUnitUI : MonoBehaviour
{
    public Button dazhaoBtn;
    public Image unitDazhao;
    public Text unitLevel;
    public LifeBarUI lifeBar;
    public EnegyBarUI enegyBar;

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
    private RectTransform trans;

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Awake()
    {
        trans = transform as RectTransform;
        EventTriggerListener.Get(dazhaoBtn.gameObject).onClick = OnDazhaoClick;
        buffView = gameObject.GetComponent<UIBuffView>();
        buffView.Init();
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
            //Hide();
            return;
        }

        dazhaoBtn.gameObject.SetActive(false);
        unitLevel.text = Unit.unit.pbUnit.level.ToString();
        string icon = Unit.unit.GetDazhao().spellData.icon;
        if (!string.IsNullOrEmpty(icon))
        {
            unitDazhao.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(icon) as Sprite;
        }
        //if (sUnit.gameObject.activeSelf)
        //{
        //    lifeBar.value = Unit.unit.curLife / (float)Unit.unit.maxLife;
        //}
        enegyBar.value = Mathf.Clamp01(Unit.unit.energy / (float)BattleConst.enegyMax);

        if (Unit.unit.energy >= BattleConst.enegyMax)
        {
            dazhaoBtn.gameObject.SetActive(true);
        }

        gameObject.SetActive(Unit.unit.isVisible == true);
        RefreshPos();
    }
    //---------------------------------------------------------------------------------------------
    public void SetEnergy(int currentVital)
    {
        enegyBar.value = Mathf.Clamp01(currentVital / (float)BattleConst.enegyMax);
        dazhaoBtn.gameObject.SetActive(currentVital >= BattleConst.enegyMax);
    }
    //---------------------------------------------------------------------------------------------
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    public void Destroy()
    {
        gameObject.SetActive(false);
        ResourceMgr.Instance.DestroyAsset(gameObject);
        //Destroy(gameObject);
    }
    //---------------------------------------------------------------------------------------------
    void OnDazhaoClick(GameObject go)
    {
		GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.DazhaoBtnClicked, Unit);
		//GameEventMgr.Instance.FireEvent<bool,bool> (GameEventList.SetMirrorModeState, false,false);
    }
    //---------------------------------------------------------------------------------------------
    void RefreshPos()
    {
        if (targetUnit == null)
            return;

        Transform targetTrans = targetUnit.transform;
        //GameObject headNode = Util.FindChildByName(targetUnit.gameObject, BattleConst.headNode);
        //if (headNode != null)
        //{
        //    targetTrans = headNode.transform;
        //}

        Vector3 targetPos = new Vector3();
        targetPos.x = targetTrans.position.x;
        targetPos.y = targetTrans.position.y + BattleConst.lifeBarDistance;
        targetPos.z = targetTrans.position.z;
        Vector3 pt = BattleCamera.Instance.CameraAttr.WorldToScreenPoint(targetPos);
        float scale = UIMgr.Instance.CanvasAttr.scaleFactor;
        trans.anchoredPosition = new Vector2(pt.x / scale, pt.y / scale);
    }
    //---------------------------------------------------------------------------------------------
}
