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
    public Image frame;
    public Sprite normalFrame;
    public Sprite emptyFrame;
    public Sprite deadFrame;
    public Sprite defaulthead;
    public Sprite toBeEntreFrame;

    public int targetId
    {
        set;
        get;
    }
    GameUnit unit;

    float maskHeight = 152;
    float lifeBarWidth = 132;
    float energyBarHeight;

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<Button>();
        RectTransform energyBarTrans = energyBar.gameObject.transform as RectTransform;
        energyBarHeight = energyBarTrans.rect.height;
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
        head.sprite = unit.headImg;
        nameText.text = unit.name;

        if (unit != null && (unit.curLife > 0 && unit.State != UnitState.Dead))
        {
            cdmask.gameObject.SetActive(false);
            lifeBar.gameObject.SetActive(true);
            energyBar.gameObject.SetActive(true);
            frame.sprite = normalFrame;
        }
        else
        {
            ShowEmpty(true);
        }

        if (BattleController.Instance.Process.SwitchPetCD > 0)
        {
            cdmask.gameObject.SetActive(true);
        }
    }

    public void ShowEmpty(bool isDead)
    {
        ShowCDmask();

        //血条为空
        var size = lifeBar.rectTransform.sizeDelta;
        size.x = 0;
        lifeBar.rectTransform.sizeDelta = size;
        lifeBar.gameObject.SetActive(false);

        //能量为空
        size = energyBar.rectTransform.sizeDelta;
        size.y = 0;
        energyBar.rectTransform.sizeDelta = size;
        energyBar.gameObject.SetActive(false);

        if (isDead)
        {
            frame.sprite = deadFrame;
        }
        else
        {
            nameText.text = "";
            frame.sprite = emptyFrame;
            head.sprite = defaulthead;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateData(GameUnit unit)
    {
        if (unit != null)
        {
            //unit发生了变化
            if (this.unit!=unit)
            {
                this.unit = unit;
                InitItem();
            }
            else
            {
                if (unit.curLife > 0 && unit.State != UnitState.Dead)
                {
                    if (BattleController.Instance.Process.SwitchPetCD > 0)
                    {
                        var size = cdmask.rectTransform.sizeDelta;
                        size.y = maskHeight * (BattleController.Instance.Process.SwitchPetCD / BattleConst.switchPetCD);
                        cdmask.rectTransform.sizeDelta = size;
                    }
                    else 
                    {
                        cdmask.gameObject.SetActive(false);
                    }

                    {
                        var size = lifeBar.rectTransform.sizeDelta;
                        size.x = lifeBarWidth * (unit.curLife / (float)unit.maxLife);
                        lifeBar.rectTransform.sizeDelta = size;

                        size = energyBar.rectTransform.sizeDelta;
                        size.y = energyBarHeight * (unit.energy / (float)BattleConst.enegyMax);
                        energyBar.rectTransform.sizeDelta = size;
                    }

                    if (unit.State == UnitState.ToBeEnter)
                    {
                        frame.sprite = toBeEntreFrame;
                        ShowCDmask();
                    }
                }
                else 
                {
                    ShowEmpty(true);
                }
            }
        }
        else
            Hide();
    }

    void ShowCDmask()
    {
        cdmask.gameObject.SetActive(true);
        var size = cdmask.rectTransform.sizeDelta;
        size.y = maskHeight;
        cdmask.rectTransform.sizeDelta = size;
    }
}
