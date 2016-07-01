#include "DealSimpleMark.h"
#include "MessageDef.h"
#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/json/json.h"
#include "../../common/string-util.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "../../logic/Player.h"
//#include "../../event/MseFriendList.pb.h"
#include "../../gamed/FriendInfoServerHandler.h"
#include "libconfig/include/grammar.h"
#include "../../logic/Clock.h"
#include "../../logic/GameConfig.h"
#include "../../logic/FuncSwitch.h"
#include "../event/EventQueue.h"

DealSimpleMark* DealSimpleMark::instance_ = NULL;

void DealSimpleMark::handle(Event* e)
{
    if (e->state() == Status_Normal_Game)
    {
        handle_selfload(e);
    }
    else if (e->state() == Status_Normal_Logic_Game )
    {
        handle_romateload(e);
    }
    else if (e->state() == Status_Normal_Back_Game )
    {
        handle_romatereturn(e);
    }
    else
    {
        LOG4CXX_ERROR(logger_, "Invalid Event.\n" << e->DebugString());
    }
}

/**
 * 本地读取
 * @param e
 */
void DealSimpleMark::handle_selfload(Event* e)
{
    GameDataHandler* pUserManager = eh_->getDataHandler();
    LoadStatus state = LOAD_INVALID;
    SimpleMarkUser* pRequest = e->mutable_simplemare(0);
    User *pDataUser = GetUser(pRequest->openid(), pRequest->uid(), &state, true);
    if (pDataUser == NULL)
    {
        if (state == LOAD_WAITING)
        {
            eh_->postBackEvent(e);
        }
        else if (state == LOAD_MISS)
        {
            e->mutable_friendinfo()->set_ret(false);
            e->mutable_forwardinfo()->set_platid( pRequest->openid() );
            e->mutable_forwardinfo()->set_uid(pRequest->uid());
            e->set_state(Status_Normal_To_World);
            eh_->sendEventToWorld(e);
        }
        return;
    }
    else
    {
        User *pUser = pUserManager->getUser(e->uid());
        if (pUser != NULL)
        {
            DealEvent(pRequest, pUser);
            return;
        }
    }
}

/**
 * 远程读取
 * @param e
 */
void DealSimpleMark::handle_romateload(Event* e)
{
    SimpleMarkUser* pRequest = e->mutable_simplemare(0);
    GameDataHandler* pUserManager = eh_->getDataHandler();
    LoadStatus state = LOAD_INVALID;
    User *pDataUser = GetUser(pRequest->openid(), pRequest->uid(), &state, true);
    if (pDataUser == NULL)
    {
        if (state == LOAD_WAITING)
        {
            eh_->postBackEvent(e);
        }
        return;
    }
    else
    {
        DealEvent(pRequest, pDataUser);
        e->set_state(Status_Normal_Back_World);
        eh_->sendEventToWorld(e);
    }
}

/**
 * 远程回复
 * @param e
 */
void DealSimpleMark::handle_romatereturn(Event* e)
{
}

/**
 * 获取玩家
 * @param pid  平台id
 * @param uid  uid
 * @param status 返回 读取状态
 * @param load 是否要从数据库加载(如果不在内存里)
 * @return User指针
 */
User* DealSimpleMark::GetUser(const string& pid, const int64 uid, LoadStatus* status, bool load)
{
    GameDataHandler* pUserManager = eh_->getDataHandler();
    if (pid.size() > 0)
    {
        return pUserManager->getUser(pid , status, true);
    }
    else
    {
        return pUserManager->getUser(uid , status, true);
    }
}

void DealSimpleMark::DealEvent(SimpleMarkUser* pRequest, User* pUser)
{
    if (pRequest == NULL || pUser == NULL)
    {
        return;
    }
    static const int FactoryDealAsGuard      = 0;
    static const int FactoryFouceStockReward = 1;
    static const int FuncSend  = 2;
    if (pRequest->type() == FactoryDealAsGuard)
    {
        //pUser->GetDbUser().mutable_player()->mutable_factory()->set_freetime(pRequest->value());
        //FriendInfoLite info;
        //FriendInfoLite * pLite = &info;
        //pUser->FillAsFriendLite(pLite, pUser->getPlatType());
        //FriendInfoServerHandler::GetInst()->SafePushFriendUpdate(pLite->openid(), pUser->getPlatType(), pLite);
    }
    if (pRequest->type() == FactoryFouceStockReward)
    {
       //bool bChanged = pUser->GetPlayer()->GetFactoryCtrl().Update();
       //    if (bChanged)
       //{
       eh_->getDataHandler()->markUserDirty(pUser);
       //}
    }
    if (pRequest->type() == FuncSend)
    {
        UserCtrl uc(pUser);
        uc.SendFunc();
    }
}

void DealSimpleMark::PushRequest(User* pSelf, int64 llTargetId, int nTid, int nValue)
{
    if (pSelf == NULL)
    {
        return;
    }
    Event* ev = eh_->getEventQueue()->allocateEvent();
    ev->set_cmd(EVENT_SIMPLE_MARK);
    ev->set_state(Status_Normal_Game);
    ev->set_time(0);
    ev->set_uid(pSelf->id());

    SimpleMarkUser* pRequest = ev->add_simplemare();
    pRequest->set_openid("");
    pRequest->set_uid(llTargetId);
    pRequest->set_type(nTid);
    pRequest->set_value(nValue);
    eh_->getEventQueue()->safePushEvent(ev);
}

void DealSimpleMark::PushRequest(int64 llUid, int nId, int nValue)
{
    Event* ev = eh_->getEventQueue()->allocateEvent();
    ev->set_cmd(EVENT_SIMPLE_MARK);
    ev->set_state(Status_Normal_Game);
    ev->set_time(0);
    ev->set_uid(llUid);

    SimpleMarkUser* pRequest = ev->add_simplemare();
    pRequest->set_openid("");
    pRequest->set_uid(llUid);
    pRequest->set_type(nId);
    pRequest->set_value(nValue);
    eh_->getEventQueue()->safePushEvent(ev);
}
