/* 
 * File:   Player.h
 * Author: Kidd
 *
 * Created on 2013年1月10日, 下午4:10
 */

#ifndef PLAYER_H
#define	PLAYER_H
#include "dbinterface.pb.h"
#include "User.h"
//class User;
class GameEventHandler;
class Event;
#define PlayerCtrlObj(obj) \
private: obj m_x##obj; \
public: inline obj& Get##obj(){return m_x##obj;}

class Player
{
public:
    Player(User* pUser, DB_Player* pDb);
    ~Player();
public:
    int64                   GetUid();
    const std::string&      GetName();
    const std::string&      GetUrl();
	bool Player::IsEmptyUser();

public:
    DB_Player*              GetDbPlayer();
    DB_User&                GetDbUserRef();
	void                    save();
public:

private:
    DB_Player*              m_pDbPlayer;
    User*                   m_pUser;
    GameEventHandler*       m_pGameEventHandler;

} ;


inline DB_Player* Player::GetDbPlayer()
{
    return m_pDbPlayer;
}

#endif	/* PLAYER_H */

