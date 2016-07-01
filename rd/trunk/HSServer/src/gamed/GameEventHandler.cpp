#define _CRT_SECURE_NO_WARNINGS

#include "event.pb.h"
#include "GameNetHandler.h"
#include "GameEventHandler.h"
#include "GameDataHandler.h"
#include "event/AutoEventHead.h"
#include "event/MessageDef.h"
#include "EventDefine.h"
//#include "../gamed/event/FriendRequest.h"
//#include "logic/HallCfgMgr.h"
#include "common/SysLog.h"
#include "common/Msg2QQ.h"
#include "common/staticfile.h"
#include "logic/GameConfig.h"
#include "common/DBC.h"
#include "MemCacheServerHandler.h"
#include <sys/types.h>
#include "CheatDetector.h"

extern int g_processingCmd;
GameEventHandler* GameEventHandler::g_pInst = NULL;

GameEventHandler::GameEventHandler(EventQueue *eq, GameDataHandler* dh, GameNetHandler *nh, int nid)
: eq_(eq), dh_(dh), nh_(nh), nid_(nid)
{
    logger_ = log4cxx::Logger::getLogger("GameEventHandler");
    eh_ = new EventHandler(eq, dh, nh, nid);
    TEST[E_EVENT_HANDLER]++;
    initialEventProcessors();
    serverIp = serverConfig.gamedIp(nid);
    string ip = serverConfig.gamedBindIp(nid);
 

    serverPort = serverConfig.gamedPort(nid);
    char name[128];
    sprintf(name, "game%d_ec.txt", nid);
    eh_->m_xEventCounter.Open(name);
    eh_->m_xEventCounter.m_nWriteMod = 100;
    nh_->setCachePoolEnable(GameConfig::GetInstance()->GetNetCacheEnableFlag());
    g_pInst = this;

    //InitRank();
    //registerToRankWorld();
    setProcessIgnore();
}

GameEventHandler::~GameEventHandler(void)
{
}

GameEventHandler*
GameEventHandler::GetInst()
{
    return g_pInst;
}

void
GameEventHandler::start()
{
    eh_->start(60);
}

void
GameEventHandler::createUserFdMap(int fd, int64 uid)
{
    nh_->addUser(fd, uid);
}

void
GameEventHandler::removeUserFdMap(int fd, int64 uid)
{
    nh_->kickUser(fd, uid);
}

void
GameEventHandler::sendEventToWorld(Event *e)
{
    LOG4CXX_DEBUG(logger_, "send event " << e->cmd() << ", state " << e->state() << " to world.");
    int fd = 0;
    if (e->has_fromworld_fd() && e->fromworld_fd() > 0)
    {
        fd = (int) e->fromworld_fd();
    }

    string text;
    e->SerializeToString(&text);
    text = "ev," + text;
    nh_->sendToWorld(fd, text);
}

void
GameEventHandler::sendEventToWorld( Event *e, int wid )
{
    string text;
    e->SerializeToString(&text);
    text = "ev," + text;
    nh_->sendToWorldByWid(wid, text);
}

bool
GameEventHandler::sendEventToWorldEx( Event *e, int wid )
{
    string text;
    e->SerializeToString(&text);
    text = "ev," + text;
    return nh_->sendToWorldByWid(wid, text);
}

void
GameEventHandler::registerToRankWorld( int rank_type /*= -1*/ )
{
    //Event e;
    //e.set_state(Status_Normal_To_World);
    //e.set_time(0);

    //e.set_gameid( nid_ );
    //e.set_worldid( GameConfig::GetInstance()->GetRankWorldId() - 1 );

    //// 达人秀不再使用老模式的排行榜
    ///*e.set_cmd(EVENT_RALENT_SHOW_LIST);
    //e.set_rankkind( 1 );
    //postBackEvent( &e );
    //e.set_rankkind( 2 );
    //postBackEvent( &e );*/

    //if (rank_type == -1)
    //{
    //    if (RankConfig::GetInstance()->have_cfg_)
    //    {
    //        std::map<int, bool> & rank_list = RankConfig::GetInstance()->getRankList();
    //        for (std::map<int, bool>::const_iterator iter = rank_list.begin(); iter != rank_list.end(); iter++)
    //        {
    //            MseRankList * pRankList = e.mutable_mse_mseranklist();
    //            pRankList->set_rank_type(iter->first);
    //            e.set_cmd(S2C_MseRankList);
    //            postBackEvent(&e);
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < 6; i++)
    //        {
    //            MseRankList * pRankList = e.mutable_mse_mseranklist();
    //            pRankList->set_rank_type(i);
    //            e.set_cmd(S2C_MseRankList);
    //            postBackEvent(&e);
    //        }
    //    }

    //}
    //else
    //{
    //    MseRankList * pRankList = e.mutable_mse_mseranklist();
    //    pRankList->set_rank_type(rank_type);
    //    e.set_cmd(S2C_MseRankList);
    //    postBackEvent(&e);
    //}
}

