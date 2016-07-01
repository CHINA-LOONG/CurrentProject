#pragma once

#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif
#include <pthread.h>

class DataHandler
{
public:
	DataHandler(int nid);
	virtual ~DataHandler(void);

	inline pthread_mutex_t* mutex() {return &mutex_;}
	inline time_t Revision() { return revision_; }
	inline void setRevision(time_t revision) { revision_ = revision; }

	virtual void tick();
	virtual void quit();

protected:
	pthread_mutex_t mutex_;
	log4cxx::LoggerPtr logger_;
	time_t revision_;
	int nid_;
};
