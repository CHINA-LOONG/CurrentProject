#pragma once

#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
#include "../../logic/User.h"

class DealActiveinvite
{
public:
	DealActiveinvite(void)
	{
	   logger_ = log4cxx::Logger::getLogger("EventHelper");
	}
	~DealActiveinvite(void)
	{


	}

	static void createInstance(GameEventHandler* eh)
	{
		instance_ = new DealActiveinvite();
		instance_->eh_ = eh;
		eh->getEventHandler()->registHandler(EVENT_INV_FRIEND_LVL_SUCC,
			(ProcessRoutine) DealActiveinvite::handle_);
	}

	static DealActiveinvite* getInstance()
	{
		return instance_;
	}

	static void handle_(Event* e)
	{
		DealActiveinvite::getInstance()->handle(e);
	}

private:
	void            handle(Event* e);
	void            handle_selfload(Event* e);
	void            handle_romateload(Event* e);
	void            handle_romatereturn(Event* e);
	User*           GetUser(const string& pid, const int64 uid, LoadStatus* status, bool load);
	void            DealEvent(InviteFriendLvlSucc* pRequest, User* pUser);
public:
	void            PushRequest(User* pSelf, int64 llTargetId, int nlvl, int nDay,string nurl);
	void            PushRequest(int64 llUid, int nlvl, int nDay,string nurl);
private:
	GameEventHandler* eh_;
	log4cxx::LoggerPtr logger_;
	static DealActiveinvite* instance_;
};
