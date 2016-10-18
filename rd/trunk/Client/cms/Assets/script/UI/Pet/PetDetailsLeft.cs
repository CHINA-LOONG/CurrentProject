using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public interface IPetDetailsLeft
{
    void OnClickEmptyField(PartType part);
    void OnClickEquipField(EquipData data);
    void OnClickUsedEXP();
}

public class PetDetailsLeft : MonoBehaviour, 
                              IEquipField,
                              IUIPetFeedList
{
    public Text textName;
    public Text textLevel;
    public Image imgProIcon;

    public Text textZhanli;
    public Button btnLock;
    public Button btnAddExp;
    public Text textExp;
    public Slider progressExp;

    public ImageView imageView;

    public EquipField[] fields;

    public IPetDetailsLeft IPetDetailsLeftDelegate;
    private GameUnit curData;
    private UnitData curUnitData;

    private UIPetFeedList uiPetFeedList;

    public Button btnDisposition;
    public Text textDisposition;
    private CharacterData dispositionData;
    public GameObject objDispositionTips;
    public Text textDisName;
    public Text textDisDesc;
    public GameObject objEquipTips;
    public Text textEquipTips;
    public bool IsLocked
    {
        get { return curData.pbUnit.IsLocked(); }
        set
        {
            curData.pbUnit.SetLocked(value);
            {
                string normal;
                string pressed;
                if (value)
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
        }
    }

    void Start()
    {
        btnLock.onClick.AddListener(OnClickLockBtn);
        btnAddExp.onClick.AddListener(OnClickAddExpBtn);
        ScrollViewEventListener.Get(btnDisposition.gameObject).onDown = OnDispositionDown;
        ScrollViewEventListener.Get(btnDisposition.gameObject).onUp = OnDispositionUp;

        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i] != null)
            {
                fields[i].iClickBack = this;
                ScrollViewEventListener.Get(fields[i].IconPos.gameObject).onDown = OnFieldPointerEnter;
                ScrollViewEventListener.Get(fields[i].IconPos.gameObject).onUp = OnFieldPointerExit;
            }
        }
    }

    public void ReloadData(GameUnit unit, bool reloadUnit = true)
    {
        curData = unit;
        curUnitData = StaticDataMgr.Instance.GetUnitRowData(curData.pbUnit.id);

        if (reloadUnit)
        {
            imageView.ReloadData(curData.pbUnit.id);
        }
        UIUtil.SetStageColor(textName, curData);

        imgProIcon.sprite= ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + unit.property) as Sprite;
        RefreshEquip(curData);

        textZhanli.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli") + curData.mBp;
        IsLocked = curData.pbUnit.IsLocked();
        RefreshLevelExp(curData.pbUnit.level, curData.pbUnit.curExp);

        dispositionData = StaticDataMgr.Instance.GetCharacterData(curData.pbUnit.character);
        SetDisposition(dispositionData.index);
        textDisposition.text = StaticDataMgr.Instance.GetTextByID(dispositionData.name);
        objDispositionTips.gameObject.SetActive(false);
    }

    void SetDisposition(int index)
    {
        Sprite sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(StaticDataMgr.Instance.GetCharacterData(index).picture);
        if (sprite != null)
        {
            btnDisposition.image.sprite = sprite;

        }
    }

    void OnDispositionDown(GameObject go)
    {
        objDispositionTips.SetActive(true);
        textDisName.text = StaticDataMgr.Instance.GetTextByID(dispositionData.name);
        textDisDesc.text = StaticDataMgr.Instance.GetTextByID(dispositionData.desc);
    }
    void OnDispositionUp(GameObject go)
    {
        objDispositionTips.SetActive(false);
    }

    public void OnFieldPointerEnter(GameObject go)
    {
        objEquipTips.SetActive(true);
        UIUtil.SetParentReset(objEquipTips.transform, go.transform);
        RectTransform rectTrans = objEquipTips.transform as RectTransform;
        rectTrans.anchoredPosition = Vector2.zero;
        EquipField field = go.GetComponentInParent<EquipField>();
        textEquipTips.text = curUnitData.EquipAttr + ItemStaticData.GetEquipPart((int)field.Part);
    }

    public void OnFieldPointerExit(GameObject go)
    {
        objEquipTips.SetActive(false);
    }

    public void RefreshLevelExp(int level, int exp)
    {
        textLevel.text = "LVL:" + level;
        int maxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(level).experience * curUnitData.levelUpExpRate);
        switch (UIUtil.CheckPetIsMaxLevel(level))
        {
            case 0:
                textExp.text = exp + "/" + maxExp;
                progressExp.value = (float)exp / (float)maxExp;
                break;
            case 1:
                textExp.text = "MAX LVL";
                progressExp.value = 0.0f;
                break;
            case 2:
                textExp.text = 0 + "/" + maxExp;
                progressExp.value = 0.0f;
                break;
        }
    }

    void RefreshEquip(GameUnit unit)
    {
        EquipData[] equips = unit.equipList;
        if (fields.Length != equips.Length)
        {
            Logger.LogError("error: field count != equip count");
            return;
        }
        for (int i = 0; i < equips.Length; i++)
        {
            fields[i].SetField(unit, (PartType)i + 1);
        }
    }
    
    void OnClickLockBtn()
    {
        PB.HSMonsterLock param = new PB.HSMonsterLock();
        param.monsterId = curData.pbUnit.guid;
        param.locked = !curData.pbUnit.IsLocked();
        GameApp.Instance.netManager.SendMessage(PB.code.MONSTER_LOCK_C.GetHashCode(), param,false);
    }
    void OnClickAddExpBtn()
    {
        if (UIUtil.CheckPetIsMaxLevel(curData.pbUnit.level) == 0)
        {
            if (uiPetFeedList == null)
            {
                uiPetFeedList = UIMgr.Instance.OpenUI_(UIPetFeedList.ViewName, false) as UIPetFeedList;
                uiPetFeedList.IUIPetFeedListDelegate = this;
            }
            uiPetFeedList.ReloadData(curData);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("monster_record_002"), (int)PB.ImType.PROMPT);
        }
    }
    
    void OnPetLockReturn(ProtocolMessage msg)
    {
        //UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("设置失败");
            //TODO： 处理设置加解锁失败的情况
            return;
        }
        PB.HSMonsterLockRet result = msg.GetProtocolBody<PB.HSMonsterLockRet>();
        if (result.monsterId == curData.pbUnit.guid)
        {
            IsLocked = result.locked;
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(result.locked ? "monster_record_006" : "monster_record_009"), (int)PB.ImType.PROMPT);
        }
        else
        {
            //设置的宠物不正确
            return;
        }

    }
    
    void ReloadPetStageNotify(GameUnit gameUnit)
    {
        if (curData == gameUnit)
        {
            ReloadData(curData, false);
        }
    }
    void ReloadPetEquipNotify(EquipData equipData)
    {
        RefreshEquip(curData);
    }
    void ReloadEquipForgeNotify(EquipData equipData)
    {
        if (equipData.monsterId==curData.pbUnit.guid)
        {
            RefreshEquip(curData);
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
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadPetStageNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipForgeNotify);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_LOCK_C.GetHashCode().ToString(), OnPetLockReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_LOCK_S.GetHashCode().ToString(), OnPetLockReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadPetStageNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipForgeNotify);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_LOCK_C.GetHashCode().ToString(), OnPetLockReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_LOCK_S.GetHashCode().ToString(), OnPetLockReturn);
    }

    public void OnSelectEquipField(PartType part, EquipData data)
    {
        if (IPetDetailsLeftDelegate != null)
        {
            if (data == null)
            {
                //TODO:
                IPetDetailsLeftDelegate.OnClickEmptyField(part);
            }
            else
            {
                //TODO:
                IPetDetailsLeftDelegate.OnClickEquipField(data);
            }
        }
    }

    #region IUIPetFeedList
    public void SetUnitLevelExp(GameUnit unit, int level, int exp)
    {
        if (unit==curData)
        {
            RefreshLevelExp(level, exp);
        }
    }

    public void UsedExpFinish(GameUnit unit)
    {
        if (unit==curData)
        {
            ReloadData(curData, false);
        }
    }
    #endregion
}
