#pragma once
#include "BaseEvent.h"
#include "../../common/json-util.h"

#define admstr_args const std::vector<string>& tokens,Value& objResponseJson,User* pUser
class User;

class DealAdminEvent : public CBaseEvent
{
public:
    DealAdminEvent();
    ~DealAdminEvent();

    static void createInstance(GameEventHandler* eh)
    {
        getInstance()->eh_ = eh;

        eh->getEventHandler()->registHandler(EVENT_ADMIN_BROADCAST,
                                             ((ProcessRoutine) DealAdminEvent::handle_));
        eh->getEventHandler()->registHandler(EVENT_ADMIN_RELOAD,
                                             ((ProcessRoutine) DealAdminEvent::handle_));
        eh->getEventHandler()->registHandler(EVENT_ADMIN_CLEAR,
                                             ((ProcessRoutine) DealAdminEvent::handle_));
        eh->getEventHandler()->registHandler(EVENT_ADMIN_ONLINE,
                                             ((ProcessRoutine) DealAdminEvent::handle_));
        eh->getEventHandler()->registHandler(EVENT_ADMIN_KICK,
                                             ((ProcessRoutine) DealAdminEvent::handle_));
        eh->getEventHandler()->registHandler(EVENT_ADMIN_STR,
                                             ((ProcessRoutine) DealAdminEvent::handle_));

    }

    static DealAdminEvent* getInstance()
    {
        static DealAdminEvent instance_;
        return &instance_;
    }

    static void handle_(Event* e)
    {
        getInstance()->handle(e);
    }


private:
    void handle(Event* e);

protected:
    void processAdminBroadCast(Event* e);
    void processAdminReload(Event* e);
    void processAdminClear(Event* e);
    void processAdminOnLineNum(Event* e);
    void processAdminKickPlayer(Event* e);
    void processAdminStr(Event* e);
    bool Admstr_SetLogNoSend(admstr_args);
    bool Admstr_friend(admstr_args);
    bool Admstr_teststrbuild(admstr_args);
    bool Admstr_removedailyvalue(admstr_args);
    bool Admstr_SetCheatBit(admstr_args);
    bool Admstr_CheckReward(admstr_args);
    bool Admstr_SetDailyCounter(admstr_args)  ;
    bool Admstr_AddDailyCounter(admstr_args);
    bool Admstr_SetInviteNum(admstr_args);
    bool Admstr_WebCallbackSender(admstr_args);
    bool Admstr_WebCallbackTaker(admstr_args);
    bool Admstr_SetCallbackHistorySender(admstr_args);
    bool Admstr_SetCallbackHistoryTaker(admstr_args);
    bool Admstr_SetPlayerLost(admstr_args);
    bool Admstr_SetZyCity(admstr_args);
    bool AdmStr_SetRegTime(admstr_args);
    bool AdmStr_FullBack(admstr_args);
    bool Admstr_FakeClientEvent(const std::vector<string>& tokens, Value& objResponseJson, User* pUser, Event* e);
	bool Admstr_Restar(admstr_args);
private:
    const static int g_nStrLogicArgStart = 3;
} ;
