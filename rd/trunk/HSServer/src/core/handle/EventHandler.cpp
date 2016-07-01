#define _CRT_SECURE_NO_WARNINGS
#include "EventHandler.h"

#ifdef _WIN32
#include <WinSock2.h>
#include <WS2tcpip.h>
#else
#include <sys/epoll.h>
#include <netdb.h>
#include <sys/time.h>
#include <sys/resource.h>
#include <unistd.h>
#endif

#include <time.h>
#include <pthread.h>
#include "common/counter.h"
#include "common/string-util.h"
#include "common/statistics.h"
#include "common/TicTac.h"
#include "common/Clock.h"
#include "EventQueue.h"
#include "common/SysLog.h"
#include "common/Msg2QQ.h"
#include "EventDefine.h"
EventHandler* global_eh;
extern int g_processingCmd;

EventHandler::EventHandler(EventQueue* eq, DataHandler* dh, NetHandler* nh, int nid)
{
    global_eh = this;
    nid_ = nid;
    logger_ = log4cxx::Logger::getLogger("EventHandler");
    //clock_ = new Clock();
    clock_ = Clock::CreatInst();
    eq_ = eq;
    dh_ = dh;
    nh_ = nh;
    eq_mutex_ = eq->mutex();
    data_mutex_ = dh->mutex();
    m_lLastCheckTime = 0;
    m_ltUpdateTime = 0;
    status_ = NORMAL;
    stat_ = new Statistics();
    event_pf_ = new Profile();
    counter_ = new Counter();
    urgent_saving_flag_ = false;
    reversion_ = 0;
    m_runFunc = NULL;
    for (int i = 0; i < maxProcessRoutines; i++) processRoutines_[i] = NULL;

    m_nEventCnt = 0;

    clearIgnore();
}

EventHandler::~EventHandler(void)
{
    delete clock_;
    delete stat_;
    delete counter_;
    delete event_pf_;
}

bool EventHandler::start(time_t ltUpdateTime /* = 0 */)
{
    m_ltUpdateTime = ltUpdateTime;
#ifdef _WIN32
    //DWORD threadid;
    return CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE) EventHandler::thread_fun,
            (void*) this, 0, &thread_id_) == NULL;
#else
    return pthread_create(&tid_, NULL, &EventHandler::thread_fun, (void*) this) == 0;
#endif
}

void* EventHandler::thread_fun(void *args)
{
#ifndef _WIN32
    sigset_t sig_mask;
    sigemptyset(&sig_mask);
    sigaddset(&sig_mask, SIGINT);
    int err;
    err = pthread_sigmask(SIG_UNBLOCK, &sig_mask, NULL);
    struct sigaction save_action;
    save_action.sa_handler = &EventHandler::urgentSaveHandler;
    save_action.sa_flags = SA_RESTART;
    // turn on/off by the following two lines
    if (err == 0) err = sigaction(SIGINT, &save_action, NULL);
    // end
    if (err == 0)
    {
#endif
        global_eh = static_cast<EventHandler*> (args);
        global_eh->run();
#ifndef _WIN32
        pthread_exit(NULL);
    }
    else
    {
        printf("register signal failed in thread.\n");
    }
#endif
    return NULL;
}

