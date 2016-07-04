using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//IPointerClickHandler
public class PetSwitchItem : MonoBehaviour, IPointerClickHandler
{
    public Image head;
    public Image lifeBar;
    public Image cdmask;
    public Text deadtext;
    public Text nameText;

    int targetId;
    GameUnit unit;

    float maskHeight = 152;
    float lifeBarWidth = 132;

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BattleController.Instance.Process.SwitchPetCD == 0)
        {
            if (unit != null && unit.curLife > 0)
            {
                GameEventMgr.Instance.FireEvent<int, int>(GameEventList.SwitchPet, targetId, unit.pbUnit.guid);
                GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
            }
        }
    }

    public void Show(int targetId, GameUnit unit)
    {
        this.targetId = targetId;
        this.unit = unit;

        InitItem();
    }

    void InitItem()
    {
        var unitData = StaticDataMgr.Instance.GetUnitRowData(unit.pbUnit.id);
        var headPath = unitData.uiAsset;
        var index = headPath.LastIndexOf('/');
        var assetbundle = headPath.Substring(0, index);
        var assetname = headPath.Substring(index + 1, headPath.Length - index - 1);
        var image = ResourceMgr.Instance.LoadAssetType<Sprite>(assetbundle, assetname) as Sprite;

        head.sprite = image;
        nameText.text = unit.name;

        if (unit.curLife > 0)
        {
            cdmask.gameObject.SetActive(false);
            deadtext.gameObject.SetActive(false);
        }
        else
        {
            cdmask.gameObject.SetActive(true);
            deadtext.gameObject.SetActive(true);

            //死亡血条为空
            var size = lifeBar.rectTransform.sizeDelta;
            size.x = 0;
            lifeBar.rectTransform.sizeDelta = size;
        }

        if (BattleController.Instance.Process.SwitchPetCD > 0)
        {
            cdmask.gameObject.SetActive(true);
        }
    }

    public void ShowEmpty()
    {
        nameText.text = "";
        cdmask.gameObject.SetActive(true);
        deadtext.gameObject.SetActive(true);
        deadtext.text = "无怪物";

        //血条为空
        var size = lifeBar.rectTransform.sizeDelta;
        size.x = 0;
        lifeBar.rectTransform.sizeDelta = size;

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
                if (unit.curLife > 0)
                {
                    if (BattleController.Instance.Process.SwitchPetCD > 0)
                    {
                        var size = cdmask.rectTransform.sizeDelta;
                        size.y = maskHeight * (BattleController.Instance.Process.SwitchPetCD / BattleConst.switchPetCD);
                        cdmask.rectTransform.sizeDelta = size;
                    }

                    {
                        var size = lifeBar.rectTransform.sizeDelta;
                        size.x = lifeBarWidth * (unit.curLife / (float)unit.maxLife);
                        lifeBar.rectTransform.sizeDelta = size;
                    }
                }
            }
        }
        else
            Hide();
    }
}
