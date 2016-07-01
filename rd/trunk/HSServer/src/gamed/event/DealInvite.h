/* 
 * File:   DealInvite.h
 * Author: Kidd
 *
 * Created on 2011年9月19日, 下午6:10
 */

#ifndef DEALINVITE_H
#define	DEALINVITE_H 
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
#include "../../logic/User.h"

class DealInvite
{
public:

    DealInvite()
    {
        logger_ = log4cxx::Logger::getLogger("EventHelper");
		
    }

    ~DealInvite()
    {
    }
public:

    static void  createInstance(GameEventHandler* eh)
    {
        instance_ = new DealInvite();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_INVITE,
                                             (ProcessRoutine) DealInvite::handle_);
    }

    static DealInvite* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        DealInvite::getInstance()->handle(e);
    }
private:
private:
    void handle(Event* e);

    void handle_selfload(Event* e);
    void handle_romateload(Event* e);
    void handle_romatereturn(Event* e);
    
private:
    GameEventHandler*       eh_;
    log4cxx::LoggerPtr      logger_;
    static DealInvite*      instance_;
	    
} ;

#endif	/* DEALINVITE_H */

