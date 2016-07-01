//#include "FriendRequest.h"
#include "friendrequest.h"
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
#include "../../logic/CallbackCtrl.h"
#include "../../event/MseFriendList.pb.h"

FriendRequest* FriendRequest::instance_ = NULL;
int FriendRequest::nLoadCnt = 0;
int64 FriendRequest::llLoadCntTimeSpan = 0l;

void
FriendRequest::handle(Event* e)
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
void
FriendRequest::handle_selfload(Event* e)
{
    GameDataHandler* pUserManager = eh_->getDataHandler();
    LoadStatus state = LOAD_INVALID;
    GWG_FriendRequest* pRespone = e->mutable_friendinfo();
    User *pDataUser = GetUser(pRespone->platid(), pRespone->tuid(), &state, true);
    if (pDataUser == NULL)
    {
        if (state == LOAD_WAITING)
        {
            eh_->postBackEvent(e);
        }
        else if (state == LOAD_MISS)
        {
            e->mutable_friendinfo()->set_ret(false);
            e->mutable_forwardinfo()->set_platid( e->friendinfo().platid() );
            e->mutable_forwardinfo()->set_uid(pRespone->tuid());
            e->set_state(Status_Normal_To_World);
            eh_->sendEventToWorld(e);
        }
        else if (state == LOAD_EMPTY)
        {
            static const vector<string> fpid;
            pDataUser = eh_->getDataHandler()->createUser(e->friendinfo().platid(), "fri", "", 0, PLAT_QZONE, false, false, 0, fpid, eh_, "openkey", 0, "friend init", "PC");
            eh_->postBackEvent(e);

        }
        return;
    }
    else
    {
        SetData(e, pDataUser);
        User *pUser = pUserManager->getUser(e->uid());
        //SendGiftToUser(e, pUser);
        if (pUser == NULL)
        {
            return;
        }
        SendInfoToUser(e, pUser);
        SetCache(e->friendinfo().info(), e);
    }
}

/**
 * 远程读取
 * @param e
 */
void
FriendRequest::handle_romateload(Event* e)
{
    GWG_FriendRequest* pRespone = e->mutable_friendinfo();
    GameDataHandler* pUserManager = eh_->getDataHandler();
    LoadStatus state = LOAD_INVALID;
    User *pDataUser = GetUser(pRespone->platid(), pRespone->tuid(), &state, true);
    if (pDataUser == NULL)
    {
        if (state == LOAD_WAITING)
        {
            eh_->postBackEvent(e);
        }
        if (state == LOAD_EMPTY)
        {
            static const vector<string> fpid;
            pDataUser = eh_->getDataHandler()->createUser(e->friendinfo().platid(), "fri", "", 0, PLAT_QZONE, false, false, 0, fpid, eh_, "openkey", 0, "friend init", "PC");
            eh_->postBackEvent(e);
        }
        return;
    }
    else
    {
        SetData(e, pDataUser);
        e->set_state(Status_Normal_Back_World);
        eh_->sendEventToWorld(e);
    }
}

/**
 * 远程回复
 * @param e
 */
void
FriendRequest::handle_romatereturn(Event* e)
{
    GameDataHandler* pUserManager = eh_->getDataHandler();
    const FriendInfoLite lite = e->friendinfo().info();
    User *pUser = pUserManager->getUser(e->uid());
    SendInfoToUser(e, pUser);
    SetCache(lite, e);


    //   MseFriendList list ;
    //   list.set_type(0);
    //   list.add_friendlist()->CopyFrom(lite);
    //   int64 llSecond = Clock::GetSecond();
    //   list.mutable_friendlist(0)->set_canhire(llSecond > lite.employeefreetime());
    //   string text;
    //   list.SerializeToString(&text);
    //   GameDataHandler* pUserManager = eh_->getDataHandler();
    //   User *pUser = pUserManager->getUser(e->uid());
    //SendGiftToUser(e,pUser);
    //   if (pUser == NULL || !pUser->Online())
    //   {
    //       return;
    //   }
    //   Player* pPlayer = pUser->GetPlayer();
    //   if (lite.nickname().length() > 0 || lite.openid().length() > 0 || lite.headurl().length() > 0)
    //   {
    //       SendInfoToUser(e, pUser);
    //       SetCache(lite, e);
    //   }

}

