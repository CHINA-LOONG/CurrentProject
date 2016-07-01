/* 
 * File:   FriendInfoServerHandler.h
 * Author: Kidd
 * 用于处理好友缓存的类
 * Created on 2011年12月23日, 上午10:32
 */

#ifndef FRIENDINFOSERVERHANDLER_H
#define	FRIENDINFOSERVERHANDLER_H
#include "common/const_def.h"
#include "common/distribution.h"
#include "common/statistics.h"
#include "common/counter.h"
#include <list>
#include <algorithm>
#include <map>
#include <pthread.h>
#ifdef _WIN32
#include "common/Logger_win.h"
#include "common/tcrdb_win.h"
#else
#include <log4cxx/logger.h>
#include <tcrdb.h>
#endif
class HttpRequestV3;
class EventQueue;
class GameEventHandler;
class FriendInfoLite;

class FriendInfoServerHandler
{
    typedef FriendInfoServerHandler     FSH_T;
    typedef std::map<int, std::string>  IPList;
    typedef std::map<int, int>          PORTList;
    typedef std::map<int, void*>        ConnList;
public:
    FriendInfoServerHandler();
    ~FriendInfoServerHandler();
public:
    static FSH_T*       GetInst();
    static FSH_T*       GetInstAlarm();
public:
    static bool         GetFriendInfo(const std::string& openid, int plat_type, std::string& dist);
    static bool         UpdateFriendInfo(const std::string& openid, int plat_type, std::string& dist);
    static void*		FriendInfoProc(void* arg);
public:

    void				acquireEventLock()
    {
        pthread_mutex_lock(eq_mutex_);
    }

    void				releaseEventLock()
    {
        pthread_mutex_unlock(eq_mutex_);
    }

    void				acquireUpdateLock()
    {
        pthread_mutex_lock(eq_update_mutex_);
    }

    void				releaseUpdateLock()
    {
        pthread_mutex_unlock(eq_update_mutex_);
    }

	Statistics&			getStat()
	{
		return m_stat;
	}

	Counter&			getCounter()
	{
		return m_counter;
	}

    void				initThread();
    void				termThread();
    void				Run();

    bool				SafePushFriendRequest(int64 uid_from, int plat_type, const string& platid, int64 uid, bool isplatfriend = true);
    bool				SafePushFriendUpdate(const string& platid, int plat_type, const FriendInfoLite * pLite);
    bool                SafePushAlarmClock(const HttpRequestV3& request);
    int                 GetServerNumber();
    int                 GetServerId(const std::string& openid);
    void                LoadConfig();
    TCRDB*              GetDB(int id);
    bool                CanUse();
    void                SetEnbale(bool enable);
    void				SetEventHandler(GameEventHandler* eh);
private:
    pthread_t			tid_friendinfo;
    bool				m_bRunning;
    EventQueue			*m_pEventQueue;
    EventQueue			*m_pUpdateQueue;
    pthread_mutex_t		*eq_mutex_;
    pthread_mutex_t		*eq_update_mutex_;

    static FSH_T*       g_pInst;
    static FSH_T*       g_pInstAlarm;
    int                 m_nServerNum;

    IPList              m_xIpList;
    PORTList            m_xProtList;
    ConnList            m_xConnList;
    bool                m_bEnabled;
    bool                m_bInited;
	Statistics			m_stat;
	Counter				m_counter;
    static log4cxx::LoggerPtr logger;
    
    GameEventHandler*	m_eh;
} ;
///////////////////////////////////////

inline bool FriendInfoServerHandler::CanUse()
{
    return m_bEnabled && m_bInited && m_nServerNum != 0 && m_eh != 0;
}
#endif	/* FRIENDINFOSERVERHANDLER_H */

