#pragma once
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"

class DealWebCheckBalance
{
public:

    DealWebCheckBalance()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
    }

    ~DealWebCheckBalance()
    {
    }

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new DealWebCheckBalance();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_WEB_CHECKBALANCE,
                                             (ProcessRoutine) DealWebCheckBalance::handle_);
    }

    static DealWebCheckBalance* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        DealWebCheckBalance::getInstance()->handle(e);
    }
private:
    void    handle(Event* e);
private:
    GameEventHandler* eh_;
    log4cxx::LoggerPtr logger_;
    static DealWebCheckBalance* instance_;
} ;

