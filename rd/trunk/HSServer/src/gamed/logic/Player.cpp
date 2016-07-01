/* 
 * File:   Player.cpp
 * Author: Kidd
 * 
 * Created on 2013年1月10日, 下午4:10
 */

#include "Player.h"
#include "User.h"
#include "EventQueue.h"
#include "EventDefine.h"
//#include "ActiveCallManger.h"
/******************************************************************************/
/*    Init                                                                    */
/******************************************************************************/
//<editor-fold desc="Init & const">
//</editor-fold>

Player::Player (User* pUser, DB_Player* pDb)
{
    m_pUser = pUser;
    m_pDbPlayer = pDb;
    if (pUser == NULL || pDb == NULL)
    {
        return ;
    }
}

void
Player::save()
{

}

Player::~Player()
{

}

int64 Player::GetUid()
{
	return m_pUser->id();
}

const std::string& Player::GetName()
{
	return m_pUser->DefaultNameForSelfLoad();
}

bool Player::IsEmptyUser()
{
    bool bRet = m_pUser == NULL || m_pUser->GetDbUser().player().inited() == false;
    return bRet;
}
