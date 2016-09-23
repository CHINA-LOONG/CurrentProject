using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIAdventureLayout : UIBase,
                                 IAdventureConditionItem,
                                 IScrollView,
                                 IAdventureSelfMonster,
                                 IAdventureGuildMonster,
                                 IAdventureTeamMonster

{
    public static string ViewName = "UIAdventureLayout";

    public Text text_Title;
    public Button btnClose;
    public Button btnCondition;
    public Button btnAdventure;

    public FixCountScrollView scrollSelf;
    private List<AdventureSelfMonsterInfo> allSelfs;    //通过检测是否为空来检测获取数据
    private List<AdventureSelfMonsterInfo> showSelfs = new List<AdventureSelfMonsterInfo>();
    public FixCountScrollView scrollGuild;
    private List<AdventureGuildMonsterInfo> allGuilds;  //通过检测是否为空来检测获取数据
    private List<AdventureGuildMonsterInfo> showGuilds = new List<AdventureGuildMonsterInfo>();
    public Text textGuildTips;

    public List<AdventureTeamMonster> teamList = new List<AdventureTeamMonster>();
    private List<int> selfMonsterId = new List<int>();
    private PB.AllianceBaseMonster hireMonster;

    public Transform baseParent;
    private List<GameObject> baseList = new List<GameObject>();
    public Transform extraParent;
    private List<GameObject> extraList = new List<GameObject>();

    public Transform conditionParent;
    private List<AdventureConditionItem> conditions = new List<AdventureConditionItem>();
    private List<AdventureConditionItem> itemPool = new List<AdventureConditionItem>();
    private int filterType = -1;
    private int filterProperty = -1;
    private int conditionStep = 0;
    public AdventureExtraConditon showConditonStep;

    private AdventureInfo curData;
    void Start()
    {
        btnClose.onClick.AddListener(OnClickCloseBtn);
        btnCondition.onClick.AddListener(OnClickRefeshConditionBtn);
        btnAdventure.onClick.AddListener(OnClickAdventureBtn);
        for (int i = 0; i < teamList.Count; i++)
        {
            teamList[i].IAdventureTeamMonsterDelegate = this;
        }
    }
    
    public void ReloadData(AdventureInfo info)
    {
        curData = info;
        #region 设置界面标题
        string title = "";
        switch (curData.adventureData.Type)
        {
            case AdventureType.QIANGHUA:
                title = StaticDataMgr.Instance.GetTextByID("adventure_qianghuashitime");
                break;
            case AdventureType.JINJIE:
                title = StaticDataMgr.Instance.GetTextByID("adventure_jinjieshitime");
                break;
            case AdventureType.BOSS:
                title = StaticDataMgr.Instance.GetTextByID("adventure_bosstime");
                break;
        }
        text_Title.text = string.Format(title, BattleConst.adventureTime[curData.adventureData.time - 1]);
        #endregion
        
        //显示基本奖励
        RefreshRewardItem(ref baseList, baseParent, curData.adventureData.basicReward);

        //显示额外奖励
        RefreshRewardItem(ref extraList, extraParent, curData.adventureData.extraReward);

        //设置筛选条件
        RefreshAdventureCondition(curData.conditions);

        //显示筛选宠物
        FilterMonster(filterType, filterProperty);

        //刷新队伍宠物
        UpdateAdventureTeamSelect();
    }
    #region 设置己方及工会宠物
    void RegisterSelfMonster(Action callBack)
    {
        allSelfs = new List<AdventureSelfMonsterInfo>();
        Dictionary<int, GameUnit> unitDict = GameDataMgr.Instance.PlayerDataAttr.allUnitDic;
        UnitData curUnitData;
        GameUnit curGameUnit;
        foreach (var item in unitDict)
        {
            curGameUnit = item.Value;
            curUnitData = StaticDataMgr.Instance.GetUnitRowData(curGameUnit.pbUnit.id);
            //if (curGameUnit.pbUnit.IsLocked())
            if (true)
            {
                allSelfs.Add(new AdventureSelfMonsterInfo() { unit = curGameUnit, unitData = curUnitData });
            }
        }
        if (callBack!=null)
        {
            callBack();
        }
    }
    void RegisterGuildMonster(Action callBack)
    {
        allGuilds = new List<AdventureGuildMonsterInfo>();
        if (GameDataMgr.Instance.SociatyDataMgrAttr.allianceID > 0)
        {
            GameDataMgr.Instance.SociatyDataMgrAttr.GetJidiBaseMonstersAsyn(delegate(List<PB.AllianceBaseMonster> guildList)
            {
                UnitData curUnitData;
                Sociatybase curSociatyData;
                PB.AllianceBaseMonster curMonster;
                for (int i = 0; i < guildList.Count; i++)
                {
                    curMonster = guildList[i];
                    if (AdventureDataMgr.Instance.CheckMonsterIsHired(curMonster))
                    {
                        continue;
                    }
                    curUnitData = StaticDataMgr.Instance.GetUnitRowData(curMonster.cfgId);
                    curSociatyData = StaticDataMgr.Instance.GetSociatybaseData(curMonster.bp);
                    allGuilds.Add(new AdventureGuildMonsterInfo() { unit = curMonster, unitData = curUnitData, sociatyData = curSociatyData });
                }
                if (callBack != null)
                {
                    callBack();
                }
            });
        }
    }
    void FilterMonster(int type,int property)
    {
        filterType = type;
        filterProperty = property;

        FilterSelfMonster();
        FilterGuildMonster();
    }
    void FilterSelfMonster()
    {
        if (allSelfs==null)
        {
            RegisterSelfMonster(FilterSelfMonster);
            return;
        }
        showSelfs.Clear();
        AdventureSelfMonsterInfo monster;
        for (int i = 0; i < allSelfs.Count; i++)
        {
            monster = allSelfs[i];
            if (CheckIsMeetCondition(monster,filterType,filterProperty))
            {
                showSelfs.Add(monster);
            }
        }
        scrollSelf.InitContentSize(showSelfs.Count, this);
    }
    void FilterGuildMonster()
    {
        if (GameDataMgr.Instance.SociatyDataMgrAttr.allianceID > 0)
        {
            if (allGuilds == null)
            {
                RegisterGuildMonster(FilterGuildMonster);
                return;
            }
            showGuilds.Clear();
            AdventureGuildMonsterInfo monster;
            for (int i = 0; i < allGuilds.Count; i++)
            {
                monster = allGuilds[i];
                if (CheckIsMeetCondition(monster,filterType,filterProperty))
                {
                    showGuilds.Add(monster);
                }
            }
            if (showGuilds.Count <= 0)
            {
                textGuildTips.gameObject.SetActive(true);
                textGuildTips.text = StaticDataMgr.Instance.GetTextByID("list_empty");
            }
            else
            {
                textGuildTips.gameObject.SetActive(false);
                scrollGuild.InitContentSize(showGuilds.Count, this);
            }
        }
        else
        {
            textGuildTips.gameObject.SetActive(true);
            textGuildTips.text = StaticDataMgr.Instance.GetTextByID("sociaty_meijiaguild");
        }
    }
    bool CheckIsSelected(int id)
    {
        if (selfMonsterId.Contains(id))
        {
            return true;
        }
        return false;
    }
    bool CheckIsMeetCondition<T>(T monster, int needType,int needProperty)
    {
        if (monster is AdventureSelfMonsterInfo)
        {
            AdventureSelfMonsterInfo thisMonster = monster as AdventureSelfMonsterInfo;
            if (((needType != -1 && thisMonster.unitData.type == needType) || (needType == -1)) && (needProperty != -1 && thisMonster.unitData.property == needProperty) || (needProperty == -1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (monster is AdventureGuildMonsterInfo)
        {
            AdventureGuildMonsterInfo thisMonster = monster as AdventureGuildMonsterInfo;
            if (((filterType != -1 && thisMonster.unitData.type == filterType) || (filterType == -1)) && (filterProperty != -1 && thisMonster.unitData.property == filterProperty) || (filterProperty == -1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region 设置奖励物品
    void RefreshRewardItem(ref List<GameObject> list, Transform parent, string rewardId)
    {
        for (int i = 0; i < list.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(list[i]);
        }
        list.Clear();
        RewardData reward = StaticDataMgr.Instance.GetRewardData(rewardId);
        for (int i = 0; i < reward.itemList.Count; i++)
        {
            GameObject go = RewardItemCreator.CreateRewardItem(reward.itemList[i].protocolData, parent, true, false);
            list.Add(go);
        }
    }
    #endregion

    #region 设置筛选条件
    void RefreshAdventureCondition(List<PB.HSAdventureCondition> conditionList)
    {
        RemoveAllConditionItem();
        for (int i = 0; i < conditionList.Count; i++)
        {
            AdventureConditionItem condition = GetConditionItem();
            condition.ReloadData(conditionList[i]);
            conditions.Add(condition);
        }
    }
    AdventureConditionItem GetConditionItem()
    {
        AdventureConditionItem item = null;
        if (itemPool.Count<=0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("AdventureConditionItem");
            UIUtil.SetParentReset(go.transform, conditionParent);
            item = go.GetComponent<AdventureConditionItem>();
            item.IAdventureConditionItemDelegate = this;
        }
        else
        {
            item = itemPool[itemPool.Count - 1];
            item.gameObject.SetActive(true);
            item.IsFilter = false;
            itemPool.Remove(item);
        }
        return item;
    }
    public void RemoveAllConditionItem()
    {
        conditions.ForEach(delegate(AdventureConditionItem item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(conditions);
        conditions.Clear();
    }
    #endregion

    #region 设置队伍宠物
    void AddAdventureTeamMonster<T>(T monster)
    {
        for (int i = 0; i < teamList.Count; i++)
        {
            if (teamList[i].type == AdventureTeamMonster.MonsterType.NULL)
            {
                if (monster is AdventureSelfMonsterInfo)
                {
                    AdventureSelfMonsterInfo thisMonster = monster as AdventureSelfMonsterInfo;
                    //Debug.Log("测试大冒险阵容调整：" + thisMonster.unitData.NickNameAttr);
                    selfMonsterId.Add(thisMonster.unit.pbUnit.guid);
                    thisMonster.IsSelect = true;
                }
                else if (monster is AdventureGuildMonsterInfo)
                {
                    AdventureGuildMonsterInfo thisMonster = monster as AdventureGuildMonsterInfo;
                    //Debug.Log("测试大冒险阵容调整：" + thisMonster.unitData.NickNameAttr);
                    hireMonster = thisMonster.unit;
                    thisMonster.IsSelect = true;
                }
                teamList[i].ReloadData(monster);
                break;
            }
        }
        UpdateAdventureTeamSelect();
    }
    void RemoveAdventureTeamMonster<T>(T monster)
    {
        if (monster is AdventureSelfMonsterInfo)
        {
            AdventureSelfMonsterInfo thisMonster = monster as AdventureSelfMonsterInfo;
            //Debug.Log("测试大冒险阵容调整：" + thisMonster.unitData.NickNameAttr);
            for (int i = 0; i < teamList.Count; i++)
            {
                if (teamList[i].type == AdventureTeamMonster.MonsterType.SELF && teamList[i].selfMonster == thisMonster)
                {
                    teamList[i].ReloadData<AdventureSelfMonsterInfo>(null);
                    selfMonsterId.Remove(thisMonster.unit.pbUnit.guid);
                    thisMonster.IsSelect = false;
                    break;
                }
            }
        }
        else if (monster is AdventureGuildMonsterInfo)
        {
            AdventureGuildMonsterInfo thisMonster = monster as AdventureGuildMonsterInfo;
            Debug.Log("测试大冒险阵容调整：" + thisMonster.unitData.NickNameAttr);
            for (int i = 0; i < teamList.Count; i++)
            {
                if (teamList[i].type == AdventureTeamMonster.MonsterType.GUILD && teamList[i].guildMonster == thisMonster)
                {
                    teamList[i].ReloadData<AdventureGuildMonsterInfo>(null);
                    hireMonster = null;
                    thisMonster.IsSelect = false;
                    break;
                }
            }
        }
        UpdateAdventureTeamSelect();
    }
    void UpdateAdventureTeamSelect()
    {
        AdventureTeamMonster item = null;
        for (int i = 0; i < teamList.Count; i++)
        {
            if (item == null && teamList[i].type == AdventureTeamMonster.MonsterType.NULL)
            {
                item = teamList[i];
                item.IsSelect = true;
            }
            else
            {
                teamList[i].IsSelect = false;
            }
        }

        int amount = conditions.Count;
        int count = 0;
        for (int i = 0; i < conditions.Count; i++)
        {
            if (CheckIsMeetCondition(conditions[i]))
            {
                count++;
                //conditions[i].setMeet();
            }
        }

        conditionStep = (int)(((float)count / amount) * 100);
        showConditonStep.RefreshStep(conditionStep);
        Logger.Log("当前满足额外奖励率：" + conditionStep + "%");
    }
    bool CheckIsMeetCondition(AdventureConditionItem condition)
    {
        int amount = condition.curData.monsterCount;    //需要怪物数量
        int count = 0;

        int type = condition.conditionData.monsterType, 
            property = condition.conditionData.monsterProperty;

        for (int i = 0; i < teamList.Count; i++)
        {
            switch (teamList[i].type)
            {
                case AdventureTeamMonster.MonsterType.SELF:
                    if (CheckIsMeetCondition(teamList[i].selfMonster,type,property))
                    {
                        count++;
                        if (count >= amount) return true;
                    }
                    break;
                case AdventureTeamMonster.MonsterType.GUILD:
                    if (CheckIsMeetCondition(teamList[i].guildMonster, type, property))
                    {
                        count++;
                        if (count >= amount) return true;
                    }
                    break;
            }
        }
        return false;
    }
    
    #endregion

    #region 界面按钮事件
    void OnClickCloseBtn()
    {
        UIMgr.Instance.CloseUI_(this);
    }
    void OnClickRefeshConditionBtn()
    {
        if (AdventureDataMgr.Instance.AdventureChange>=1)//拥有变更次数
        {
            PrompMsgRefreshCondition prompt = PrompMsgRefreshCondition.Open(StaticDataMgr.Instance.GetTextByID("adventure_tipschange"),
                                                                           0,
                                                                           PrompButtonRefreshCallBack);
        }
        else//没有变更次数
        {
            PrompMsgRefreshCondition prompt = PrompMsgRefreshCondition.Open(StaticDataMgr.Instance.GetTextByID("adventure_tipsbuy"),
                                                                           50,
                                                                           PrompButtonBuyCallBack);
        }
    }
    void PrompButtonRefreshCallBack(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;
        PB.HSAdventureNewCondition param = new PB.HSAdventureNewCondition();
        param.type = curData.adventureData.type;
        param.gear = curData.adventureData.time;
        GameApp.Instance.netManager.SendMessage(PB.code.ADVENTURE_NEW_CONDITION_C.GetHashCode(), param);
    }
    void PrompButtonBuyCallBack(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;
        if (GameDataMgr.Instance.PlayerDataAttr.gold < 50)//钻石不足
        {
            GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
        }
        else//钻石足够
        {
            GameApp.Instance.netManager.SendMessage(PB.code.ADVENTURE_BUY_CONDITION_C.GetHashCode(), new PB.HSAdventureBuyCondition());
        }
    }
    void OnClickAdventureBtn()
    {
        if (!(selfMonsterId.Count == 4 && hireMonster != null)&&!(selfMonsterId.Count==5))
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("adventure_record_007"), (int)PB.ImType.PROMPT);
        }
        else
        {
            Dictionary<int, GameUnit> allUnitDic = GameDataMgr.Instance.PlayerDataAttr.allUnitDic;
            int index = 0;//剩余怪物计数
            foreach (var item in allUnitDic)
            {
                if (!item.Value.pbUnit.IsInAdventure() && !CheckIsSelected(item.Value.pbUnit.guid))
                {
                    index++;
                    if (index >= 5)
                    {
                        break;
                    }
                }
            }
            if (index < 5)//剩余少于5只怪物
            {
                MsgBox.PromptMsg prompt = MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,
                                                              StaticDataMgr.Instance.GetTextByID("adventure_numbuzuti"),
                                                              SubmitAdventureInfo);
            }
            else
            {
                SubmitAdventureInfo(MsgBox.PrompButtonClick.OK);
            }
        }
    }
    void SubmitAdventureInfo(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;
        if (selfMonsterId.Count == 4 && hireMonster != null)
        {
            UnitData unitData = StaticDataMgr.Instance.GetUnitRowData(hireMonster.cfgId);
            Sociatybase sociatyBase = StaticDataMgr.Instance.GetSociatybaseData(hireMonster.bp);
            PrompMsgAdventureConfirm promp = PrompMsgAdventureConfirm.Open(StaticDataMgr.Instance.GetTextByID("adventure_tipsguyong"),
                                                                         StaticDataMgr.Instance.GetTextByID("adventure_tipsweight"), 
                                                                         conditionStep,
                                                                         string.Format(StaticDataMgr.Instance.GetTextByID("adventure_tipsguyongxinxi"), hireMonster.nickname, unitData.NickNameAttr),
                                                                         sociatyBase != null ? sociatyBase.coinHire : 0,
                                                                         SubmitAdventure);

        }
        else if (selfMonsterId.Count == 5)
        {
            PrompMsgAdventureConfirm promp = PrompMsgAdventureConfirm.Open(StaticDataMgr.Instance.GetTextByID("adventure_tipstanxian"),
                                                                StaticDataMgr.Instance.GetTextByID("adventure_tipsweight"), 
                                                                conditionStep,
                                                                "",
                                                                0,
                                                                SubmitAdventure);
        }
        else
        {
            Logger.LogError("选择大冒险阵容异常");
        }
    }
    void SubmitAdventure(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;
        
        PB.HSAdventureEnter param = new PB.HSAdventureEnter();
        param.teamId = AdventureDataMgr.Instance.GetUnusedAdventureTeamId();
        param.type = curData.adventureData.type;
        param.gear = curData.adventureData.time;
        if (selfMonsterId.Count == 4 && hireMonster != null)
        {
            Sociatybase sociatyBase = StaticDataMgr.Instance.GetSociatybaseData(hireMonster.bp);
            if (sociatyBase==null||GameDataMgr.Instance.PlayerDataAttr.coin<sociatyBase.coinHire)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.JinbiNoEnough();
                return;
            }
            param.selfMonsterId.AddRange(selfMonsterId);
            param.hireMonster = hireMonster;
        }
        else if (selfMonsterId.Count == 5)
        {
            param.selfMonsterId.AddRange(selfMonsterId);
        }
        else
        {
            Logger.LogError("大冒险上阵数据异常");
        }
        GameApp.Instance.netManager.SendMessage(PB.code.ADVENTURE_ENTER_C.GetHashCode(), param);
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("adventure_record_004"), (int)PB.ImType.PROMPT);
    }

    #endregion

    #region UIBase
    public override void Init()
    {
        allSelfs = null;
        allGuilds = null;
        selfMonsterId.Clear();
        hireMonster = null;
        filterType = -1;
        filterProperty = -1;
        for (int i = 0; i < teamList.Count; i++)
        {
            teamList[i].ReloadData<object>(null);
        }
    }
    public override void Clean()
    {
        scrollSelf.CleanContent();
        scrollGuild.CleanContent();
    }
    #endregion

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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_NEW_CONDITION_C.GetHashCode().ToString(), OnAdventureNewConditionReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_NEW_CONDITION_S.GetHashCode().ToString(), OnAdventureNewConditionReturn);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_CONDITION_C.GetHashCode().ToString(), OnAdventureBuyConditionReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_CONDITION_S.GetHashCode().ToString(), OnAdventureBuyConditionReturn);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_ENTER_C.GetHashCode().ToString(), OnAdventureEnterReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_ENTER_S.GetHashCode().ToString(), OnAdventureEnterReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_NEW_CONDITION_C.GetHashCode().ToString(), OnAdventureNewConditionReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_NEW_CONDITION_S.GetHashCode().ToString(), OnAdventureNewConditionReturn);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_CONDITION_C.GetHashCode().ToString(), OnAdventureBuyConditionReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_BUY_CONDITION_S.GetHashCode().ToString(), OnAdventureBuyConditionReturn);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_ENTER_C.GetHashCode().ToString(), OnAdventureEnterReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_ENTER_S.GetHashCode().ToString(), OnAdventureEnterReturn);
    }
    void OnAdventureNewConditionReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("刷新条件失败");
            return;
        }
        PB.HSAdventureNewConditionRet result = msg.GetProtocolBody<PB.HSAdventureNewConditionRet>();

        if (result.adventure.adventureId==curData.adventureData.id)
        {
            AdventureDataMgr.Instance.AdventureChange = result.changeCount;
            AdventureDataMgr.Instance.AdventureChangeBeginTime = result.changeCountBeginTime;
            curData.conditions = result.adventure.condition;
            RefreshAdventureCondition(curData.conditions);
            FilterMonster(-1, -1);
        }
        else
        {
            Logger.LogError("刷新条件超时错误");
            UIMgr.Instance.CloseUI_(this);
        }
        
    }
    void OnAdventureBuyConditionReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("购买大冒险刷新次数失败");
            return;
        }
        PB.HSAdventureBuyConditionRet result = msg.GetProtocolBody<PB.HSAdventureBuyConditionRet>();
        AdventureDataMgr.Instance.AdventureChange = result.changeCount;
        AdventureDataMgr.Instance.AdventureChangeBeginTime = result.changeCountBeginTime;

        OnClickRefeshConditionBtn();
    }
    void OnAdventureEnterReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("冒险进入失败");
            return;
        }
        PB.HSAdventureEnterRet result = msg.GetProtocolBody<PB.HSAdventureEnterRet>();

        #region 设置大冒险数据更新
        AdventureTeam team = new AdventureTeam()
        {
            teamId = result.teamId,
            adventure = curData,
            selfIdList = new List<int>(selfMonsterId),
            guildMonster = hireMonster
        };
        curData.adventureTeam = team;
        curData.EndTime = result.endTime;
        AdventureDataMgr.Instance.AdventureTeamUpdate(team);
        #endregion

        #region 设置本地宠物状态更新
        if (hireMonster!=null)
        {
            AdventureDataMgr.Instance.AddHiredMonster(hireMonster.monsterId);
        }
        for (int i = 0; i < selfMonsterId.Count; i++)
        {
            GameUnit unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(selfMonsterId[i]);
            if (unit != null)
            {
                unit.pbUnit.SetInAdventure(true);
            }
        }
        #endregion

        GameEventMgr.Instance.FireEvent(GameEventList.AdventureChange);

        UIMgr.Instance.CloseUI_(this);
    }
    #endregion

    #region IAdventureConditionItem
    public void onSelectionCondition(AdventureConditionItem condition, bool filter)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i] != condition)
            {
                conditions[i].IsFilter = false;
            }
            else
            {
                if (filter)
                {
                    //condition.IsFilter = true;
                    AdventureConditionTypeData conditionData = StaticDataMgr.Instance.GetAdventureConditionType(condition.curData.conditionTypeCfgId);
                    FilterMonster(conditionData.monsterType, conditionData.monsterProperty);
                }
                else
                {
                    //condition.IsFilter = false;
                    FilterMonster(-1, -1);
                }
            }
        }
    }
    #endregion

    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
        if (scrollView==scrollSelf)
        {
            AdventureSelfMonster monster = item.GetComponent<AdventureSelfMonster>();
            monster.ReloadData(showSelfs[index]);
        }
        else if (scrollView==scrollGuild)
        {
            AdventureGuildMonster monster = item.GetComponent<AdventureGuildMonster>();
            monster.ReloadData(showGuilds[index]);
        }
    }

    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        Transform trans=null;
        if (scrollView == scrollSelf)
        {
            MonsterIcon icon = MonsterIcon.CreateIcon();
            trans = icon.transform;
            UIUtil.SetParentReset(trans, parent);
            AdventureSelfMonster selfMonster = trans.gameObject.AddComponent<AdventureSelfMonster>();
            selfMonster.IAdventureSelfMonsterDelegate = this;
        }
        else if(scrollView==scrollGuild)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("AdventureGuildMonster");
            trans = go.transform;
            UIUtil.SetParentReset(trans, parent);
            AdventureGuildMonster guildMonster = go.GetComponent<AdventureGuildMonster>();
            guildMonster.IAdventureGuildMonsterDelegate = this;
        }
        return trans;
    }

    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
        itemList.Clear();
    }
    #endregion

    #region IAdventureSelfMonster
    public void OnClickSelfMonster(AdventureSelfMonsterInfo monster)
    {
        if (CheckIsSelected(monster.unit.pbUnit.guid))
        {
            RemoveAdventureTeamMonster(monster);
        }
        else if ((selfMonsterId.Count < 4 && hireMonster != null) || (selfMonsterId.Count < 5))
        {
            AddAdventureTeamMonster(monster);
        }
    }
    #endregion

    #region IAdventureGuildMonster
    public void OnClickGuildMonster(AdventureGuildMonsterInfo monster)
    {
        if (hireMonster==null)
        {
            AddAdventureTeamMonster(monster);
        }
        else if(hireMonster==monster.unit)
        {
            RemoveAdventureTeamMonster(monster);
        }
        else
        {
            Logger.Log("上阵宠物已经包含公会宠物");
        }
    }
    #endregion

    #region IAdventureTeamMonster
    public void OnClickTeamMonster<T>(T monster)
    {
        if (monster==null)
        {
            Logger.LogError("大冒险移除队员出现异常");
        }
        else
        {
            RemoveAdventureTeamMonster(monster);
        }
    }
    #endregion
}
