using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDataMgr : MonoBehaviour 
{
    private Dictionary<int, PlayerData> playerList = new Dictionary<int,PlayerData>();
    private Dictionary<int, BattleObject> unitList = new Dictionary<int, BattleObject>();

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
            AddPlayerData(mainPlayer);
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
    //---------------------------------------------------------------------------------------------
	public void Init()
	{
        DontDestroyOnLoad(gameObject);

		GameObject userDataGo = new GameObject("UserData");
		userDataGo.transform.SetParent (transform);
		userData = userDataGo.AddComponent<UserData>();
		userData.Init ();

        BindListener();
	}
    //---------------------------------------------------------------------------------------------
    void OnDestroy()
    {
        UnBindListener();
    }
    //---------------------------------------------------------------------------------------------
    public void AddPlayerData(PlayerData data)
    {
        if (playerList.ContainsKey(data.playerId) == true)
        {
            return;
        }

        playerList.Add(data.playerId, data);
        data.InitMainUnitList();

        var itor = data.mainUnitList.GetEnumerator();
        while (itor.MoveNext())
        {
            if (unitList.ContainsKey(itor.Current.guid) == false)
            {
                unitList.Add(itor.Current.guid, itor.Current);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void RemovePlayerData(int guid)
    {
        PlayerData data;
        if (playerList.TryGetValue(guid, out data) == false)
        {
            return;
        }

        var itor = data.mainUnitList.GetEnumerator();
        while (itor.MoveNext())
        {
            unitList.Remove(itor.Current.guid);
        }
    }
    //---------------------------------------------------------------------------------------------

    public Dictionary<int, PlayerData> GetPlayers()
    {
        return playerList;
    }
    //---------------------------------------------------------------------------------------------
    public Dictionary<int, BattleObject> GetUnits()
    {
        return unitList;
    }
    //---------------------------------------------------------------------------------------------
	void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_INFO_SYNC_S.GetHashCode().ToString(), OnPlayerInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_INFO_SYNC_S.GetHashCode().ToString(), OnMonsterInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_INFO_SYNC_S.GetHashCode().ToString(), OnItemInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INFO_SYNC_S.GetHashCode().ToString(), OnEquipInfoSync);
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
		//GameEventMgr.Instance.RemoveListener<Coin>(GameEventList.EatCoin, OnEatCoin);
    }
    //---------------------------------------------------------------------------------------------
    void OnPlayerInfoSync(ProtocolMessage msg)
    {
        if (mainPlayer == null)
        {
            mainPlayer = new PlayerData();
        }
        mainPlayer.SyncPlayerInof(msg);
    }
    //---------------------------------------------------------------------------------------------
    void OnStatisticsInfoSync(ProtocolMessage msg)
    {
        PB.HSStatisticsInfoSync playerSync = msg.GetProtocolBody<PB.HSStatisticsInfoSync>();

    }
    //---------------------------------------------------------------------------------------------
    void OnMonsterInfoSync(ProtocolMessage msg)
    {
        PB.HSMonsterInfoSync monsterSync = msg.GetProtocolBody<PB.HSMonsterInfoSync>();
    }
    //---------------------------------------------------------------------------------------------
    void OnItemInfoSync(ProtocolMessage msg)
    {
        PB.HSItemInfoSync itemSync = msg.GetProtocolBody<PB.HSItemInfoSync>();
    }
    //---------------------------------------------------------------------------------------------
    void OnEquipInfoSync(ProtocolMessage msg)
    {
        PB.HSEquipInfoSync equpSync = msg.GetProtocolBody<PB.HSEquipInfoSync>();
    }
    //---------------------------------------------------------------------------------------------

}
