using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerData 
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
    //上阵宠物
    public List<PbUnit> mainUnitPb = new List<PbUnit>();
    public List<BattleObject> mainUnitList = new List<BattleObject>();

    //---------------------------------------------------------------------------------------------
    public void InitMainUnitList()
    {
        foreach (PbUnit pb in mainUnitPb)
        {
            GameUnit curUnit = GameUnit.FromPb(pb, true);
            //TODO: use event to create battleobject
            //Vector3 testPos = new Vector3(-0.5f + (i % 3) * 0.5f, 0.5f - (int)(i / 3) * 0.5f);
            BattleObject bo = ObjectDataMgr.Instance.CreateBattleObject(curUnit, null, Vector3.zero, Quaternion.identity);
            bo.gameObject.SetActive(false);
            mainUnitList.Add(bo);
        }
    }
    //---------------------------------------------------------------------------------------------
    public List<BattleObject> GetMainUnits()
    {
        return mainUnitList;
    }
    //---------------------------------------------------------------------------------------------
    public void SyncPlayerInof(ProtocolMessage msg)
    {
        //TODO: test only
        //create fake main player
        for (int i = 0; i < 6; i++)
        {
            PbUnit pbUnit = new PbUnit();
            pbUnit.camp = UnitCamp.Player;
            pbUnit.guid = 10 + i;
            pbUnit.level = 50;
            if (i == 0)
                pbUnit.id = "Unit_Demo_qingniao";
            if (i == 1)
                pbUnit.id = "Unit_Demo_zhuyan";
            if (i == 2)
                pbUnit.id = "Unit_Demo_zhuyan";
            if (i == 3)
                pbUnit.id = "Unit_Demo_ershu";
            if (i == 4)
                pbUnit.id = "Unit_Demo_qingniao";
            if (i == 5)
                pbUnit.id = "Unit_Demo_ershu";
            pbUnit.slot = i;
            if (i > 2)
                pbUnit.slot = BattleConst.offsiteSlot;
            pbUnit.character = 4;
            pbUnit.lazy = 4;

            mainUnitPb.Add(pbUnit);
        }

        PB.HSPlayerInfoSync playerSync = msg.GetProtocolBody<PB.HSPlayerInfoSync>();

        PB.PlayerInfo playerInfo = playerSync.info;

        playerId = playerInfo.playerId;
        GameDataMgr.Instance.PlayerDataAttr = this;
        nickName = playerInfo.nickname;
        career = playerInfo.career;
        {
            level = playerInfo.level;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.LevelChanged, level);
        }
        {
            exp = playerInfo.exp;
        }
        {
            gold = playerInfo.gold;
        }
        {
            coin = playerInfo.coin;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.CoinChanged, level);
        }
        gender = playerInfo.gender;
        eye = playerInfo.eye;
        hair = playerInfo.hair;
        hairColor = playerInfo.hairColor;
        recharget = playerInfo.recharge;
        vipLevel = playerInfo.vipLevel;
    }
    //---------------------------------------------------------------------------------------------
}
