/* 
 * File:   DealWebInvite.cpp
 * Author: Kidd
 * 
 * Created on 2011年9月19日, 下午6:10
 */

#include "DealWebInvite.h"
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
#include "../../logic/InviteCtrl.h"
#include "../../logic/ActiveInvite.h"
#include "../../logic/LogCtrl.h"


DealWebInvite* DealWebInvite::instance_ = NULL;

void DealWebInvite::handle(Event* e)
{
    // 被邀请
    const WebInvite& request = e->webinvite();
    GameDataHandler* pUserManager = eh_->getDataHandler();
    LoadStatus state = LOAD_INVALID;
    User* pUser = pUserManager->getUser(request.platid() , &state, true);

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
    if (pPlayer->GetInviteCtrl().IsBeenInvited())
    {
        return;
    }
    pPlayer->GetInviteCtrl().SetInviter(request.inviterid());
    //  pPlayer->MarkInvitehandled();
    //  
    //  /*if ( (FuncSwitch::IsEnable( MultiLevelFriend::FUNC_SWITCH_INDEX, pPlayer )) )
    //  {*/
    //pPlayer->GetMultiLevelFriend()->SetPreOpenId( request.inviterid() );
    //pUserManager->markUserDirty(pUser);
    //  //}
    //  
    Event* ev = eh_->getEventQueue()->allocateEvent();
    ev->set_cmd(EVENT_INVITE);
    ev->set_uid(pUser->id());
    ev->set_state(Status_Normal_Game);
    ev->set_time(-1);
    Invite* pRequest = ev->mutable_invite();
    pRequest->set_platid(request.platid());
    pRequest->set_inviterid(request.inviterid());
    pUser->FillForwardInfo(ev->mutable_forwardinfo());
    eh_->getEventQueue()->safePushEvent(ev);
    //日志记录被邀请者
	pPlayer->GetLogCtrl().LogBegin(LT_LogTypeNormal)->
		LogItem("web端被邀请好友id",pPlayer->GetUid())->
		LogItem("UnivereCnt", 1)->
		LogEnd("TmpDragboatFvl");
	
}


