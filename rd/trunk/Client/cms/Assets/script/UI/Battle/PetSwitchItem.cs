using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//IPointerClickHandler
public class PetSwitchItem : MonoBehaviour, IPointerClickHandler
{
    public Image head;
    public Image lifeBar;
    public Image energyBar;
    public Image cdmask;
    public Text nameText;
    public Image tips;
    public Sprite emptyFrame;
    public Sprite deadFrame;
    public Sprite defaulthead;
    public RectTransform mBackgroud;

    public int targetId
    {
        set;
        get;
    }
    GameUnit unit;

    float maskHeight;
    RectTransform lifebarSize;
    float lifebarX;
    float lifebarY;
    RectTransform energybarSize;
    float energybarX;
    float energybarY;

    // Use this for initialization
    void Awake()
    {
        maskHeight = cdmask.rectTransform.sizeDelta.y;

        lifebarSize = lifeBar.transform as RectTransform;
        lifebarX = lifebarSize.sizeDelta.x;
        lifebarY = lifebarSize.sizeDelta.y;
    
        energybarSize = energyBar.transform as RectTransform;
        energybarX = energybarSize.sizeDelta.x;
        energybarY = energybarSize.sizeDelta.y;

        ShowEmpty(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BattleController.Instance.Process.SwitchPetCD == 0)
        {
            if (unit != null &&
                unit.curLife > 0 &&
                unit.State != UnitState.Dead &&
                unit.State != UnitState.ToBeEnter
                )
            {
                GameEventMgr.Instance.FireEvent<int, int>(GameEventList.SwitchPet, targetId, unit.pbUnit.guid);
                GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
            }
        }
    }

    //public void Show(int targetId, GameUnit unit)
    //{
    //    this.targetId = targetId;
    //    this.unit = unit;

    //    InitItem();
    //}

    void InitItem()
    {
        if (unit==null)
        {
            ShowEmpty(false);
        }
        else
	    {
            head.sprite = unit.headImg;
            nameText.text = StaticDataMgr.Instance.GetTextByID(unit.name);

            if ((unit.curLife > 0 && unit.State != UnitState.Dead))
            {
                lifeBar.gameObject.SetActive(true);
                energyBar.gameObject.SetActive(true);
                tips.gameObject.SetActive(false);
            }
            else 
            {
                ShowEmpty(true);
            }
        }
    }

    public void ShowEmpty(bool isDead)
    {
        //血条为空
        lifebarSize.sizeDelta = Vector2.zero;
        lifeBar.gameObject.SetActive(false);

        //能量为空
        energybarSize.sizeDelta = Vector2.zero;
        energyBar.gameObject.SetActive(false);

        tips.gameObject.SetActive(true);
        if (isDead)
        {
            tips.sprite = deadFrame;
            cdmask.gameObject.SetActive(true);
            var size = cdmask.rectTransform.sizeDelta;
            size.y = maskHeight;
            cdmask.rectTransform.sizeDelta = size;
        }
        else
        {
            unit = null;
            nameText.text = "";
            //tips.sprite = emptyFrame;
            tips.gameObject.SetActive(false);
            head.sprite = defaulthead;
        }
        tips.SetNativeSize();
    }

    public void UpdateData(GameUnit unit, bool forceRefresh = false)
    {
        //unit发生了变化
        if (this.unit!=unit || forceRefresh)
        {
            this.unit = unit;
            InitItem();
        }
        if(unit!=null)
        {
            lifebarSize.sizeDelta = new Vector2(Mathf.Clamp01(unit.curLife / (float)unit.maxLife) * lifebarX, lifebarY);
            energybarSize.sizeDelta = new Vector2(Mathf.Clamp01(unit.energy / (float)BattleConst.enegyMax) * energybarX, energybarY);
            tips.gameObject.SetActive(false);
            
            if (unit.State == UnitState.Dead)
	        {
                ShowEmpty(true);
	        }
            else if (BattleController.Instance.Process.SwitchPetCD > 0)
            {
                if (!cdmask.gameObject.activeInHierarchy)
                {
                    cdmask.gameObject.SetActive(true);
                }
                var size = cdmask.rectTransform.sizeDelta;
                size.y = maskHeight * (BattleController.Instance.Process.SwitchPetCD / BattleConst.switchPetCD);
                cdmask.rectTransform.sizeDelta = size;
            }
            else
            {
                if (cdmask.gameObject.activeInHierarchy)
                {
                    cdmask.gameObject.SetActive(false);
                }
            }
        }
    }

    public void InverseX()
    {
        mBackgroud.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
    }
}
