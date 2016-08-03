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

	private	PB.HSStatisticsExpLeftTimeSync	expLeftTime;
	public	PB.HSStatisticsExpLeftTimeSync	ExpLeftTimeAttr
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
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.STATISTICS_EXP_LEFT_TIMES.GetHashCode ().ToString (), OnExpLeftTimesSync);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.STATISTICS_SHOP_REFRESH.GetHashCode ().ToString (), OnShopNeedRefreshSync);
        DontDestroyOnLoad(gameObject);
    }

    void Destroy()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSync);  
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.STATISTICS_EXP_LEFT_TIMES.GetHashCode ().ToString (), OnExpLeftTimesSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.STATISTICS_SHOP_REFRESH.GetHashCode().ToString(), OnShopNeedRefreshSync);
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

        UpdateServerTime(staticsticsData.timeStamp);

        GameDataMgr.Instance.PlayerDataAttr.UpdateHuoli(staticsticsData.fatigue, staticsticsData.fatigueBeginTime);
    }

	void	OnExpLeftTimesSync(ProtocolMessage message)
	{
		PB.HSStatisticsExpLeftTimeSync msgBody = message.GetProtocolBody<PB.HSStatisticsExpLeftTimeSync> ();
		expLeftTime = msgBody;
	}

	void	 OnShopNeedRefreshSync(ProtocolMessage message)
	{
		PB.HSStatisticsShopRefresh msgBody = message.GetProtocolBody<PB.HSStatisticsShopRefresh> ();
		GameDataMgr.Instance.ShopDataMgrAttr.RefreshShopWithFree (msgBody.shopType, false);
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
