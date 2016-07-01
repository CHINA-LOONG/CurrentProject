#pragma once
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
//#include "../../event/FriendInfoLite.pb.h"

class User;
//enum LoadStatus;
//class FriendInfoLite{};

class DealSimpleMark
{
public:

    DealSimpleMark()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
    }

    ~DealSimpleMark()
    {
    }

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new DealSimpleMark();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_SIMPLE_MARK,
                                             (ProcessRoutine) DealSimpleMark::handle_);
    }

    static DealSimpleMark* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        DealSimpleMark::getInstance()->handle(e);
    }

private:
    void            handle(Event* e);
    void            handle_selfload(Event* e);
    void            handle_romateload(Event* e);
    void            handle_romatereturn(Event* e);
    User*           GetUser(const string& pid, const int64 uid, LoadStatus* status, bool load);
    void            DealEvent(SimpleMarkUser* pRequest, User* pUser);
public:
    void            PushRequest(User* pSelf,int64 llTargetId, int nTid, int nValue);
    void            PushRequest(int64 llUid,int nId,int nValue);
private:
    GameEventHandler* eh_;
    log4cxx::LoggerPtr logger_;
    static DealSimpleMark* instance_;
} ;
