/* 
 * File:   DealCenterLogicCounter.h
 * Author: Kidd
 *
 * Created on 2013年8月16日, 下午2:33
 */
#pragma once
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
//#include "../../event/FriendInfoLite.pb.h"

class User;
//enum LoadStatus;
//class FriendInfoLite{};

class DealCenterLogicCounter
{
public:

    DealCenterLogicCounter()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
    }

    ~DealCenterLogicCounter()
    {
    }

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new DealCenterLogicCounter();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_CenterLogicCounter,
                                             (ProcessRoutine) DealCenterLogicCounter::handle_);
    }

    static DealCenterLogicCounter* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        DealCenterLogicCounter::getInstance()->handle(e);
    }

private:
    void            handle(Event* e);
public:
    void            PushRequest(const std::string& key, int nValue);
    void            PushSyncRequest(const std::string& key);

private:
    GameEventHandler* eh_;
    log4cxx::LoggerPtr logger_;
    static DealCenterLogicCounter* instance_;
} ;



