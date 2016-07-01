/* 
 * File:   FriendInfoServerHandler.cpp
 * Author: Kidd
 *
 * Created on 2011年12月23日, 上午10:32
 */

#include <list>

#include "../event/EventQueue.h"
#include "common/DBC.h"
#include "common/string-util.h"
#include "common/counter.h"
#include "../event/EventDefine.h"
#include "../gamed/GameEventHandler.h"

#include "FriendInfoServerHandler.h"
#include "logic/HttpSender.h"
#include "logic/Clock.h"

FriendInfoServerHandler::FSH_T* FriendInfoServerHandler::g_pInst = NULL;
FriendInfoServerHandler::FSH_T* FriendInfoServerHandler::g_pInstAlarm = NULL;
log4cxx::LoggerPtr FriendInfoServerHandler::logger = log4cxx::Logger::getLogger("FriendServer");

FriendInfoServerHandler::FriendInfoServerHandler()
{
    m_bRunning			= false;
    m_bEnabled			= false;
    m_bInited			= false;
    m_pEventQueue		= new EventQueue;
    m_pUpdateQueue		= new EventQueue;
    eq_mutex_			= m_pEventQueue->mutex();
    eq_update_mutex_	= m_pUpdateQueue->mutex();
    m_eh				= NULL;
}

FriendInfoServerHandler::~FriendInfoServerHandler()
{
    if (m_pEventQueue != NULL)
    {
        delete m_pEventQueue;
        m_pEventQueue = NULL;
    }

    if (m_pUpdateQueue != NULL)
    {
        delete m_pUpdateQueue;
        m_pUpdateQueue = NULL;
    }

}

FriendInfoServerHandler::FSH_T* FriendInfoServerHandler::GetInst()
{
    if (g_pInst == NULL)
    {
        g_pInst = new FriendInfoServerHandler();
        TEST[E_FRIEND_HANDLER]++;
    }
    return g_pInst;
}

FriendInfoServerHandler::FSH_T* FriendInfoServerHandler::GetInstAlarm()
{
    if (g_pInstAlarm == NULL)
    {
        g_pInstAlarm = new FriendInfoServerHandler();
        //TEST[E_FRIEND_HANDLER]++;
    }
    return g_pInstAlarm;
}

int FriendInfoServerHandler::GetServerId(const std::string& openid)
{
    int hash = getPlatidHash(openid);
    return hash % GetServerNumber() + 1;
}

void* FriendInfoServerHandler::FriendInfoProc(void* arg)
{
    FriendInfoServerHandler* friendinfo = static_cast<FriendInfoServerHandler*> (arg);
    friendinfo->Run();
    pthread_exit(NULL);
    return NULL;
}

void FriendInfoServerHandler::initThread()
{
    if (m_bRunning)
    {
        return;
    }
    m_bRunning = true;
    int ret = 0;
    ret = pthread_create(&tid_friendinfo, NULL, FriendInfoServerHandler::FriendInfoProc, (void *) this);
    if (ret != 0)
    {
        m_bRunning = false;
        LOG4CXX_ERROR(logger, "ERROR creating friendinfo thread");
    }

}

void FriendInfoServerHandler::termThread()
{
    m_bRunning = false;
    pthread_join(tid_friendinfo, NULL);
}

