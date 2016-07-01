#include "DealActiveinvite.h"
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
DealActiveinvite* DealActiveinvite::instance_ = NULL;

void DealActiveinvite::handle(Event* e)
{
	if (e->state() == Status_Normal_Game)
	{
		//handle_selfload(e);
	}
	else if (e->state() == Status_Normal_Logic_Game )
	{
		handle_romateload(e);
	}
	else if (e->state() == Status_Normal_Back_Game )
	{
		//handle_romatereturn(e);
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
void DealActiveinvite::handle_selfload(Event* e)
{
	GameDataHandler* pUserManager = eh_->getDataHandler();
	LoadStatus state = LOAD_INVALID;
	InviteFriendLvlSucc* pRequest = e->mutable_invitefriddata();
	User *pDataUser = GetUser(pRequest->openid(),pRequest->uid(), &state, true);
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
void DealActiveinvite::handle_romateload(Event* e)
{
	InviteFriendLvlSucc* pRequest = e->mutable_invitefriddata();
	GameDataHandler* pUserManager = eh_->getDataHandler();
	LoadStatus state = LOAD_INVALID;
	User *pDataUser_A = pUserManager->getUser(e->mutable_forwardinfo()->platid(),e);//GetUser(pRequest->openid(), pRequest->uid(), &state, true);
	if (pDataUser_A == NULL)
	{
		return;
	}
	else
	{
		DealEvent(pRequest, pDataUser_A);
		e->set_state(Status_Normal_Back_World);
		eh_->sendEventToWorld(e);
	}
}

/**
* 远程回复
* @param e
*/
void DealActiveinvite::handle_romatereturn(Event* e)
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
User* DealActiveinvite::GetUser(const string& pid, const int64 uid, LoadStatus* status, bool load)
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

void DealActiveinvite::DealEvent(InviteFriendLvlSucc* pRequest, User* pUser)
{
	if (pRequest == NULL || pUser == NULL)
	{
		return;
	}
    
	int64  uid  = pRequest->uid();
	string pid  = pRequest->openid();
	string url  = pRequest->url();
	int   levl  = pRequest->lvl();
	int  lgnday = pRequest->nowloginday();
	pUser->GetPlayer()->GetActiveInvite().fillchangeData(uid,pid,url,levl,lgnday);
    pUser->GetPlayer()->save();
    eh_->getDataHandler()->markUserDirty(pUser);
}

void DealActiveinvite::PushRequest(User* pSelf, int64 llTargetId, int nlvl, int nDay,string nurl)
{
	if (pSelf == NULL)
	{
		return;
	}
	Event* ev = eh_->getEventQueue()->allocateEvent();
	ev->set_cmd(EVENT_INV_FRIEND_LVL_SUCC);
	ev->set_state(Status_Normal_Game);
	ev->set_time(0);
	ev->set_uid(pSelf->id());

	InviteFriendLvlSucc* pRequest = ev->mutable_invitefriddata();
	pRequest->set_openid("");
	pRequest->set_uid(llTargetId);
    pRequest->set_nowloginday(nDay);
	pRequest->set_lvl(nlvl);
	pRequest->set_url(nurl);
	eh_->getEventQueue()->safePushEvent(ev);
}

void DealActiveinvite::PushRequest(int64 llUid, int nlvl, int nDay,string nurl)
{
	Event* ev = eh_->getEventQueue()->allocateEvent();
	ev->set_cmd(EVENT_INV_FRIEND_LVL_SUCC);
	ev->set_state(Status_Normal_Game);
	ev->set_time(0);
	ev->set_uid(llUid);

	InviteFriendLvlSucc* pRequest = ev->mutable_invitefriddata();
	pRequest->set_openid("");
	pRequest->set_uid(llUid);
	pRequest->set_nowloginday(nDay);
	pRequest->set_lvl(nlvl);
	pRequest->set_url(nurl);
	eh_->getEventQueue()->safePushEvent(ev);
}