/**
 * 将好友信息缓存到FriendServer
 * @param lite
 * @param e
 */
void
FriendRequest::SetCache(const FriendInfoLite& lite, Event* e, bool bAdd /*=true*/)
{
    if (e == NULL)
    {
        return;
    }
    if (FriendInfoServerHandler::GetInst()->CanUse())
    {
        GWG_FriendRequest* pRespone = e->mutable_friendinfo();
        if (pRespone && !pRespone->ret())
        {
            FriendInfoServerHandler::GetInst()->SafePushFriendUpdate(lite.openid(), e->friendinfo().plat_type(), &lite);
        }
    }

    /*if (e == NULL)
    {
        return;
    }
    string friendcache;
    lite.SerializeToString(&friendcache);
    int64 uid = -1;
    bool ret = safe_atoll(lite.uid(), uid);
    GameDataHandler* pUserManager = eh_->getDataHandler();
    if (FriendInfoServerHandler::GetInst()->CanUse())
    {
        if (GameConfig::GetInstance()->EnableFriendInfoServerThread())
        {
            GWG_FriendRequest* pRespone = e->mutable_friendinfo();
            if (pRespone && !pRespone->ret())
            {
                FriendInfoServerHandler::GetInst()->SafePushFriendUpdate(lite.openid(), e->friendinfo().plat_type(), &lite);
            }
        }
        else
        {
            FriendInfoServerHandler::UpdateFriendInfo(lite.openid(), e->friendinfo().plat_type(), friendcache);
        }
    }
    else
    {
        pUserManager->setFriendInfo(lite.openid(), uid, friendcache, (PLAT_TYPE) e->friendinfo().plat_type() );
    }

    if (bAdd)
    {
        User *pUser = pUserManager->getUser(e->uid());
        if (pUser == NULL)
        {
            return;
        }
        Player* pPlayer = pUser->GetPlayer();
        pUser->addFriendID(uid);
        if (uid != pUser->id())
        {
            pPlayer->GetFriendActionSet().AllocEvent(uid, pUser->friends_id().size() - 1);
            pPlayer->GetFriendActionSet().AddFriendLvl(uid,lite.level());
        }
    }*/
}

/**
 * 发送好友信息给玩家
 * @param e
 * @param pUser
 */
