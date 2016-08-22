using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AllianceInfoItem : MonoBehaviour
{
    public Image selectEffect;
    public Button operationButton;
    public Text[] textArray;
    public Button itemButton;

    public PB.AllianceSimpleInfo itemInfoData;
    private bool hasJoinSociaty = false;

    public  static  AllianceInfoItem CreateWith(PB.AllianceSimpleInfo itemInfo,bool hasJoinSociaty = false)
    {
        GameObject go = null;
        if(hasJoinSociaty)
        {
            go = ResourceMgr.Instance.LoadAsset("OtherSociatyItem");
        }
        else
        {
            go = ResourceMgr.Instance.LoadAsset("AllianceInfoItem");
        }
        
        AllianceInfoItem allianceItem = go.GetComponent<AllianceInfoItem>();
        allianceItem.InitWith(itemInfo,hasJoinSociaty);
        allianceItem.SetSelect(false);
        return allianceItem;
    }

    // Use this for initialization
    void Start ()
    {
        if(operationButton != null)
            operationButton.onClick.AddListener(OnOperationButtonClick);

        if(itemButton != null)
        {
            itemButton.onClick.AddListener(OnItemButtonClick);
        }
	}

    public  void    InitWith(PB.AllianceSimpleInfo itemInfo, bool hasJoinSociaty)
    {
        this.hasJoinSociaty = hasJoinSociaty;
        itemInfoData = itemInfo;
        textArray[0].text = itemInfo.id.ToString();
        textArray[1].text = itemInfo.name;
        textArray[2].text = itemInfo.level.ToString();
        textArray[3].text = itemInfo.captaionName;
        textArray[4].text = itemInfo.currentPop.ToString();
        textArray[5].text = itemInfo.contribution3day.ToString();
        textArray[6].text = itemInfo.minLevel.ToString();

        RefreshButtonTitle();
    }

    void RefreshButtonTitle()
    {
        if (null != operationButton)
        {
            if (itemInfoData.apply)
            {
                UIUtil.SetButtonTitle(operationButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_cancelshenqing"));
            }
            else
            {
                UIUtil.SetButtonTitle(operationButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_shenqing"));
            }
        }
    }

    public  void SetSelect(bool isSel)
    {
        selectEffect.gameObject.SetActive(isSel);
    }

    void OnItemButtonClick()
    {
        if(hasJoinSociaty)
        {
            SociatyContentOther.Instance.OnAllianceInfoItemClick(this);
        }
        else
        {
            SociatyList.Instance.OnAllianceInfoItemClick(this);
        }  
    }

    void OnOperationButtonClick()
    {
        if(itemInfoData.apply)
        {
            RequestCancelApply();
        }
        else
        {
            if (itemInfoData.currentPop == itemInfoData.maxPop)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_002"), (int)PB.ImType.PROMPT);
                return;
            }
            if (itemInfoData.minLevel > GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_038"), (int)PB.ImType.PROMPT);
                return;
            }

            if(itemInfoData.autoAccept)
            {
                MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("sociaty_tips1"), AppplyOption);
            }
            else
            {
                RequestApply();
            }
        }
    }

    void AppplyOption(MsgBox.PrompButtonClick buttonClick)
    {
        if(buttonClick == MsgBox.PrompButtonClick.OK)
        {
            RequestApply();
        }
    }

    void RequestCancelApply()
    {
        GameDataMgr.Instance.SociatyDataMgrAttr.RequestCancelApply(itemInfoData.id, OnCancelApplyFinish);
    }

    void OnCancelApplyFinish(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if(msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = msg.GetProtocolBody<PB.HSErrorCode>();
            // switch(errorCode)
            // {
            // case PB.allianceError.ALLIANCE_ALREADY_APPLY
            //}
            return;
        }

        itemInfoData.apply = false;

        RefreshButtonTitle();

    }

    void RequestApply()
    {
        GameDataMgr.Instance.SociatyDataMgrAttr.RequestApply(itemInfoData.id, OnApplyFinish);
    }

    void OnApplyFinish(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = msg.GetProtocolBody<PB.HSErrorCode>();
            switch (errorCode.errCode)
            {
                case (int)PB.allianceError.ALLIANCE_ALREADY_FULL:
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_002"), (int)PB.ImType.PROMPT);
                    break;
                case (int)PB.allianceError.ALLIANCE_LEVEL_NOT_ENOUGH:
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_010"), (int)PB.ImType.PROMPT);
                    break;
                case (int)PB.allianceError.ALLIANCE_FRIZEN_TIME:
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_037"), (int)PB.ImType.PROMPT);
                    break;
                case (int)PB.allianceError.ALLIANCE_MAX_APPLY:
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_055"), (int)PB.ImType.PROMPT);
                    break;
            }
            return;
        }

        itemInfoData.apply = true;

        PB.HSAllianceApplyRet msgRet = msg.GetProtocolBody<PB.HSAllianceApplyRet>();

        if (msgRet.allianceId > 0)
        {
            GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = itemInfoData.id;
            UIMgr.Instance.CloseUI_(SociatyList.ViewName);
            GameDataMgr.Instance.SociatyDataMgrAttr.OpenSociaty();
        }
        else
        {
            RefreshButtonTitle();
        }
    }
    
}