void FriendInfoServerHandler::Run()
{
    vector<Event*> events, events_update;
    events.reserve(10240);
    events_update.reserve(10240);
    while (m_bRunning)
    {
        acquireEventLock();
        events.clear();

        while (!m_pEventQueue->isEmpty())
        {
            events.push_back(m_pEventQueue->popEvent());
        }
        releaseEventLock();

        for (size_t i = 0; i < events.size(); i++)
        {
            Event *pEvent = events[i];
            if (pEvent != NULL && pEvent->cmd() == EVENT_GWG_FRIEND_REQUEST)
            {
                GWG_FriendRequest* pRespone = pEvent->mutable_friendinfo();
                string strFriendInfo;
                bool hasinfo = GetFriendInfo(pRespone->platid(), pRespone->plat_type(), strFriendInfo);
                if (hasinfo)
                {
                    FriendInfoLite*         pLite    = pEvent->mutable_friendinfo()->mutable_info();
                    pLite->ParseFromString(strFriendInfo);
                   
					//printf("%s\n",pLite->ShortDebugString().c_str());
                }
                pRespone->set_ret(hasinfo);
                if (hasinfo)
                {
                    pEvent->set_state(Status_Normal_Back_Game);
                }
            }

        }

        for (size_t i = 0; i < events.size(); i++)
        {
            m_eh->getEventQueue()->safePushEvent(events[i]);
        }

        acquireUpdateLock();
        for (size_t i = 0; i < events_update.size(); i++)
        {
            events_update[i]->Clear();
            m_pUpdateQueue->freeEvent(events_update[i]);
        }
        events_update.clear();
        m_stat.capture("alarm_queue_size", m_pUpdateQueue->getSize());
        while (!m_pUpdateQueue->isEmpty())
        {
            events_update.push_back(m_pUpdateQueue->popEvent());
        }
        releaseUpdateLock();

        for (size_t i = 0; i < events_update.size(); i++)
        {
            Event *pEvent = events_update[i];
            if (pEvent != NULL && pEvent->cmd() == EVENT_GWG_FRIEND_REQUEST)
            {
                GWG_FriendRequest* pRespone = pEvent->mutable_friendinfo();
                string strFriendInfo;
                FriendInfoLite *pLite = pRespone->mutable_info();
                pLite->SerializeToString(&strFriendInfo);
                FriendInfoServerHandler::UpdateFriendInfo(pLite->openid(), pRespone->plat_type(), strFriendInfo);
                //printf("update_friend_info\n");
            }
            if (pEvent != NULL && pEvent->cmd() == EVENT_ALARM_POSTDATA)
            {
                time_t time_begin = Clock::getUTime();
                const HttpRequestV3& http = pEvent->http();
                HttpSender::GetInst()->SendPost(http.url(), http.postdata());
                time_t time_last = Clock::getUTime();
                m_stat.capture("alarm_process_time", (float) (time_last - time_begin));
            }
        }

        usleep(1000);
    }
}

bool FriendInfoServerHandler::GetFriendInfo(const std::string& openid, int plat_type, std::string& dist)
{
    if (g_pInst == NULL || !g_pInst->CanUse())
    {
        return false;
    }
    int     dbid    = g_pInst->GetServerId(openid);
    TCRDB*  pConn   = g_pInst->GetDB(dbid);
    if (pConn == NULL)
    {
        g_pInst->SetEnbale(false);
        return false;
    }
    std::string key = toString<int>(plat_type) + openid ;
    int klen = key.length();
    int len  = 0;
    //char* buffer    = (char*) tcrdbget2(pConn, key.c_str());
    char* buffer = (char*) tcrdbget(pConn, key.c_str(), klen, &len);
    if (buffer == NULL)
    {
        int ecode = tcrdbecode(pConn);
        if (ecode != TTENOREC)
        {
            g_pInst->SetEnbale(false);
        }
        return false;
    }
    std::string outbuf(buffer, len);
    dist = outbuf;
    free(buffer);
    return true;
}

bool FriendInfoServerHandler::UpdateFriendInfo(const std::string& openid, int plat_type, std::string& dist)
{
    if (g_pInst == NULL || !g_pInst->CanUse())
    {
        return false;
    }
    int     dbid    = g_pInst->GetServerId(openid);
    TCRDB*  pConn   = g_pInst->GetDB(dbid);
    if (pConn == NULL)
    {
#ifndef WIN32
        g_pInst->SetEnbale(false);
#endif

        return false;
    }

    const char* buf = dist.c_str();
    int         len = dist.length();
    std::string key = toString<int>(plat_type) + openid ;
    int klen = key.length();
    if (!tcrdbput(pConn, key.c_str(), klen, buf, len))
    {
        g_pInst->SetEnbale(false);
        return false;
    }
    return true;
}

int FriendInfoServerHandler::GetServerNumber()
{
    return m_nServerNum;
}

