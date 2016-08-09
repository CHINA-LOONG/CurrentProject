using UnityEngine;
using System.Collections;



public class StatisticsDataMgr : MonoBehaviour {
    
    private  int SkillPoints = 0;
    private  int skillTimeBegin = 0;

    private  int systemTime = 0;
    private int timeDiffer = 0;

	public	int	TimeDiffer
	{
		get
		{
			return timeDiffer;
		}
	}

    //多倍经验剩余次数
    private PB.HSSyncExpLeftTimes expLeftTime;
    public PB.HSSyncExpLeftTimes ExpLeftTimeAttr
	{
		get
		{
			return	expLeftTime;
		}
	}
	public	int	gold2coinExchargeTimes = 0;

    static StatisticsDataMgr mInst = null;
    public static StatisticsDataMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("StatisticsDataMgr");
                mInst = go.AddComponent<StatisticsDataMgr>();
            }
            return mInst;
        }
    }

    public void Init()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SYNC_EXP_LEFT_TIMES_S.GetHashCode().ToString(), OnExpLeftTimesSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SYNC_SHOP_REFRESH_S.GetHashCode().ToString(), OnShopNeedRefreshSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SYNC_DAILY_REFRESH_S.GetHashCode().ToString(), OnDailyRefreshSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SYNC_MONTHLY_REFRESH_S.GetHashCode().ToString(), OnMonthlyRefreshSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.gm.GM_INSTANCE_PUSH_S.GetHashCode().ToString(), OnGMInstanceStateChange);
        DontDestroyOnLoad(gameObject);
    }

    void Destroy()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SYNC_EXP_LEFT_TIMES_S.GetHashCode().ToString(), OnExpLeftTimesSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SYNC_SHOP_REFRESH_S.GetHashCode().ToString(), OnShopNeedRefreshSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SYNC_DAILY_REFRESH_S.GetHashCode().ToString(), OnDailyRefreshSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SYNC_MONTHLY_REFRESH_S.GetHashCode().ToString(), OnMonthlyRefreshSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.gm.GM_INSTANCE_PUSH_S.GetHashCode().ToString(), OnGMInstanceStateChange);
    }

    void OnStatisticsInfoSync(ProtocolMessage message)
    {
        PB.HSStatisticsInfoSync staticsticsData = message.GetProtocolBody<PB.HSStatisticsInfoSync>();
        SkillPoints = staticsticsData.skillPoint;
        skillTimeBegin = staticsticsData.skillPointBeginTime;
		GameDataMgr.Instance.UserDataAttr.orderServerKey =  staticsticsData.orderServerKey;
		GameDataMgr.Instance.ShopDataMgrAttr.monthCardLeft = staticsticsData.monthCardLeft;
		GameDataMgr.Instance.ShopDataMgrAttr.listRechageState = staticsticsData.rechargeState;
		expLeftTime = staticsticsData.expLeftTimes;
		gold2coinExchargeTimes = staticsticsData.gold2CoinTimes;
        GameDataMgr.Instance.PlayerDataAttr.InitCollectPet(staticsticsData.monsterCollect);

        GameDataMgr.Instance.PlayerDataAttr.gameItemData.SynItemState(staticsticsData.itemState);

        UpdateServerTime(staticsticsData.timeStamp);

        GameDataMgr.Instance.PlayerDataAttr.UpdateHuoli(staticsticsData.fatigue, staticsticsData.fatigueBeginTime);

        if (staticsticsData.instanceState != null)
        {
            InstanceMapService.Instance.RefreshInstanceMap(staticsticsData.instanceState);
        }

        InstanceMapService.Instance.instanceResetTimes = staticsticsData.instanceResetCount;

        if (staticsticsData.chapterState != null)
        {
            InstanceMapService.Instance.chapterState = staticsticsData.chapterState;
        }

        GameDataMgr.Instance.SyncHoleData(staticsticsData.holeState);
        GameDataMgr.Instance.SyncTowerData(staticsticsData.towerState);
    }

	void OnExpLeftTimesSync(ProtocolMessage message)
	{
        PB.HSSyncExpLeftTimes msgBody = message.GetProtocolBody<PB.HSSyncExpLeftTimes>();
		expLeftTime = msgBody;
	}

	void OnShopNeedRefreshSync(ProtocolMessage message)
	{
        PB.HSSyncShopRefresh msgBody = message.GetProtocolBody<PB.HSSyncShopRefresh>();
		GameDataMgr.Instance.ShopDataMgrAttr.RefreshShopWithFree (msgBody.shopType, false);
	}
    
    void OnDailyRefreshSync(ProtocolMessage message)
    {
        PB.HSSyncDailyRefresh msgBody = message.GetProtocolBody<PB.HSSyncDailyRefresh>();

        //清理物品使用次数
        GameDataMgr.Instance.PlayerDataAttr.gameItemData.SynItemState(null);
        GameEventMgr.Instance.FireEvent(GameEventList.RefreshUseHuoliWithZeroClock);

        //清理副本挑战次数
        InstanceMapService.Instance.ResetCountDaily();
        //清理副本 重置次数
        InstanceMapService.Instance.instanceResetTimes = 0;
        //清理通天塔洞次数
        GameDataMgr.Instance.mHoleInvalidate = true;
        GameDataMgr.Instance.SyncHoleData(msgBody.holeState);
    }
    void OnMonthlyRefreshSync(ProtocolMessage message)
    {
        //PB.HSSyncMonthlyRefresh msgBody = message.GetProtocolBody<PB.HSSyncMonthlyRefresh>();
        GameDataMgr.Instance.mTowerInvalidate = true;
        GameDataMgr.Instance.mTowerRefreshed = true;
        GameDataMgr.Instance.SyncTowerData(null);
    }

    void OnGMInstanceStateChange(ProtocolMessage messge)
    {
        PB.GMInstancePush gmInstance = messge.GetProtocolBody<PB.GMInstancePush>();
        if (null == gmInstance)
            return;
        if (gmInstance.instanceState != null)
        {
            InstanceMapService.Instance.RefreshInstanceMap(gmInstance.instanceState);
        }

        if (gmInstance.chapterState != null)
        {
            InstanceMapService.Instance.chapterState = gmInstance.chapterState;
        }

    }

    public void ResetSkillPointState(int currentPoint, int beginTime)
    {
        skillTimeBegin = beginTime;
        SkillPoints = currentPoint;
    }

    public int GetSkillPoint()
    {
        int clientTime = GameTimeMgr.Instance.TimeStamp();
        int currentPoint = (GameTimeMgr.Instance.TimeStamp() - timeDiffer - skillTimeBegin) / GameConfig.SkillPointTime + SkillPoints;
        return currentPoint > GameConfig.MaxSkillPoint ? GameConfig.MaxSkillPoint : currentPoint;  
    }

    public void SetSkillPoint(int point)
    {
        SkillPoints = point;
    }

    public int GetSkillPointLeftTime()
    {
        return GameConfig.SkillPointTime - (GameTimeMgr.Instance.TimeStamp() - timeDiffer - skillTimeBegin) % GameConfig.SkillPointTime;
    }

    public bool isMaxPoint()
    {
        return GetSkillPoint() >= GameConfig.MaxSkillPoint;
    }

    public void BeginHeartBreak()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.sys.HEART_BEAT.GetHashCode().ToString(), OnHeartBreakResponse);
        StartCoroutine(SendHeartBreak());
    }

    IEnumerator SendHeartBreak() 
    {
       while(true)
       {
        PB.HSHeartBeat heartBeat = new PB.HSHeartBeat();
        GameApp.Instance.netManager.SendMessage(PB.sys.HEART_BEAT.GetHashCode(), heartBeat, false);
        yield return new WaitForSeconds(GameConfig.HeartBreakInterval);
       }
    }

    void OnHeartBreakResponse(ProtocolMessage message)
    {
        PB.HSHeartBeat heartBreak = message.GetProtocolBody<PB.HSHeartBeat>();
        UpdateServerTime(heartBreak.timeStamp);
    }

    void UpdateServerTime(int serverTime)
    {
        int currentTimeDiff = (int)GameTimeMgr.Instance.TimeStamp() - serverTime;
        if (currentTimeDiff - timeDiffer < -GameConfig.TimSynInterval || currentTimeDiff - timeDiffer > GameConfig.TimSynInterval)
        {
            timeDiffer = currentTimeDiff;
        }
    }
}