void
GameEventHandler::sendChangeToRankWorld( int _kind, int64 _llUid, std::string _strName, int _llWeight )
{
    Event e;
    e.set_state(Status_Normal_To_World);
    e.set_time(0);
    e.set_gameid( nid_ );
    e.set_cmd(EVENT_RALENT_SHOW);

    e.set_rankkind( _kind );
    e.mutable_talent_show()->set_name( _strName );
    e.mutable_talent_show()->set_uid( _llUid );
    e.mutable_talent_show()->set_weight( _llWeight );

    sendEventToWorld( &e, GameConfig::GetInstance()->GetRankWorldId() - 1 );
}

void
GameEventHandler::sendDataToUser(int fd, int cmd, const string &text)
{
    LOG4CXX_DEBUG(logger_, "send to sock:" << fd << " cmd:" << cmd << " len:" << text.length());
    time_t first_time = Clock::getUTime();
    nh_->sendDataToClient(fd, cmd, text);
    time_t second_time = Clock::getUTime();
    //	dh_->getStatistics().capture("send_process_time(us)",(float)(second_time-first_time));
}

void
GameEventHandler::sendFdString(int fd, const char* str, size_t size)
{
    LOG4CXX_DEBUG(logger_, "send to sock:" << fd << " len:" << size);
    nh_->sendFdString(fd, str, size);
}

void
GameEventHandler::SendErrorToUser(User* pUser, Event* e)
{
    //BseError* pError = e->mutable_bse_bseerror();
    //   string text;
    //   pError->SerializeToString(&text);
    //   sendDataToUser(pUser->fd(), S2C_BseError, text);
    //   LOG4CXX_ERROR(logger_, "Send Error To User:" << pUser->id() << ",Error:" << pError->errorid());

}

void
GameEventHandler::SendErrorToUser(User* pUser, int emErrType)
{
    /*BseError bseError;
    bseError.set_errorid(emErrType);
    string text;
    bseError.SerializeToString(&text);
    sendDataToUser(pUser->fd(), S2C_BseError, text);*/

}

void
GameEventHandler::sendToGlobalUser(int64 uid, int cmd, const string &text)
{
    Event e;
    e.set_cmd(EVENT_SEND_REMOTE_USER);
    e.set_state(SendRemoteUser_GW_Req);
    e.set_time(0);
    e.set_uid(uid);
    SendRemoteUser_Req* req = e.mutable_sendremoteuser_req();
    req->set_cmd(cmd);
    req->set_text(base64_encode(text));
    req->set_uid(uid);
    sendEventToWorld(&e);
}

#include "logic/User.h"

void
GameEventHandler::SendToAllOnlineUser(int cmd, const string& text)
{
    GameDataHandler* pUserManager = getDataHandler();
    const hash_map<int64, User*>& usermap = pUserManager->getUsers();
    for (hash_map<int64, User*>::const_iterator iter = usermap.begin(); iter != usermap.end(); iter ++)
    {
        User* pUser = iter->second;
        if (pUser != NULL && pUser->Online())
        {
            sendDataToUser(pUser->fd(), cmd, text);
        }
    }
}