void EventHandler::run()
{
    //status_ = NORMAL;
    //eq_->pushSaveData(nid_, 1, -1, clock_->refreshNow()+30*1000, 0);
    //long interval = gameConfig.gamed_heartBeatInterval(nid_);
    long interval = 20; // 20ms
#ifdef _WIN32
    int dummyfd = socket(PF_INET, SOCK_STREAM, 0);
    fd_set dummyread;
    FD_ZERO(&dummyread);
    TIMEVAL timeout;
#else
    timeval timeout;
#endif
    timeout.tv_sec = 0;
    timeout.tv_usec = interval * 1000;
    vector<Event*> events;
    events.reserve(40960);
    last_check_wait_event_ = clock_->refreshNow();
    TicTac timer;
    TicTac event_watch;
    TicTac timer_tick;
    while (true)
    {
        if (nh_ == NULL)
        {
            usleep(100);
            continue;
        }
        timer.tic();
        time_t now = clock_->refreshNow();
        reversion_ = clock_->now();
        if (dh_ != NULL)
        {
            dh_->setRevision(reversion_);
        }
        CHG_LOG_FILE();

        acquireEventLock();
        for (size_t i = 0; i < events.size(); i++)
        {
            events[i]->Clear();
            eq_->freeEvent(events[i]);
        }
        events.clear();
        if (status_ != NORMAL)
        {
            while (!eq_->isEmpty() && 0 > eq_->topTime())
            {
                events.push_back(eq_->popEvent());
            }
        }
        else
        {
            while (!eq_->isEmpty() && now >= eq_->topTime())
            {
                events.push_back(eq_->popEvent());
            }
        }
        releaseEventLock();
        if (m_runFunc && status_ == NORMAL)
            (*m_runFunc)((void *) this, (void*) eq_);
        if (!urgent_saving_flag_)
        {
            timer_tick.tic();
            tick();
            int timeTick = (int) timer_tick.tac();
            stat_->capture("tick.process-time", (float) timeTick);

            if (last_check_wait_event_ < now - 300 && !wait_event_list_.empty())
            {
                events.insert(events.end(), wait_event_list_.begin(), wait_event_list_.end());
                wait_event_list_.clear();
                last_check_wait_event_ = now;
            }

            for (size_t i = 0; i < events.size(); i++)
            {
                event_watch.tic();
                acquireDataLock();
                dispatchEvent(events[i]);
                m_nEventCnt ++;
                releaseDataLock();
                event_pf_->hitEvent(events[i]->cmd(), (double) event_watch.tac());
            }
            if (m_nEventCnt > 0x0FFFFFFF)
            {
                m_nEventCnt = 0;
            }
        }
        else
        {
            acquireDataLock();
            safeQuit();
            releaseDataLock();
            break;
        }

        stat_->capture("event.num", (float) events.size());
        int elapsed = (int) timer.tac();
        stat_->capture("event.process-time", (float) elapsed);
        //		stat_->capture("user.online", (float)counter_->count("user.online"));

        if (elapsed >= interval)
        {
            if (elapsed >= 10 * interval)
                LOG4CXX_WARN(logger_, "Heart beat overload, used " << elapsed << " ms. (interval:" << interval << " ms)");
            stat_->capture("cpu_overload", (float) (elapsed));
            stat_->capture("cpu_overload_cnt", (float) (counter_->increase("cpu_over_load")));
            continue;
        }
        else
        {
            counter_->clear("cpu_over_load");
        }
        timeout.tv_usec = (interval - elapsed) * 1000;

#ifdef _WIN32

        FD_SET(dummyfd, &dummyread);
        int r;
        if ((r = select(dummyfd + 1, &dummyread, NULL, NULL, &timeout)) < 0)
        {
            int err = WSAGetLastError();
            LOG4CXX_ERROR(logger_, "select failed in thread, return value:" << r << " error no:" << err);
        }
#else
        m_xEventCounter.WriteFirstLine(m_nEventCnt);
        int r;
        if ((r = select(1, NULL, NULL, NULL, &timeout)) < 0)
        {
            LOG4CXX_ERROR(logger_, "select failed in thread, return value:" << r);
        }
#endif
    }
    exit(0);
}

void EventHandler::dispatchEvent(Event *e)
{
    if (e->IsInitialized() && processRoutines_[e->cmd()] != NULL)
    {
        if ( m_processIgnore[e->cmd()] )
            return;

        g_processingCmd = e->cmd();
        (*processRoutines_[e->cmd()])(e);
        g_processingCmd = 0;
    }
}

void EventHandler::clearIgnore()
{
    for (int i = 0; i < maxProcessRoutines; i++)
        m_processIgnore[i] = false;
}

void EventHandler::addIgnore( int _cmd )
{
    if ( (_cmd < 0) || (_cmd >= maxProcessRoutines) )
        return;

    m_processIgnore[_cmd] = true;
}

void EventHandler::urgentSaveHandler(int signo)
{
    printf("Catching signal SIGINT. Terminating...");
    global_eh->setUrgentSaving(true);
}

void EventHandler::postBackEvent(Event* e)
{
    Event* ne = eq_->allocateEvent();
    ne->CopyFrom(*e);
    ne->set_time(clock_->now() + 1000);
    wait_event_list_.push_back(ne);
    ne->set_pushbackcnt(e->pushbackcnt() + 1);
    //LOG4CXX_DEBUG(logger_, "Push back event:"<<ne->cmd());
}

void EventHandler::tick()
{
    if (m_ltUpdateTime > 0)
    {
        if (reversion_ - m_lLastCheckTime > m_ltUpdateTime)
        {
            //printf("in_timer");
            m_lLastCheckTime = reversion_;
            Event* e = eq_->allocateEvent();
            e->set_cmd(EVENT_TIMER);
            e->set_state(Status_Normal_Game);
            e->set_time(0);
            eq_->safePushEvent(e);
        }
    }
    if (dh_ != NULL)
    {
        dh_->tick();
    }
}

void EventHandler::safeQuit()
{
    if (dh_ != NULL)
    {
        dh_->quit();
    }
}

void EventHandler::registHandler(int cmd, ProcessRoutine handler, bool bAutoRange /*=false*/)
{
    if (processRoutines_[cmd] == NULL || !bAutoRange)
    {
        processRoutines_[cmd] = handler;
    }

}
