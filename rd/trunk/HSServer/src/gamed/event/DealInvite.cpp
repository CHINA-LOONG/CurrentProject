/* 
 * File:   DealInvite.cpp
 * Author: Kidd
 * 
 * Created on 2011年9月19日, 下午6:10
 */
#include "DealInvite.h"
#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/json/json.h"
#include "../../common/string-util.h"
#include "../../logic/User.h"
#include "../../logic/Player.h"
#include "../../common/SysLog.h"
#include "../../event/EventQueue.h"
#include "../../logic/Player.h"
#include "../../logic/UserCtrl.h"
#include "../../logic/FuncSwitch.h"

DealInvite* DealInvite::instance_ = NULL;

void
DealInvite::handle(Event* e)
{
    if (e->state() == Status_Normal_Game)
    {
        handle_selfload(e);
    }
    else if (e->state() == Status_Normal_Logic_Game )
    {
        handle_romateload(e);
    }
    else
    {
        LOG4CXX_ERROR(logger_, "Invalid Event.\n" << e->DebugString());
    }

}

void
DealInvite::handle_selfload(Event* e)
{
    const Invite& request = e->invite();
    GameDataHandler* pUserManager = eh_->getDataHandler();
    LoadStatus state = LOAD_INVALID;
    User* pUser = pUserManager->getUser(request.inviterid() , &state, true);

    if (pUser == NULL)
    {
        if (state == LOAD_WAITING)
        {
            eh_->postBackEvent(e);
        }
        if (state == LOAD_MISS)
        {
            ForwardInfo* pForward = e->mutable_forwardinfo();
            pForward->set_platid(request.inviterid());
            e->set_state(Status_Normal_To_World);
            eh_->sendEventToWorld(e);
        }
        return;
    }
    Player* pPlayer = pUser->GetPlayer();
    UserCtrl uc(pUser);
    pUser->GetPlayer()->GetInviteCtrl().AddInviteCount();
    pPlayer->GetBag().AddCash(3);
   
    //pPlayer->AddInviteCount(request.platid());
	//处理邀请任务数据
	string url = e->forwardinfo().senderurl();//pPlayer->GetUrl();
	int64  uid = e->uid();// pPlayer->GetUid();
	int    day = 1 ;//pPlayer->GetLevel();

	pPlayer->GetActiveInvite().setInitThirdFrid(uid,url,1);

	pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
		LogItem("web端被邀请好友id",uid)->
		LogItem("loginday", day)->
		LogItem("url", url)->
		LogEnd("dwInvitefrinedactive");
	
 

	
    ForwardInfo* pForward = e->mutable_forwardinfo();
    //pPlayer->AddAcceptInviteHistory(pForward->uid(), pForward->sendername(), pForward->senderurl());


    //pPlayer->GetDallyTask().OnAction(DallyTask::E_AT_INVITE,&uc);
	//pUserManager->markUserDirty(pUser);
}

void
DealInvite::handle_romateload(Event* e)
{
    const Invite& request = e->invite();
    GameDataHandler* pUserManager = eh_->getDataHandler();
    LoadStatus state = LOAD_INVALID;
    User* pUser = pUserManager->getUser(request.inviterid() , &state, true);

    if (pUser == NULL)
    {
        if (state == LOAD_WAITING)
        {
            eh_->postBackEvent(e);
        }
        return;
    }
    Player* pPlayer = pUser->GetPlayer();
    UserCtrl uc(pUser);
    pUser->GetPlayer()->GetInviteCtrl().AddInviteCount();
    pPlayer->GetBag().AddCash(3);

	string url = e->forwardinfo().senderurl(); //pPlayer->GetUrl();
	int64  uid = e->uid(); // pPlayer->GetUid();
	int    loginday = 1; //pPlayer->GetLevel();

	pPlayer->GetActiveInvite().setInitThirdFrid(uid,url,loginday);

    // pPlayer->AddInviteCount(request.platid());
	pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
		LogItem("web端被邀请好友id",uid)->
		LogItem("loginday",loginday)->
		LogItem("url", url)->
		LogEnd("dwInvitefrinedactive");


    ForwardInfo* pForward = e->mutable_forwardinfo();
    // pPlayer->AddAcceptInviteHistory(pForward->uid(), pForward->sendername(), pForward->senderurl());

    /* if ( (FuncSwitch::IsEnable( MultiLevelFriend::FUNC_SWITCH_INDEX, pPlayer )) )
     {
         pPlayer->GetMultiLevelFriend()->AddLowUserId( e->uid(), pForward->sendername(), pForward->senderurl() );
         pUserManager->markUserDirty(pUser);
     }*/

    //pPlayer->GetDallyTask().OnAction(DallyTask::E_AT_INVITE,&uc);
}

void
DealInvite::handle_romatereturn(Event* e)
{
}