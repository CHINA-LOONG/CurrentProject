/* 
 * File:   UserCtrl.h
 * Author: Kidd
 *
 * Created on 2011年7月26日, 下午5:31
 */

#ifndef USERCTRL_H
#define	USERCTRL_H
#include "common/const_def.h"
#include <string>
#include <map>
#include "event/MessageDef.h"
using namespace std;

class Player;
class User;
class LeaveMessage;
class DB_ActionRecord;
class GameEventHandler;

class UserCtrl
{
public:
    UserCtrl(User* pUser );
    UserCtrl(int64 id);
    ~UserCtrl();

public:
    Player*             GetPlayer();
    User*               GetUser();
    void                Send(int cmd, const string &text);
private:
    friend class        Daemon;
    static void         SetEventHandler(GameEventHandler* pEh);
public:
    static GameEventHandler* GetEventHandler();
    static int          GetGameId();
private:
    static GameEventHandler*   m_pEventHandler;
    User*                      m_pUser;
    Player*                    pPlayer;

} ;
//////////////////////////////////////////////////////////////////


#endif	/* USERCTRL_H */

