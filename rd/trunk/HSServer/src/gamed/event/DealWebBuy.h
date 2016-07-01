#pragma once
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
class User;

class DealWebBuy
{
public:

    DealWebBuy()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
    }

    ~DealWebBuy()
    {
    }

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new DealWebBuy();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_WEB_BUY,
                                             (ProcessRoutine) DealWebBuy::handle_);
    }

    static DealWebBuy* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        DealWebBuy::getInstance()->handle(e);
    }
private:
    void    handle(Event* e);
    void    handle_per(Event* e);
    void    handle_after(Event* e,bool direct_give = false);
    bool    WebBuyCheck(User* pUser, const WebBuy& request,WebBuy* pResponse);
    User*   GetUser(Event* e);
private:
    GameEventHandler* eh_;
    log4cxx::LoggerPtr logger_;
    static DealWebBuy* instance_;
} ;

