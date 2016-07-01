#include "Clock.h"
#include <ctime>
#include <stdio.h>
#include "common/string-util.h"
Clock* Clock::m_pInst = NULL;

#ifdef _WIN32

time_t
Clock::getCurrentSystemTime()
{
    FILETIME filetime;
    GetSystemTimeAsFileTime(&filetime);
    //return ((time_t(filetime.dwHighDateTime)<<32)+filetime.dwLowDateTime)/10000;
    return ((time_t(filetime.dwHighDateTime - 0x019DB1DE) << 32) + filetime.dwLowDateTime - 0xD53E8000) / 10000;
}
#else

time_t
Clock::getCurrentSystemTime()
{
    timeval systime;
    gettimeofday(&systime, NULL);
    return (time_t(systime.tv_sec)*1000 + systime.tv_usec / 1000);
}
#endif

Clock::Clock(void) : ratio_(1.0f)
{
    last_refresh_sys_ = getCurrentSystemTime();
    now_ = last_refresh_sys_;
    m_llDebugTimeFix = 0;
}

Clock::~Clock(void)
{
}

void
Clock::refresh()
{
    time_t systime = getCurrentSystemTime();
    time_t time_incr = (time_t) ((systime - last_refresh_sys_) * ratio_);
    now_ += time_incr;
    last_refresh_sys_ = systime;
}

void
Clock::setNow(time_t now)
{
    now_ = now;
    time_t systime = getCurrentSystemTime();
    last_refresh_sys_ = systime;
}

void
Clock::sleep(Time_val &timeout)
{
#ifdef _WIN32
    int dummyfd = socket(PF_INET, SOCK_STREAM, 0);
    fd_set dummyread;
    FD_ZERO(&dummyread);
    FD_SET(dummyfd, &dummyread);
    select(dummyfd + 1, &dummyread, NULL, NULL, &timeout);
#else
    int r;
    if ((r = select(1, NULL, NULL, NULL, &timeout)) < 0)
    {
        perror("select");
        return;
    }
#endif
}

/**
 * 得到当前的日期数. 适用于判断跨天的操作
 * @param hour
 * @return
 */
int64
GetDayByHour_timezone8(int64 hour)
{
    return (hour + 8) / 24;
}

bool
Clock::IsToday(int64 llSecond)
{
    int64 llHour = llSecond / 3600;
    int64 day1   = GetDayByHour_timezone8(llHour);
    int64 day2   = GetDayByHour_timezone8(GetHour());
    return day1 == day2;
}

bool
Clock::IsWeekend(int64 llDay)
{
    int64 tmp = llDay % 7;
    if ( (tmp == 2) || (tmp == 3) )
        return true;
    else
        return false;
}

int64
Clock::GetWeek(int64 llDay)
{
    return (llDay + 3) / 7;
}

int64
Clock::GetDayInWeek(int64 llDay)
{
    return (llDay + 3) % 7;
}

int64
Clock::GetDayByWeekDay(int64 llWeek, int diw)
{
    return  llWeek * 7 + diw - 3;
}

int
Clock::GetIntervalDays( int64 llSecond )
{
    int64 temp = llSecond;
    bool minus_flag = GetSecond() < llSecond ? true : false;
    int days(0);
    if (llSecond < 0)
    {
        return days;
    }

    while (IsToday(temp) == false)
    {
        if (minus_flag)
        {
            days--;
            temp -= 3600 * 24;
        }
        else
        {
            days++;
            temp += 3600 * 24;
        }
    }
    return days;
}

#ifdef _WIN32

time_t
Clock::getUTime()
{
    return getCurrentSystemTime();
}

#else

time_t
Clock::getUTime()
{
    timeval systime;
    gettimeofday(&systime, NULL);
    return (time_t(systime.tv_sec)*1000 * 1000 + systime.tv_usec);
}
#endif

int
Clock::Get8Date(int64 llday)
{
    int64 llSecond = llday * llDaySecond;
    struct tm  tm_val;

    time_t timep = llSecond;
    tm_val = *localtime ( &timep );

    int v = (tm_val.tm_year + 1900)*10000 + (1 + tm_val.tm_mon)*100 + tm_val.tm_mday;
    return v;
}

bool
Clock::NowAfter(const std::string& date)
{
    return GetSecond() >= GetTimeVal(date);
}

