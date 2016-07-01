/* 
 * File:   DealWebInvite.h
 * Author: Kidd
 *
 * Created on 2011年9月19日, 下午6:10
 */

#ifndef DEALWEBINVITE_H
#define	DEALWEBINVITE_H
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
#include "../../logic/User.h"

class DealWebInvite
{
public:

    DealWebInvite()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
    }

    ~DealWebInvite()
    {
    }
public:

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new DealWebInvite();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_WEB_INVITE,
                                             (ProcessRoutine) DealWebInvite::handle_);
    }

    static DealWebInvite* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        DealWebInvite::getInstance()->handle(e);
    }
private:
    void handle(Event* e);
private:
    GameEventHandler*       eh_;
    log4cxx::LoggerPtr      logger_;
    static DealWebInvite*   instance_;
} ;

#endif	/* DEALWEBINVITE_H */

