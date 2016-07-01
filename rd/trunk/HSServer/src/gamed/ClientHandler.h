#pragma once

#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif

#include <string>
#include <vector>
#include "net/ProtocolHandler.h"
#include "CheatDetector.h"
#include "net/NetCache.h"
#include "common/const_def.h"

using namespace std;

class GameNetHandler;

class ClientHandler : public ProtocolHandler
{
public:
    ClientHandler(GameNetHandler *nh, int fd, int nid_, NetCache *cache);
    ~ClientHandler(void);
    void handle(int64 uid, string &req);
    void HandleAutoEvent(int64 uid, int nType, string& req);
    int handlerType();
    void leave(int64 uid);

private:
    GameNetHandler *nh;
    int fd;
    int nid_;
    static string policy_content;
    void processPolicy();
    log4cxx::LoggerPtr logger_;
    CheatDetector* pCheatDetector;

} ;
