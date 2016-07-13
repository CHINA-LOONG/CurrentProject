using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PetDetailLeft : MonoBehaviour {

    public Text     name;
    public Text     zhanli;
    public Text     level;
    public Text     property;
    public Image     proIcon;

    public Text     expLabel;
    public Text     characterLabel;
    public Text     character;
    public Slider   expProgress;
    public Text     expContent;

    public Text     attrLabel;
    public Text     health;
    public Text     strength;
    public Text     defense;
    public Text     intellgence;
    public Text     speed;
    public Image    stageBadge;
    public Button   addExpBtn;

    public Button   changeCharacterBtn;
    public Button   detailAttrBtn;
    public Button   skillpBtn;
    public Button   stageBtn;
    public Button   advanceBtn;
    public Button   mainPet;

    public RawImage modelImage;

    GameUnit m_unit;

	// Use this for initialization
	void Start () {
        GameEventMgr.Instance.AddListener(PetViewConst.ReloadPetStageNotify, ReloadPetStage);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        GameEventMgr.Instance.RemoveListener(PetViewConst.ReloadPetStageNotify, ReloadPetStage);
    }

    void ReloadPetStage()
    {
        if (m_unit != null)
        {
            ReloadData(m_unit, false);
        }
    }

    public void ReloadData(GameUnit unit, bool reloadUnit = true)
    {
        m_unit = unit;
        if (reloadUnit == true)
        {
            GameObject petCamera = GameObject.Find(PetViewConst.UIPetModelCameraAssetName);
            GameObject monster = Util.FindChildByName(petCamera, "monsterModel");
            if (monster != null)
            {
                ResourceMgr.Instance.DestroyAsset(monster);
                monster = null;
            }

            monster = ResourceMgr.Instance.LoadAsset(unit.assetID);
            monster.transform.SetParent(petCamera.transform);
            monster.name = "monsterModel";
            monster.transform.localPosition = new Vector3(0, -0.5f, 1.5f);
            monster.transform.localScale = Vector3.one;
            monster.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -180, transform.localEulerAngles.z);

            modelImage.texture = petCamera.GetComponent<Camera>().targetTexture;
        }
        
        UIUtil.SetStageColor(name, unit);

        expLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftexperience);
        characterLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftcharacter);
        attrLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttr);
        character.text = unit.pbUnit.character.ToString();
        zhanli.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftBattle), unit.attackCount);
        level.text = "Lv." + unit.pbUnit.level;
        property.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftProprety);

        var image = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + unit.property) as Sprite;
        if (image != null)
        {
            proIcon.sprite = image;
        }
        
        if (unit.pbUnit.level >= GameConfig.MaxMonsterLevel)
        {
            expContent.text = "max";
            expProgress.value = 1.0f;
            addExpBtn.interactable = false;
        }
        else
        {
            int maxExp = StaticDataMgr.Instance.GetUnitBaseRowData(unit.pbUnit.level).experience;
            expContent.text = unit.pbUnit.curExp + "/" + maxExp;
            expProgress.value = unit.pbUnit.curExp * 1.0f / maxExp;
            addExpBtn.interactable = true;
        }

        mainPet.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftMainPet);
        changeCharacterBtn.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftChangeCharacter);
        detailAttrBtn.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftDetailAttr);
        skillpBtn.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftSkill);
        stageBtn.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftStage);
        advanceBtn.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAdvance);

        int healthValue, strengthValue, defenceValue, inteligenceValue, speedValue;
        UIUtil.GetAttrValue(unit, unit.pbUnit.stage, out healthValue, out strengthValue, out inteligenceValue, out defenceValue, out speedValue);

        health.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrHealth), healthValue);
        strength.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrDefence), strengthValue);
        defense.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrSpeed), defenceValue);
        intellgence.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrStrenth), inteligenceValue);
        speed.text = string.Format(StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAttrIntelligence), speedValue);

        if (UIUtil.CheckIsEnoughMaterial(unit) == true)
        {
            stageBadge.gameObject.SetActive(true);
        }
        else
        {
            stageBadge.gameObject.SetActive(false);
        }
        
    }
}	   