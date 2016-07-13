using UnityEngine;
using System.Collections;



public class StatisticsDataMgr : MonoBehaviour {
    
    private  int SkillPoints = 0;
    private  int skillTimeBegin = 0;

    private  int systemTime = 0;
    private int timeDiffer = 0;

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
        DontDestroyOnLoad(gameObject);
    }

    void Destroy()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSync);       
    }

    void OnStatisticsInfoSync(ProtocolMessage message)
    {
        PB.HSStatisticsInfoSync staticsticsData = message.GetProtocolBody<PB.HSStatisticsInfoSync>();
        SkillPoints = staticsticsData.skillPoint;
        skillTimeBegin = staticsticsData.skillPointTimeStamp;
        UpdateServerTime(staticsticsData.timeStamp);
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
