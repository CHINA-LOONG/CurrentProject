#pragma once

#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif

class MyLog
{
public:
	static log4cxx::LoggerPtr getLoger()
	{
		return logger;
	}
public:
	static log4cxx::LoggerPtr logger;
};
