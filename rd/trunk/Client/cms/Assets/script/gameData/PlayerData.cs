using UnityEngine;
using System.Collections.Generic;
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
    //上阵宠物
    public List<int> mainUnitID = new List<int>();
    public List<BattleObject> mainUnitList = new List<BattleObject>();
    public Dictionary<int, PbUnit> unitPbList = new Dictionary<int, PbUnit>();

    //---------------------------------------------------------------------------------------------
    public void InitMainUnitList()
    {
        mainUnitList.Clear();
        foreach (int unitID in mainUnitID)
        {
            PbUnit pb = null;
            if (unitPbList.TryGetValue(unitID, out pb))
            {
                GameUnit curUnit = GameUnit.FromPb(pb, true);
                //TODO: use event to create battleobject
                //Vector3 testPos = new Vector3(-0.5f + (i % 3) * 0.5f, 0.5f - (int)(i / 3) * 0.5f);
                BattleObject bo = ObjectDataMgr.Instance.CreateBattleObject(curUnit, null, Vector3.zero, Quaternion.identity);
                bo.gameObject.SetActive(false);
                mainUnitList.Add(bo);
            }
            else
            {
                Logger.LogErrorFormat("can not find monster id={0}", unitID);
            }
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
        //for (int i = 0; i < 6; i++)
        //{
        //    PbUnit pbUnit = new PbUnit();
        //    pbUnit.camp = UnitCamp.Player;
        //    pbUnit.guid = 10 + i;
        //    pbUnit.level = 50;
        //    if (i == 0)
        //        pbUnit.id = "Unit_Demo_qingniao";
        //    if (i == 1)
        //        pbUnit.id = "Unit_Demo_zhuyan";
        //    if (i == 2)
        //        pbUnit.id = "Unit_Demo_zhuyan";
        //    if (i == 3)
        //        pbUnit.id = "Unit_Demo_ershu";
        //    if (i == 4)
        //        pbUnit.id = "Unit_Demo_qingniao";
        //    if (i == 5)
        //        pbUnit.id = "Unit_Demo_ershu";
        //    pbUnit.slot = i;
        //    if (i > 2)
        //        pbUnit.slot = BattleConst.offsiteSlot;
        //    pbUnit.character = 4;
        //    pbUnit.lazy = 4;

        //    mainUnitPb.Add(pbUnit);
        //}
        //PB

    }
    //---------------------------------------------------------------------------------------------
}
