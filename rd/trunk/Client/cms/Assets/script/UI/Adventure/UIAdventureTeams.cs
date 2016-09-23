using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIAdventureTeams : UIBase,
                                IScrollView,
                                IAdventureTeamItem
{
    public static string ViewName = "UIAdventureTeams";

    public Text textTitle;
    public Text textCount;

    public Transform itemParent;

    public Button btnAddTeam;
    public Text textAddTeam;

    public Button btnClose;

    public FixCountScrollView scrollView;
    private List<AdventureTeam> teams=new List<AdventureTeam>();
    private UIAdventureReward uiAdventureReward;
    void Start()
    {
        textTitle.text = StaticDataMgr.Instance.GetTextByID("adventure_title");
        textAddTeam.text = StaticDataMgr.Instance.GetTextByID("adventure_addteam");

        btnClose.onClick.AddListener(OnClickCloseBtn);
        btnAddTeam.onClick.AddListener(OnClickAddTeamBtn);
    }
    public override void Init()
    {
        RefreshData();
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(uiAdventureReward);
    }

    void RefreshData()
    {
        textCount.text = string.Format(StaticDataMgr.Instance.GetTextByID("adventure_teamnum"),AdventureDataMgr.Instance.teamCount);
        btnAddTeam.gameObject.SetActive(AdventureDataMgr.Instance.teamCount < BattleConst.maxAdventureTeam);

        teams = AdventureDataMgr.Instance.adventureTeams;
        teams.Sort();
        scrollView.InitContentSize(teams.Count, this);
    }

    void OnClickAddTeamBtn()
    {
        AdventureTeamPriceData teamPriceData = StaticDataMgr.Instance.GetAdventureTeamPriceData(AdventureDataMgr.Instance.teamCount + 1);
        MsgBox.PrompCostMsg.Open(teamPriceData.gold, StaticDataMgr.Instance.GetTextByID("adventure_addsure"), "", ConformOpenTask);
    }
    void ConformOpenTask(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;
        GameApp.Instance.netManager.SendMessage(PB.code.ADVENTURE_BUY_TEAM_C.GetHashCode(), new PB.HSAdventureBuyTeam());
    }

    void OnClickCloseBtn()
    {
        UIMgr.Instance.CloseUI_(this);
    }


    #region BindListener
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
        GameEventMgr.Instance.AddListener(GameEventList.AdventureAddTeam, RefreshData);
        GameEventMgr.Instance.AddListener(GameEventList.AdventureChange, RefreshData);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_TEAM_C.GetHashCode().ToString(), OnAdventureBuyTeamReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_TEAM_S.GetHashCode().ToString(), OnAdventureBuyTeamReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_SETTLE_C.GetHashCode().ToString(), OnAdventureSettleReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_SETTLE_S.GetHashCode().ToString(), OnAdventureSettleReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.AdventureAddTeam, RefreshData);
        GameEventMgr.Instance.RemoveListener(GameEventList.AdventureChange, RefreshData);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_TEAM_C.GetHashCode().ToString(), OnAdventureBuyTeamReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_TEAM_S.GetHashCode().ToString(), OnAdventureBuyTeamReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_SETTLE_C.GetHashCode().ToString(), OnAdventureSettleReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_SETTLE_S.GetHashCode().ToString(), OnAdventureSettleReturn);
    }

    void OnAdventureBuyTeamReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("添加队伍失败");
            return;
        }
        PB.HSAdventureBuyTeamRet result = msg.GetProtocolBody<PB.HSAdventureBuyTeamRet>();
        AdventureDataMgr.Instance.AdventureAddTeam(result.teamId);
        GameEventMgr.Instance.FireEvent(GameEventList.AdventureAddTeam);
    }
    void OnAdventureSettleReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("领取奖励失败");
            return;
        }
        PB.HSAdventureSettleRet result = msg.GetProtocolBody<PB.HSAdventureSettleRet>();
        
        uiAdventureReward = UIMgr.Instance.OpenUI_(UIAdventureReward.ViewName) as UIAdventureReward;
        uiAdventureReward.ReloadData(result.basicReward, result.extraReward);

        AdventureDataMgr.Instance.AdventureInfoUpdate(result.adventure);
        AdventureDataMgr.Instance.AdventureTeamUpdate(result.teamId);
        GameEventMgr.Instance.FireEvent(GameEventList.AdventureChange);
        
    }
    #endregion

    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
        AdventureTeamItem team = item.GetComponent<AdventureTeamItem>();
        team.RefreshData(teams[index]);
    }

    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("AdventureTeamItem");
        if (go!=null)
        {
            AdventureTeamItem team = go.GetComponent<AdventureTeamItem>();
            UIUtil.SetParentReset(go.transform, parent);
            team.IAdventureTeamItemDelegate = this;
            return go.transform;
        }
        return null;
    }

    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
        itemList.Clear();
    }
    #endregion

    #region IAdventureTeamItem
    public void OnClickToAdventure()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    public void OnClickToPaySubmit(AdventureTeam team)
    {
        PB.HSAdventureSettle param = new PB.HSAdventureSettle();
        param.teamId = team.teamId;
        param.pay = true;
        GameApp.Instance.netManager.SendMessage(PB.code.ADVENTURE_SETTLE_C.GetHashCode(), param);
    }

    public void OnClickToSubmit(AdventureTeam team)
    {
        PB.HSAdventureSettle param = new PB.HSAdventureSettle();
        param.teamId = team.teamId;
        param.pay = false;
        GameApp.Instance.netManager.SendMessage(PB.code.ADVENTURE_SETTLE_C.GetHashCode(), param);
    }
    #endregion
}