TCRDB* FriendInfoServerHandler::GetDB(int id)
{
    TCRDB* pResult = (TCRDB*) (m_xConnList[id]);
    if (pResult == NULL)
    {
        pResult = tcrdbnew();
        std::string ip = m_xIpList[id];
        if (tcrdbopen(pResult, ip.c_str() , m_xProtList[id]))
        {
            m_xConnList[id] = pResult;
        }

    }
    return pResult;
}

void FriendInfoServerHandler::LoadConfig()
{
    const char* filename = "FriendServer.dat";
    DBCFile file(0);
    file.OpenFromTXT(filename);

    enum FS
    {
        FS_ID = 0,
        FS_IP = 1,
        FS_PROT = 2,
    } ;
    int cnt = file.GetRecordsNum();
    m_nServerNum = 0;
    for (int line = 0; line < cnt; line++)
    {

        int ind         = file.Search_Posistion(line, FS_ID)->iValue;
        std::string ip  = file.Search_Posistion(line, FS_IP)->pString;
        int prot        = file.Search_Posistion(line, FS_PROT)->iValue;
        m_xIpList[ind]  = ip;
        m_xProtList[ind] = prot;
        if (ind > m_nServerNum)
        {
            m_nServerNum = ind;
        }
    }
    m_bInited = true;
    SetEnbale(true);
}

void FriendInfoServerHandler::SetEnbale(bool enable)
{
    m_bEnabled = enable;
}

void FriendInfoServerHandler::SetEventHandler( GameEventHandler* eh )
{
    m_eh = eh;
}

bool FriendInfoServerHandler::SafePushFriendRequest(int64 uid_from, int plat_type, const string& platid, int64 uid, bool isplatfriend /* = true */)
{
    if (CanUse() == false)
    {
        m_bEnabled = false;
        return false;
    }

    Event* ev = m_eh->getEventQueue()->allocateEvent();
    ev->set_cmd(EVENT_GWG_FRIEND_REQUEST);
    ev->set_state(Status_Normal_Game);
    ev->set_time(0);
    ev->set_uid(uid_from);

    GWG_FriendRequest* pRequest = ev->mutable_friendinfo();
    pRequest->set_ret(false);
    pRequest->set_plat_type(plat_type);
    pRequest->set_tuid(uid);
    pRequest->set_platid(platid);
    pRequest->set_is_plat_friend(isplatfriend);

    m_pEventQueue->safePushEvent(ev);
    return true;
}

bool FriendInfoServerHandler::SafePushFriendUpdate(const string& platid, int plat_type, const FriendInfoLite * pLite)
{
    if (CanUse() == false)
    {
        m_bEnabled = false;
        return false;
    }

    Event* ev = m_pUpdateQueue->allocateEvent();
    ev->set_cmd(EVENT_GWG_FRIEND_REQUEST);
    ev->set_state(Status_Normal_Game);
    ev->set_time(0);
    ev->set_uid(0);


    GWG_FriendRequest* pRequest = ev->mutable_friendinfo();
    FriendInfoLite *pLiteDes = pRequest->mutable_info();
    pLiteDes->CopyFrom(*pLite);
    pRequest->set_ret(false);
    pRequest->set_plat_type(plat_type);
    pRequest->set_tuid(0);
    pRequest->set_platid(platid);
    pRequest->set_is_plat_friend(false);

    m_pUpdateQueue->safePushEvent(ev);
    return true;
}

bool FriendInfoServerHandler::SafePushAlarmClock(const HttpRequestV3& request)
{
    Event e;
    Event* ev = &e; //m_pUpdateQueue->allocateEvent();
    ev->set_cmd(EVENT_ALARM_POSTDATA);
    ev->set_state(Status_Normal_Game);
    ev->set_time(0);
    ev->set_uid(0);

    HttpRequestV3* pHttp = ev->mutable_http();
    pHttp->CopyFrom(request);
    m_pUpdateQueue->safePushCopyEvent(ev, GameConfig::GetInstance()->m_nAlarmQueueMax);
    return true;
}