void
FriendRequest::SendInfoToUser(Event* e, User* pUser)
{

    if (pUser == NULL || e == NULL)
    {
        return;
    }
    FriendInfoLite*         pLite    = e->mutable_friendinfo()->mutable_info();
    Player*                    pPlayer = pUser->GetPlayer();
    UniverseCtrl&           uni       = pPlayer->GetUniverseCtrl();
    int64 uid = llInvalidId;
    safe_atoll(pLite->uid(), uid);

    //如果是因为送礼物而产生的数据. 则不进入排行榜
    bool bSkipRank = false;
    if (pUser->qqgroup_friends_platid_.size() > 0)
    {
        for (int i = 0; !bSkipRank && i < pUser->qqgroup_friends_platid_.size(); i++)
        {
            if (pUser->qqgroup_friends_platid_[i] == pLite->openid())
            {
                bSkipRank = true;
            }
        }
    }


    for (int i = 0; !bSkipRank && i < pLite->lvlscores_size(); i++)
    {
        uni.AddRank(i + 1, uid, pLite->lvlscores(i));
    }



    if ( !bSkipRank && Clock::GetWeek(Clock::GetDay()) == Clock::GetWeek(pLite->update_day()))
    {
        for (int i = 0; i < pLite->dallylvlscores_size() && i < DallyUniverseMgr::g_nRoundSize; i++)
        {
            if (pLite->dallylvlscores(i) > 0)
            {
                uni.m_xDallyRankDetail[i].AddUid(uid, pLite->dallylvlscores(i));
            }
        }
        uni.AddRankDally(uid, pLite->dallytotalsocre());
    }

    pUser->GetPlayer()->GetActiveCtrl().OnSendFriend(uid, pLite->lvlscores_size());
    MseFriendList pushfriendinfo ;
    pushfriendinfo.set_jsonresult(pLite->jsonstr());
    pPlayer->GetCallbackCtrl().OnSendFrined(pLite);

    if (!bSkipRank )
    {
        pUser->GetPlayer()->FriendStillNeedSendCnt--;
    }

    if (pUser->GetPlayer()->FriendStillNeedSendCnt <= 0)
    {
        pushfriendinfo.set_dbm("last");
        pUser->GetPlayer()->GetUniverseCtrl().RebuildTopPlayerList();
        pPlayer->GetCallbackCtrl().OnAllFriendSent();

        pPlayer->GetActiveInvite().SetFriendLvlCnt(pPlayer->GetActiveCtrl().nCmp_lvl_25,
                pPlayer->GetActiveCtrl().nCmp_lvl_35,
                pPlayer->GetActiveCtrl().nCmp_lvl_45 );


    }
    if (pUser->Online())
    {
        std::string text;
        pushfriendinfo.SerializeToString(&text);
        eh_->sendDataToUser(pUser->fd(), S2C_MseFriendList, text);
    }
    //   if (e == NULL || pUser == NULL)
    //   {
    //       return;
    //   }
    //   FriendInfoLite* pLite = e->mutable_friendinfo()->mutable_info();
    //   if (pLite->nickname().length() <= 0)
    //   {
    //       //return;
    //       pLite->set_nickname(" ");
    //   }

    //   MseFriendList list ;
    //   list.add_friendlist()->CopyFrom(*pLite);
    //     int64 llSecond = Clock::GetSecond();
    //   list.mutable_friendlist(0)->set_canhire(llSecond > pLite->employeefreetime());
    //   int64 uid = -1;
    //   safe_atoll(pLite->uid() , uid);
    //   Player* pPlayer = pUser->GetPlayer();
    //   list.mutable_friendlist(0)->set_friendtype(0);
    //   list.set_type(0);
    //   CheckFirstVist(&list, pUser);
    //   SetCache(*pLite, e);
    //   CheckRandomEvent(&list, pUser);
    //   if (pUser->GetPlayer()->GetWheelLottery().UpdateWheelLimit(pLite->level()))
    //   {
    //       UserCtrl uc(pUser);
    //       uc.SendWheelLotteryToSelf();
    //   };

    //   if ( FuncSwitch::IsEnable( Fortune::FUNC_SWITCH_INDEX, pPlayer ) )
    //   {
    //       for ( int i = 0; i < list.friendlist_size(); i++ )
    //           pPlayer->GetFortune()->SetLite( list.mutable_friendlist(i) );
    //   }
    //   
    //if ( FuncSwitch::IsEnable( FriendRecall::FUNC_SWITCH_INDEX, pPlayer ) )
    //{
    //	bool flg = false;
    //	for ( int i = 0; i < list.friendlist_size(); i++ )
    //	{
    //		flg |= pPlayer->GetFriendRecall()->UpdateCanRecallList( list.mutable_friendlist(i) );
    //		flg |= pPlayer->GetFriendRecall()->UpdateRecallList( list.mutable_friendlist(i) );
    //	}
    //
    //	if ( flg )
    //	{
    //		GameDataHandler* pUserManager = eh_->getDataHandler();
    //		pUserManager->markUserDirty(pUser);
    //	}
    //}

    //   //发送好友信息
    //   std::string text;
    //   list.SerializeToString(&text);
    //   eh_->sendDataToUser(pUser->fd(), S2C_MseFriendList , text);
    //     if (e->friendinfo().has_action_id())
    //     {
    //         const std::string id = e->friendinfo().action_id();
    //         DB_ActionRecord* pAr = pPlayer->GetAction(id);
    //         if (pAr == NULL)
    //         {
    //             return;
    //         }
    //         pAr->set_head_url(pLite->headurl());
    //         pAr->set_name(pLite->nickname());
    //         UserCtrl uc(pUser);
    //         uc.SendSingleFriendAction(pAr);
    //     }
}

