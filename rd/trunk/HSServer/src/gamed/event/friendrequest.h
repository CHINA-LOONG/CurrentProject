#pragma once
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
//#include "../../event/FriendInfoLite.pb.h"

class User;
//enum LoadStatus;
//class FriendInfoLite{};

class FriendRequest
{
public:

    FriendRequest()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
    }

    ~FriendRequest()
    {
    }

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new FriendRequest();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_GWG_FRIEND_REQUEST,
                                             (ProcessRoutine) FriendRequest::handle_);
    }

    static FriendRequest* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        FriendRequest::getInstance()->handle(e);
    }

private:
    void            handle(Event* e);
    void            handle_selfload(Event* e);
    void            handle_romateload(Event* e);
    void            handle_romatereturn(Event* e);
    void            SendInfoToUser(Event* e, User* pUser);
    void			SendGiftToUser(Event* e, User* pUser);
    void            SetData(Event* e, User* pDataUser);

    User*           GetUser(const string& pid, const int64 uid, LoadStatus* status, bool load);
public:
    void            SetCache(const FriendInfoLite& lite, Event* e, bool bAdd = true);
    void            PushFriendRequestList(User* pUser);

    //static void     CheckFirstVist(MseFriendList* pProto, User* pUser);
    //static void     CheckRandomEvent(MseFriendList* pProto, User* pUser);
    static int      nLoadCnt;
    static int64    llLoadCntTimeSpan;
private:
    GameEventHandler* eh_;
    log4cxx::LoggerPtr logger_;
    static FriendRequest* instance_;
} ;