////#include "event/UpdateWorkingStatus.h"
//#include "event/UserLogin.h"
//#include "event/UserAuth.h"
//#include "event/SendRemoteUser.h"
//// #include "event/DealHall2Game.h"
//#include "event/DealAdminEvent.h"
//// #include "event/FriendRequest.h"
//#include "event/DealWebBuy.h"
//#include "event/DealWebBuy3.h"
//// #include "event/DealWebLengthenIndate.h"
//// #include "event/DealWebCheckBalance.h"
////#include "event/UpdateGuestNum.h"
////#include "event/DealWebUnlock.h"
////#include "event/DealWebAddEmployee.h"
//#include "event/DealWebBuy.h"
//#include "event/DealWebInvite.h"
//#include "event/DealInvite.h"
//#include "event/DealWebAsk.h"
////#include "event/DealWebFreeGift.h"
////#include "event/DealWebGive.h"
////#include "event/DealQQUnionEvent.h"
//#include "common/Msg2QQ.h"
////#include "event/DealMultiLevelFriendPoint.h"
////#include "event/DealTalentShowList.h"
////#include "event/DealFriendSendGift.h"
////#include "event/DealFortuneSteal.h"
////#include "event/DealTalentShow.h"
////#include "event/DealWapEvent.h"
////#include "event/DealFriendRecallSend.h"
////#include "event/DealFriendRecallBack.h"
////#include "event/DealFriendLogTimeRequest.h"
////#include "event/DealReturnGift.h"
//#include "event/DealSimpleMark.h"
//#include "event/DealActiveinvite.h"
////#include "event/DealReturnGiftY.h"
////#include "event/DealChiefVote.h"
//#include "event/DealTimer.h"
//#include "event/DealCenterLogicCounter.h"

void
GameEventHandler::initialEventProcessors()
{
    //// add Event Processors here
    ////UpdateWorkingStatus::createInstance(this);
    //UserLogin::createInstance(this);
    //UserAuth::createInstance(this);
    ////     SendRemoteUser::createInstance(this);
    ////     DealHall2Game::createInstance(this);
    //DealAdminEvent::createInstance(this);
    //FriendRequest::createInstance(this);
    //DealWebBuy::createInstance(this);
    //DealWebInvite::createInstance(this);
    //DealInvite::createInstance(this);
    //DealSimpleMark::createInstance(this);
    //DealTimer::createInstance(this);
    //DealActiveinvite::createInstance(this);
    //DealCenterLogicCounter::createInstance(this);
    ////DealWebLengthenIndate::createInstance(this);
    ////DealWebCheckBalance::createInstance(this);
    ////initAutoEventProcessors函数为工具自动生成
    //initAutoEventProcessors();
}

int
GameEventHandler::AllocHallSrvID(User* pUser)
{
    //     if (pUser == NULL)
    //         return 1;
    //     return HallCfgMgr::GetInst()->AllocHallSrvID(pUser->GetUserLevel());
    return 1;
}

int
GameEventHandler::GetHallSrvIDByRoomID(int nRoomID)
{
    int nHallSrvID = nRoomID / HALLSRV_OFFSET;
    if (nHallSrvID >= MAX_HALLSRV_COUNT || nHallSrvID <= 0)
        return 1;
    else
    {
        return nHallSrvID;
    }
}

unsigned int
GameEventHandler::getCacheFdSize()
{
    return nh_->getFdCacheSize();
}

void
GameEventHandler::setProcessIgnore()
{
    char* configFile = "Config/ProcessIgnore.dat";

    eh_->clearIgnore();

    DBCFile file_(0);
    file_.OpenFromTXT(configFile);
    int nCnt = file_.GetRecordsNum();

    int cmd;
    for (int line = 0; line < nCnt; line++)
    {
        cmd = file_.Search_Posistion(line, 0)->iValue;
        eh_->addIgnore( cmd );
    }
}

void
GameEventHandler::sendChangeToRankWorldEx( int _kind, int64 _llUid, std::string _strName, int _llWeight, int64 rank_version /* = -1  */, std::string _strUrl /* = */ )
{
    //Event e;
    //e.set_state(Status_Normal_To_World);
    //e.set_time(0);
    //e.set_gameid( nid_ );
    //e.set_cmd(S2C_MseRank);

    //e.set_rankkind( _kind );
    //MseRank * request = e.mutable_mse_mserank();
    //SingleRankItem * pItem = e.mutable_mse_mserank()->mutable_rank_item();
    //pItem->set_name( _strName );
    //pItem->set_uid( _llUid );
    //pItem->set_num( _llWeight );
    //pItem->set_url(_strUrl);
    //request->set_rank_type(_kind);
    //request->set_rank_version(rank_version);

    //sendEventToWorld( &e, GameConfig::GetInstance()->GetRankWorldId() - 1 );
}

