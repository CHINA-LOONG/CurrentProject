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

    public Button btnLock;
    public Button btnChange;

    public Button btnSkill;
    public Button btnStage;
    public Image stageBadge;
    public Button btnAdvance;

    public ImageView imageView;
    //public RawImage modelImage;

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

    public bool IsLocked
    {
        get { return m_unit.pbUnit.locked; }
        set 
        { 
            m_unit.pbUnit.locked = value;
            SetLockButton(value);
        }
    }
    void SetLockButton(bool isLocked)
    {
        string normal;
        string pressed;
        if (isLocked)
        {
            normal = "anniu_suoding";
            pressed = "anniu_suoding_anxia";
        }
        else
        {
            normal = "anniu_jiesuo";
            pressed = "anniu_jiesuo_anxia";
        }
        btnLock.image.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(normal);
        SpriteState state = btnLock.spriteState;
        state.pressedSprite = ResourceMgr.Instance.LoadAssetType<Sprite>(pressed);
        btnLock.spriteState = state;
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

        btnLock.onClick.AddListener(LockPetButtonDown);

        btnAdvance.interactable = false;


        btnDetailAttr.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_detail_attr");
        btnSkill.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_skill");
        btnStage.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage");
        btnAdvance.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_advance");

        foreach (EquipField item in fields)
        {
            item.iClickBack = this;
        }
    }
    void OnEnable()
    {
        BindListener();
        
    }
    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadPetStage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_LOCK_C.GetHashCode().ToString(), OnPetLockReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_LOCK_S.GetHashCode().ToString(), OnPetLockReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadPetStage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_LOCK_C.GetHashCode().ToString(), OnPetLockReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_LOCK_S.GetHashCode().ToString(), OnPetLockReturn);
    }


    void ReloadPetStage(GameUnit gameUnit)
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
        IsLocked = m_unit.pbUnit.locked;
        #region set monster role

        if (reloadUnit)
        {
            imageView.ReloadData(unit.pbUnit.id);
        }

        //if (reloadUnit == true)
        //{
        //    GameObject petCamera = GameObject.Find(PetViewConst.UIPetModelCameraAssetName);
        //    GameObject monster = Util.FindChildByName(petCamera, "monsterModel");
        //    if (monster != null)
        //    {
        //        ResourceMgr.Instance.DestroyAsset(monster);
        //        monster = null;
        //    }

        //    monster = ResourceMgr.Instance.LoadAsset(unit.assetID);
        //    monster.transform.SetParent(petCamera.transform);
        //    monster.name = "monsterModel";
        //    monster.transform.localPosition = new Vector3(0, -0.65f, 2.2f);
        //    monster.transform.localEulerAngles = new Vector3(5.0f, 180, 0);
        //    monster.transform.localScale = Vector3.one;
        //    //monster.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -180, transform.localEulerAngles.z);

        //    modelImage.texture = petCamera.GetComponent<Camera>().targetTexture;
        //}

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

        if (UIUtil.CheckPetIsMaxLevel(level) > 0)
        {
            textExp.text = "Max LVL";
            progressExp.value = 0.0f;
            btnAddExp.interactable = false;
        }
        else
        {
            int maxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(level).experience * s_unit.levelUpExpRate);
            textExp.text = exp + "/" + maxExp;
            progressExp.value = (float)exp / (float)maxExp;
            //Debug.Log("exe:" + exp + "\n" + "max:" + maxExp + "value:" + ((float)exp / (float)maxExp));
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
            fields[i].SetField(m_unit, (PartType)i+1);
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

    void LockPetButtonDown()
    {
        PB.HSMonsterLock param = new PB.HSMonsterLock();
        param.monsterId = m_unit.pbUnit.guid;
        param.locked = !m_unit.pbUnit.locked;
        GameApp.Instance.netManager.SendMessage(PB.code.MONSTER_LOCK_C.GetHashCode(), param);
    }
    void OnPetLockReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("设置失败");
            //TODO： 处理设置加解锁失败的情况
            return;
        }
        PB.HSMonsterLockRet result = msg.GetProtocolBody<PB.HSMonsterLockRet>();
        if (result.monsterId == m_unit.pbUnit.guid)
        {
            IsLocked = result.locked;
        }
        else
        {
            //设置的宠物不正确
            return;
        }
        
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