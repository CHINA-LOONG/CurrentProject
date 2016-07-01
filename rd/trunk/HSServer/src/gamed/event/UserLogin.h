#pragma once
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
#include "../../logic/User.h"

class User;
//enum LoadStatus;

class UserLogin
{
public:

    UserLogin()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
    }

    ~UserLogin()
    {
    }

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new UserLogin();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_USER_LOGIN,
                                             (ProcessRoutine) UserLogin::handle_);
        eh->getEventHandler()->registHandler(EVENT_USER_LOGOUT,
                                             (ProcessRoutine) UserLogin::handle_);

    }

    static UserLogin* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        UserLogin::getInstance()->handle(e);
    }
private:
    void HandleUserLogin(Event* e);
    void HandleUserLogout(Event* e);
private:
    User* processUserLogin(const string& platid, int siteid, const string& name,
                           const string& profile_link, int gender, vector<string> friends_platid,
                           bool isYellowDmd, bool isYellowDmdYear, int lvlYellowDmd, LoadStatus& status,const string &openkey,int register_type,const string& action_from,const string& device_from);
    void handle(Event* e);
    void handle_WG_UserLogin(Event* e);
    void SendOffLine(User* pUser);
    void PushOnlineStatus(User* pUser, int64 uid);
private:
    GameEventHandler* eh_;
    log4cxx::LoggerPtr logger_;
    static UserLogin* instance_;
} ;

