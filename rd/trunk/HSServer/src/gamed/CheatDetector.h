#pragma once

#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif
#include "common/const_def.h"
#include "event/MessageDef.h"

class CheatDetector
{
public:
    CheatDetector();

    bool cmdLr();
    bool cmdLf();

    void OnEvent(int nEventId, int64 uid);
    void KickUser(int nEventId, int64 uid);
public:
    static void         SetConfig(int nDl, int nCs, bool bAb, int nBt);
    static void         Load();
    static void         GlableEvent(int nEventId, int64 uid);
    static int          nDeadLine;
    static int          nCheckSpan;
    static bool         bAutoBan;
    static int          nBanTime;
private:
    log4cxx::LoggerPtr  cheat_logger_;
    bool                did_lr_;
    const static int    nMessageSize = C2S_Max_NotAMessage - C2S_EVENT_BASE;
    void                blockRobot(const char *msg);
    void                Clear();
    int                 pnEventCount[nMessageSize];
    int64               llLastCheckTime;
} ;
