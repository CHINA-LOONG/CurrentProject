#include "UserLogin.h"
#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/json/json.h"
#include "../../common/string-util.h"
#include "../../logic/User.h"
#include "../../logic/Player.h"
#include "../../common/SysLog.h"
#include "../../common/Msg2QQ.h"
#include "../../event/EventQueue.h"

#include "../../logic/Player.h"
#include "../../logic/AlarmClock2QQ.h"

#include "../../logic/UserCtrl.h"
#include "../../logic/Clock.h"
#include "../../logic/DailyCounter.h"
#include "../../logic/AlarmClock.h"

//#include "../../logic/OnlineRewardMgr.h"
UserLogin* UserLogin::instance_ = NULL;

void
UserLogin::handle(Event* e)
{
    switch (e->cmd())
    {
        case EVENT_USER_LOGIN:
            HandleUserLogin(e);
            break;
        case EVENT_USER_LOGOUT:
            HandleUserLogout(e);
            break;
        default:
            break;
    }
}

void
UserLogin::HandleUserLogin(Event* e)
{
    if (e->state() == UserLogin_WG_Req)
    {
        if (!e->has_userlogin_req())
        {
            return;
        }
        handle_WG_UserLogin(e);
    }
    else
    {
        LOG4CXX_ERROR(logger_, "Invalid Event.\n" << e->DebugString());
    }
}

void
UserLogin::HandleUserLogout(Event* e)
{
    int64 nUserID = e->uid();
    switch (e->state())
    {
        case Status_Normal_Game:
        {
            GameDataHandler* dh = eh_->getDataHandler();
            User* pUser = dh->getUser(nUserID, NULL, false);
            if (pUser == NULL)
            {
                dh->getCounter().increase("user_equal_null");
                return;
            }
            SendOffLine(pUser);
            WG_UserLeave* pUserLeave = e->mutable_wg_userleave();
            if (pUserLeave->fd() != pUser->fd())
            {
                dh->getCounter().increase("fd_error");
                return;
            }

            pUser->setOnline(false);


            //设置沉默用户和流失用户状态
            //pUser->SetSilenceUser(false);
            //pUser->SetLostUser(false);
            pUser->SetUsertype(1);
            dh->getCounter().increase("user_log_out");
            pUser->setFd(0);
            //             BceLeaveHall bceLR;
            // 			eh_->HandleLeaveHall(pUser,bceLR);
            time_t lt = time(NULL) - pUser->lastLoginTime();

            bool bStatUser = CSysLog::GetInstance()->IsStatUser(nUserID);
            //
            Player* pPlayer = pUser->GetPlayer();
            AlarmClock2QQ::SendAlarm(pPlayer);
            //开启闹钟
            //AlarmClock::instance().SendAlarm(pPlayer);
            //            SYS_LOG(nUserID, LT_LogOut, 0, 0, lt << pUser->GetUserLevel() << pUser->m_StatInfo.m_nVisitFrdCount
            //                    << pPlayer->GetMoney() << 0 << 0
            //                    << pUser->GetPlayer()->GetExp() << pUser->getPlatType());
            pPlayer->GetLogCtrl().LogBegin(LT_LogOut)->LogItem("在线时长", (int) ( Clock::GetSecond() - pUser->lastLoginTime() ))->
                    LogItem("", 0)->LogItem("", 0)->LogItem("", 0)->
                    LogItem("生命消耗数", pPlayer->GetLiveCtrl().GetLives())->
                    LogItem("水果糖购买数", 0)->
                    LogItem("水果糖消耗数", 0)->
                    LogItem("水果糖获得数", 0)->
                    LogEnd("LogOut");
            // 			DB_Player* pData = pUser->GetDbUser().mutable_player();
            // 			int64 onlineMin = pData->total_online_min() + lt/60;
            // 			pData->set_total_online_min(onlineMin);
            //
            // 			DB_PlayerInfo* pInfo = pData->mutable_info();
            // 			const int curStep = pInfo->onlineawdstep();
            // 			OnlineAwdInfo* pStep = NULL/*OnlineAwdMgr::GetInst()->GetStepInfo(curStep)*/;
            // 			if (pStep == NULL)	return;
            // 			time_t runTime = time(NULL) - pUser->GetPlayer()->GetOnlineAwdTime();
            // 			int remainTime = pInfo->onlineawdremain() - (int)runTime;
            // 			if (remainTime < 0)
            // 			{
            // 				remainTime = 0;
            // 			}
            // 			pInfo->set_onlineawdremain(remainTime);
            //
            //CMsg2QQ::GetInstance()->TellMsg_deleted_function(MQ_Logout, pUser, 0, 0, 0);
            //CMsg2QQ::GetInstance()->TellMsg(MQ_Logout, pUser, 0, 0, 0);
            CMsg2QQ::GetInstance()->TellMsgLoginOut(pUser);
            //
            dh->markUserDirty(pUser);
        }
    }

}

