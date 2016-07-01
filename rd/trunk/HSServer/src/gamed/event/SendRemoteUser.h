#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "MessageDef.h"

class EventQueue;

class SendRemoteUser {
public:
	SendRemoteUser()
	{
		logger_ = log4cxx::Logger::getLogger("EventHelper");
	}

	~SendRemoteUser() {}

	static void createInstance(GameEventHandler* eh)
	{
		getInstance()->eh_ = eh;
		eh->getEventHandler()->registHandler(EVENT_SEND_REMOTE_USER,
			(ProcessRoutine)SendRemoteUser::handle_);
	}

	static SendRemoteUser* getInstance()
	{
		static SendRemoteUser instance_;
		return &instance_;
	}

	static void handle_(Event* e)
	{
		getInstance()->handle(e);
	}

private:
	void handle(Event* e);
	void handle_WG_Req(Event* e);
private:
	GameEventHandler* eh_;
	log4cxx::LoggerPtr logger_;
};
