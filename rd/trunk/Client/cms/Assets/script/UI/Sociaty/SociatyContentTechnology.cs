using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SociatyTecEnum
{
    Sociaty_Tec_Lvl = 1,
    Sociaty_Tec_Member,
    Sociaty_Tec_Coin,
    Sociaty_Tec_Exp,

    Num_Sociaty_Tec
}
public class SociatyContentTechnology : SociatyContentBase
{
    public Text mContributeValue;
    public RectTransform mTechnologyList;

    private SociatyTechnologyItem mLvlItem;
    private SociatyTechnologyItem mMemberItem;
    private SociatyTechnologyItem mCoinItem;
    private SociatyTechnologyItem mExpItem;

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start ()
    {
        PB.AllianceInfo allianceData = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData;
        mLvlItem = AddItemInternal(SociatyTecEnum.Sociaty_Tec_Lvl, allianceData.level);
        mMemberItem = AddItemInternal(SociatyTecEnum.Sociaty_Tec_Member, allianceData.memLevel);
        mCoinItem = AddItemInternal(SociatyTecEnum.Sociaty_Tec_Coin, allianceData.coinLevel);
        mExpItem = AddItemInternal(SociatyTecEnum.Sociaty_Tec_Exp, allianceData.expLevel);
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {

    }
    //---------------------------------------------------------------------------------------------
    private SociatyTechnologyItem AddItemInternal(SociatyTecEnum type, int lvl)
    {
        SociatyTechnologyItem item = SociatyTechnologyItem.Create();
        item.transform.SetParent(mTechnologyList.transform, false);
        item.SetTechnologyData((int)type, lvl);

        return item;
    }
    //---------------------------------------------------------------------------------------------
    public override void RefreshUI()
    {
        mContributeValue.text = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData.contribution.ToString();
    }
    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LEVEL_UP_C.GetHashCode().ToString(), OnLevelUpResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LEVEL_UP_S.GetHashCode().ToString(), OnLevelUpResult);
    }
    //---------------------------------------------------------------------------------------------
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LEVEL_UP_C.GetHashCode().ToString(), OnLevelUpResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LEVEL_UP_S.GetHashCode().ToString(), OnLevelUpResult);
    }
    //---------------------------------------------------------------------------------------------
    void OnLevelUpResult(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(error.errCode);
        }
        else
        {
            UIIm.Instance.ShowSystemHints("level up success", (int)PB.ImType.PROMPT);
            mContributeValue.text = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData.contribution.ToString();
            //TODO: broadcast in sociaty channel
        }
    }
    //---------------------------------------------------------------------------------------------
    public void RefreshTechnologyInfo(int type)
    {
        PB.AllianceInfo allianceData = GameDataMgr.Instance.SociatyDataMgrAttr.allianceData;
        switch ((SociatyTecEnum)type)
        {
            case SociatyTecEnum.Sociaty_Tec_Lvl:
                mLvlItem.SetTechnologyData(type, allianceData.level);
                break;
            case SociatyTecEnum.Sociaty_Tec_Member:
                mMemberItem.SetTechnologyData(type, allianceData.memLevel);
                break;
            case SociatyTecEnum.Sociaty_Tec_Coin:
                mCoinItem.SetTechnologyData(type, allianceData.coinLevel);
                break;
            case SociatyTecEnum.Sociaty_Tec_Exp:
                mExpItem.SetTechnologyData(type, allianceData.expLevel);
                break;
        }
    }
    //---------------------------------------------------------------------------------------------
}
