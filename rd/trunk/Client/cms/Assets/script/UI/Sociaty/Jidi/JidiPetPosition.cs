using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JidiPetPosition : MonoBehaviour
{
    public enum PositionState:int   
    {
        Open = 0,
        Locked,
        HasPet
    }
    public ImageView petImageView;
    public Text lockTipsText;
    public Button itemButton;

    public GameObject hasPetPanel;
    public Text nameText;
    public Text lvText;
    public Text stayTimeText;
    public Text stayTimeLbelText;
    public Text shouruLabelText;
    public Text shouruValueText;
    public Button reCallButton;

    public  int positionIndex = 0;
    PositionState positionState;
    private int selectZhushouGuid = 0;

    PB.AllianceBaseMonster petData = null;
    int stayTimeSecond = 0;
    // Use this for initialization
    void Awake ()
    {
        itemButton.onClick.AddListener(OnItemButtonClick);
        reCallButton.onClick.AddListener(OnReCallButtonClick);
        petImageView.InitJidiPosoitonModel();
        SetPositionState(PositionState.Locked);
    }

    public  void  SetPetData(PB.AllianceBaseMonster petdata)
    {
        petData = petdata;
        if(null != petData)
        {
            SetPositionState(PositionState.HasPet);
        }
        else
        {
            petImageView.model.DestroyBattleObject();
            if (GetOpenContributionWithIndex(positionIndex) > GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData.totalContribution)
            {
                SetPositionState(PositionState.Locked);
            }
            else
            {
                SetPositionState(PositionState.Open);
            }
        }
    }
    public void SetPositionState(PositionState state)
    {
        positionState = state;

        lockTipsText.gameObject.SetActive(state == PositionState.Locked);

        hasPetPanel.SetActive(state == PositionState.HasPet);

        itemButton.gameObject.SetActive(state != PositionState.HasPet);
        petImageView.jidiPositionViewModel.ShowOpenObject(state == PositionState.Open);
        petImageView.jidiPositionViewModel.ShowLockedObject(state == PositionState.Locked);       

        if (positionState == PositionState.Locked)
        {
            lockTipsText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_tipslock"),
                GetOpenContributionWithIndex(positionIndex));
        }
        else if (positionState == PositionState.HasPet)
        {
            RefreshPetInformation();
        }
    }
    void RefreshPetInformation()
    {
        if (null == petData)
            return;
        string monsterId = petData.cfgId;
        petImageView.model.ReloadData(monsterId);

        UnitData unitData = StaticDataMgr.Instance.GetUnitRowData(petData.cfgId);
        if(null == unitData)
        {
            Logger.LogError("JidePet Error for not find unit with Id = " + petData.cfgId);
            return;
        }

        string nameStr = null;
        int quallity = 0;
        int plusQuality = 0;
        UIUtil.CalculationQuality(petData.stage, out quallity, out plusQuality);
        if(plusQuality > 0)
        {
            nameStr = string.Format("{0}+{1}", unitData.NickNameAttr, plusQuality);
        }
        else
        {
            nameStr = unitData.NickNameAttr;
        }
        nameText.text = nameStr;

        lvText.text = string.Format("LVL{0}", petData.level);
        stayTimeSecond = GameTimeMgr.Instance.GetServerTimeStamp() - petData.sendTime;
        int hour = stayTimeSecond / 3600;
        int minute = (stayTimeSecond - hour * 3600) / 60;
        stayTimeText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_zhushoutime1"), hour, minute);
        stayTimeLbelText.text = StaticDataMgr.Instance.GetTextByID("sociaty_zhushoutime");
        shouruLabelText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_leijishouru"));
        shouruValueText.text = petData.reward.ToString();
    }

    void OnItemButtonClick()
    {
        if(positionState == PositionState.Open)
        {
            SelectPet.Open(OnSelectPet);
        }
        else if (positionState == PositionState.Locked)
        {

        }
    }
    void OnSelectPet(int petGuid)
    {
        selectZhushouGuid = petGuid;
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("sociaty_paichuqueren"), OnConformZhushou);
    }
    void OnConformZhushou(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            GameDataMgr.Instance.SociatyDataMgrAttr.RequestZhushou(selectZhushouGuid,positionIndex,OnRequestZhushouFinish);
        }
    }
    void OnRequestZhushouFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSAllianceBaseSendMonsterRet msgRet = message.GetProtocolBody<PB.HSAllianceBaseSendMonsterRet>();
        PB.AllianceBaseMonster zhushouMonster = new PB.AllianceBaseMonster();
        zhushouMonster.id = selectZhushouGuid;
        zhushouMonster.sendTime = msgRet.sendTime;

        GameUnit gameUnit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(zhushouMonster.id);
        zhushouMonster.bp = gameUnit.mBp;
        zhushouMonster.cfgId = gameUnit.pbUnit.id;
        zhushouMonster.stage = gameUnit.pbUnit.stage;
        zhushouMonster.level = gameUnit.pbUnit.level;
        zhushouMonster.position = positionIndex;

        SociatyMyJidi.Instance.UpdatePositionData(positionIndex, zhushouMonster);
        SetPetData(zhushouMonster);
        SelectPet.Close();
    }

    void OnReCallButtonClick()
    {
        if(stayTimeSecond < 3*60)//todo
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_067"), (int)PB.ImType.PROMPT);
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.RequestRecallZhushou(positionIndex, OnRequestRecallFinished);
    }
    void OnRequestRecallFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(error.errCode);
            return;
        }
        PB.HSAllianceBaseRecallMonsterRet msgRet = message.GetProtocolBody<PB.HSAllianceBaseRecallMonsterRet>();
        JidiRewardConform.OpenWith(msgRet.coinDefend, msgRet.coinHire);
        SetPetData(null);
        SociatyMyJidi.Instance.UpdatePositionData(positionIndex, null);
    }

    int GetOpenContributionWithIndex(int index)
    {
        if(0==index)
        {
            return 0;
        }
        else if ( 1== index)
        {
            return 1000;
        }
        else
        {
            return 2000;
        }
    }
}
