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
            if (unit.curLife > 0)
            {
                GameEventMgr.Instance.FireEvent<int, int>(GameEventList.SwitchPet, targetId, unit.pbUnit.guid);
                GameEventMgr.Instance.FireEvent(GameEventList.HideSwitchPetUI);
            }
        }
    }

    public void Show(int targetId, GameUnit unit)
    {
        this.targetId = targetId;
        this.unit = unit;

        var unitData = StaticDataMgr.Instance.GetUnitRowData(unit.pbUnit.id);
        var headPath = unitData.uiAsset;
        var index = headPath.LastIndexOf('/');
        var assetbundle = headPath.Substring(0, index);
        var assetname = headPath.Substring(index + 1, headPath.Length - index - 1);
        var image = ResourceMgr.Instance.LoadAssetType<Sprite>(assetbundle, assetname) as Sprite;

        head.sprite = image;

        if (unit.curLife > 0)
        {
            cdmask.gameObject.SetActive(false);
            deadtext.gameObject.SetActive(false);
        }
        else
        {
            cdmask.gameObject.SetActive(true);
            deadtext.gameObject.SetActive(true);
        }

        if (BattleController.Instance.Process.SwitchPetCD > 0)
        {
            cdmask.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if ( unit.curLife > 0)
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
