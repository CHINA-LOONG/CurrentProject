///* 
// * File:   DealWebAsk.h
// * Author: Kidd
// *
// * Created on 2011年9月19日, 下午6:10
// */
//
//#ifndef DEALWEBASK_H
//#define	DEALWEBASK_H
//#include "../../event/EventDefine.h"
//#include "../GameEventHandler.h"
//#include "../GameDataHandler.h"
//#include "../../logic/User.h"
//
//class DealWebAsk
//{
//public:
//
//    DealWebAsk()
//    {
//        logger_ = log4cxx::Logger::getLogger("EventHelper");
//    }
//
//    ~DealWebAsk()
//    {
//    }
//public:
//
//    static void  createInstance(GameEventHandler* eh)
//    {
//        instance_ = new DealWebAsk();
//        instance_->eh_ = eh;
//        eh->getEventHandler()->registHandler(EVENT_WEB_ASK,
//                                             (ProcessRoutine) DealWebAsk::handle_);
//    }
//
//    static DealWebAsk* getInstance()
//    {
//        return instance_;
//    }
//
//    static void handle_(Event* e)
//    {
//        DealWebAsk::getInstance()->handle(e);
//    }
//private:
//private:
//    void handle(Event* e);
//
//    void handle_selfload(Event* e);
//    void handle_romateload(Event* e);
//    void handle_romatereturn(Event* e);
//    
//private:
//    GameEventHandler*       eh_;
//    log4cxx::LoggerPtr      logger_;
//    static DealWebAsk*      instance_;
//} ;
//
//#endif	/* DEALWebAsk_H */
//
