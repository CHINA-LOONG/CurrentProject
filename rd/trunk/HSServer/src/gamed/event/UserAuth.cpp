#include "UserAuth.h"
#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/json/json.h"
#include "../../common/string-util.h"
#include "../../logic/User.h"
#include "../../logic/Player.h"
#include "../../event/EventQueue.h"
#include "../../common/SysLog.h"
#include "../../common/Msg2QQ.h"
#include "../../event/MseAuthState.pb.h"
#include "../../logic/UserCtrl.h"
#include "../../common/SysLog.h"
#include "../../logic/Clock.h"

void
UserAuth::addEvent(EventQueue* eq, int64 uid, int64 secret, int fd)
{
    Event* e = eq->allocateEvent();
    e->set_cmd(EVENT_USER_AUTH);
    e->set_state(UserAuth_CG_Req);
    e->set_time(-2);
    //e->set_openid
    UserAuth_Req* req = e->mutable_userauth_req();
    req->set_uid(uid);
    req->set_secret(secret);
    req->set_sockfd(fd);
    eq->safePushEvent(e);
}

void
UserAuth::handle(Event* e)
{
    if (e->state() == UserAuth_CG_Req)
    {
        if (!e->has_userauth_req())
        {
            return;
        }
        handle_CG_Req(e);
    }
    else
    {
        LOG4CXX_ERROR(logger_, "Invalid Event.\n" << e->DebugString());
    }
}

void
UserAuth::handle_CG_Req(Event* e)
{
    GameDataHandler* dh = eh_->getDataHandler();
    const UserAuth_Req& req = e->userauth_req();
    int64 uid = req.uid();
    int64 secret = req.secret();
    int fd = req.sockfd();

    if (uid <= 0)
        return;
    User *user = dh->getUser(uid, e);
    if (user == NULL)
    {
        return;
    }

    int type = 0;
    int leftMin = 0;
    int totalMin = 0;
    bool pass1 = user->checkSecret(secret, dh->Revision());
    if (!pass1)
    {
        type = 1;
        LOG4CXX_ERROR(logger_, "check_secret_error\t" << "uid:" << user->id() << "\tclient_secret:" << secret << "\tuser_secret:" << user->GetDbUser().secret() << "	" << dh->Revision() << "	" << user->GetDbUser().secret_gentime());
    }

    DB_Player* pPlayer = user->GetDbUser().mutable_player();
    DB_BanLogin* login = pPlayer->mutable_banlogin();
    int64 delay = time(NULL) - login->time();
    totalMin    = login->totaltime();
    if (login->banlogin() && delay >= 0)
    {
        login->set_banlogin(false);
        //       dh->markUserDirty(user);
    }

    if (delay < 0)
    {
        leftMin = 0 - (int) (delay / 60);
    }

    bool pass2 = !login->banlogin();
    if (!pass2)
    {
        type = 2;
    }

    UserCtrl uc(user);
    bool pass3 = user->getPlatType() != PLAT_WAP;
    if (!pass3)
    {
        type = 1;
        if (GameConfig::GetInstance()->GetSendKickOnAuth())
        {
            uc.SendNoReconnect();
        }
        eh_->removeUserFdMap(fd, uid);
    }

    bool pass = pass1 && pass2 && pass3;
    bool bKickOnline = false;
    if (pass)
    {
        if (user->fd() != 0 && user->fd() != fd)
        {
            if (GameConfig::GetInstance()->GetSendKickOnAuth())
            {
                uc.SendNoReconnect();
            }
            eh_->removeUserFdMap(user->fd(), uid);
            bKickOnline = true;
        }

        eh_->createUserFdMap(fd, uid);
        user->setFd(fd);
        user->setOnline(true);
        eh_->getDataHandler()->getCounter().increase("user_log_in");
        user->Logon(dh);
        //       dh->markUserDirty(user);
        bool bStatUser = CSysLog::GetInstance()->IsStatUser(uid);
        Player* pPlayer = user->GetPlayer();
        //        SYS_LOG(uid, LT_LogOn, 0, 0, bStatUser << user->gender() << user->GetUserLevel()
        //                << user->isYellowDmd() << user->isYellowDmdYear() << user->getYellowDmdLv()
        //                << user->friends_platid().size()
        //                << pPlayer->GetMoney() << 0 << 0 
        //                << user->GetPlayer()->GetExp() << user->platform_id().c_str()
        //                << user->getPlatType() << user->getOpenKey().c_str()
        //                << (bKickOnline ? (1) : (0))
        //                << public_praise_log << user->czDeviceType.c_str());
        User*pUser = user;
        //pPlayer->GetLogCtrl().LogBegin(LT_LogOn)->LogItem("playtyep", (int) pUser->getPlatType())->
        //        LogItem("friend cnt", (int) (pUser->friends_platid().size()))->
        //        LogItem("in game reflash", false)->
        //        LogItem("in game reflash", pPlayer->GetUniverseCtrl().GetSeed() > 0)->
        //        LogItem("dev from", pPlayer->GetUser()->czDeviceType.c_str())->
        //        LogEnd("LogOn");
        //CMsg2QQ::GetInstance()->TellMsg_deleted_function(MQ_Logon, user, 0, 0, 0);
        //CMsg2QQ::GetInstance()->TellMsg(MQ_Logon, user, 0, 0, 0);
        CMsg2QQ::GetInstance()->TellMsgLogin0n(user, user->getActionFrom());
        if (GameConfig::GetInstance()->GetSendGuestOnAuth())
        {
            // uc.SendGuestInfo();
        }

    }

    MseAuthState state;
    user->BuildKickPass();
    state.set_pass(pass);
    state.set_type(type);
    state.set_leftmin(leftMin);
    state.set_banmin(totalMin);
    //state.set_adpath(AdvertisementManager::GetInst()->GetUrl());
    state.set_adval( user->GetDbUser().checkval_first());
    state.set_sw(user->GetDbUser().checkval_second());
    string text;
    state.SerializeToString(&text);
    eh_->sendDataToUser(fd, S2C_MseAuthState, text);
}
