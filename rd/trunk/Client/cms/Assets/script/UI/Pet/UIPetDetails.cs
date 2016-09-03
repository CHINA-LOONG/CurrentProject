
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class UIPetDetails : UIBase,
                            IPetDetailsLeft,
                            IPetDetailsRight
{
    public static string ViewName = "UIPetDetails";

    public override void Init()
    {
        if (coinBtn == null)
        {
            coinBtn = CoinButton.CreateWithType(CoinButton.CoinType.Jinbi);
            UIUtil.SetParentReset(coinBtn.transform, coinButtonPos);
        }
        if (goldBtn == null)
        {
            goldBtn = CoinButton.CreateWithType(CoinButton.CoinType.Zuanshi);
            UIUtil.SetParentReset(goldBtn.transform, goldButtonPos);
        }
    }
    public override void Clean()
    {
        leftView.imageView.CleanImageView();
        UIMgr.Instance.DestroyUI(uiSelectEquipList);
    }


    public Button btnClose;
    public Button btnNext;
    public Button btnPrevious;

    public Button btnDetails;
    public Button btnSkill;
    public Button btnStage;
    public Button btnRoles;
    public Button btnAdvance;

    public PetDetailsLeft leftView;
    private PetDetailsRight rightView;

    public Transform rightViewPos;
    private PetDetailsEquipInfo petDetailsEquipInfo;
    private PetDetailsAbilities petDetailsAbilities;
    private PetDetailsAdvance petDetailsAdvance;

    //popup
    private UISelectEquipList uiSelectEquipList;

    public Transform coinButtonPos;
    public Transform goldButtonPos;
    private CoinButton coinBtn;
    private CoinButton goldBtn;


    private List<GameUnit> curUnitList;
    private int curUnitIndex;
    private GameUnit curData;
    public GameUnit CurData
    {
        get { return curUnitList[curUnitIndex]; }
    }


    void Start()
    {
        btnClose.onClick.AddListener(OnClickCloseBtn);
        btnNext.onClick.AddListener(OnClickNextBtn);
        btnPrevious.onClick.AddListener(OnClickPreviousBtn);

        btnDetails.onClick.AddListener(OnClickDetailsBtn);
        btnSkill.onClick.AddListener(OnClickSkillBtn);
        btnStage.onClick.AddListener(OnClickStageBtn);
        btnRoles.onClick.AddListener(OnClickRolesBtn);
        btnAdvance.onClick.AddListener(OnClickAdvanceBtn);

        btnDetails.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_detail_attr");
        btnSkill.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_skill");
        btnStage.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage");
        btnRoles.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_roles");
        btnAdvance.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_advance");
        btnDetails.gameObject.SetActive(false);
        btnRoles.gameObject.SetActive(false);

        leftView.IPetDetailsLeftDelegate = this;
    }

    public void SetTypeList(GameUnit unit, List<GameUnit> unitList)
    {
        curUnitList = unitList;
        curUnitIndex = curUnitList.IndexOf(unit);
        Refresh();
    }
    
    void Refresh()
    {
        curData = CurData;
        leftView.ReloadData(CurData);
        OpenPetDetailsAbilities(CurData);
    }
    
    #region 界面按钮事件
    
    void OnClickDetailsBtn()
    {

    }
    void OnClickSkillBtn()
    {
        OpenPetDetailsAbilities(CurData);
    }
    void OnClickStageBtn()
    {
        OpenPetDetailsAdvance(CurData);
    }
    void OnClickRolesBtn()
    {

    }
    void OnClickAdvanceBtn()
    {
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("monster_record_012"), (int)PB.ImType.PROMPT);
    }

    void OnClickPreviousBtn()
    {
        if (curUnitList.Count == 1)
        {
            return;
        }
        curUnitIndex = (curUnitIndex - 1 + curUnitList.Count) % curUnitList.Count;
        Refresh();
    }
    void OnClickNextBtn()
    {
        if (curUnitList.Count == 1)
        {
            return;
        }
        curUnitIndex = (curUnitIndex + 1) % curUnitList.Count;
        Refresh();
    }

    void OnClickCloseBtn()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    #endregion

    #region 开启子界面接口
    void OpenRightView<T>(string ViewName,ref T openView)where T: PetDetailsRight
    {
        if (rightView == null || rightView != openView)
        {
            if (openView == null)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset(ViewName);
                UIUtil.SetParentReset(go.transform, rightViewPos);
                openView = go.GetComponent<T>() as T;
                openView.IPetDetailsRight = this;
            }
            else
            {
                openView.gameObject.SetActive(true);
            }
            if (rightView != null)
            {
                rightView.gameObject.SetActive(false);
            }
            rightView = openView;
        }
    }
    public void OpenPetDetailsAbilities(GameUnit unit)
    {
        OpenRightView(PetDetailsAbilities.ViewName, ref petDetailsAbilities);
        petDetailsAbilities.ReloadData(unit, 0);
    }
    public void OpenPetDetailsEquipInfo(EquipData data)
    {
        OpenRightView(PetDetailsEquipInfo.ViewName, ref petDetailsEquipInfo);
        petDetailsEquipInfo.Refresh(data);
    }
    public void OpenPetDetailsAdvance(GameUnit unit)
    {
        OpenRightView(PetDetailsAdvance.ViewName, ref petDetailsAdvance);
        petDetailsAdvance.ReloadData(unit);
    }

    //打开选择装备列表弹窗
    public void OpenSelectEquipList(PartType part)
    {
        uiSelectEquipList = UIMgr.Instance.OpenUI_(UISelectEquipList.ViewName) as UISelectEquipList;
        uiSelectEquipList.ReloadData(CurData, part);
    }
    #endregion


    
    #region PetDetailLeft Interface
    public void OnClickEmptyField(PartType part)
    {
        OpenSelectEquipList(part);
    }
    public void OnClickEquipField(EquipData data)
    {
        OpenPetDetailsEquipInfo(data);
    }
    public void OnClickUsedEXP()
    {

    }
    #endregion

    #region PetDetailRight Interface
    public void OnClickChangeEquip(PartType part)
    {
        OpenSelectEquipList(part);
    }

    //public void OnRemoveEquip()
    //{
    //    OpenPetDetailsAbilities(CurData);
    //}


    #endregion


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
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetLevelNotify, ReloadPetLevelNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.AddListener<int,int>(GameEventList.ReloadPetBPNotify, ReloadPetBPNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipForgeNotify);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetLevelNotify, ReloadPetLevelNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.RemoveListener<int,int>(GameEventList.ReloadPetBPNotify, ReloadPetBPNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipForgeNotify);
    }
    void ReloadPetLevelNotify(GameUnit gameUnit)
    {
        if (curData==gameUnit)
        {
            curUnitIndex = curUnitList.IndexOf(gameUnit);
        }
        if (rightView==petDetailsAdvance)
        {
            OpenPetDetailsAdvance(CurData);
        }
    }
    void ReloadPetEquipNotify(EquipData equipData)
    {
        if (equipData!=null&&equipData.monsterId==CurData.pbUnit.guid)
        {
            if (rightView==petDetailsEquipInfo)
            {
                OpenPetDetailsEquipInfo(equipData);
            }
        }
        else
        {
            OpenPetDetailsAbilities(CurData);
        }
    }
    void ReloadEquipForgeNotify(EquipData equipData)
    {
        if (equipData.monsterId == CurData.pbUnit.guid && rightView == petDetailsEquipInfo)
        {
            ItemStaticData curItem = StaticDataMgr.Instance.GetItemData(petDetailsEquipInfo.CurData.equipId);
            ItemStaticData tarItem = StaticDataMgr.Instance.GetItemData(equipData.equipId);
            if (curItem.part == tarItem.part)
            {
                OpenPetDetailsEquipInfo(equipData);
            }
        }
    }
    void ReloadPetBPNotify(int guid,int value)
    {
        if (CurData.pbUnit.guid==guid)
        {
            if (value > 0)
            {
                UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("monster_record_005"), value), (int)PB.ImType.PROMPT);
            }
            else if (value < 0)
            {
                UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("monster_record_015"), Mathf.Abs(value)), (int)PB.ImType.PROMPT);
            }
            leftView.ReloadData(CurData, false);
        }
    }

}