void
UserLogin::handle_WG_UserLogin(Event* e)
{
    GameDataHandler* dh = eh_->getDataHandler();
    const UserLogin_Req& req = e->userlogin_req();
    const string& platid = req.platform_id();
    int siteid = req.siteid();
    const string& open_key = req.open_key();
    const string& name = req.name();
    const string& profile_link = req.profile_link();
    int gender = (req.gender() == 0) ? 1 : 2 ;
    vector<string> friends_platid;

    //string friendPlatidList;

    for (int i = 0; i < req.friends_platid_size(); i ++)
    {
        friends_platid.push_back(req.friends_platid(i));
        //friendPlatidList += req.friends_platid(i) + ",";
    }

    bool isYellowDmd = req.is_yellow_dmd();
    bool isYellowDmdYear = req.is_yellow_dmd_year();
    int lvlYellowDmd = req.yellow_dmd_lv();

    int webfd = req.webfd();
    int sid = req.sid();
    LOG4CXX_INFO(logger_, "World request UserLogin with openid: " << platid);
    dh->getCounter().increase("user_create");
    dh->login_platid_[platid] = true;

    LoadStatus state = LOAD_INVALID;
    User* user = processUserLogin(platid, siteid, name, profile_link, gender,
            friends_platid, isYellowDmd, isYellowDmdYear, lvlYellowDmd, state, open_key, req.register_type(), req.action_from(), req.device_from());
    if (user != NULL)
    {
        // SYS_LOG(user->id(), LT_FriendsPlatid, 0, 0, friendPlatidList );

        UserLogin_Rsp* rsp = e->mutable_userlogin_rsp();
        rsp->set_server(eh_->getServerIp());
        rsp->set_port(eh_->getServerPort());
        rsp->set_uid(user->id());
        if (req.device_from() != "")
        {
            user->czDeviceType = req.device_from();
        }
        if (req.device_from() == "" || req.device_from() == "PC")
        {
            user->GetPlayer()->GetDailyCounter().AddCounter(DL_LOGIN_PC);
        }
        else
        {
            user->GetPlayer()->GetDailyCounter().AddCounter(DL_LOGIN_MOBILE);
        }

        const std::string inv_oid = user->GetDbUser().player().inviter_openid();
        if (siteid == PLAT_WAP)
        {
            rsp->set_secret(user->secret_ex(eh_->getReversion(), true));
            bool bStatUser = CSysLog::GetInstance()->IsStatUser(user->id());
            Player* pPlayer = user->GetPlayer();

            std::string public_praise_log;

            SYS_LOG(user->id(), LT_LogOn, 0, 0, bStatUser << user->gender() << user->GetUserLevel()
                    << user->isYellowDmd() << user->isYellowDmdYear() << user->getYellowDmdLv()
                    << user->friends_platid().size()
                    << pPlayer->GetMoney() << 0 << 0
                    << user->GetPlayer()->GetExp() << user->platform_id().c_str()
                    << user->getPlatType() << user->getOpenKey().c_str()
                    << 1
                    << inv_oid.c_str()  //)
                    << req.device_from().c_str()
                    );
        }
        else
        {
            Player* pPlayer = user->GetPlayer();
            rsp->set_secret(user->secret(eh_->getReversion()));
            user->GetPlayer()->GetLogCtrl().LogBegin(LT_LogOn)->LogItem("playtyep", (int) user->getPlatType())->
                    LogItem("friend cnt", (int) (user->friends_platid().size()))->
                    LogItem("in game reflash", false)->
                    LogItem("in game reflash", user->GetPlayer()->GetUniverseCtrl().GetSeed() > 0)->
                    LogItem("dev from", user->czDeviceType.c_str())->
                    LogItem("inv_oid", inv_oid.c_str())->
                    LogItem("pc_login_cnt", pPlayer->GetDailyCounter().GetCounter(DL_LOGIN_PC))->
                    LogItem("mobile_login_cnt", pPlayer->GetDailyCounter().GetCounter(DL_LOGIN_MOBILE))->
                    LogEnd("LogOn");
        }
        // AlarmClock::instance().SendAlarm(user->GetPlayer());
        rsp->set_webfd(webfd);
        rsp->set_sid(sid);
        // 		string argv;
        // 		argv +="lvl:";
        // 		argv += toString<int>(user->GetUserLevel());
        int userlvl = user->GetUserLevel();
        if (userlvl <= 0)
        {
            userlvl = 1;
        }

        char argv[64] = {0};
        sprintf(argv, "lvl:%d", userlvl);
        // 		argv += "|";
        // 		argv +="exp:";
        // 		argv += toString<int64>(user->GetPlayer()->GetExp());
        rsp->set_argv(argv);
        e->clear_userlogin_req();
        e->set_state(UserLogin_GW_Rsp);
        eh_->sendEventToWorld(e);
        UserCtrl uc(user);
        if (GameConfig::GetInstance()->GetSendKickOnWebLogin())
        {
            uc.SendNoReconnect();
        }
        dh->markUserDirty(user);
    }
    else
    {
        if (state == LOAD_WAITING)
        {
            eh_->postBackEvent(e);
        }
    }
}

