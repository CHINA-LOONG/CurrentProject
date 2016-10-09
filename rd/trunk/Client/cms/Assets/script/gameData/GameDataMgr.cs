using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDataMgr : MonoBehaviour
{
    //private Dictionary<int, PlayerData> playerList = new Dictionary<int,PlayerData>();
    //private Dictionary<int, BattleObject> unitList = new Dictionary<int, BattleObject>();

    static GameDataMgr mInst = null;

    //some global viriable
    //TODO: move to playerdata?
    public int lastStage = -1;
    public float mainStageRotAngle = 0.0f;
    public int curInstanceType;//当前副本类型
    public TowerType curTowerType;//当前通天塔类型
    public int curTowerShilianFloor = 0;//试炼塔层数
    public int curTowerJuewangFloor = 0;//绝望塔层数
    public int curTowerSiwangFloor = 0;//死亡塔层数
    public HoleType curHoleType;//当前洞类型
    public List<PB.HoleState> mHoleStateList;// = new List<PB.HoleState>();
    public bool mHoleInvalidate = false;
    public bool mTowerInvalidate = false;
    public bool mTowerRefreshed = false;
    //public List<PB.TowerState> mTowerStateList;
    public int summonZuanshi;
    public int summonJinbi;
    public int freeJinbiSumNum;
    bool mGoldMaxHinted = false;
    bool mCoinMaxHinted = false;

    void Awake()
    {
        mGoldMaxHinted = false;
        mCoinMaxHinted = false;
    }

    public static GameDataMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("GameDataMgr");
                mInst = go.AddComponent<GameDataMgr>();
            }
            return mInst;
        }
    }

    [SerializeField]
    PlayerData mainPlayer;
    public PlayerData PlayerDataAttr
    {
        get
        {
            return mainPlayer;
        }
        set
        {
            mainPlayer = value;
            //AddPlayerData(mainPlayer);
        }
    }

    [SerializeField]
    UserData userData;
    public UserData UserDataAttr
    {
        get
        {
            return userData;
        }
    }


	[SerializeField]
	ShopDataMgr shopDataMgr;
	public ShopDataMgr ShopDataMgrAttr
	{
		get
		{
			return shopDataMgr;
		}
	}

    [SerializeField]
    HuoliRestore huoliRestore;
    public HuoliRestore HuoliRestoreAtrr
    {
        get
        {
            return huoliRestore;
        }
    }

    [SerializeField]
    SociatyDataMgr sociatyDataMgr;
    public SociatyDataMgr SociatyDataMgrAttr
    {
        get
        {
            return sociatyDataMgr;
        }
    }

    [SerializeField]
    PvpDataMgr pvpDataMgr;
    public PvpDataMgr PvpDataMgrAttr
    {
        get
        {
            return pvpDataMgr;
        }
    }

    //--------------------------------------------------------------------------------------------
    public void ClearAllData()//重新登录重置删除数据
    {
        mainPlayer.ClearData();
        shopDataMgr.ClearData();
        sociatyDataMgr.ClearData();
        pvpDataMgr.ClearData();
    }
    //---------------------------------------------------------------------------------------------
    public void Init()
    {
        DontDestroyOnLoad(gameObject);

        GameObject playerData = new GameObject("PlayerData");
        playerData.transform.SetParent(transform);
        mainPlayer = playerData.AddComponent<PlayerData>();

        GameObject userDataGo = new GameObject("UserData");
        userDataGo.transform.SetParent(transform);
        userData = userDataGo.AddComponent<UserData>();
        userData.Init();

		GameObject shopDataGo = new GameObject ("ShopDataMgr");
		shopDataGo.transform.SetParent (transform);
		shopDataMgr = shopDataGo.AddComponent<ShopDataMgr> ();
		shopDataMgr.Init ();

        GameObject huoliGo = new GameObject("HuoliRestore");
        huoliGo.transform.SetParent(transform);
        huoliRestore = huoliGo.AddComponent<HuoliRestore>();

        GameObject sociatyGo = new GameObject("SociatyDataMgr");
        sociatyGo.transform.SetParent(transform);
        sociatyDataMgr = sociatyGo.AddComponent<SociatyDataMgr>();

        GameObject pvpGo = new GameObject("PvpDataMgr");
        pvpGo.transform.SetParent(transform);
        pvpDataMgr = pvpGo.AddComponent<PvpDataMgr>();

        BindListener();
    }
    //---------------------------------------------------------------------------------------------
    void OnDestroy()
    {
        UnBindListener();
    }
    //---------------------------------------------------------------------------------------------
    void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_INFO_SYNC_S.GetHashCode().ToString(), OnPlayerInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SETTING_INFO_SYNC_S.GetHashCode().ToString(), OnSettingInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_INFO_SYNC_S.GetHashCode().ToString(), OnMonsterInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_INFO_SYNC_S.GetHashCode().ToString(), OnItemInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INFO_SYNC_S.GetHashCode().ToString(), OnEquipInfoSync);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_INFO_SYNC_S.GetHashCode().ToString(), OnQuestInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_ACCEPT_S.GetHashCode().ToString(), OnQuestAccept);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_UPDATE_S.GetHashCode().ToString(), OnQuestUpdate);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_REMOVE_S.GetHashCode().ToString(), OnQuestRemove);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_INFO_SYNC_S.GetHashCode().ToString(), OnMailInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_NEW_S.GetHashCode().ToString(), OnMailNew);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_CONSUME_S.GetHashCode().ToString(), OnConsume);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseItemFinished);
        //GameEventMgr.Instance.AddListener<Coin>(GameEventList.EatCoin, OnEatCoin);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_INFO_SYNC_S.GetHashCode().ToString(), OnAdventureInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ADVENTURE_CONDITION_PUSH_S.GetHashCode().ToString(), OnAdventureUpdate);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_DEFENCE_SYNC_S.GetHashCode().ToString(), OnSelfPvpDefenseSync);
    }
    //---------------------------------------------------------------------------------------------
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_INFO_SYNC_S.GetHashCode().ToString(), OnPlayerInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SETTING_INFO_SYNC_S.GetHashCode().ToString(), OnSettingInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_INFO_SYNC_S.GetHashCode().ToString(), OnMonsterInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_INFO_SYNC_S.GetHashCode().ToString(), OnItemInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INFO_SYNC_S.GetHashCode().ToString(), OnEquipInfoSync);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_INFO_SYNC_S.GetHashCode().ToString(), OnQuestInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_ACCEPT_S.GetHashCode().ToString(), OnQuestAccept);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_UPDATE_S.GetHashCode().ToString(), OnQuestUpdate);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_REMOVE_S.GetHashCode().ToString(), OnQuestRemove);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_INFO_SYNC_S.GetHashCode().ToString(), OnMailInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_NEW_S.GetHashCode().ToString(), OnMailNew);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_CONSUME_S.GetHashCode().ToString(), OnConsume);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseItemFinished);
        //GameEventMgr.Instance.RemoveListener<Coin>(GameEventList.EatCoin, OnEatCoin);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_INFO_SYNC_S.GetHashCode().ToString(), OnAdventureInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ADVENTURE_CONDITION_PUSH_S.GetHashCode().ToString(), OnAdventureUpdate);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_DEFENCE_SYNC_S.GetHashCode().ToString(), OnSelfPvpDefenseSync);
    }
    //---------------------------------------------------------------------------------------------
    public void OnBattleStart()
    {
        mHoleInvalidate = false;
        mTowerInvalidate = false;

        if (curInstanceType == (int)InstanceType.Hole)
        {
            for (int i = 0; i < mHoleStateList.Count; ++i)
            {
                if (mHoleStateList[i].holeId == (int)curHoleType)
                {
                    ++mHoleStateList[i].countDaily;
                    break;
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnBattleOver(bool isSuccess)
    {

        if(curInstanceType == (int)InstanceType.Tower)
        {
            if (mTowerInvalidate == true)
            {
                mTowerInvalidate = false;
                return;
            }

            TowerData curTowerData = StaticDataMgr.Instance.GetTowerData((int)curTowerType);
            if (curTowerData != null)
            {
                switch (curTowerType)
                {
                    case TowerType.Tower_Juewang:
                        {
                            if (isSuccess == true)
                            {
                                curTowerJuewangFloor++;
                                if (curTowerJuewangFloor > curTowerData.floorList.Count)
                                {
                                    curTowerJuewangFloor = curTowerData.floorList.Count;
                                }
                            }
                        }
                        break;
                    case TowerType.Tower_Shilian:
                        {
                            if (isSuccess == true)
                            {
                                curTowerShilianFloor++;
                                if (curTowerShilianFloor > curTowerData.floorList.Count)
                                {
                                    curTowerShilianFloor = curTowerData.floorList.Count;
                                }
                            }
                        }
                        break;
                    case TowerType.Tower_Siwang:
                        {
                            if (isSuccess == true)
                            {
                                curTowerSiwangFloor++;
                                if (curTowerSiwangFloor > curTowerData.floorList.Count)
                                {
                                    curTowerSiwangFloor = curTowerData.floorList.Count;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public int GetHoleDailyCount(int holeType)
    {
        for (int i = 0; i < mHoleStateList.Count; ++i)
        {
            if (mHoleStateList[i].holeId == holeType)
            {
                return mHoleStateList[i].countDaily;
            }
        }

        return 0;
    }
    //---------------------------------------------------------------------------------------------
    public void SyncHoleData(List<PB.HoleState> holeStateList)
    {
        mHoleStateList = holeStateList;

        GameEventMgr.Instance.FireEvent(GameEventList.DailyRefresh);
    }
    //---------------------------------------------------------------------------------------------
    public void SyncTowerData(List<PB.TowerState> towerStateList)
    {
        curTowerShilianFloor = 0;
        curTowerSiwangFloor = 0;
        curTowerJuewangFloor = 0;
        //mTowerStateList = towerStateList;

        if (towerStateList != null)
        {
            int count = towerStateList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (towerStateList[i].towerId == (int)TowerType.Tower_Juewang)
                {
                    curTowerJuewangFloor = towerStateList[i].floor;
                }
                else if (towerStateList[i].towerId == (int)TowerType.Tower_Siwang)
                {
                    curTowerSiwangFloor = towerStateList[i].floor;
                }
                else if (towerStateList[i].towerId == (int)TowerType.Tower_Shilian)
                {
                    curTowerShilianFloor = towerStateList[i].floor;
                }
            }
        }

        //GameEventMgr.Instance.FireEvent(GameEventList.DailyRefresh);
    }
    //---------------------------------------------------------------------------------------------
    public void GetNextTowerFloor(out string nextInstanceID, out int floor)
    {
        nextInstanceID = null;
        floor = -1;
        TowerData curTowerData = StaticDataMgr.Instance.GetTowerData((int)curTowerType);
        switch (curTowerType)
        {
            case TowerType.Tower_Shilian:
                if(curTowerShilianFloor + 1 <= curTowerData.floorList.Count)
                {
                    nextInstanceID = curTowerData.floorList[curTowerShilianFloor];
                    floor = curTowerShilianFloor + 1;
                }
                break;
            case TowerType.Tower_Juewang:
                if (curTowerJuewangFloor + 1 <= curTowerData.floorList.Count)
                {
                    nextInstanceID = curTowerData.floorList[curTowerJuewangFloor];
                    floor = curTowerJuewangFloor + 1;
                }
                break;
            case TowerType.Tower_Siwang:
                if (curTowerSiwangFloor + 1 <= curTowerData.floorList.Count)
                {
                    nextInstanceID = curTowerData.floorList[curTowerSiwangFloor];
                    floor = curTowerSiwangFloor + 1;
                }
                break;
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnPlayerInfoSync(ProtocolMessage msg)
    {
        if (mainPlayer == null)
        {
            mainPlayer = new PlayerData();
        }
        PB.HSPlayerInfoSync playerSync = msg.GetProtocolBody<PB.HSPlayerInfoSync>();
        PB.PlayerInfo playerInfo = playerSync.info;
        //int mainUnitCount = playerInfo.battleMonster.Count;
        //for (int i = 0; i < mainUnitCount; ++i)
        //{
        //    mainPlayer.mainUnitID.Add(playerInfo.battleMonster[i]);
        //}
        mainPlayer.playerId = playerInfo.playerId;
        mainPlayer.nickName = playerInfo.nickname;
        mainPlayer.career = playerInfo.career;
        {
            mainPlayer.LevelAttr = playerInfo.level;
           // GameEventMgr.Instance.FireEvent<int>(GameEventList.LevelChanged, mainPlayer.level);
        }
        {
            mainPlayer.ExpAttr = playerInfo.exp;
        }
        {
            mainPlayer.gold = playerInfo.gold;
			GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged,mainPlayer.gold);
        }
        {
            mainPlayer.coin = playerInfo.coin;
            GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, mainPlayer.coin);
        }

        mainPlayer.TowerCoinAttr = playerInfo.towerCoin;
        mainPlayer.HonorAtr = playerInfo.honor;

        mainPlayer.gender = playerInfo.gender;
        mainPlayer.eye = playerInfo.eye;
        mainPlayer.hair = playerInfo.hair;
        mainPlayer.hairColor = playerInfo.hairColor;
        mainPlayer.recharget = playerInfo.recharge;
        mainPlayer.vipLevel = playerInfo.vipLevel;
    }
    //---------------------------------------------------------------------------------------------
    void OnSettingInfoSync(ProtocolMessage msg)
    {
        if (mainPlayer == null)
        {
            mainPlayer = new PlayerData();
        }

        PB.HSSettingInfoSync settingInfo = msg.GetProtocolBody<PB.HSSettingInfoSync>();
        PB.HSSetting hsSetting = settingInfo.setting;
        PlayerPrefs.SetString("serverLanguage", hsSetting.language);
        int count = settingInfo.setting.blockPlayerId.Count;
        for (int i = 0; i < count; ++i)
        {
            mainPlayer.mBlockPlayerList.Add(settingInfo.setting.blockPlayerId[i]);
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnMonsterInfoSync(ProtocolMessage msg)
    {
        PB.HSMonsterInfoSync monsterSync = msg.GetProtocolBody<PB.HSMonsterInfoSync>();
        int monsterCount = monsterSync.monsterInfo.Count;
        for (int i = 0; i < monsterCount; ++i)
        {
            PbUnit unit = Util.CreatePbUnitFromHsMonster(monsterSync.monsterInfo[i], UnitCamp.Player);
            mainPlayer.unitPbList.Add(unit.guid, unit);
            mainPlayer.allUnitDic.Add(unit.guid, GameUnit.FromPb(unit, true));
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnItemInfoSync(ProtocolMessage msg)
    {
        PB.HSItemInfoSync itemSync = msg.GetProtocolBody<PB.HSItemInfoSync>();

        foreach (PB.ItemInfo itemInfo in itemSync.itemInfos)
        {
            mainPlayer.gameItemData.AddItem(itemInfo.itemId, itemInfo.count);
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnEquipInfoSync(ProtocolMessage msg)
    {
        PB.HSEquipInfoSync equpSync = msg.GetProtocolBody<PB.HSEquipInfoSync>();

        Logger.Log("equipCount:" + equpSync.equipInfos.Count);
        foreach (PB.EquipInfo equipInfo in equpSync.equipInfos)
        {
            EquipData equip = EquipData.valueof(equipInfo.id, equipInfo.equipId, equipInfo.stage, equipInfo.level,equipInfo.monsterId, equipInfo.gemItems);

            mainPlayer.gameEquipData.AddEquip(equip);
            //要在添加装备之后
            mainPlayer.AddEquipTypePart(equip);
            
            if (mainPlayer.allUnitDic.ContainsKey(equipInfo.monsterId))
            {
                ItemStaticData item = StaticDataMgr.Instance.GetItemData(equipInfo.equipId);
                //mainPlayer.allUnitDic[equipInfo.monsterId].equipList[item.part] = equip;
                //TODO: refresh once instead of refresh every time
                mainPlayer.allUnitDic[equipInfo.monsterId].SetEquipData(item.part, equip, true);
            }
        }
    }
    //quest------------------------------------------------------------------------------------------
    void OnQuestInfoSync(ProtocolMessage msg)
    {
        PB.HSQuestInfoSync questSync = msg.GetProtocolBody<PB.HSQuestInfoSync>();
        //Logger.Log("questCount:" + questSync.questInfo.Count);
        mainPlayer.gameQuestData.QuestInfoSync(questSync.questInfo);
    }
    void OnQuestAccept(ProtocolMessage msg)
    {
        PB.HSQuestAccept questAccept = msg.GetProtocolBody<PB.HSQuestAccept>();
        mainPlayer.gameQuestData.QuestAccept(questAccept.quest);
    }
    void OnQuestRemove(ProtocolMessage msg)
    {
        PB.HSQuestRemove questRemove = msg.GetProtocolBody<PB.HSQuestRemove>();
        mainPlayer.gameQuestData.QuestRemove(questRemove.questId);
    }
    void OnQuestUpdate(ProtocolMessage msg)
    {
        PB.HSQuestUpdate questUpdate = msg.GetProtocolBody<PB.HSQuestUpdate>();
        mainPlayer.gameQuestData.QuestUpdate(questUpdate.quest);
    }
    //adventure-------------------------------------------------------------------------------------
    void OnAdventureInfoSync(ProtocolMessage msg)
    {
        PB.HSAdventureInfoSync adventureSync = msg.GetProtocolBody<PB.HSAdventureInfoSync>();
        AdventureDataMgr.Instance.AdvestureInfoSync(adventureSync);
    }
    void OnAdventureUpdate(ProtocolMessage msg)
    {
        PB.HSAdventureConditionPush adventureUpdate = msg.GetProtocolBody<PB.HSAdventureConditionPush>();
        AdventureDataMgr.Instance.AdvestureInfoUpdate(adventureUpdate);
    }
    //mail----------------------------------------------------------------------------------------------
    void OnSelfPvpDefenseSync(ProtocolMessage msg)
    {
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSGetPVPDefenceMonsterRet msgRet = msg.GetProtocolBody<PB.HSGetPVPDefenceMonsterRet>();
        PvpDataMgrAttr.ClearDefensePosition();
        for (int i = 0; i < msgRet.monsterId.Count; ++i)
        {
            PvpDataMgrAttr.defenseTeamList[i] = msgRet.monsterId[i].ToString();
        }     
    }
    //mail----------------------------------------------------------------------------------------------
    void OnMailInfoSync(ProtocolMessage msg)
    {
        PB.HSMailInfoSync mailSync = msg.GetProtocolBody<PB.HSMailInfoSync>();
        Logger.Log("mailCount:" + mailSync.mailInfo.Count);
        mainPlayer.gameMailData.ClearMail();
        foreach (PB.HSMail mailInfo in mailSync.mailInfo)
        {
            mainPlayer.gameMailData.AddMail(mailInfo);
            //Logger.Log("mail" + mailInfo.mailId + "\t" + mailInfo.reward.Count + "\t" + mailInfo.senderId + "\t" + mailInfo.senderName +"\t"+ mailInfo.sendTimeStamp);
        }
        GameEventMgr.Instance.FireEvent<int>(GameEventList.MailAdd, 0);
    }
    void OnMailNew(ProtocolMessage msg)
    {
        PB.HSMailNew mailNew = msg.GetProtocolBody<PB.HSMailNew>();

        if (mailNew.overflowMailId != 0)
        {
            mainPlayer.gameMailData.RemoveMail(mailNew.overflowMailId);
        }
        if (mailNew.mail!=null)
        {
            mainPlayer.gameMailData.AddMail(mailNew.mail);
        }
        GameEventMgr.Instance.FireEvent<int>(GameEventList.MailAdd, mailNew.mail.mailId);
    }
    //---------------------------------------------------------------------------------------------
    public void CheckCoinFull()
    {
        if (PlayerDataAttr.coin >= 999999999)
        {
            if (mCoinMaxHinted == false)
            {
                mCoinMaxHinted = true;
                PlayerPrefs.SetInt("coinHintMax", 1);
                MsgBox.PromptMsg.Open(
                    MsgBox.MsgBoxType.Conform,
                    string.Format(StaticDataMgr.Instance.GetTextByID("coin_max")),
                    null,
                    true,
                    false
                    );
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void CheckDiamondFull()
    {
        if (PlayerDataAttr.gold >= 999999999)
        {
            //int goldMaxHint = PlayerPrefs.GetInt("goldHintMax");
            if (mGoldMaxHinted == false)
            {
                //PlayerPrefs.SetInt("goldHintMax", 1);
                mGoldMaxHinted = true;
                MsgBox.PromptMsg.Open(
                    MsgBox.MsgBoxType.Conform,
                    string.Format(StaticDataMgr.Instance.GetTextByID("gold_max")),
                    null,
                    true,
                    false
                    );
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();

        if(reward.hsCode == (int) PB.code.ALLIANCE_INSTANCE_REWARD_S)
        {
            sociatyDataMgr.allianceInstanceReward.Add(reward);
        }

        if (reward.playerAttr != null)
        {
            //change later in uiscore
            if (reward.hsCode != PB.code.INSTANCE_SETTLE_C.GetHashCode())
            {
                PlayerDataAttr.LevelAttr = reward.playerAttr.level;
                PlayerDataAttr.ExpAttr = reward.playerAttr.exp;
                PlayerDataAttr.UpdateHuoli(reward.playerAttr.fatigue, reward.playerAttr.fatigueBeginTime);
                //PlayerDataAttr.HuoliAttr = reward.playerAttr.fatigue;
            }
            PlayerDataAttr.coin = reward.playerAttr.coin;
            GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, PlayerDataAttr.coin);
            PlayerDataAttr.gold = reward.playerAttr.gold;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged, mainPlayer.gold);

            PlayerDataAttr.TowerCoinAttr = reward.playerAttr.towerCoin;
            PlayerDataAttr.GonghuiCoinAttr = reward.playerAttr.contribution;

            //change later in pvp score
            if (reward.hsCode == PB.code.PVP_SETTLE_C.GetHashCode())
            {
                BattleController.Instance.PvpHornorPointGet = reward.playerAttr.honor - PlayerDataAttr.HonorAtr;
            }
            PlayerDataAttr.HonorAtr = reward.playerAttr.honor;
        }

        GameUnit unit = null;
        foreach (PB.SynMonsterAttr item in reward.monstersAttr)
        {
            unit = mainPlayer.GetPetWithKey(item.monsterId);
            if (unit==null)
            {
                Logger.Log("不存在的宠物");
                continue;
            }
            //battle module,use score to sync level
            if (GameMain.Instance.IsCurModule<BattleModule>() == false)
            {
                unit.RefreshUnitLvl(item.level, item.exp);
                GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.ReloadPetLevelNotify, unit);
            }
        }

        foreach (PB.RewardItem item in reward.RewardItems)
        {
            //if (item.type == (int)PB.itemType.PLAYER_ATTR)
            //{
            //    if ((int)PB.changeType.CHANGE_COIN == int.Parse(item.itemId))
            //    {
            //        GameDataMgr.Instance.mainPlayer.coin += item.count;
            //        GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, mainPlayer.coin);
            //    }
            //    else if ((int)PB.changeType.CHANGE_GOLD == int.Parse(item.itemId) || (int)PB.changeType.CHANGE_GOLD_BUY == int.Parse(item.itemId))
            //    {
            //        GameDataMgr.Instance.mainPlayer.gold += item.count;
            //        GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged,mainPlayer.gold);
            //    }
            //}
            //if (item.type == (int)PB.itemType.MONSTER_ATTR)
            //{ 
            //    //此处不做处理，通过外部的SynMonsterAttr来同步怪物属性
            //}
            if (item.type == (int)PB.itemType.ITEM)
            {
                GameDataMgr.Instance.mainPlayer.gameItemData.AddItem(item.itemId, (int)item.count);
            }
            else if (item.type == (int)PB.itemType.EQUIP)
            {
                GameDataMgr.Instance.mainPlayer.gameEquipData.AddEquip(EquipData.valueof(item.id, item.itemId, item.stage, item.level, -1, new List<PB.GemPunch>()));
                GameDataMgr.Instance.mainPlayer.AddEquipTypePart(item.id);
            }
            else if (item.type == (int)PB.itemType.MONSTER)
            {
                //TODO: duplicate code
                PbUnit pbUnit = new PbUnit();
                PB.HSMonster monster = item.monster;
                pbUnit.slot = -1;
                //pbUnit.guid = (int)item.id;
                //pbUnit.id = item.itemId;
                //pbUnit.stage = item.stage;
                pbUnit.camp = UnitCamp.Player;
                pbUnit.guid = monster.monsterId;
                pbUnit.id = monster.cfgId;
                pbUnit.stage = monster.stage;
                pbUnit.character = monster.disposition;
                pbUnit.lazy = monster.lazy;
                pbUnit.level = monster.level;
                pbUnit.curExp = monster.exp;
                pbUnit.spellPbList = monster.skill;
                mainPlayer.unitPbList.Add(pbUnit.guid, pbUnit);
                mainPlayer.allUnitDic.Add(pbUnit.guid, GameUnit.FromPb(pbUnit, true));
                PlayerDataAttr.AddCollectPet(pbUnit.id);
            }
            else if (item.type == (int)PB.itemType.PLAYER_ATTR)
            {
                if ((int)PB.changeType.CHANGE_COIN == int.Parse(item.itemId))
                {
                    if (reward.hsCode != PB.code.INSTANCE_SETTLE_C.GetHashCode())
                    {
                        CheckCoinFull();
                    }
                }
                else if ((int)PB.changeType.CHANGE_GOLD == int.Parse(item.itemId))
                {
                    if (reward.hsCode != PB.code.INSTANCE_SETTLE_C.GetHashCode())
                    {
                        CheckDiamondFull();
                    }
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnConsume(ProtocolMessage msg)
    {
        PB.HSConsumeInfo reward = msg.GetProtocolBody<PB.HSConsumeInfo>();
        if (reward.playerAttr!=null)
        {
            //if (reward.hsCode != PB.code.INSTANCE_SETTLE_C.GetHashCode())
            {
                //PlayerDataAttr.LevelAttr = reward.playerAttr.level;
                //PlayerDataAttr.ExpAttr = reward.playerAttr.exp;
                //PlayerDataAttr.HuoliAttr = reward.playerAttr.fatigue;
                PlayerDataAttr.UpdateHuoli(reward.playerAttr.fatigue, reward.playerAttr.fatigueBeginTime);
            }
            PlayerDataAttr.coin = reward.playerAttr.coin;
            GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, PlayerDataAttr.coin);
            PlayerDataAttr.gold = reward.playerAttr.gold;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged, mainPlayer.gold);

            PlayerDataAttr.TowerCoinAttr = reward.playerAttr.towerCoin;
            PlayerDataAttr.GonghuiCoinAttr = reward.playerAttr.contribution;

            PlayerDataAttr.HonorAtr = reward.playerAttr.honor;
        }
        GameUnit unit = null;
        foreach (PB.SynMonsterAttr item in reward.monstersAttr)
        {
            unit = mainPlayer.GetPetWithKey(item.monsterId);
            if (unit == null)
            {
                Logger.Log("不存在的宠物");
                continue;
            }
            //battle module,use score to sync level
            if (GameMain.Instance.IsCurModule<BattleModule>() == false)
            {
                unit.RefreshUnitLvl(item.level, item.exp);
            }
        }

        foreach (PB.ConsumeItem item in reward.consumeItems)
        {
            //if (item.type == (int)PB.itemType.PLAYER_ATTR)
            //{
            //    if ((int)PB.changeType.CHANGE_COIN == int.Parse(item.itemId))
            //    {
            //        GameDataMgr.Instance.mainPlayer.coin -= item.count;
            //        GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, mainPlayer.coin);
            //    }
            //    else if ((int)PB.changeType.CHANGE_GOLD == int.Parse(item.itemId))
            //    {
            //        GameDataMgr.Instance.mainPlayer.gold -= item.count;
            //        GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged,mainPlayer.gold);
            //    }

            //}
            if (item.type == (int)PB.itemType.ITEM)
            {
                GameDataMgr.Instance.mainPlayer.gameItemData.RemoveItem(item.itemId, (int)item.count);
            }
            else if (item.type == (int)PB.itemType.EQUIP)
            {
                GameDataMgr.Instance.mainPlayer.RemoveEquipTypePart(item.id);
                GameDataMgr.Instance.mainPlayer.gameEquipData.RemoveEquip(item.id);
            }
            else if (item.type == (int)PB.itemType.MONSTER)
            {
                GameDataMgr.Instance.mainPlayer.RemoveUnit((int)item.id);
            }
        }
    }


    void OnUseItemFinished(ProtocolMessage msg)
    {
        PB.HSItemUseRet itemUseReturn = msg.GetProtocolBody<PB.HSItemUseRet>();
        if(null != itemUseReturn)
        {
            if(itemUseReturn.useCountDaily != -1)
            {
                PlayerDataAttr.gameItemData.UpdateItemState(itemUseReturn.itemId, itemUseReturn.useCountDaily);
            }


            ItemStaticData stData = StaticDataMgr.Instance.GetItemData(itemUseReturn.itemId);
            bool isHuoli = (stData.addAttrType == 7);
            if(isHuoli)
            {
                UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("energy_get"), stData.addAttrValue ),(int)PB.ImType.PROMPT);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    //public void AddPlayerData(PlayerData data)
    //{
    //    if (playerList.ContainsKey(data.playerId) == true)
    //    {
    //        return;
    //    }

    //    playerList.Add(data.playerId, data);
    //    data.InitMainUnitList();

    //    var itor = data.mainUnitList.GetEnumerator();
    //    while (itor.MoveNext())
    //    {
    //        if (unitList.ContainsKey(itor.Current.guid) == false)
    //        {
    //            unitList.Add(itor.Current.guid, itor.Current);
    //        }
    //    }
    //}
    //---------------------------------------------------------------------------------------------
    //public void RemovePlayerData(int guid)
    //{
    //    PlayerData data;
    //    if (playerList.TryGetValue(guid, out data) == false)
    //    {
    //        return;
    //    }

    //    var itor = data.mainUnitList.GetEnumerator();
    //    while (itor.MoveNext())
    //    {
    //        unitList.Remove(itor.Current.guid);
    //    }
    //}
    ////---------------------------------------------------------------------------------------------

    //public Dictionary<int, PlayerData> GetPlayers()
    //{
    //    return playerList;
    //}
    ////---------------------------------------------------------------------------------------------
    //public Dictionary<int, BattleObject> GetUnits()
    //{
    //    return unitList;
    //}
    //---------------------------------------------------------------------------------------------


    //
}
