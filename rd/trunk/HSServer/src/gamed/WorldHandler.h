#pragma once
#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif

#include "net/ProtocolHandler.h"
#include <string>
using namespace std;

class EventQueue;
class NetHandler;

class WorldHandler : public ProtocolHandler
{
public:
    WorldHandler(EventQueue *eq, int fd, NetHandler *nh);
    ~WorldHandler();
    void handle(int64 uid, string &req);

    int handlerType()
    {
        return ProtocolHandler::WORLD;
    }

    void OnReg2World();
protected:
    int fd_;
    EventQueue *eq_;
    NetHandler *nh_;
    log4cxx::LoggerPtr logger_;
} ;

