/* 
 * File:   DealCenterLogicCounter.cpp
 * Author: Kidd
 * 
 * Created on 2013年8月16日, 下午2:33
 */

#include "DealCenterLogicCounter.h"
#include "../../logic/ConfigUnit/CLCounterMgr.h"
#include "../event/EventQueue.h"

DealCenterLogicCounter* DealCenterLogicCounter::instance_ = NULL;
/******************************************************************************/
/*    handle                                                                  */
/******************************************************************************/
//<editor-fold desc="handle   \">

void
DealCenterLogicCounter::handle(Event* e)
{
    if (e == NULL)
    {
        return ;
    }
    if (e->state() == Status_Normal_Back_Game)
    {
        //if (e->center_logic_value().oper() == (int) CLO_SYNC)
        {
            int v = e->center_logic_value().value();
            const std::string& key = e->center_logic_value().key();
            CLCounterClinetUnit* pUnit = CLCounterMgr::GetInst()->GetItem(key);
            if (v >= pUnit->value || pUnit->sync_state == CLCounterClinetUnit::Unsynced)
            {
                pUnit->value = v;
            }
            pUnit->sync_state = CLCounterClinetUnit::Synecd ;
        }
    }
}
//</editor-fold>

/******************************************************************************/
/*    push event                                                              */
/******************************************************************************/
//<editor-fold desc="push event">

/**
 * 触发一个"变量同步" 事件给world
 * @param key
 * @param nValue
 */
void
DealCenterLogicCounter::PushRequest(const std::string& key, int nValue)
{
    int64 uid = eh_->getDataHandler()->GameId();
    Event* ev = eh_->getEventQueue()->allocateEvent();
    ev->set_cmd(EVENT_CenterLogicCounter);
    ev->set_state(Status_Normal_Game);
    ev->set_time(0);
    ev->set_uid(uid);

    CenterLogicValue* pRequest = ev->mutable_center_logic_value();
    pRequest->set_key(key);
    pRequest->set_value(nValue);
    pRequest->set_oper(CLO_ADD);
    eh_->sendEventToWorld(ev, 0);
}

void
DealCenterLogicCounter::PushSyncRequest(const std::string& key)
{
    int64 uid = eh_->getDataHandler()->GameId();
    Event* ev = eh_->getEventQueue()->allocateEvent();
    ev->set_cmd(EVENT_CenterLogicCounter);
    ev->set_state(Status_Normal_Game);
    ev->set_time(0);
    ev->set_uid(uid);

    CenterLogicValue* pRequest = ev->mutable_center_logic_value();
    pRequest->set_key(key);
    pRequest->set_value(0);
    pRequest->set_oper(CLO_SYNC);
    eh_->sendEventToWorld(ev, 0);
}
//</editor-fold>
