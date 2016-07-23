using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDataMgr : MonoBehaviour
{
    //private Dictionary<int, PlayerData> playerList = new Dictionary<int,PlayerData>();
    //private Dictionary<int, BattleObject> unitList = new Dictionary<int, BattleObject>();

    static GameDataMgr mInst = null;
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
    InstanceState instanceState;
    public InstanceState InstanceStateAttr
    {
        get
        {
            return instanceState;
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

        GameObject instanceGo = new GameObject("InstanceState");
        instanceGo.transform.SetParent(transform);
        instanceState = userDataGo.AddComponent<InstanceState>();
        instanceState.Init();

		GameObject shopDataGo = new GameObject ("ShopDataMgr");
		shopDataGo.transform.SetParent (transform);
		shopDataMgr = shopDataGo.AddComponent<ShopDataMgr> ();
		shopDataMgr.Init ();


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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSync);
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
        //GameEventMgr.Instance.AddListener<Coin>(GameEventList.EatCoin, OnEatCoin);
    }
    //---------------------------------------------------------------------------------------------
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_INFO_SYNC_S.GetHashCode().ToString(), OnPlayerInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSync);
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
        //GameEventMgr.Instance.RemoveListener<Coin>(GameEventList.EatCoin, OnEatCoin);
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
            mainPlayer.level = playerInfo.level;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.LevelChanged, mainPlayer.level);
        }
        {
            mainPlayer.exp = playerInfo.exp;
        }
        {
            mainPlayer.gold = playerInfo.gold;
			GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged,mainPlayer.gold);
        }
        {
            mainPlayer.coin = playerInfo.coin;
            GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, mainPlayer.coin);
        }
        mainPlayer.gender = playerInfo.gender;
        mainPlayer.eye = playerInfo.eye;
        mainPlayer.hair = playerInfo.hair;
        mainPlayer.hairColor = playerInfo.hairColor;
        mainPlayer.recharget = playerInfo.recharge;
        mainPlayer.vipLevel = playerInfo.vipLevel;
    }
    //---------------------------------------------------------------------------------------------
    void OnStatisticsInfoSync(ProtocolMessage msg)
    {
        PB.HSStatisticsInfoSync playerSync = msg.GetProtocolBody<PB.HSStatisticsInfoSync>();
    }
    //---------------------------------------------------------------------------------------------
    void OnMonsterInfoSync(ProtocolMessage msg)
    {
        mainPlayer.unitPbList.Clear();
        mainPlayer.allUnitDic.Clear();

        PB.HSMonsterInfoSync monsterSync = msg.GetProtocolBody<PB.HSMonsterInfoSync>();
        int monsterCount = monsterSync.monsterInfo.Count;
        for (int i = 0; i < monsterCount; ++i)
        {
            PbUnit unit = new PbUnit();
            PB.HSMonster monster = monsterSync.monsterInfo[i];
            unit.slot = -1;
            unit.guid = monster.monsterId;
            unit.camp = UnitCamp.Player;
            unit.id = monster.cfgId;
            unit.character = monster.disposition;
            unit.lazy = monster.lazy;
            unit.level = monster.level;
            unit.curExp = monster.exp;
            unit.stage = monster.stage;
            unit.spellPbList = monster.skill;
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
            
            if (mainPlayer.allUnitDic.ContainsKey(equipInfo.monsterId))
            {
                ItemStaticData item = StaticDataMgr.Instance.GetItemData(equipInfo.equipId);
                mainPlayer.allUnitDic[equipInfo.monsterId].equipList[item.part] = equip;
            }
        }
    }
    //quest------------------------------------------------------------------------------------------
    void OnQuestInfoSync(ProtocolMessage msg)
    {
        PB.HSQuestInfoSync questSync = msg.GetProtocolBody<PB.HSQuestInfoSync>();
        Logger.Log("questCount:" + questSync.questInfo.Count);
        mainPlayer.gameQuestData.ClearQuest();
        foreach (PB.HSQuest questInfo in questSync.questInfo)
        {
            mainPlayer.gameQuestData.AddQuest(questInfo.questId, questInfo.progress);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
    }
    void OnQuestAccept(ProtocolMessage msg)
    {
        PB.HSQuestAccept questAccept = msg.GetProtocolBody<PB.HSQuestAccept>();
        foreach (PB.HSQuest questInfo in questAccept.quest)
        {
            mainPlayer.gameQuestData.AddQuest(questInfo.questId, questInfo.progress);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
    }
    void OnQuestUpdate(ProtocolMessage msg)
    {
        PB.HSQuestUpdate questUpdate = msg.GetProtocolBody<PB.HSQuestUpdate>();

        foreach (PB.HSQuest questInfo in questUpdate.quest)
        {
            mainPlayer.gameQuestData.AddQuest(questInfo.questId, questInfo.progress);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
    }
    void OnQuestRemove(ProtocolMessage msg)
    {
        PB.HSQuestRemove questRemove = msg.GetProtocolBody<PB.HSQuestRemove>();
        foreach (int questId in questRemove.questId)
        {
            mainPlayer.gameQuestData.RemoveQuest(questId);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
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
            //Debug.Log("mail" + mailInfo.mailId + "\t" + mailInfo.reward.Count + "\t" + mailInfo.senderId + "\t" + mailInfo.senderName +"\t"+ mailInfo.sendTimeStamp);
        }
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
    }

    //---------------------------------------------------------------------------------------------
    void OnReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        foreach (PB.RewardItem item in reward.RewardItems)
        {
            if (item.type == (int)PB.itemType.PLAYER_ATTR)
            {
                if ((int)PB.changeType.CHANGE_COIN == int.Parse(item.itemId))
                {
                    GameDataMgr.Instance.mainPlayer.coin += item.count;
                    GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, mainPlayer.coin);
                }
                else if ((int)PB.changeType.CHANGE_GOLD == int.Parse(item.itemId))
                {
                    GameDataMgr.Instance.mainPlayer.gold += item.count;
					GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged,mainPlayer.gold);
                }

            }
            else if (item.type == (int)PB.itemType.ITEM)
            {
                GameDataMgr.Instance.mainPlayer.gameItemData.AddItem(item.itemId, item.count);
            }
            else if (item.type == (int)PB.itemType.EQUIP)
            {
                GameDataMgr.Instance.mainPlayer.gameEquipData.AddEquip(EquipData.valueof(item.id, item.itemId, item.stage, item.level, -1, new List<PB.GemPunch>()));
            }
            else if (item.type == (int)PB.itemType.MONSTER)
            {

            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnConsume(ProtocolMessage msg)
    {
        PB.HSConsumeInfo reward = msg.GetProtocolBody<PB.HSConsumeInfo>();
        foreach (PB.ConsumeItem item in reward.consumeItems)
        {
            if (item.type == (int)PB.itemType.PLAYER_ATTR)
            {
                if ((int)PB.changeType.CHANGE_COIN == int.Parse(item.itemId))
                {
                    GameDataMgr.Instance.mainPlayer.coin -= item.count;
                    GameEventMgr.Instance.FireEvent<long>(GameEventList.CoinChanged, mainPlayer.coin);
                }
                else if ((int)PB.changeType.CHANGE_GOLD == int.Parse(item.itemId))
                {
                    GameDataMgr.Instance.mainPlayer.gold -= item.count;
					GameEventMgr.Instance.FireEvent<int>(GameEventList.ZuanshiChanged,mainPlayer.gold);
                }

            }
            if (item.type == (int)PB.itemType.ITEM)
            {
                GameDataMgr.Instance.mainPlayer.gameItemData.RemoveItem(item.itemId, item.count);
            }
            else if (item.type == (int)PB.itemType.EQUIP)
            {
                GameDataMgr.Instance.mainPlayer.gameEquipData.RemoveEquip(item.id);
            }
            else if (item.type == (int)PB.itemType.MONSTER)
            {
                GameDataMgr.Instance.mainPlayer.RemoveUnit((int)item.id);
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
