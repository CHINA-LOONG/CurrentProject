#pragma once

#include <vector>
//#include "EventHandler.h"
#include "DataHandler.h"
#include "common/json-util.h"
#include "common/const_def.h"
#include "config/ServerConfig.h"
#include <google/protobuf/repeated_field.h>
#ifndef _WIN32
#include <signal.h>
#include <log4cxx/logger.h>
#else
#include "common/Logger_win.h"
#endif
#include "net/NetHandler.h"

#include <pthread.h>
#include "common/staticfile.h"
using namespace std;

class Event;
class EventQueue;
class Clock;
class Statistics;
class Profile;
class Counter;
class StaticFile;
class User;
typedef void (*ProcessRoutine)(Event*);
typedef void (*GatewayProcessRoutine)(Event*, User*);
typedef void (*runFunc)(void *, void *) ;
extern ServerConfig serverConfig;

class EventHandler
{
public:
    EventHandler(EventQueue* eq, DataHandler* dh, NetHandler* nh, int nid);
    ~EventHandler(void);
    bool start(time_t ltUpdateTime = 0);
    void postBackEvent(Event *e);
    void registHandler(int cmd, ProcessRoutine handler,bool bAutoRange = false);
    void clearIgnore();
    void addIgnore( int _cmd );
    static void urgentSaveHandler(int signo);
public:

    void setUrgentSaving(bool flag)
    {
        urgent_saving_flag_ = flag;
    }

    void setProcessRunFunc(runFunc func)
    {
        m_runFunc = func;
    }

    inline time_t getReversion()
    {
        return reversion_;
    }

    inline NetHandler* getNetHandler()
    {
        return nh_;
    }

    inline DataHandler* getDataHandler()
    {
        return dh_;
    }

    inline Clock* getClock()
    {
        return clock_;
    }

    inline GameWorkingStatus getWorkingStatus()
    {
        return status_;
    }

    inline void setWorkingStatus(GameWorkingStatus status)
    {
        status_ = status;
    }

    inline Profile* getEventProfile(void)
    {
        return event_pf_;
    }

    inline Statistics* getStatistics()
    {
        return stat_;
    }
protected:

    inline void acquireEventLock()
    {
        pthread_mutex_lock(eq_mutex_);
    }

    inline void releaseEventLock()
    {
        pthread_mutex_unlock(eq_mutex_);
    }

    inline void acquireDataLock()
    {
        pthread_mutex_lock(data_mutex_);
    }

    inline void releaseDataLock()
    {
        pthread_mutex_unlock(data_mutex_);
    }
private:
    void dispatchEvent(Event *e);
    void run();
    static void* thread_fun(void* args);
    void tick();
    void safeQuit();
public:
    static const int maxProcessRoutines = 3096;
private:
#ifdef _WIN32
    DWORD thread_id_;
#endif
    pthread_t tid_;
    pthread_mutex_t *eq_mutex_;
    pthread_mutex_t *data_mutex_;
    EventQueue *eq_;
    Clock *clock_;
    log4cxx::LoggerPtr logger_;
    Statistics * stat_;
    Counter * counter_;
    bool urgent_saving_flag_;

    enum GameWorkingStatus status_;
    int nid_;
    Profile *event_pf_;
    time_t last_check_wait_event_;
    vector<Event*> wait_event_list_;
    time_t m_lLastCheckTime;
    time_t m_ltUpdateTime;
    ProcessRoutine processRoutines_[maxProcessRoutines];
    bool			m_processIgnore[maxProcessRoutines];
    time_t reversion_;
    NetHandler* nh_;
    DataHandler* dh_;
    runFunc m_runFunc;
public:
    StaticFile m_xEventCounter;
    int        m_nEventCnt;
} ;
