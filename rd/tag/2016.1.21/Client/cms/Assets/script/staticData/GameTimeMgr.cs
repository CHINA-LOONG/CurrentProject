using UnityEngine;
using System.Collections;
using System;

public class GameTimeMgr
{
    static GameTimeMgr mInst = null;
    public static GameTimeMgr Instance
    {
        get
        {
            if (mInst==null)
            {
                mInst = new GameTimeMgr();
            }
            return mInst;
        }
    }

	private TimeStaticData timeNow = new TimeStaticData();
	private	TimeStaticData serverTime = new TimeStaticData();

    public DateTime Now
    {
        get { return DateTime.Now; }
    }

    public int TimeStamp()
    { 
        return (int)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
    }
    public double TimeStampAsMilliseconds()
    {
        return (double)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds;
    }

    

	public	TimeStaticData GetServerTime()
	{
		DateTime serverDateTime = GetServerDateTime();
		serverTime.hour = serverDateTime.Hour;
		serverTime.minute = serverDateTime.Minute;
        serverTime.month = serverDateTime.Month;

		return serverTime;
	}
    public DateTime GetServerDateTime()
    {
        int serverTimeStamp = GetServerTimeStamp();
        DateTime serverDateTime = GetTime(serverTimeStamp);
        return serverDateTime;
    }
    public  int GetServerTimeStamp()
    {
        int serverTimeStamp = TimeStamp() - StatisticsDataMgr.Instance.TimeDiffer;
        return serverTimeStamp;
    }

    public static int GetTimeStamp(DateTime time)
    {
        return (int)(time - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
    }
    public static DateTime GetTime(long timestamp)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return startTime.AddSeconds(timestamp);
    }

}