//bool GameEventHandler::FillRankInfo(int _kind, int from, int to, MseRankInfoAsk &response)
//{
//    Rank * pRank = getRank(_kind);
//    if (pRank == NULL)
//    {
//        return false;
//    }
//
//    time_t version = time(NULL);
//    std::list<RankItem*> *pList = pRank->GetList(version);
//    if (pList == NULL)
//    {
//        return false;
//    }
//
//    int num_max = pList->size() - 1;
//    if (from < 0 || from > num_max)
//    {
//        return false;
//    }
//
//    if (to < 0)
//    {
//        return false;
//    }
//
//    if (from > to)
//    {
//        return false;
//    }
//
//    std::list<RankItem *>::const_iterator iter = pList->begin();
//    int i = 0;
//    while (i < from)
//    {
//        i++;
//        iter++;
//    }
//
//    for (; iter != pList->end(); iter++)
//    {
//        i++;
//        if (i > to)
//        {
//            break;
//        }
//
//        RankItem * pItem = *iter;
//        if (pItem)
//        {
//            SingleRankItem * pSingle = response.add_rankinfo();
//            pSingle->set_uid(pItem->Uid());
//            pSingle->set_name(pItem->Name());
//            pSingle->set_num(pItem->Point());
//            pSingle->set_uid_str(toString<int64 > (pItem->Uid()));
//            pSingle->set_url(pItem->Url());
//            pSingle->set_wave(pItem->Wave());
//        }
//    }
//
//    response.set_rank_from(from);
//    response.set_rank_to(to);
//    response.set_rank_type(_kind);
//
//    return true;
//}
//
//void GameEventHandler::InitRank()
//{
//    if (RankConfig::GetInstance()->have_cfg_)
//    {
//        std::map<int, bool> & rank_list = RankConfig::GetInstance()->getRankList();
//        for (std::map<int, bool>::const_iterator iter = rank_list.begin(); iter != rank_list.end(); iter++)
//        {
//            if (m_mapRank.find(iter->first) == m_mapRank.end())
//            {
//                m_mapRank[iter->first] = new Rank();
//                if (m_mapRank[iter->first])
//                {
//                    m_mapRank[iter->first]->NeedFile( false );
//                    m_mapRank[iter->first]->RankType(iter->first);
//                    if (iter->first == 39)
//                    {
//                        m_mapRank[iter->first]->SetMaxRankSize(1000); 
//                    }
//                }
//
//                if (RankConfig::GetInstance()->IsEnable(iter->first))
//                {
//                    registerToRankWorld(iter->first);
//                }
//
//            }
//        }
//    }
//    else
//    {
//        //error
//    }
//}
//
//bool GameEventHandler::FillIntervalRankInfo( int _kind, int from, int to, MseRankInfoAsk &response )
//{
//    Rank * pRank = getRank(_kind);
//    if (pRank == NULL)
//    {
//        return false;
//    }
//
//    std::list<RankItem*> *pList = pRank->GetIntervalList();
//    if (pList == NULL)
//    {
//        return false;
//    }
//
//    int num_max = pList->size() - 1;
//    if (from < 0 || from > num_max)
//    {
//        return false;
//    }
//
//    if (to < 0)
//    {
//        return false;
//    }
//
//    if (from > to)
//    {
//        return false;
//    }
//
//    std::list<RankItem *>::const_iterator iter = pList->begin();
//    int i = 0;
//    while (i < from)
//    {
//        i++;
//        iter++;
//    }
//
//    for (; iter != pList->end(); iter++)
//    {
//        i++;
//        if (i > to)
//        {
//            break;
//        }
//
//        RankItem * pItem = *iter;
//        if (pItem)
//        {
//            SingleRankItem * pSingle = response.add_rankinfo();
//            pSingle->set_uid(pItem->Uid());
//            pSingle->set_name(pItem->Name());
//            pSingle->set_num(pItem->Point());
//            pSingle->set_uid_str(toString<int64 > (pItem->Uid()));
//            pSingle->set_url(pItem->Url());
//        }
//    }
//
//    response.set_rank_from(from);
//    response.set_rank_to(to);
//    response.set_rank_type(_kind);
//
//    return true;
//}
//
//bool GameEventHandler::UpdateIntervalRank()
//{
//    int64 version = eh_->getReversion();
//    std::map<int, bool> & rank_list = RankConfig::GetInstance()->getRankList();
//    for (std::map<int, bool>::const_iterator iter = rank_list.begin(); iter != rank_list.end(); iter++)
//    {
//        if (RankConfig::GetInstance()->IsIntervalRank(iter->first))
//        {
//            int interval = RankConfig::GetInstance()->Interval(iter->first);
//            if (interval > 0 && version % interval == 0)
//            {
//
//            }
//        }
//    }
//    return true;
//}