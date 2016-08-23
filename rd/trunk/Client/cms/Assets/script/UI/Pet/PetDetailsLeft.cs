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

    public bool IsLocked
    {
        get { return curData.pbUnit.locked; }
        set
        {
            curData.pbUnit.locked = value;
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

        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i] != null)
            {
                fields[i].iClickBack = this;
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
        IsLocked = curData.pbUnit.locked;
        RefreshLevelExp(curData.pbUnit.level, curData.pbUnit.curExp);
        
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
        param.locked = !curData.pbUnit.locked;
        GameApp.Instance.netManager.SendMessage(PB.code.MONSTER_LOCK_C.GetHashCode(), param);
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
        UINetRequest.Close();
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
    void ReloadPetEquipNotify(GameUnit gameUnit)
    {
        if (curData == gameUnit)
        {
            //ReloadData(curData, false);
            RefreshEquip(curData);
        }
    }
    void ReloadPetEquipNotify(EquipData equipData)
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
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_LOCK_C.GetHashCode().ToString(), OnPetLockReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_LOCK_S.GetHashCode().ToString(), OnPetLockReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadPetStageNotify);
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadPetEquipNotify);
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