User*
UserLogin::processUserLogin(const string& platid, int siteid, const string& name,
        const string& profile_link, int gender,
        vector<string> friends_platid, bool isYellowDmd,
        bool isYellowDmdYear, int lvlYellowDmd, LoadStatus& state,
        const string &openkey, int register_type, const string& action_from, const string& device_from)
{
    GameDataHandler* dh = eh_->getDataHandler();
    User *pUser = dh->getUser(platid, &state, true);
    bool bKickOnline = false;
    if (pUser == NULL)
    {
        if (state == LOAD_EMPTY)
        {
            pUser = dh->createUser(platid, name, profile_link, gender, (PLAT_TYPE) siteid,
                    isYellowDmd, isYellowDmdYear, lvlYellowDmd, friends_platid, eh_, openkey, register_type, action_from, device_from);
        }
    }
    else
    {
        if (pUser->fd() != 0 && siteid == PLAT_WAP )
        {
            UserCtrl uc(pUser);
            if (GameConfig::GetInstance()->GetSendKickOnWebLogin())
            {
                uc.SendNoReconnect();
            }
            eh_->removeUserFdMap(pUser->fd(), pUser->id());
            pUser->SetFd(0);
        }

        dh->updateUser(pUser, name, profile_link, gender, (PLAT_TYPE) siteid,
                isYellowDmd, isYellowDmdYear, lvlYellowDmd,	friends_platid, eh_, openkey, register_type, action_from);
    }
    if (pUser != NULL)
    {
        dh->login_platid_.erase(platid);
        // pUser->GetPlayer()->GetDallyTask().SetYellDmd(isYellowDmd || isYellowDmdYear);
        //SYS_LOG(pUser->id(), LT_Register, 0, 0, platid.c_str());
    }

    return pUser;
}

void
UserLogin::SendOffLine(User* pUser)
{
    if (pUser == NULL)
    {
        return;
    }
    Player* pPlayer = pUser->GetPlayer();
    GameDataHandler* pUserManager = eh_->getDataHandler();
    /*for (int i = 0; i < pPlayer->GetFanList().uid_size(); i++)
    {
        PushOnlineStatus(pUser, pPlayer->GetFanList().uid(i));
    }
    for (int i = 0; i < pPlayer->GetFriendList().uid_size(); i++)
    {
        PushOnlineStatus(pUser, pPlayer->GetFriendList().uid(i));
    }
    for (int i = 0; i < pUser->friends_platid().size(); i++)
    {
        GameFriendInfo* pInfo = pUserManager->getFriendInfo(pUser->friends_platid()[i], pUser->getPlatType() );
        if (pInfo != NULL)
        {
            PushOnlineStatus(pUser, pInfo->fid);
        }
    }*/
}

void
UserLogin::PushOnlineStatus(User* pUser, int64 uid)
{
    //Event* ev = eh_->getEventQueue()->allocateEvent();
    //ev->set_cmd(S2C_BseUpdateOnlineStatus);
    //ev->set_state(Status_Normal_Game);
    //ev->set_time(0);
    //ev->set_uid(pUser->id());

    //BseUpdateOnlineStatus* pRequest = ev->mutable_bse_bseupdateonlinestatus();
    ////下线了. 所以要发送下线通知
    //pRequest->set_online(false);
    //pRequest->set_uid(toString<int64 > (uid));

    //eh_->getEventQueue()->safePushEvent(ev);
}
