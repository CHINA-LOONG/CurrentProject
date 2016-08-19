
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
    public Transform rightViewPos;
    private PetDetailsEquipInfo petDetailsEquipInfo;

    //popup
    private UISelectEquipList uiSelectEquipList;

    public Transform coinButtonPos;
    public Transform goldButtonPos;
    private CoinButton coinBtn;
    private CoinButton goldBtn;


    private List<GameUnit> curUnitList;
    private int curUnitIndex;
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

        leftView.IPetDetailsLeftDelegate = this;
    }

    public void SetTypeList(GameUnit unit, List<GameUnit> unitList)
    {

        curUnitList = unitList;
        curUnitIndex = unitList.IndexOf(unit);
        Refresh();
    }


    void Refresh()
    {
        ReloadLeft();
    }
    void ReloadLeft()
    {
        leftView.ReloadData(CurData);

    }
    void ReloadRight(string rightView)
    {

    }
    
    #region On Click Button Down

    void OnClickDetailsBtn()
    {

    }
    void OnClickSkillBtn()
    {

    }
    void OnClickStageBtn()
    {

    }
    void OnClickRolesBtn()
    {

    }
    void OnClickAdvanceBtn()
    {

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
    
    public void OpenPetDetailsEquipInfo(EquipData data)
    {
        //打开装备详细资料界面
        if (petDetailsEquipInfo==null)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("PetDetailsEquipInfo");
            UIUtil.SetParentReset(go.transform, rightViewPos);
            petDetailsEquipInfo = go.GetComponent<PetDetailsEquipInfo>();
            petDetailsEquipInfo.IPetDetailsRight = this;
        }
        petDetailsEquipInfo.Refresh(data);
    }
    //打开选择装备列表弹窗
    public void OpenSelectEquipList(PartType part)
    {
        uiSelectEquipList = UIMgr.Instance.OpenUI_(UISelectEquipList.ViewName) as UISelectEquipList;
        uiSelectEquipList.ReloadData(CurData, part);
    }

    

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
    

    #endregion

}