/**
 * 用User来填充FirendLite
 * @param e
 * @param pDataUser
 */
void
FriendRequest::SetData(Event* e, User* pDataUser)
{
    if (e == NULL || pDataUser == NULL)
    {
        return;
    }
    if (pDataUser->GetPlayer() == NULL)
    {
        if (e->has_friendinfo())
        {
            LOG4CXX_ERROR(logger_, "FriendRequest ERROR!!!!!" << e->friendinfo().DebugString());
        }
        //LOG4CXX_ERROR(logger_,"FriendRequest ERROR!!!!!"<<pDataUser->id()<<pDataUser->platform_id());
        return;
    }
    FriendInfoLite*         pLite    = e->mutable_friendinfo()->mutable_info();
    GWG_FriendRequest*      pRespone = e->mutable_friendinfo();

    if (pLite == NULL || pRespone == NULL)
    {
        return;
    }

    pDataUser->FillAsFriendLite(pLite, (PLAT_TYPE) pRespone->plat_type() );

    e->mutable_friendinfo()->set_ret(true);
    if (e->pushbackcnt() > 0)
    {
        //this means DataUser is just loaded from database;
        eh_->getDataHandler()->getStatistics().capture("FriendRequestUserLoad", nLoadCnt++);
        if (Clock::GetHour() != llLoadCntTimeSpan)
        {
            llLoadCntTimeSpan = Clock::GetHour();
            nLoadCnt = 0;
        }
    }
}

/**
 * 获取玩家
 * @param pid  平台id
 * @param uid  uid
 * @param status 返回 读取状态
 * @param load 是否要从数据库加载(如果不在内存里)
 * @return User指针
 */
User*
FriendRequest::GetUser(const string& pid, const int64 uid, LoadStatus* status, bool load)
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

void
FriendRequest::SendGiftToUser( Event* e, User* pUser )
{
    if (e == NULL || pUser == NULL)
    {
        return;
    }
    Player *pPlayer = pUser->GetPlayer();
    if (pPlayer == NULL)
    {
        return;
    }

    FriendInfoLite* pLite = e->mutable_friendinfo()->mutable_info();
    if (pLite->nickname().length() <= 0)
    {
        pLite->set_nickname(" ");
    }

    if (e->friendinfo().has_action_id())
    {
    }
}

/******************************************************************************/
/*    Push                                                               */
/******************************************************************************/
//<editor-fold desc="Push">

void
FriendRequest::PushFriendRequestList(User* pUser)
{
    FriendInfoServerHandler* pInfoServerHandler = FriendInfoServerHandler::GetInst();
    if (pUser == NULL || pInfoServerHandler == NULL)
    {
        return;
    }
    for (size_t i = 0; i < pUser->qqgroup_friends_platid_.size(); i++)
    {
        FriendInfoServerHandler::GetInst()->SafePushFriendRequest(pUser->id(), pUser->getPlatType(), pUser->qqgroup_friends_platid_[i], -1);
    }
    for (size_t i = 0; i < pUser->friends_platid().size(); i++)
    {
        FriendInfoServerHandler::GetInst()->SafePushFriendRequest(pUser->id(), pUser->getPlatType(), pUser->friends_platid()[i], -1);
    }

}
//</editor-fold>

