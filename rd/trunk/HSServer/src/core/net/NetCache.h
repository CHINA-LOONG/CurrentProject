#pragma once

#ifdef _WIN32
	#include <WinSock2.h>
	#include "common/Logger_win.h"
#else
	#include <sys/types.h>
	#include <sys/socket.h>
	#include <netinet/in.h>
	#include <arpa/inet.h>
	#include <log4cxx/logger.h>
#endif

#include <pthread.h>
#include <string>
#include <deque>
#include "common/const_def.h"
using namespace std;

class ProtocolHandler;

class NetCache
{
public:
	int fd;
	int64 uid;
	bool remove;
	bool aborted; // remote disconnected
	bool idle; // if idle, should be kicked out
	ProtocolHandler *ph;
	static const size_t DEFAULT_READ_SIZE;// = 4096;
	static const size_t INIT_WRITE_SIZE;// = 16384;
	static const size_t WEBSERVER_READ_SIZE;// = 131072;

	NetCache(int fd, struct sockaddr_in addr, size_t rsize);
	NetCache();
	~NetCache(void);

	bool read(void);
	bool write(bool block=false);
	bool prepareWrite(const char *str, size_t size);
	bool assemble(string &str);
	char *addrstr();
	bool waitToWrite();
	bool init(int fd,struct sockaddr_in addr);
	size_t getReadSize(){return rsize;}
private:
	char *rbuf;
	char *cmdbuf;
	char *wbuf;
	size_t wsize; // write buffer size
	size_t rsize; // read buffer size
	struct sockaddr_in addr;
	int rpos, wpos;
	pthread_mutex_t write_mutex;
	log4cxx::LoggerPtr logger_;


	inline void lockWrite()
	{
		pthread_mutex_lock(&write_mutex);
	}
	inline void unlockWrite()
	{
		pthread_mutex_unlock(&write_mutex);
	}
};


class NetCacheManager
{
public:
	NetCacheManager();
	~NetCacheManager();
	NetCache*	getCache();
	void		RemoveCache(NetCache* pCache);
	static NetCacheManager* getInst();
private:
	deque<NetCache*> m_CacheQueue; 
	static NetCacheManager* m_pInst;
//	static const int nNetCacheMax = 1024 * 10;
};
