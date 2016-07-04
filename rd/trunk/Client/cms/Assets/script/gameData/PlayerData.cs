using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	public	int	playerId = 0;
	public	string nickName;
	public	int career;
	public	int level;
	public	int	exp;
	public	int	gold;
	public	long coin;
	public	int	gender;
	public	int	eye;
	public	int	hair;
	public	int	hairColor;
	public	int	recharget;
	public	int	vipLevel;


    //主角装备加成
    public int equipHealth;
    public int equipStrength;
    public int equipIntelligence;
    public int equipSpeed;
    public int equipDefense;
    public int equipEndurance;
    public float criticalRatio;
    public float hitRatio;

    public float equipHealthRatio;
    public float equipStrengthRatio;
    public float equipIntelligenceRatio;
    public float equipSpeedRatio;
    public float equipDefenseRatio;
    public float equipEnduranceRatio;
    //五行加成
    public float goldDamageRatio;
    public float woodDamageRatio;
    public float waterDamageRatio;
    public float fireDamageRatio;
    public float earthDamageRatio;

	public void Init()
	{
		BindListener ();
	}

	void OnDestroy()
	{
		UnBindListener ();
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.PLAYER_INFO_SYNC_S.GetHashCode().ToString (), OnPlayerInfoSync);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.STATISTICS_INFO_SYNC_S.GetHashCode ().ToString (), OnStatisticsInfoSync);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.MONSTER_INFO_SYNC_S.GetHashCode().ToString (), OnMonsterInfoSync);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.ITEM_INFO_SYNC_S.GetHashCode ().ToString (), OnItemInfoSync);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.EQUIP_INFO_SYNC_S.GetHashCode().ToString (), OnEquipInfoSync);

	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.PLAYER_INFO_SYNC_S.GetHashCode().ToString (), OnPlayerInfoSync);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.STATISTICS_INFO_SYNC_S.GetHashCode ().ToString (), OnStatisticsInfoSync);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.MONSTER_INFO_SYNC_S.GetHashCode().ToString (), OnMonsterInfoSync);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.ITEM_INFO_SYNC_S.GetHashCode ().ToString (), OnItemInfoSync);
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.EQUIP_INFO_SYNC_S.GetHashCode().ToString (), OnEquipInfoSync);
	} 

	void OnPlayerInfoSync(ProtocolMessage msg)
	{
		PB.HSPlayerInfoSync playerSync = msg.GetProtocolBody<PB.HSPlayerInfoSync> ();

		PB.PlayerInfo playerInfo = playerSync.info;

		playerId = playerInfo.playerId;
		nickName = playerInfo.nickname;
		career = playerInfo.career;
		{
			level = playerInfo.level;
			GameEventMgr.Instance.FireEvent<int>(GameEventList.LevelChanged,level);
		}
		{
			exp = playerInfo.exp;
		}
		{
			gold = playerInfo.gold;
		}
		{
			coin = playerInfo.coin;
			GameEventMgr.Instance.FireEvent<int>(GameEventList.CoinChanged,level);
		}
		gender = playerInfo.gender;
		eye = playerInfo.eye;
		hair = playerInfo.hair;
		hairColor = playerInfo.hairColor;
		recharget = playerInfo.recharge;
		vipLevel = playerInfo.vipLevel;
	}

	void OnStatisticsInfoSync(ProtocolMessage msg)
	{
		PB.HSStatisticsInfoSync playerSync = msg.GetProtocolBody<PB.HSStatisticsInfoSync> ();

	}

	void OnMonsterInfoSync(ProtocolMessage msg)
	{
		PB.HSMonsterInfoSync monsterSync = msg.GetProtocolBody<PB.HSMonsterInfoSync> ();
	}

	void OnItemInfoSync(ProtocolMessage msg)
	{
		PB.HSItemInfoSync itemSync = msg.GetProtocolBody<PB.HSItemInfoSync> ();
	}

	void OnEquipInfoSync(ProtocolMessage msg)
	{
		PB.HSEquipInfoSync equpSync = msg.GetProtocolBody<PB.HSEquipInfoSync> ();
	}

}
