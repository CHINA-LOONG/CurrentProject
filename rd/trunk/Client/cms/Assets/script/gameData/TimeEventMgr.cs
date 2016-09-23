using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void perCountDownTime(int remainTime);
//public delegate void countDownFinish();
public class TimeEventWrap
{
    public int endTime;
    public List<perCountDownTime> updateList=new List<perCountDownTime>();
    public Action finishEvent;
    public TimeEventWrap(int time,Action finish=null)
    {
        this.endTime = time;
        this.finishEvent = finish;
        TimeEventMgr.Instance.add(this);
    }
    public void refresh(int realtime)
    {
        if (updateList.Count> 0)
        {
            update(endTime - realtime);
        }
        if (endTime - realtime <= 0)
        {
            finish();
        }
    }

    void update(int time)
    {
        perCountDownTime subPerCount = null;
        for (int i=0;i<updateList.Count;++i)
        {
            subPerCount = updateList[i];
            subPerCount(time);
        }
    }
    void finish()
    {
        if (finishEvent!=null)
        {
            finishEvent();
        }
        TimeEventMgr.Instance.remove(this);
    }

    public void AddUpdateEvent(perCountDownTime update)
    {
        if (!updateList.Contains(update))
        {
            updateList.Add(update);
        }
        else
        {
            Logger.LogError("已经包含这个更新事件");
        }
    }
    public void RemoveUpdateEvent(perCountDownTime update)
    {
        if (updateList.Contains(update))
        {
            updateList.Remove(update);
        }
        else
        {
            Logger.LogError("不包含这个更新事件");
        }
    }
    public void RemoveTimeEvent()
    {
        TimeEventMgr.Instance.remove(this);
    }
    
}

public class TimeEventMgr : MonoBehaviour
{
    static TimeEventMgr mInst = null;
    public static TimeEventMgr Instance
    {
        get
        {
            if (mInst==null)
            {
                GameObject go = new GameObject();
                mInst = go.AddComponent<TimeEventMgr>();
                go.name = mInst.GetType().ToString();
                DontDestroyOnLoad(go);
            }
            return mInst;
        }
    }

    private List<TimeEventWrap> times = new List<TimeEventWrap>();
    private float startTime = 0f;

    public void add(TimeEventWrap time)
    {
        if (!times.Contains(time))
        {
            times.Add(time);
        }
    }

    public void remove(TimeEventWrap time)
    {
        if (times.Contains(time))
        {
            times.Remove(time);
        }
    }

    void FixedUpdate()
    {
        if (Time.realtimeSinceStartup-startTime>=1.0f)
        {
            startTime = Time.realtimeSinceStartup;
            int count = times.Count;
            try
            {
                for (int i = (count - 1); i >= 0; i--)
                {
                    times[i].refresh(GameTimeMgr.Instance.GetServerTimeStamp());
                }
            }
            catch (Exception)
            {
                Logger.LogError("...............");
            }
        }
    }

}
