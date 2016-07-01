#pragma once
#ifndef CLOCK_CLASS
#define CLOCK_CLASS
#ifdef _WIN32
#include <WinSock2.h>
#include <Windows.h>
#else
#include <sys/time.h>
#include <stdlib.h>
#endif

#ifdef _WIN32
typedef long long time_t;
typedef TIMEVAL Time_val;
#else
typedef timeval Time_val;
#endif
#include "common/const_def.h"
#include "common/string-util.h"

class Clock
{
public:
    static const int nHourFix = 8;

    Clock(void);
    ~Clock(void);

    void refresh();

    inline time_t now()
    {
        return now_;
    }

    inline time_t time()
    {
        return (time_t) (last_refresh_sys_ / 1000000);
    }

    inline time_t refreshNow()
    {
        refresh();
        return now_;
    }

    float ratio()
    {
        return ratio_;
    }

    void setRatio(float ratio)
    {
        ratio_ = ratio;
    }
    void setNow(time_t now);

    // The unit of time is milliseconds
    static time_t getCurrentSystemTime();
    static void sleep(Time_val &timeout);
    static Clock* GetInst();
    static Clock* CreatInst();
    static time_t getUTime();

public:
    static int64 SetDebugFix(int64 llDebugSecond);
    static int64 GetSecond();
    static int64 GetMinute();
    static int64 GetHour();
    static int64 GetDay();
    static int64 GetWeek(int64 llDay);
    static int64 GetDayInWeek(int64 llDay);
    static int64 GetDayByWeekDay(int64 llWeek, int diw);

    static int64 GetDayWithHour(int nHour);
    static bool  IsToday(int64 llSecond);
    static bool  IsWeekend(int64 llDay);
    static int	 GetIntervalDays(int64 llSecond);
    static int64 GetHourForHourInDay(int nHour);
    static bool  InTimeRange(int64 llSecondLeft, int64 llSecondRight, int64 llTestValue);
    static int64 CalcDayByHour(int nHour);
    static int64 GetDayBySecond(int64 llSecond);
    static int   Get8Date(int64 llday);
    static int64 GetSecondInToday();
    static bool  NowAfter(const std::string& date);
    //static int64 GetSecondByData(int year,int mon,int day,int hour,int min,int sec);
protected:
    // The unit of time is milliseconds
    time_t now_;
    time_t last_refresh_sys_;

    float ratio_;
    int64 m_llDebugTimeFix;
    static Clock* m_pInst;
public:
    const static int64  llDaySecond  = 60 * 60 * 24;
    const static int64  llHourSecnod = 60 * 60;
} ;

inline Clock* Clock::GetInst()
{
    return m_pInst;
}

inline Clock* Clock::CreatInst()
{
    if (m_pInst == NULL)
    {
        m_pInst = new Clock();
        return m_pInst;
    }
    return m_pInst;
}

inline int64 Clock::GetSecond()
{
    return (GetInst()->now_ + GetInst()->m_llDebugTimeFix * 1000 ) / 1000;
}

inline int64 Clock::GetMinute()
{
    return (GetInst()->now_ + GetInst()->m_llDebugTimeFix * 1000 ) / 1000 / 60;
}

inline int64 Clock::GetHour()
{
    return (GetInst()->now_ + GetInst()->m_llDebugTimeFix * 1000 ) / 1000 / 60 / 60;
}

inline int64 Clock::GetDay()
{
    return (GetHour() + nHourFix) / 24;
}

inline int64 Clock::GetDayWithHour(int nHour)
{
    return (nHourFix + nHour) / 24;
}

inline int64 Clock::SetDebugFix(int64 llDebugSecond)
{
    GetInst()->m_llDebugTimeFix = llDebugSecond;
    return llDebugSecond;
}

inline int64 Clock::GetHourForHourInDay(int nHour)
{
    int64 nDay =  GetDay();
    return nDay * 24 - nHourFix + nHour;
}

inline bool Clock::InTimeRange(int64 t1, int64 t2, int64 value)
{
    return t1 <= value && value <= t2;
}

inline int64 Clock::CalcDayByHour(int nHour)
{
    return (nHourFix + nHour) / 24;
}

inline int64 Clock::GetDayBySecond(int64 llSecond)
{
    return Clock::GetDayWithHour( llSecond / 3600);
}

inline int64 Clock::GetSecondInToday()
{
    return  GetSecond()- ((GetDay()) * llDaySecond - nHourFix * llHourSecnod);
}
#endif
