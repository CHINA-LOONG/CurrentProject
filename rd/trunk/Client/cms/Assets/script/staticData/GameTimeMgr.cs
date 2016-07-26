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
        return (int)(Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
    }


    public TimeStaticData GetTime()
    {
       // TimeStaticData time = new TimeStaticData();
		timeNow.hour = GetHour();
		timeNow.minute = GetMinute();
		return timeNow;
    }

	public	TimeStaticData GetServerTime()
	{
		int serverTimeStamp = TimeStamp() - StatisticsDataMgr.Instance.TimeDiffer;
		DateTime serverDateTime = GetTime (serverTimeStamp);
		serverTime.hour = serverDateTime.Hour;
		serverTime.minute = serverDateTime.Minute;

		return serverTime;
	}

    public int GetYear()
    {
        return Now.Year;
    }
    public int GetMonth()
    {
        return Now.Month;
    }
    public int GetDay()
    {
        return Now.Day;
    }
    public int GetHour()
    {
        return Now.Hour;
    }
    public int GetMinute()
    {
        return Now.Minute;
    }
    public int GetSecond()
    {
        return Now.Second;
    }

    public static DateTime GetTime(long timestamp)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return startTime.AddSeconds(timestamp);
    }

}
