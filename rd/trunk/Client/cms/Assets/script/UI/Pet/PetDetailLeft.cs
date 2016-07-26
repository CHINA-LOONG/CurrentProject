using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PetDetailLeft : MonoBehaviour,IEquipField
{
    public Text textName;
    public Text textType;
    public Text textZhanli;
    public Text textLevel;
    public Image imgProIcon;

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
    UnitData s_unit;

    public enum BtnState
    {
        None,
        Skill,
        Stage,
        Advance
    }
    private BtnState state;

    public BtnState State
    {
        get { return state; }
        set
        {
            if (state != value)
            {
                state = value;
                //TODO: 设置按钮的正常状态：
                btnSkill.GetComponent<Animator>().SetBool("Selected", false);
                btnStage.GetComponent<Animator>().SetBool("Selected", false);
                btnAdvance.GetComponent<Animator>().SetBool("Selected", false);
                switch (state)
                {
                    case BtnState.None:
                        break;
                    case BtnState.Skill:
                        btnSkill.GetComponent<Animator>().SetTrigger("Normal");
                        btnSkill.GetComponent<Animator>().SetBool("Selected", true);
                        break;
                    case BtnState.Stage:
                        btnStage.GetComponent<Animator>().SetTrigger("Normal");
                        btnStage.GetComponent<Animator>().SetBool("Selected", true);
                        break;
                    case BtnState.Advance:
                        btnAdvance.GetComponent<Animator>().SetTrigger("Normal");
                        btnAdvance.GetComponent<Animator>().SetBool("Selected", true);
                        break;
                }
            }
        }
    }


    // Use this for initialization
    void Start()
    {
        //EventTriggerListener.Get(btnDetailAttr.gameObject).onClick = DetailAttrButtonDown;
        btnDetailAttr.onClick.AddListener(DetailAttrButtonDown);
        btnAddExp.onClick.AddListener(AddEXPButtonDown);
        btnSkill.onClick.AddListener(SkillButtonDown);
        btnStage.onClick.AddListener(StageButtonDown);
        btnAdvance.onClick.AddListener(AdvanceButtonDown);

        btnAdvance.interactable = false;


        btnDetailAttr.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftDetailAttr);
        btnSkill.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftSkill);
        btnStage.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftStage);
        btnAdvance.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailLeftAdvance);

        foreach (EquipField item in fields)
        {
            item.iClickBack = this;
        }
    }
    void OnEnable()
    {
        GameEventMgr.Instance.AddListener(GameEventList.ReloadPetStageNotify, ReloadPetStage);
    }
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.ReloadPetStageNotify, ReloadPetStage);
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
        s_unit = StaticDataMgr.Instance.GetUnitRowData(m_unit.pbUnit.id);

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
            monster.transform.localPosition = new Vector3(0, -0.65f, 2.2f);
            monster.transform.localEulerAngles = new Vector3(5.0f, 180, 0);
            monster.transform.localScale = Vector3.one;
            //monster.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -180, transform.localEulerAngles.z);

            modelImage.texture = petCamera.GetComponent<Camera>().targetTexture;
        }

        #endregion

        UIUtil.SetStageColor(textName, m_unit);
        textType.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetListType);
        //刷新装备
        RefreshEquip(m_unit.equipList);


        Sprite image = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + unit.property) as Sprite;
        if (image != null)
        {
            imgProIcon.sprite = image;
        }

        textZhanli.text = m_unit.attackCount.ToString();

        RefreshTempUnit(m_unit.pbUnit.level,m_unit.pbUnit.curExp);
        //int healthValue, strengthValue, defenceValue, inteligenceValue, speedValue;
        //UIUtil.GetAttrValue(unit, unit.pbUnit.stage, out healthValue, out strengthValue, out inteligenceValue, out defenceValue, out speedValue);


        if (UIUtil.CheckIsEnoughMaterial(unit) == true)
        {
            stageBadge.gameObject.SetActive(true);
        }
        else
        {
            stageBadge.gameObject.SetActive(false);
        }
    }

    public void RefreshTempUnit(int level,int exp)
    {
        textLevel.text = level.ToString();

        if (level >= GameConfig.MaxMonsterLevel)
        {
            textExp.text = "max";
            progressExp.value = 1.0f;
            btnAddExp.interactable = false;
        }
        else
        {
            int maxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(level).experience * s_unit.levelUpExpRate);
            textExp.text = exp + "/" + maxExp;
            progressExp.value = (float)exp / (float)maxExp;
            Debug.Log("exe:" + exp + "\n" + "max:" + maxExp + "value:" + ((float)exp / (float)maxExp));
            btnAddExp.interactable = true;
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
            fields[i].SetField(m_unit, i);
        }
    }

    void DetailAttrButtonDown()
    {
        ParentNode.DetailAttrButtonDown();
    }

    void AddEXPButtonDown()
    {
        ParentNode.AddEXPButtonDown();
    }

    void SkillButtonDown()
    {
        ParentNode.SkillButtonDown();
    }

    void StageButtonDown()
    {
        ParentNode.StageButtonDown();
    }

    void AdvanceButtonDown()
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