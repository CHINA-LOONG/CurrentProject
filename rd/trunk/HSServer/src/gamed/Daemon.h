#pragma once
#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif

class EventQueue;
class GameDataHandler;
class GameEventHandler;
class GameNetHandler;

class Daemon
{
public:
	Daemon(int nid);
	virtual ~Daemon();
	void start();
	static void LoadConfig();
private:
	GameEventHandler *eh;
	EventQueue	 *eq;
	GameDataHandler *dh;
	GameNetHandler *nh;
	log4cxx::LoggerPtr logger_;
    
};

