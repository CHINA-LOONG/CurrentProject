#include "CheatDetector.h"
#include "common/SysLog.h"
#include "logic/User.h"
#include "logic/Player.h"
#include "logic/UserCtrl.h"


int  CheatDetector::nDeadLine    = 100;
int  CheatDetector::nCheckSpan   = 10;
bool CheatDetector::bAutoBan     = false;
int  CheatDetector::nBanTime     = 5;

CheatDetector::CheatDetector()
{
    did_lr_ = false;
    cheat_logger_ = log4cxx::Logger::getLogger("cheat");
    Clear();
}

bool CheatDetector::cmdLr()
{
    did_lr_ = true;
    return false;
}

bool CheatDetector::cmdLf()
{
    if (!did_lr_)
    {
        blockRobot("lr");
        return true;
    }
    else
    {
        return false;
    }
}

void CheatDetector::blockRobot(const char *msg)
{
    //    if (ncache_ != NULL)
    //    {
    //        int64 uid = ncache_->uid;
    //        LOG4CXX_INFO(cheat_logger_, "[cd] uid:" << uid << ", IP:" << ncache_->addrstr() << ", action:" << msg);
    //    }
}

void CheatDetector::Clear()
{
    llLastCheckTime = time(NULL);
    memset(pnEventCount, 0, nMessageSize * sizeof (int) );
}

void CheatDetector::OnEvent(int nEventId, int64 uid)
{
    int nPos = nEventId - C2S_EVENT_BASE - 1;
    if (nPos < 0 || nPos >= nMessageSize)
    {
        return;
    }
    /*if (nEventId == C2S_MceEndGroupBuyingEx)
    {
        return;
    }*/
    int cnt = ++pnEventCount[nPos];
    if (cnt > nDeadLine)
    {
        KickUser(nEventId, uid);
    }
    if ( time(NULL) - llLastCheckTime > nCheckSpan)
    {
        Clear();
    }
}

void CheatDetector::KickUser(int nEventId, int64 uid)
{
    int nPos = nEventId - C2S_EVENT_BASE - 1;
   // SYS_LOG(uid, LT_CheatDetector, 0, 0, nEventId << pnEventCount[nPos] << nDeadLine << bAutoBan << nBanTime);
    UserCtrl uc(uid);
    User* pUser = uc.GetUser();
    Player* pPlayer = uc.GetPlayer();
    if (pUser != NULL && pPlayer != NULL)
    {
        uc.GetEventHandler()->removeUserFdMap(pUser->fd(), pUser->id());
        pUser->setOnline(false);
        pUser->setFd(0);
        if (bAutoBan)
        {
            int64 llSecond    = time(NULL);
            int64 llDBBanTime = pUser->GetDbUser().player().banlogin().time();
            if (llSecond + nBanTime > llDBBanTime)
            {
               
            }
        }
    }
}

void CheatDetector::SetConfig(int nDl, int nCs, bool bAb, int nBt)
{
    nDeadLine = nDl;
    nCheckSpan = nCs;
    nBanTime = nBt;
    bAutoBan = bAb;
}

#include"common/Ini.h"

void CheatDetector::Load()
{
    Ini cd_config;
    if (cd_config.Open("Config/ChectDetect.ini"))
    {
        int nbt =  cd_config.Readint("Cheat", "BanTime_Minute");
        int ncs =  cd_config.Readint("Cheat", "CheckSpan_Second");
        int ndl =  cd_config.Readint("Cheat", "DeadLine");
        bool bab =  cd_config.Readint("Cheat", "AutoBan");
        SetConfig( ndl, ncs,  bab,  nbt);
    }
}

void CheatDetector::GlableEvent(int nEventId, int64 uid)
{
    UserCtrl uc(uid);
    if (uc.GetPlayer() != NULL)
    {
       
    }
}
