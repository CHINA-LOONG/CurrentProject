using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PetDetailLeft : MonoBehaviour,IEquipField
{
    public Text textName;
    public Text textZhanli;
    public Text textLevel;
    public Image imgProIcon;

    public Text text_Exp;
    public Text textExp;
    public Slider progressExp;

    public Button btnAddExp;

    public Button btnDetailAttr;

    public Button btnSkill;
    public Button btnStage;
    public Image stageBadge;
    public Button btnAdvance;

    public RawImage modelImage;

    public EquipField[] fields;

    private UIPetDetail parentNode;
    public UIPetDetail ParentNode
    {
        get 
        {
            if (parentNode==null)
            {
                parentNode = transform.GetComponentInParent<UIPetDetail>();
            }
            return parentNode; 
        }
    }

    GameUnit m_unit;

    // Use this for initialization
    void Start()
    {
        GameEventMgr.Instance.AddListener(PetViewConst.ReloadPetStageNotify, ReloadPetStage);

        text_Exp.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftexperience);

        EventTriggerListener.Get(btnDetailAttr.gameObject).onClick = DetailAttrButtonDown;
        EventTriggerListener.Get(btnSkill.gameObject).onClick = SkillButtonDown;
        EventTriggerListener.Get(btnStage.gameObject).onClick = StageButtonDown;
        EventTriggerListener.Get(btnAdvance.gameObject).onClick = AdvanceButtonDown;

        btnDetailAttr.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftDetailAttr);
        btnSkill.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftSkill);
        btnStage.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftStage);
        btnAdvance.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAdvance);

        foreach (EquipField item in fields)
        {
            item.iClickBack = this;
        }
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
        #region set monster role
        
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

        #endregion

        UIUtil.SetStageColor(textName, unit);
        //刷新装备
        RefreshEquip(unit.equipList);

        textZhanli.text = unit.attackCount.ToString();
        textLevel.text = unit.pbUnit.level.ToString();

        var image = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + unit.property) as Sprite;
        if (image != null)
        {
            imgProIcon.sprite = image;
        }

        if (unit.pbUnit.level >= GameConfig.MaxMonsterLevel)
        {
            textExp.text = "max";
            progressExp.value = 1.0f;
            btnAddExp.interactable = false;
        }
        else
        {
            int maxExp = StaticDataMgr.Instance.GetUnitBaseRowData(unit.pbUnit.level).experience;
            textExp.text = unit.pbUnit.curExp + "/" + maxExp;
            progressExp.value = unit.pbUnit.curExp * 1.0f / maxExp;
            btnAddExp.interactable = true;
        }


        int healthValue, strengthValue, defenceValue, inteligenceValue, speedValue;
        UIUtil.GetAttrValue(unit, unit.pbUnit.stage, out healthValue, out strengthValue, out inteligenceValue, out defenceValue, out speedValue);


        if (UIUtil.CheckIsEnoughMaterial(unit) == true)
        {
            stageBadge.gameObject.SetActive(true);
        }
        else
        {
            stageBadge.gameObject.SetActive(false);
        }
    }

    void RefreshEquip(EquipData[] equips)
    {
        if (fields.Length!=equips.Length)
        {
            Logger.LogError("error: field count != equip count");
            return;
        }
        for (int i = 0; i < equips.Length; i++)
        {
            fields[i].Part = (PartType)i;
            fields[i].Data=equips[i];
        }
    }

    void SkillButtonDown(GameObject go)
    {
        ParentNode.SkillButtonDown();
    }

    void DetailAttrButtonDown(GameObject go)
    {
        ParentNode.DetailAttrButtonDown();
    }

    void StageButtonDown(GameObject go)
    {
        ParentNode.StageButtonDown();
    }

    void AdvanceButtonDown(GameObject go)
    {
        ParentNode.AdvanceButtonDown();
    }

    //接口函数
    public void OnSelectEquipField(PartType part, EquipData data)
    {
        if (data==null)
        {
            ParentNode.OpenEquipList(part);
        }
        else
        {
            ParentNode.OpenEquipInfo(part, data);
        }
    }
}