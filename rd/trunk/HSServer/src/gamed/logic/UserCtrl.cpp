/* 
 * File:   UserCtrl.cpp
 * Author: Kidd
 * 
 * Created on 2011年7月26日, 下午5:31
 */

#include "logic/UserCtrl.h"
#include "common/string-util.h"

#include "GameConfig.h"
#include "EventQueue.h"
#include "logic/User.h"
#include "logic/Player.h"
#include "GameEventHandler.h"
#include "AutoEvent_PDHead.h"
#include "GameDataHandler.h"
#include "common/Clock.h"

//////////////////////////////////////////////////////////////////////////////
//  BaseDefine                                                              //
//////////////////////////////////////////////////////////////////////////////
//<editor-fold desc="BaseDefine">
#define M_Check() \
if (m_pUser == NULL || m_pEventHandler == NULL || !m_pUser->Online()) \
{ \
    return; \
}

GameEventHandler * UserCtrl::m_pEventHandler = NULL;

UserCtrl::UserCtrl(User* pUser)
{
    m_pUser = pUser;
    if (m_pUser != NULL)
    {
        pPlayer = pUser->GetPlayer();
    }
    else
    {
        pPlayer = NULL;
    }

}

UserCtrl::UserCtrl(int64 uid)
{
    m_pUser = m_pEventHandler->getDataHandler()->getUser(uid);
    if (m_pUser != NULL)
    {
        pPlayer = m_pUser->GetPlayer();
    }
    else
    {
        pPlayer = NULL;
    }
}

/**
 * 析构的时候自动存盘
 */
UserCtrl::~UserCtrl()
{
    M_Check()
            //m_pEventHandler->getDataHandler()->markUserDirty(m_pUser);
}

void
UserCtrl::SetEventHandler(GameEventHandler* pEh)
{
    m_pEventHandler = pEh;
}

Player*
UserCtrl::GetPlayer()
{
    if (m_pUser != NULL)
    {
        return m_pUser->GetPlayer();
    }
    return NULL;
}

User*
UserCtrl::GetUser()
{
    return m_pUser;
}

void
UserCtrl::Send(int cmd, const string& text)
{
    m_pEventHandler->sendDataToUser(m_pUser->fd(), cmd, text);
}

GameEventHandler*
UserCtrl::GetEventHandler()
{
    return m_pEventHandler;
}

int
UserCtrl::GetGameId()
{
    return m_pEventHandler->GetSrvID();
}

