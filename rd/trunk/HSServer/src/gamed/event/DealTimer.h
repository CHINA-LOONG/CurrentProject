/* 
 * File:   DealTimer.h
 * Author: Kidd
 *
 * Created on 2012年11月22日, 上午10:35
 */

#ifndef DEALTIMER_H
#define	DEALTIMER_H
#include "../../event/EventDefine.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"

class DealTimer
{
public:

    DealTimer();

    ~DealTimer()
    {
    }

    static void createInstance(GameEventHandler* eh)
    {
        instance_ = new DealTimer();
        instance_->eh_ = eh;
        eh->getEventHandler()->registHandler(EVENT_TIMER,
                                             (ProcessRoutine) DealTimer::handle_);
    }

    static DealTimer* getInstance()
    {
        return instance_;
    }

    static void handle_(Event* e)
    {
        DealTimer::getInstance()->handle(e);
    }
    void            handle(Event* e);
	void			LoadRankIntervalTime();
	void			SaveRankIntervalTime();
	void			UpdataRankInterval();
private:
    GameEventHandler* eh_;
    log4cxx::LoggerPtr logger_;
    static DealTimer* instance_;
	std::map<int,int64> interval_time_list_;
} ;

#endif	/* DEALTIMER_H */

